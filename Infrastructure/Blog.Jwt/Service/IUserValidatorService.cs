
using System.Threading.Tasks;
using Blog.Jwt.Dtos;

namespace Blog.Jwt.Service
{
    public interface IUserValidatorService
    {
        Task ValidateAsync(UserValidatorContext context);
    }
}
