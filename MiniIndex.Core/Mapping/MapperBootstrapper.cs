using AgileObjects.AgileMapper;
using System;
using System.Collections.Generic;

namespace MiniIndex.Core.Mapping
{
    public class MapperBootstrapper
    {
        public MapperBootstrapper(IEnumerable<IMapperConfiguration> configurations)
        {
            _configurations = configurations;
        }

        private readonly IEnumerable<IMapperConfiguration> _configurations;

        public IMapper CreateMapper(IServiceProvider serviceProvider)
        {
            IMapper mapper = Mapper.CreateNew();
            mapper.WhenMapping.UseServiceProvider(serviceProvider);

            foreach (IMapperConfiguration config in _configurations)
            {
                config.Configure(mapper);
            }

            return mapper;
        }
    }
}