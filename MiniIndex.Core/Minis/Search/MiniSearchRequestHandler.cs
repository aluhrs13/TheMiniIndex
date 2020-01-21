using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Minis.Handlers
{
    public class MiniSearchRequestHandler : IRequestHandler<MiniSearchRequest, IEnumerable<Mini>>
    {
        public MiniSearchRequestHandler(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public async Task<IEnumerable<Mini>> Handle(MiniSearchRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Mini> search = _context
                .Set<Mini>()
                .Include(m => m.Creator)
                .Where(m => m.Status == Status.Approved)
                .OrderByDescending(m => m.ID)
                .Skip(request.PageInfo.PageSize * request.PageInfo.PageIndex)
                .Take(request.PageInfo.PageSize);

            return search;
        }
    }
}