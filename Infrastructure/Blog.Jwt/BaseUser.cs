using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Jwt
{
    public class BaseUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}
