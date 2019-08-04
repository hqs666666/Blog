using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Entity.Users
{
    [Table("user_profile")]
    public class UserProfile : BaseEntity<string>
    {
        public string Name { get; set; }
        public string HeadImg { get; set; }
        public int Gender { get; set; }
        public string Mobilephone { get; set; }
        public string Email { get; set; }
    }
}
