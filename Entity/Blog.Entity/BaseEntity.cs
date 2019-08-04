using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Entity
{
    public class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime ModifyTime { get; set; }
        public string ModifyBy { get; set; }

        public BaseEntity()
        {
            CreateTime = DateTime.Now;
            CreateBy = "-1";
            ModifyTime = DateTime.Now;
            ModifyBy = "-1";
        }
    }
}
