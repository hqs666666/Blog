
using System.Security.Principal;

namespace Blog.Jwt.AuthHelper.Authorization
{
    public class JwtPrincipal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity { get; private set; }

        public string SubjectId { get; set; }
        public string UserName { get; set; }

        public JwtPrincipal(string username,string sub)
        {
            this.Identity = new JwtIdentity(username,false);
            SubjectId = sub;
            UserName = username;
        }
    }
}
