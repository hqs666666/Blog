
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Blog.Jwt.AuthHelper.Authentication;
using Blog.Jwt.Dtos;
using Blog.Jwt.Extensions;
using Microsoft.AspNetCore.Http;

namespace Blog.Jwt.AuthHelper.Authorization
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtFactory _jwtFactory;

        public JwtAuthorizationMiddleware(RequestDelegate next, IJwtFactory jwtFactory)
        {
            _next = next;
            _jwtFactory = jwtFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/api/auth/token")
            {
                await _next(context);
                return;
            }

            var header = context.Request.Headers["Authorization"].ToString();
            if (!header.Contains("Bearer"))
            {
                await context.Response.WriteJsonAsync(new ErrorResultDto("access_token invalid"), HttpStatusCode.Unauthorized);
                return;
            }

            var token = header.Replace("Bearer ", string.Empty);
            var tokenResult = await _jwtFactory.ParsedTokenAsync(token);
            if (!tokenResult.Result)
            {
                await context.Response.WriteJsonAsync(new ErrorResultDto(tokenResult.Message), HttpStatusCode.Unauthorized);
                return;
            }

            IPrincipal pr = new JwtPrincipal("hqs", "123");
            var id = new JwtIdentity("hqs", true);
            id.Claims = tokenResult.Claims;
            context.User = new ClaimsPrincipal(id);
            await _next(context);
        }
    }
}
