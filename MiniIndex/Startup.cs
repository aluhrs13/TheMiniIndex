using Lamar;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
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

            services.AddCors(options =>
            {
                options.AddPolicy("SpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("https://tmireact.azurewebsites.net", "https://theminiindex.com", "https://wwww.theminiindex.com", "http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSwaggerGen();

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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITelemetryInitializer, TelemetryEnrichment>();
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

            app.UseStatusCodePages();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();
            app.UseCors();
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
