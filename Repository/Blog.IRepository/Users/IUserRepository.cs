using Blog.Entity.Users;

namespace Blog.IRepository.Users
{
    public interface IUserRepository : IBaseRepository<User, string>
    {
        User GetUser(string id);
        User GetUser(string username, string password);
    }
}
