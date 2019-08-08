
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Jwt.Models
{
    [Table("refresh_token")]
    public class RefreshToken
    {
        public int Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
