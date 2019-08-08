
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Entity.Users
{
    [Table("user_role")]
    public class UserRole : BaseEntity<string>
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
