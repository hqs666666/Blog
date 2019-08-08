using System.Threading.Tasks;
using Blog.ApiFramework.Controllers;
using Blog.IService.Users;
using Blog.Jwt.Dtos;
using Blog.Jwt.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiBaseController
    {
        #region Ctor

        private readonly IUserService _userService;
        private readonly IAppTokenService _appTokenService;

        public AuthController(IAppTokenService appTokenService,
            IUserService userService)
        {
            _appTokenService = appTokenService;
            _userService = userService;
        }

        #endregion

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody]RequestTokenDto requestDto)
        {
            var result = await _appTokenService.GenerateTokenAsync(requestDto);
            if (!result.Result)
                return BadRequest(result.Message);
            return Ok(result.Token);
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("123");
        }
    }
}
