using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.ApiFramework.Controllers;
using Blog.Dto.AuthDtos;
using Blog.IService.Users;
using Blog.Jwt;
using Blog.Jwt.AuthHelper.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Blog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiBaseController
    {
        #region Ctor

        private readonly IJwtFactory _jwtFactory;
        private readonly IUserService _userService;

        public AuthController(IJwtFactory jwtFactory,
            IUserService userService)
        {
            _jwtFactory = jwtFactory;
            _userService = userService;
        }

        #endregion

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody]RequestTokenDto requestDto)
        {
            var user = _userService.GetUser(requestDto.UserName, requestDto.Password);
            if (user == null)
                return BadRequest("username or password is valid!");
            
            var userinfo = new BaseUser
            {
                Id = user.Id,
                UserName = user.Name,
                Roles = new List<string> { "RegisterUser" }
            };
            var claimsIdentity = _jwtFactory.GenerateClaimsIdentity(userinfo);
            var token = await _jwtFactory.GenerateEncodedToken(user.Id, claimsIdentity);
            return new OkObjectResult(token);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenDto requestDto)
        {
            var token = await _jwtFactory.RefreshToken(requestDto.RefreshToken);
            return new OkObjectResult(token);
        }
    }
}
