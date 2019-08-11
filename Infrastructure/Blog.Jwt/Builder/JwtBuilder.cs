using System;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Jwt.Builder
{
    public class JwtBuilder : IJwtBuilder
    {

        public JwtBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}
