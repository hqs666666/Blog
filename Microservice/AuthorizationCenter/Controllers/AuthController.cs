using System.Collections.Generic;
using System.Threading.Tasks;
using AuthorizationCenter.Dtos;
using Blog.Jwt;
using Blog.Jwt.AuthHelper.Authentication;
using Blog.Jwt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Ctor

        private readonly IJwtFactory _jwtFactory;

        public AuthController(IJwtFactory jwtFactory)
        {
            _jwtFactory = jwtFactory;
        }

        #endregion

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody]RequestTokenDto requestDto)
        {
            ////判断用户是否存在
            //var user = new JwtUser
            //{
            //    Id = "123",
            //    UserName = requestDto.UserName,
            //    Roles = new List<string> { "RegisterUser" }
            //};
            //var claimsIdentity = _jwtFactory.GenerateClaimsIdentity(user);
            //var token = await _jwtFactory.GenerateEncodedToken(user.Id, claimsIdentity);
            //return new OkObjectResult(token);
            return Ok();
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenDto requestDto)
        {
            //var token = await _jwtFactory.RefreshToken(requestDto.RefreshToken);
            //return new OkObjectResult(token);
            return Ok();
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("2312");
        }
    }
}
