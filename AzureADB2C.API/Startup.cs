using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureADB2C.API
{
    public class Startup
    {
        public static string ReadScope;

        public static string WriteScope;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["RBEIADB2C:tenat"]}/{Configuration["RBEIADB2C:SignUpSignInPolicyId"]}/V2.0/";
                jwtOptions.Audience = Configuration["RBEIADB2C:ClientID"];
                jwtOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = AuthenticationFailed
                };
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ReadScope = Configuration["REBIADB2C:ReadScope"];
            WriteScope = Configuration["REBIADB2C:WriteScope"];
            app.UseMvc();
        }

        private Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            var errorMessage = $"Authentication Failed : { context.Exception.Message}";
            context.Response.ContentLength = errorMessage.Length;
            context.Response.Body.Write(System.Text.Encoding.UTF8.GetBytes(errorMessage), 0, errorMessage.Length);
            return Task.FromResult(0);
        }
    }
}
