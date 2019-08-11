using Microsoft.Extensions.DependencyInjection;

namespace Blog.Jwt.Builder
{
    public interface IJwtBuilder
    {
        IServiceCollection Services { get; }
    }
}
