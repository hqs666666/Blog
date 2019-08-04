using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
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
        }

        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<string> GenerateEncodedToken(string sub, ClaimsIdentity identity)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, sub),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst(ClaimTypes.Name),
                identity.FindFirst("id")
            };
            claims.AddRange(identity.FindAll(ClaimTypes.Role));

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                refresh_token = GenerateRefreshToken(),
                expires_in = (_jwtOptions.ExpireMinutes * 60) -1,
                token_type = "Bearer"
            };

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<string> RefreshToken(string refreshToken)
        {
            var sub = "";
            var identity = GenerateClaimsIdentity(new BaseUser());
            return await GenerateEncodedToken(sub, identity);
        }

        /// <summary>
        /// 生成用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ClaimsIdentity GenerateClaimsIdentity(BaseUser user)
        {
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"));
            claimsIdentity.AddClaim(new Claim("id", user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            foreach (var role in user.Roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            return claimsIdentity;
        }
    }
}
