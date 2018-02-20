using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using AzureADB2C.WebApp.Shared;


namespace AzureADB2C.WebApp
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
            services.Configure<AzureAdB2cOptions>(Configuration.GetSection("RBEIWEBAPP"));
            services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession(options => 
            {
                options.IdleTimeOut = TimeSpan.FromHours(1);
                options.CookieHttpOnly = true;
            });
            services.AddAuthentication(sharedOptions=>sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc();            

            app.UseSession();

            app.UseCookieAuthentication();

            app.UseOpenIdConnectAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
