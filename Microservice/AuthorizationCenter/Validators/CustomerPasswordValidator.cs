using System;
using System.Threading.Tasks;
using Blog.IService.Users;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace AuthorizationCenter.Validators
{
    public class CustomerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserService _userService;

        public CustomerPasswordValidator(IUserService userService)
        {
            _userService = userService;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var result = _userService.GetUser(context.UserName, context.Password);
            if (result != null)
            {
                context.Result = new GrantValidationResult(
                    result.Id,
                    OidcConstants.AuthenticationMethods.Password,
                    DateTime.UtcNow,
                    result.Claims);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }

            return Task.CompletedTask;
        }
    }
}
