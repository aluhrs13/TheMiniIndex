using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Core.Tags
{
    public class GetTagsRequestHandler : IRequestHandler<GetTagsRequest, IEnumerable<string>>
    {
        public GetTagsRequestHandler(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public async Task<IEnumerable<string>> Handle(GetTagsRequest request, CancellationToken cancellationToken)
        {
            return await _context.Set<Tag>()
                .Select(t => t.TagName)
                .ToListAsync(cancellationToken);
        }
    }
}