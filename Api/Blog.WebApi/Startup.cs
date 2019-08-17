using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Blog.ApiFramework.Middlewares;
using Blog.Jwt.Extensions;
using Blog.Repository;
using Blog.Service;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;

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
            #region DotNetCore

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            #endregion

            #region AutoMapper

            services.AddAutoMapper(Assembly.Load("Blog.AutoMapperConfig"));

            #endregion

            #region Redis

            var redisString = Configuration["Redis:ConnectionString"];
            var csredis = new CSRedis.CSRedisClient(redisString);
            //初始化 RedisHelper
            RedisHelper.Initialization(csredis);
            //注册mvc分布式缓存
            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));

            #endregion

            #region Dapper

            services.Configure<DbOption>(Configuration.GetSection("DbOption"));
            services.AddSingleton<ConnectionFactory>();

            #endregion

            #region IdentityServer4

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "testapi";
                    options.RequireHttpsMetadata = false;
                });

            //services.AddJwt(options =>
            //{
            //    var secretKey = Configuration["JwtIssuerOptions:SecretKey"];
            //    options.Issuer = Configuration["JwtIssuerOptions:Issuer"];
            //    options.Audience = Configuration["JwtIssuerOptions:Audience"];
            //    options.ExpireMinutes = int.Parse(Configuration["JwtIssuerOptions:ExpireMinutes"]);
            //    options.ConnectionString = Configuration["DbOption:ConnectionString"];
            //    options.SigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            //    options.Secret = Configuration["JwtIssuerOptions:SecretKey"];
            //});

            #endregion

            #region Cors

            services.AddCors(options =>
            {
                options.AddPolicy("blog-api", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            #endregion

            #region Swagger

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "API Docs",
                    Version = "v1",
                });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(basePath, xmlFile);
                options.IncludeXmlComments(xmlPath);

                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                options.AddSecurityRequirement(security);
                options.AddSecurityDefinition("Bearer", new Swashbuckle.AspNetCore.Swagger.ApiKeyScheme
                {
                    Description = "Format: Bearer {access_token}",
                    Name = "Authorization",
                    In = "header",
                });
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("blog-api");
            logger.AddNLog();
            env.ConfigureNLog("NLog.config");
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            app.UseMvc();
        }
    }
}
