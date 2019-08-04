using Microsoft.AspNetCore.Mvc;

namespace Blog.ApiFramework.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult Json(object data)
        {
            return new JsonResult(data);
        }
    }
}
