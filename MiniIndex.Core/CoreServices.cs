using AgileObjects.AgileMapper;
using Lamar;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MiniIndex.Core.Http;
using MiniIndex.Core.Mapping;
using MiniIndex.Core.Minis.Parsers;
using MiniIndex.Core.Minis.Parsers.Thingiverse;
using System;

namespace MiniIndex.Core
{
    public class CoreServices : ServiceRegistry
    {
        public CoreServices()
        {
            RegisterMediatrTypes();
            RegisterMapperTypes();

            Scan(scan =>
            {
                scan.TheCallingAssembly();

                scan.AddAllTypesOf<IParser>();
            });

            this.AddHttpClient<ThingiverseClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(10))
                .ApplyResiliencePolicies();
        }

        private void RegisterMapperTypes()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();

                scan.AddAllTypesOf<IMapperConfiguration>();
            });

            For<MapperBootstrapper>()
                .Use<MapperBootstrapper>()
                .Singleton();

            For<IMapper>()
                .Use(services => services.GetInstance<MapperBootstrapper>().CreateMapper(services))
                .Singleton();
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