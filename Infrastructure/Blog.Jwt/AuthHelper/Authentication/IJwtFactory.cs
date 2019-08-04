using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Jwt.AuthHelper.Authentication
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string sub, ClaimsIdentity identity);
        Task<string> RefreshToken(string refreshToken);
        ClaimsIdentity GenerateClaimsIdentity(BaseUser user);
    }
}
