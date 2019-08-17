using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Entity.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AuthorizationCenter.Extensions
{
    public static class HttpContextExtensions
    {
        public static Task SignInAsync(this HttpContext context, User user)
        {
            return Task.CompletedTask;
        }
        
        private static Task SignInAsync(this HttpContext context, ClaimsPrincipal principal)
        {
            return context.SignInAsync((string) null, principal, (AuthenticationProperties) null);
        }
        
    }
}