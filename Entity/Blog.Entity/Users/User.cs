using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Entity.Users
{
    [Table("user")]
    public class User : BaseEntity<string>
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Gender { get; set; }
        public string Mobilephone { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
        public List<Role> Roles { get; set; }
    }
}
