using System.Collections.Generic;

namespace Blog.Jwt.Models
{
    public class JwtUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
