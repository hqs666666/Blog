using Blog.Jwt.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationCenter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            // 配置Jwt
            services.AddJwt(options =>
            {
                var secretKey = Configuration["JwtIssuerOptions:SecretKey"];
                options.Issuer = Configuration["JwtIssuerOptions:Issuer"];
                options.Audience = Configuration["JwtIssuerOptions:Audience"];
                options.ExpireMinutes = int.Parse(Configuration["JwtIssuerOptions:ExpireMinutes"]);
                options.SigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
