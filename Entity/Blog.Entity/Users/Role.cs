using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Entity.Users
{
    [Table("role")]
    public class Role : BaseEntity<string>
    {
        public string Name { get; set; }
        public string DispalyName { get; set; }
    }
}
