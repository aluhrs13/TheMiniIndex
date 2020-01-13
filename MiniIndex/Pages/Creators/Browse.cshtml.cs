using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Creators
{
    public class BrowseModel : PageModel
    {
        public BrowseModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;
        public Dictionary<Creator, int> CreatorCounts { get; set; }

        public async Task OnGetAsync()
        {
            var countQuery = await _context.Set<Mini>()
                .Include(m => m.Creator).ThenInclude(c => c.Sites)
                .Select(m => m.Creator)
                .ToListAsync();

            CreatorCounts = countQuery
                .GroupBy(x => x)
                .ToDictionary(k => k.Key, v => v.Count());
        }
    }
}