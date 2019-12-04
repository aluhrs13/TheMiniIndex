using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebPWrecover.Services;
using MiniIndex.Services;

namespace MiniIndex
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MiniIndexContext>();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            ;

            services.AddDbContext<MiniIndexContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("MiniIndexContext")));

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
            ;

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddApplicationInsightsTelemetry();


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //TODO - Done as part of move to .NET Core 3 - Is it right?
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

        }
    }
}
