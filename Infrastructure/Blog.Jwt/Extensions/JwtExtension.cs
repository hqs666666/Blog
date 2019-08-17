using System;
using System.Threading.Tasks;
using Blog.Jwt.AuthHelper.Authentication;
using Blog.Jwt.Builder;
using Blog.Jwt.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Jwt.Extensions
{
    public static class JwtExtension
    {
        public static IJwtBuilder AddJwt(this IServiceCollection services,
            Action<JwtIssuerOptions> jwtOptionsAction)
        {
            var builder = services.AddIdentityServerBuilder();

            var jwtAppSettingOptions = new JwtIssuerOptions();
            builder.Services.AddSingleton(jwtAppSettingOptions);
            jwtOptionsAction?.Invoke(jwtAppSettingOptions);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IJwtFactory, JwtFactory>();
            builder.Services.AddTransient<IAppTokenService, AppTokenService>();
            builder.Services.AddTransient<IUserValidatorService, UserValidatorService>();

            #region Authentication

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

            #endregion

            return builder;
        }

        public static void AddJwtAuthentication(this AuthenticationBuilder builder, Action<JwtIssuerOptions> jwtOptionsAction)
        {
            var jwtAppSettingOptions = new JwtIssuerOptions();
            builder.Services.AddSingleton(jwtAppSettingOptions);
            jwtOptionsAction?.Invoke(jwtAppSettingOptions);

            builder.AddJwtBearer(options =>
            {
                var jwtEvent = new JwtEventEx();
                options.Events = jwtEvent.JwtEvent();
            });
        }

        

        public static IJwtBuilder AddUserValidation<T>(this IJwtBuilder builder) where T : class, IUserValidatorService
        {
            builder.Services.AddTransient<IUserValidatorService, T>();
            return builder;
        }

        public static IJwtBuilder AddIdentityServerBuilder(this IServiceCollection services)
        {
            return new JwtBuilder(services);
        }
    }

    public class JwtEventEx
    {
        static readonly Func<HttpRequest, string> InternalTokenRetriever = request => request.HttpContext.Items[""] as string;
        internal JwtBearerEvents JwtEvent()
        {
            var bearer = new JwtBearerEvents();
            return new JwtBearerEvents()
            {

                OnMessageReceived = e =>
                {
                    e.Token = InternalTokenRetriever(e.Request);
                    return bearer.MessageReceived(e);
                },
                OnTokenValidated = e => bearer.TokenValidated(e),
                OnAuthenticationFailed = e => bearer.AuthenticationFailed(e),
                OnChallenge = e => bearer.Challenge(e)
            };
        }
    }
}
