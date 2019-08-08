using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Jwt.AuthHelper.Authentication;
using Blog.Jwt.Dtos;
using Blog.Jwt.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace Blog.Jwt.Service
{
    public class AppTokenService : IAppTokenService
    {
        #region Ctor

        private readonly IJwtFactory _jwtFactory;
        private readonly IUserValidatorService _userValidatorService;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly HttpContext _httpContext;

        public AppTokenService(IJwtFactory jwtFactory,
            JwtIssuerOptions jwtOptions,
            IUserValidatorService userValidatorService,
            IHttpContextAccessor httpContextAccessor)
        {
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions;
            _userValidatorService = userValidatorService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Utils

        private async Task<JwtToken> RefreshTokenAsync(string token)
        {
            using (var conn = new MySqlConnection(_jwtOptions.ConnectionString))
            {
                var sql = @"select * from `refresh_token` where token=@token and expireTime>=@now";
                var tokenInfo = await conn.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token, DateTime.Now });
                if (tokenInfo == null)
                    throw new Exception("the refresh token is expire");

                var sql1 = @"select Id, Name from `user` where id=@id";
                var user = await conn.QueryFirstOrDefaultAsync<JwtUser>(sql1, new { id = tokenInfo.UserId });
                if (user == null)
                    throw new Exception("the refresh token is not found user");

                var claims = new List<Claim>
                {
                    new Claim("id",user.Id),
                    new Claim("name",user.Name)
                };

                return await _jwtFactory.GenerateEncodedTokenAsync(user.Id, claims);
            }
        }

        private async Task<JwtToken> PasswordAsync(string name,string password)
        {
            var claims = await _userValidatorService.ValidateAsync(name, password);
            var userId = claims.First(p => p.Type == "id").Value;

            return await _jwtFactory.GenerateEncodedTokenAsync(userId, claims);
        }

        private async Task<bool> ExistClientAsync(string clientId, string secret)
        {
            using (var conn = new MySqlConnection(_jwtOptions.ConnectionString))
            {
                var sql = @"select * from app_client where ClientId=@clientId and ClientSecret=@secret";
                var client = await conn.QueryFirstOrDefaultAsync<AppClient>(sql, new { clientId, secret });
                return client != null;
            }
        }

        private JwtTokenContext CreateJwtContext(bool res, JwtToken token, string message = null)
        {
            return new JwtTokenContext
            {
                Result = res,Token = token,Message = message
            };
        }

        #endregion

        public async Task<JwtTokenContext> GenerateTokenAsync(RequestTokenDto dto)
        {
            if (!await ExistClientAsync(dto.ClientId, dto.ClientSecret))
                return CreateJwtContext(false, null, "client is not exist");

            var context = CreateJwtContext(true, null);
            switch (dto.GrantType)
            {
                case GrantType.Password:
                    context.Token = await PasswordAsync(dto.UserName, dto.Password);
                    break;
                case GrantType.RefreshToken:
                    context.Token = await RefreshTokenAsync(dto.RefreshToken);
                    break;
                default:
                    return CreateJwtContext(false, null, "don't support grant type");
            }

            return context;
        }
    }
}
