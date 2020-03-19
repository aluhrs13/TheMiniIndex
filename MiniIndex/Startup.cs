using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniIndex.Core;
using MiniIndex.Core.Utilities;
using MiniIndex.Persistence;
using MiniIndex.Services;
using WebPWrecover.Services;

namespace MiniIndex
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddLogging(config =>
            {
                config.ClearProviders();

                config.AddConfiguration(Configuration.GetSection("Logging"));
                config.AddApplicationInsights();
            });

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
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorOptions(ConfigureRazor);

            services.AddDbContext<MiniIndexContext>(ConfigureEntityFramework);

            string facebookAppId = Configuration["Authentication:Facebook:AppId"];
            string facebookAppSecret = Configuration["Authentication:Facebook:AppSecret"];

            if (facebookAppId != null && facebookAppSecret != null)
            {
                services.AddAuthentication()
                    .AddFacebook(facebookOptions =>
                    {
                        facebookOptions.AppId = facebookAppId;
                        facebookOptions.AppSecret = facebookAppSecret;
                    });
            }

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightsTelemetryProcessor<AppInsightsFilter>();
            services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));


            services.IncludeRegistry<CoreServices>();
            services.IncludeRegistry<WebAppServices>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
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

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }

        private void ConfigureEntityFramework(DbContextOptionsBuilder options)
        {
            options
                .UseSqlServer(
                    Configuration.GetConnectionString("MiniIndexContext"),
                    sqlServer => sqlServer.EnableRetryOnFailure(3));
        }

        private void ConfigureRazor(RazorViewEngineOptions razor)
        {
            razor.ViewLocationFormats.Add("/{1}/{0}" + RazorViewEngine.ViewExtension);
            razor.ViewLocationFormats.Add("/{1}/Views/{0}" + RazorViewEngine.ViewExtension);
            razor.ViewLocationFormats.Add("/Shared/{0}" + RazorViewEngine.ViewExtension);
        }
    }
}
