using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Blog.Jwt.AuthHelper.Authentication
{
    public class JwtIssuerOptions
    {
        /// <summary>
        /// 4.1.1.  "iss" (Issuer) Claim - The "iss" (issuer) claim identifies the principal that issued the JWT.
        /// 颁发机构
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 4.1.2.  "sub" (Subject) Claim - The "sub" (subject) claim identifies the principal that is the subject of the JWT.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 4.1.3.  "aud" (Audience) Claim - The "aud" (audience) claim identifies the recipients that the JWT is intended for.
        /// 颁发给谁
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 4.1.4.  "exp" (Expiration Time) Claim - The "exp" (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for processing.
        /// </summary>
        public DateTime Expiration => IssuedAt.AddMinutes(ExpireMinutes);

        /// <summary>
        /// 4.1.5.  "nbf" (Not Before) Claim - The "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.
        /// </summary>
        public DateTime NotBefore => DateTime.UtcNow;

        /// <summary>
        /// 4.1.6.  "iat" (Issued At) Claim - The "iat" (issued at) claim identifies the time at which the JWT was issued.
        /// </summary>
        public DateTime IssuedAt => DateTime.UtcNow;

        /// <summary>
        /// Set the timespan the token will be valid for (default is 20 min)
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(20);

        public string ConnectionString { get; set; }



        /// <summary>
        /// "jti" (JWT ID) Claim (default ID is a GUID)
        /// </summary>
        public Func<Task<string>> JtiGenerator =>
          () => Task.FromResult(Guid.NewGuid().ToString("N"));

        /// <summary>
        /// The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials SigningCredentials =>
            new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);

        public SecurityKey SigningKey { get; set; }

        public int ExpireMinutes { get; set; } = 10;
    }
}
