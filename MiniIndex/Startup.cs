using Lamar;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
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
using Hangfire;
using Hangfire.SqlServer;
using System;

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

            //Response Compression - https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0
            services.AddResponseCompression();

            services.AddCors(options =>
            {
                options.AddPolicy("SpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("https://tmireact.azurewebsites.net", "https://theminiindex.com", "https://wwww.theminiindex.com", "https://localhost:44410")
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

            services.AddDbContext<MiniIndexContext>(ConfigureEntityFramework);

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MiniIndexContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<IdentityUser, MiniIndexContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorOptions(ConfigureRazor);

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
                
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //Response Compression - https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-5.0
            app.UseResponseCompression();
            app.UseStatusCodePages();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

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
