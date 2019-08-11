
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace Blog.Jwt.AuthHelper.Authorization
{
    public class JwtIdentity : IIdentity
    {
        public string AuthenticationType { get; }
        public bool IsAuthenticated { get; }
        public string Name { get; }
        public List<Claim> Claims { get; set; }

        public JwtIdentity(string name, bool auth)
        {
            Name = name;
            IsAuthenticated = auth;
        }
    }
}
