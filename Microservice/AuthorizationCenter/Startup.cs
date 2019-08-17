using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Config;
using AuthorizationCenter.Profiles;
using AuthorizationCenter.Validators;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blog.Repository;
using Blog.Service;
using IdentityServer4.MongoDB.Interfaces;
using IdentityServer4.MongoDB.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region Dapper

            services.Configure<DbOption>(Configuration.GetSection("DbOption"));
            services.AddSingleton<ConnectionFactory>();

            #endregion

            #region IdentityServer4

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<CustomerPasswordValidator>()
                .AddConfigurationStore(options =>
                {
                    options.ConnectionString = Configuration["MongoDB"];
                    options.Database = "AuthCenter";
                })
                .AddOperationalStore(options =>
                {
                    options.ConnectionString = Configuration["MongoDB"];
                    options.Database = "AuthCenter";
                });

            #endregion

            #region Autofac

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            //Use IdentityServer
            InitializeDatabase(app).Wait();
            app.UseIdentityServer();
            app.UseIdentityServerMongoDBTokenCleanup(applicationLifetime);

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        #region Utils

        /// <summary>
        /// InitializeIdentityServerDatabase
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns></returns>
        private async Task InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IConfigurationDbContext>();
                if (!context.Clients.Any())
                {
                    foreach (var client in InMemoryConfiguration.Clients())
                    {
                        await context.AddClient(client.ToEntity());
                    }
                }
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in InMemoryConfiguration.ApiResources())
                    {
                        await context.AddApiResource(resource.ToEntity());
                    }
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var identity in InMemoryConfiguration.IdentityResources())
                    {
                        await context.AddIdentityResource(identity.ToEntity());
                    }
                }

            }
        }

        #endregion

    }
}
