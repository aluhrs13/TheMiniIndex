using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Search
{
    public class SearchSupportingInfoHandler : IRequestHandler<GetSearchInfoRequest, SearchSupportingInfo>
    {
        public SearchSupportingInfoHandler(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public async Task<SearchSupportingInfo> Handle(GetSearchInfoRequest request, CancellationToken cancellationToken)
        {
            var tags = await _context.Set<Tag>()
                .ToListAsync(cancellationToken);

            return new SearchSupportingInfo
            {
                Tags = tags
            };
        }
    }
}