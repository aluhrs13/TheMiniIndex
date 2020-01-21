using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Minis.Handlers
{
    public class MiniSearchRequestHandler : IRequestHandler<MiniSearchRequest, PaginatedList<Mini>>
    {
        public MiniSearchRequestHandler(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public async Task<PaginatedList<Mini>> Handle(MiniSearchRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Mini> search = _context
                .Set<Mini>()
                .Include(m => m.Creator)
                .Where(m => m.Status == Status.Approved)
                .OrderByDescending(m => m.ID);

            if (!String.IsNullOrEmpty(request.SearchString))
            {
                search = search.Where(m => m.Name.Contains(request.SearchString));
            }

            return await PaginatedList.CreateAsync(search, request.PageInfo);
        }
    }
}