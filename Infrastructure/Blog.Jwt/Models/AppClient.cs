
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Jwt.Models
{
    [Table("app_client")]
    public class AppClient
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime ModifyTime { get; set; }
        public string ModifyBy { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ReturnUrl { get; set; }
    }
}
