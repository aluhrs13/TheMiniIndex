using AgileObjects.AgileMapper;
using MiniIndex.Core.Utilities;

namespace MiniIndex.Core.Mapping.Configurations
{
    public class StringMappingConfiguration : IMapperConfiguration
    {
        public void Configure(IMapper mapper)
        {
            mapper
                .WhenMapping
                .From<string>()
                .To<string>()
                .Map(x => x.Source.Trim().AsNullIfWhiteSpaceOrEmpty())
                .ToTarget();
        }
    }
}