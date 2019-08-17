using System.Linq;
using System.Threading.Tasks;
using Blog.IService.Users;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace AuthorizationCenter.Profiles
{
    public class CustomProfileService : IProfileService
    {
        private readonly IUserService _userService;

        public CustomProfileService(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 只要有关用户的身份信息单元被请求（例如在令牌创建期间或通过用户信息终点），就会调用此方法
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims = context.Subject.Claims.ToList();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 验证用户是否有效 例如：token创建或者验证
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = _userService.GetUser(context.Subject.GetSubjectId());
            context.IsActive = user != null;
            return Task.CompletedTask;
        }
    }
}
