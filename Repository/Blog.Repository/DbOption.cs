using Blog.Common;

namespace Blog.Repository
{
    public class DbOption
    {
        public DatabaseType DatabaseType { get; set; }
        public string ConnectionString { get; set; }
    }
}
