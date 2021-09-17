using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace MiniIndex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLamar()
                .ConfigureAppConfiguration((context, config) =>
                {
                    Assembly assembly = typeof(Program).Assembly;
                    string basePath = assembly.Location.Replace(assembly.ManifestModule.Name, String.Empty, StringComparison.Ordinal);

                    config
                        .SetBasePath(basePath)
                        .AddJsonFile("localsettings.json", optional: true);
                })
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost.UseStartup<Startup>();
                });
    }
}