using System;
using System.Collections.Generic;
using System.Text;

namespace Blog.Jwt.Dtos
{
    public class RequestTokenDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public GrantType GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
    }

    public enum GrantType
    {
        Password,
        RefreshToken,
        AuthorizationCode,
        Implicit,
        ClientCredentials
    }
}
