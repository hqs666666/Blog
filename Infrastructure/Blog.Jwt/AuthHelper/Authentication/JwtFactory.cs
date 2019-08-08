using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
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
                var row = await conn.ExecuteAsync(sql, new { createTime = now, createBy = sub, token = token, userId = sub, expireTime = now.AddDays(7)});
                if (row < 1)
                    throw new Exception("refresh token save fail");
            }

            return token;
        }

        #endregion

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="customClaims"></param>
        /// <returns></returns>
        public async Task<JwtToken> GenerateEncodedTokenAsync(string sub, List<Claim> customClaims)
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
                expires_in = (_jwtOptions.ExpireMinutes * 60) -1,
                token_type = "Bearer"
            };
            return token;
        }
    }
}
