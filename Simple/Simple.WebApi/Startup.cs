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
            // ��֤
            services.AddAuthentication(defaultScheme: "Bearer")
            .AddJwtBearer(authenticationScheme: "Bearer", options =>
            {
                // OIDC��ַ
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            // ��Ȩ
            services.AddAuthorization(options => 
            {
                options.AddPolicy("ApiScope", builder => 
                {
                    // �ж��û���û��ͨ����֤
                    builder.RequireAuthenticatedUser();
                    // ��Ӽ���API��Χ�Ĺ���
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

            // ��֤�м��  ����֤����Ȩ
            app.UseAuthentication();

            // ��Ȩ�м��
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                // ȫ����Ȩ Ӧ�õ����е�API��
                .RequireAuthorization("ApiScope");
            });
        }
    }
}
