using System;
using System.Net;
using System.Threading.Tasks;
using Blog.Jwt.AuthHelper.Authentication;
using Blog.Jwt.Dtos;
using Blog.Jwt.Extensions;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppTokenService(IJwtFactory jwtFactory,
            JwtIssuerOptions jwtOptions,
            IUserValidatorService userValidatorService,
            IHttpContextAccessor httpContextAccessor)
        {
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions;
            _userValidatorService = userValidatorService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Utils

        private async Task RefreshTokenAsync(string token)
        {
            using (var conn = new MySqlConnection(_jwtOptions.ConnectionString))
            {
                var sql = @"select * from `refresh_token` where token=@token and expireTime>=@now";
                var tokenInfo = await conn.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { token, DateTime.Now });
                if (tokenInfo == null)
                {
                    await _httpContextAccessor.HttpContext.Response.WriteJsonAsync(new ErrorResultDto("the refresh token is expire"), HttpStatusCode.BadRequest);
                    return;
                }

                var sql1 = @"select Id, Name from `user` where id=@id";
                var user = await conn.QueryFirstOrDefaultAsync<JwtUser>(sql1, new { id = tokenInfo.UserId });
                if (user == null)
                {
                    await _httpContextAccessor.HttpContext.Response.WriteJsonAsync(new ErrorResultDto("the refresh token is not found user"), HttpStatusCode.BadRequest);
                    return;
                }

                await PasswordAsync(user.Name, user.Password);
            }
        }

        private async Task PasswordAsync(string name,string password)
        {
            var context = new UserValidatorContext
            {
                UserName = name, Password = password
            };
            await _userValidatorService.ValidateAsync(context);
            if (!context.Result)
            {
                await _httpContextAccessor.HttpContext.Response.WriteJsonAsync(new ErrorResultDto(context.Message), HttpStatusCode.BadRequest);
                return;
            }

            //var token = await _jwtFactory.GenerateEncodedTokenAsync(context.SubjectId, context.Claims);
            var token = await _jwtFactory.GenerateEncodedTokenForIdentityModelAsync(context.SubjectId, context.Claims);
            await _httpContextAccessor.HttpContext.Response.WriteJsonAsync(token);
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

        #endregion

        public async Task GenerateTokenAsync(RequestTokenDto dto)
        {
            if (!await ExistClientAsync(dto.ClientId, dto.ClientSecret))
            {
                await _httpContextAccessor.HttpContext.Response.WriteJsonAsync(new ErrorResultDto("client is not exist"), HttpStatusCode.BadRequest);
                return;
            }

            switch (dto.GrantType)
            {
                case GrantType.Password:
                    await PasswordAsync(dto.UserName, dto.Password);
                    break;
                case GrantType.RefreshToken:
                    await RefreshTokenAsync(dto.RefreshToken);
                    break;
                default:
                    await _httpContextAccessor.HttpContext.Response.WriteJsonAsync(new ErrorResultDto("don't support grant type"), HttpStatusCode.BadRequest);
                    return;
            }
        }
    }
}
