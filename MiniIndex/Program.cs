using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
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

                    if (context.HostingEnvironment.IsProduction())
                    {
                        IConfigurationRoot builtConfig = config.Build();

                        AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                        KeyVaultClient keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        config.AddAzureKeyVault(
                            $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost.UseStartup<Startup>();
                });
    }
}