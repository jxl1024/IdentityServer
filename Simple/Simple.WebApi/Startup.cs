using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.WebApi
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
            // 认证
            services.AddAuthentication(defaultScheme: "Bearer")
            .AddJwtBearer(authenticationScheme: "Bearer", options =>
            {
                // OIDC地址
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            // 授权
            services.AddAuthorization(options => 
            {
                options.AddPolicy("ApiScope", builder => 
                {
                    // 判断用户有没有通过认证
                    builder.RequireAuthenticatedUser();
                    // 添加鉴定API范围的规则
                    builder.RequireClaim("scope", "simple_api");
                });
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Simple.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // 认证中间件  先认证在授权
            app.UseAuthentication();

            // 授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                // 全局授权 应用到所有的API上
                .RequireAuthorization("ApiScope");
            });
        }
    }
}
