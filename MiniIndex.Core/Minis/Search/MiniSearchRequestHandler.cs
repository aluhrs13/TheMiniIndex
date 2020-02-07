﻿using MediatR;
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

            if (request.FreeOnly)
            {
                search = search.Where(m => m.Cost == 0);
            }

            foreach (var tag in request.Tags)
            {
                search = search
                    .Where(m => m.MiniTags
                        .Select(x => x.Tag)
                        .Any(t => t.TagName == tag)
                    );
            }

            if (!String.IsNullOrEmpty(request.SearchString))
            {
                var searchTerms = request.SearchString
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .Select(s => s.Trim().ToUpperInvariant())
                    .Where(s => !String.IsNullOrEmpty(s))
                    .ToArray();

                foreach (var term in searchTerms)
                {
                    search = search
                        .Where(m => m.Name.Contains(term))
                        .OrderByDescending(m => m.Name.ToUpper().Equals(term))
                        .ThenByDescending(m => m.Name.ToUpper().StartsWith($"{term} "))
                        .ThenByDescending(m => m.Name.ToUpper().Contains($" {term} "))
                        .ThenByDescending(m => m.Name.ToUpper().EndsWith($" {term}"))
                        .ThenBy(m => m.Name.ToUpper().IndexOf(term))
                        .ThenByDescending(m => m.ApprovedTime)
                        .ThenBy(m => m.Name);
                }
            }

            return await PaginatedList.CreateAsync(search, request.PageInfo);
        }
    }
}