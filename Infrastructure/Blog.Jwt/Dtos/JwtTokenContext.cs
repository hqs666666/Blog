
namespace Blog.Jwt.Dtos
{
    public class JwtTokenContext
    {
        public JwtToken Token { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
