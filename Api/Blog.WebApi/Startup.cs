using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blog.Jwt.AuthHelper.Extension;
using Blog.Repository;
using Blog.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions();

            #region Configure Dapper

            services.Configure<DbOption>(Configuration.GetSection("DbOption"));
            services.AddSingleton<ConnectionFactory>();

            #endregion

            #region Configure Jwt

            services.AddJwt(options =>
            {
                var secretKey = Configuration["JwtIssuerOptions:SecretKey"];
                options.Issuer = Configuration["JwtIssuerOptions:Issuer"];
                options.Audience = Configuration["JwtIssuerOptions:Audience"];
                options.ExpireMinutes = int.Parse(Configuration["JwtIssuerOptions:ExpireMinutes"]);
                options.SigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            });

            #endregion

            #region Configure Autofac

            var builder = new ContainerBuilder();//实例化Autofac容器
            builder.Populate(services);

            builder.RegisterAssemblyTypes(typeof(BaseRepository<,>).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();

            var container = builder.Build();
            return new AutofacServiceProvider(container);

            #endregion
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
