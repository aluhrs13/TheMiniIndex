using System;
using Lamar;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MiniIndex.Core.Http;
using MiniIndex.Core.Minis.Parsers.Thingiverse;

namespace MiniIndex.Core
{
    public class CoreServices : ServiceRegistry
    {
        public CoreServices()
        {
            RegisterMediatrTypes();

            this.AddHttpClient<ThingiverseClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(10))
                .ApplyResiliencePolicies();
        }

        private void RegisterMediatrTypes()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();

                scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });

            For<IMediator>()
                .Use<Mediator>()
                .Transient();

            For<ServiceFactory>()
              .Use(ctx => ctx.GetInstance);
        }
    }
}