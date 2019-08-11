using Blog.Jwt.AuthHelper.Authorization;
using Microsoft.AspNetCore.Builder;

namespace Blog.Jwt.Extensions
{
    public static class AuthExtension
    {
        public static void UseJwtAuthentication(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<JwtAuthorizationMiddleware>();
        }
    }
}
