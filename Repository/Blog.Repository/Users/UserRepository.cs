
using Blog.Entity.Users;
using Blog.IRepository.Users;

namespace Blog.Repository.Users
{
    public class UserRepository : BaseRepository<User, string>, IUserRepository
    {
        public UserRepository(ConnectionFactory factory) : base(factory)
        {

        }

        public User GetUser(string id)
        {
            var sql = "select * from `user` where id=@id";
            return Get(sql, new {id = id});
        }

        public User GetUser(string username, string password)
        {
            var sql = "select * from `user` where name=@username and password=@password";
            return Get(sql, new {username, password});
        }
    }
}
