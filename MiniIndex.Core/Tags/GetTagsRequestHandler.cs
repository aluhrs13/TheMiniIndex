using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
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
            IQueryable<Tag> tagsQuery = _context.Set<Tag>();

            if (!String.IsNullOrEmpty(request.SearchTerm.Trim()))
            {
                tagsQuery = tagsQuery
                    .Where(m => m.TagName.Contains(request.SearchTerm))
                    .OrderByDescending(m => m.TagName.ToUpper().Equals(request.SearchTerm))
                    .ThenByDescending(m => m.TagName.ToUpper().StartsWith($"{request.SearchTerm} "))
                    .ThenByDescending(m => m.TagName.ToUpper().Contains($" {request.SearchTerm} "))
                    .ThenByDescending(m => m.TagName.ToUpper().EndsWith($" {request.SearchTerm}"))
                    .ThenBy(m => m.TagName.ToUpper().IndexOf(request.SearchTerm))
                    .ThenBy(m => m.TagName);
            }

            var tags = await tagsQuery
                .Select(t => t.TagName)
                .ToListAsync();

            return tags;
        }
    }
}