﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Jwt.Dtos;

namespace Blog.Jwt.AuthHelper.Authentication
{
    public interface IJwtFactory
    {
        Task<JwtToken> GenerateEncodedTokenAsync(string sub, List<Claim> customClaims);
    }
}
