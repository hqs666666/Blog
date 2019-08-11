

using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Jwt.AuthHelper.Authentication;
using Blog.Jwt.Dtos;
using Blog.Jwt.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Blog.Jwt.Service
{
    public class UserValidatorService : IUserValidatorService
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public UserValidatorService(JwtIssuerOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        public async Task ValidateAsync(UserValidatorContext context)
        {
            using (var conn = new MySqlConnection(_jwtOptions.ConnectionString))
            {
                var sql = @"select Id,Name from `user` where Name=@name and Password=@password;";
                var user = await conn.QueryFirstOrDefaultAsync<JwtUser>(sql,
                    new { name = context.UserName, password = context.Password });

                if (user == null)
                {
                    context.CreateErrorResult("username or password is valid!");
                    return;
                }


                var claims = new List<Claim>
                {
                    new Claim("id",user.Id),
                    new Claim("name",user.Name)
                };
                context.SubjectId = user.Id;
                context.Claims = claims;
            }
        }
    }
}
