using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Blog.Jwt.Dtos;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Blog.Jwt.AuthHelper.Authentication
{
    public class JwtFactory : IJwtFactory
    {
        #region Ctor

        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(JwtIssuerOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        #endregion

        #region Utils

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                                 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);

        private static DateTime TimeStampToDateTime(long seconds) => DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime;

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ExpireMinutes <= 0)
            {
                throw new ArgumentException("Must be a non-zero ExpireMinutes.", nameof(JwtIssuerOptions.ExpireMinutes));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }

            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.ConnectionString));
            }
        }

        private async Task<string> GenerateRefreshTokenAsync(string sub)
        {
            var token = Guid.NewGuid().ToString("N");
            using (var conn = new MySqlConnection(_jwtOptions.ConnectionString))
            {
                var now = DateTime.Now;
                var sql = @"insert into `refresh_token`(createTime,CreateBy,Token,UserId,ExpireTime) values(@createTime,@createBy,@token,@userId,@expireTime);";
                var row = await conn.ExecuteAsync(sql, new { createTime = now, createBy = sub, token = token, userId = sub, expireTime = now.AddDays(7) });
                if (row < 1)
                    throw new Exception("refresh token save fail");
            }

            return token;
        }

        private string ToBase64(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            var toBase64 = Convert.ToBase64String(bytes);
            return toBase64;
        }

        private T Base64ToData<T>(string str)
        {
            var bytes = Convert.FromBase64String(str);
            var json = Encoding.Default.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string HmacSHA256(string message, string secret)
        {
            var encoding = new UTF8Encoding();
            var keyByte = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        private Task<string> GenerateHeader()
        {
            var header = new Dictionary<string, string>()
            {
                {"alg", "HS256"}, {"typ", "JWT"}
            };
            var toBase64 = ToBase64(header);
            return Task.FromResult(toBase64);
        }

        private async Task<string> GeneratePayload(string sub, List<Claim> customClaims)
        {
            var claims = new Dictionary<string, object>
            {
                {"sub", sub},
                {"jti", await _jwtOptions.JtiGenerator()},
                {"iss", _jwtOptions.Issuer},
                {"aud", _jwtOptions.Audience},
                {"iat", ToUnixEpochDate(_jwtOptions.IssuedAt)},
                {"nbf", ToUnixEpochDate(_jwtOptions.NotBefore)},
                {"exp", ToUnixEpochDate(_jwtOptions.Expiration)},
            };

            var custom = customClaims.Select(p => new { p.Type, p.Value });
            foreach (var item in custom)
            {
                claims.Add(item.Type, item.Value);
            }

            var toBase64 = ToBase64(claims);
            return toBase64;
        }

        private Task<string> GenerateSignature(string header, string payload)
        {
            var headerAndPayload = HmacSHA256($"{header}.{payload}", _jwtOptions.Secret);
            return Task.FromResult(headerAndPayload);
        }

        private TokenValidateResult CreateTokenResult(bool result, string message = null)
        {
            return new TokenValidateResult
            {
                Result = result, Message = message
            };
        }

        #endregion

        #region Token For Custom

        public async Task<JwtToken> GenerateEncodedTokenAsync(string sub, List<Claim> customClaims)
        {
            var header = await GenerateHeader();
            var payload = await GeneratePayload(sub, customClaims);
            var signature = await GenerateSignature(header, payload);
            var token = new JwtToken
            {
                access_token = $"{header}.{payload}.{signature}",
                refresh_token = await GenerateRefreshTokenAsync(sub),
                expires_in = (_jwtOptions.ExpireMinutes * 60) - 1,
                token_type = "Bearer"
            };
            return token;
        }

        public async Task<TokenValidateResult> ParsedTokenAsync(string token)
        {
            var tokenInfo = token.Split('.');
            var header = tokenInfo[0];
            var payload = tokenInfo[1];
            var signature = tokenInfo[2];

            var generateSignture = await GenerateSignature(header, payload);
            if (!generateSignture.Equals(signature))
                return CreateTokenResult(false, "signature invalid");

            var userInfo = Base64ToData<Dictionary<string, object>>(payload);
            var now = ToUnixEpochDate(DateTime.Now);
            var expireTime = (long) userInfo["exp"];
            if (expireTime <= now)
                return CreateTokenResult(false, "token expired");

            var claims = new List<Claim>();
            foreach (var item in userInfo)
            {
                var claim = new Claim(item.Key, item.Value.ToString());
                claims.Add(claim);
            }

            var result = CreateTokenResult(true);
            result.Claims = claims;
            return result;
        }

        #endregion

        #region Token For Identity

        public async Task<JwtToken> GenerateEncodedTokenForIdentityModelAsync(string sub, List<Claim> customClaims)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, sub),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            };
            claims.AddRange(customClaims);

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new JwtToken
            {
                access_token = encodedJwt,
                refresh_token = await GenerateRefreshTokenAsync(sub),
                expires_in = (_jwtOptions.ExpireMinutes * 60) - 1,
                token_type = "Bearer"
            };
            return token;
        }

        #endregion
    }
}
