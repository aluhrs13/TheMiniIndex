using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class UnindexedModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<Mini> Mini { get; set; }

        public UnindexedModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Mini = await _context.Mini
                        .Where(m => m.Status == Status.Unindexed)
                        .AsNoTracking()
                        .ToListAsync();
        }
    }
}