
using System.Linq;
using Blog.ApiFramework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ApiBaseController
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            var user = HttpContext.User.Claims.Select(p => new { p.Type, p.Value });
            return Ok(user);
        }
    }
}
