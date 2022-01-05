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
            /*
             * 
             * Section 1: No filtering, most recently approved first, then IDs
             * 
             */
            IQueryable<Mini> search = _context
                .Set<Mini>()
                .Include(m => m.Creator)
                .OrderByDescending(m => m.ApprovedTime)
                    .ThenByDescending(m => m.ID);

            /*
             * 
             * Section 2: Basic filtering on creator and Free Only
             * 
             */
            if(request.Creator!=null && request.Creator.ID > 0)
            {
                search = search.Where(m => m.Creator == request.Creator);
            }
            else
            {
                search = search.Where(m => (m.Status == Status.Approved || m.Status == Status.Pending));
            }

            if (request.FreeOnly)
            {
                search = search.Where(m => m.Cost == 0);
            }

            //TODO: Move this section lower
            //TODO: Tag status needs to be approved lol
            /*
             * 
             * Section 3: Only Minis with the given tag(s)
             * 
             */
            foreach (var tag in request.Tags)
            {
                search = search
                    .Where(m => m.MiniTags
                        .Where(x => x.Status == Status.Approved)
                        .Select(x => x.Tag)
                        .Any(t => t.TagName == tag)
                    );
            }

            /*
             * 
             * Section 4: Analyze the search terms
             * 
             */
            //TODO: If SearchString can be deconstructed into tags, do it and then do a tag search (swap this section and the one above).

            /*
             * 
             * Section 5: Text search
             * 
             */

            /*
             * 
             * Section 6: Sort each result set
             * 
             */

            /*
             * 
             * Section 7: Concat the two sets of results
             * 
             */


            if (!String.IsNullOrEmpty(request.SearchString))
            {
                //TODO: Also include pairs of words here.
                string[] searchTerms = request.SearchString
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .Select(s => s.Trim().ToUpperInvariant())
                    .Where(s => !String.IsNullOrEmpty(s))
                    .ToArray();

                IQueryable<Mini> tagSearch = search;

                //TODO: Move sort to end
                foreach (string term in searchTerms)
                {
                    search = search
                        .Where(m => m.Name.Contains(term))
                        .OrderByDescending(m => m.Name.ToUpper().Equals(term))              // match where the term *is* the model name
                        .ThenByDescending(m => m.Name.ToUpper().StartsWith($"{term} "))     // \
                        .ThenByDescending(m => m.Name.ToUpper().Contains($" {term} "))      // -- these three lines do whole-word matching; there's a more concise way, but not if we want it to translate to SQL
                        .ThenByDescending(m => m.Name.ToUpper().EndsWith($" {term}"))       // /
                        .ThenBy(m => m.Name.ToUpper().IndexOf(term))                        // the earlier our term appears in the name, the more likely it is to be relevant (particularly with substring matches)
                        .ThenByDescending(m => m.ApprovedTime)
                        .ThenBy(m => m.Name);

                    //Lots of people are trying to search only with one or two words that really should just be tag searches
                    //If there's no tags passed, this try to do a tag search in addition to the text-based search
                    if(request.Tags.Count() == 0)
                    {
                        tagSearch = tagSearch
                            .Where(m => m.MiniTags
                                .Where(x => x.Status == Status.Approved)
                                .Select(x => x.Tag)
                                .Any(t => t.TagName == term)
                            );
                    }
                }

                if (request.Tags.Count() == 0)
                {
                    search = tagSearch.Union(search);
                }
            }

            //TODO: Just hacking this in for now. We're double sorting...
            if(request.SortType == "newest")
            {
                search = search.OrderByDescending(m => m.ID);
            }

            return await PaginatedList.CreateAsync(search, request.PageInfo);
        }
    }
}