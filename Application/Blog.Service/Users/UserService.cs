

using System.Collections.Generic;
using System.Security.Claims;
using Blog.Entity.Users;
using Blog.IRepository.Users;
using Blog.IService.Users;

namespace Blog.Service.Users
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetUser(string id)
        {
           return _userRepository.GetUser(id);
        }

        public User GetUser(string username, string password)
        {
            var user = _userRepository.GetUser(username, password);
            user.Claims = new List<Claim>
            {
                new Claim("email","hqs@qq.com")
            };
            return user;
        }

        public bool VerifyUser(string username, string password)
        {
            return GetUser(username, password) != null;
        }
    }
}
