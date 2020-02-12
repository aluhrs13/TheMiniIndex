using AgileObjects.AgileMapper;
using MiniIndex.Core.Mapping;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Utilities;
using MiniIndex.Minis;
using System;
using System.Linq;

namespace MiniIndex.Mapping
{
    public class MiniSearchRequestMappingConfiguration : IMapperConfiguration
    {
        public void Configure(IMapper mapper)
        {
            mapper
                .WhenMapping
                .From<MiniSearchModel>()
                .To<MiniSearchRequest>()
                .Map(x => x.Source.Tags
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.AsNullIfWhiteSpaceOrEmpty())
                    )
                .To(x => x.Tags);
        }
    }
}