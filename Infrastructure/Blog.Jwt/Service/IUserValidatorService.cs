

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Jwt.Service
{
    public interface IUserValidatorService
    {
        Task<List<Claim>> ValidateAsync(string name, string password);
    }
}
