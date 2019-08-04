

using Blog.Entity.Users;

namespace Blog.IService.Users
{
    public interface IUserService : IBaseService
    {
        User GetUser(string id);
        User GetUser(string username, string password);
    }
}
