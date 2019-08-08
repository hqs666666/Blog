
using System.Threading.Tasks;
using Blog.Jwt.Dtos;

namespace Blog.Jwt.Service
{
    public interface IAppTokenService
    {
        Task<JwtTokenContext> GenerateTokenAsync(RequestTokenDto dto);
    }
}
