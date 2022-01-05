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
    public class AdminModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<Mini> Mini { get; set; }

        public AdminModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {

            Mini = await _context.Mini
                        .Include(m => m.MiniTags)
                            .ThenInclude(mt => mt.Tag)
                        .Where(m => (m.Status == Status.Pending && m.MiniTags.Count>1))
                        .AsNoTracking()
                        .ToListAsync();
        }
    }
}
