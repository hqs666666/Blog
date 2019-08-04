using System;
using Blog.Jwt.AuthHelper.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Jwt.AuthHelper.Extension
{
    public static class JwtExtension
    {
        /// <summary>
        /// 自定义的jwt
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jwtOptionsAction"></param>
        public static void AddJwt(this IServiceCollection services,
            Action<JwtIssuerOptions> jwtOptionsAction)
        {
            var jwtAppSettingOptions = new JwtIssuerOptions();
            services.AddSingleton(jwtAppSettingOptions);
            jwtOptionsAction?.Invoke(jwtAppSettingOptions);
            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.ClaimsIssuer = jwtAppSettingOptions.Issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettingOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAppSettingOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtAppSettingOptions.SigningKey,
                    RequireExpirationTime = false, // 是否要求Token的Claims中必须包含Expires
                    ValidateLifetime = true,   //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    ClockSkew = TimeSpan.Zero   //允许的服务器时间偏移量
                };
                options.SaveToken = true;
            });
        }
    }
}
