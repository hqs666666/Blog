using System.Collections.Generic;
using System.Security.Claims;

namespace Blog.Jwt.Dtos
{
    public class ErrorResultDto
    {
        public string err_message { get; set; }

        public ErrorResultDto(string msg)
        {
            err_message = msg;
        }
    }

    public class TokenValidateResult
    {
        public bool Result { get; set; } = true;
        public string Message { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
