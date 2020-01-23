using AgileObjects.AgileMapper;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Utilities;
using MiniIndex.Minis;
using System;
using System.Linq;

namespace MiniIndex.Core.Mapping.Configurations
{
    public class SearchParameterMappingConfiguration : IMapperConfiguration
    {
        public void Configure(IMapper mapper)
        {
            mapper
                .WhenMapping
                .From<MiniSearchModel>()
                .To<MiniSearchRequest>()
                .Map(x => x.Source.Tags
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().AsNullIfWhiteSpaceOrEmpty())
                    .Where(t => t != null))
                .To(x => x.Tags);
        }
    }
}