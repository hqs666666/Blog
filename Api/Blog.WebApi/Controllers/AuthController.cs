using System.Linq;
using System.Threading.Tasks;
using Blog.ApiFramework.Controllers;
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

        private readonly IAppTokenService _appTokenService;

        public AuthController(IAppTokenService appTokenService)
        {
            _appTokenService = appTokenService;
        }

        #endregion

        [HttpPost("token")]
        public async Task GetToken([FromBody]RequestTokenDto requestDto)
        {
            await _appTokenService.GenerateTokenAsync(requestDto);
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            var user = HttpContext.User.Claims.Select(p => new { p.Type, p.Value });
            return Ok(user);
        }
    }
}
