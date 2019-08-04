

namespace Blog.Dto.AuthDtos
{
    public class RequestTokenDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }
}
