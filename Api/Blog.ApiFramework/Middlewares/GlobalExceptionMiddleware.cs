using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;

namespace Blog.ApiFramework.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var message = $"{context.Request.Host + context.Request.Path + context.Request.QueryString}；" +
                          $"请求方式：{context.Request.Method}";
            
            try
            {
                _logger.Log(LogLevel.Info, message);
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex.Message);
            }
        }
    }
}
