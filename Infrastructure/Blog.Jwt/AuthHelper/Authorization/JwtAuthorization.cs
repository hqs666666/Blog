
using System.IO;
using System.Threading.Tasks;
using Blog.Jwt.Dtos;
using Blog.Jwt.Service;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Blog.Jwt.AuthHelper.Authorization
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppTokenService _appTokenService;

        public JwtAuthorizationMiddleware(RequestDelegate next, IAppTokenService appTokenService)
        {
            _next = next;
            _appTokenService = appTokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/oauth/token")
            {
                if (context.Request.Method != HttpMethods.Post)
                {
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
                }

                if (context.Request.ContentType != "application/json")
                {
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    return;
                }

                var str = new StreamReader(context.Request.Body).ReadToEnd();
                var data = JsonConvert.DeserializeObject<RequestTokenDto>(str);
                await _appTokenService.GenerateTokenAsync(data);
                return;
            }
            await _next(context);
        }
    }
}
