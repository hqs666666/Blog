using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Blog.Jwt.Dtos
{
    public class UserValidatorContext
    {
        public string SubjectId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<Claim> Claims { get; set; }
        public bool Result { get; set; } = true;
        public string Message { get; set; }

        public void CreateErrorResult(string message)
        {
            Result = false;
            Message = message;
        }
    }
}
