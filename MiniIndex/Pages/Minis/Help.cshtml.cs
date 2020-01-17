using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class HelpModel : PageModel
    {
        public HelpModel(
            UserManager<IdentityUser> userManager,
            MiniIndexContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;

        public IList<Mini> Mini { get; set; }

        public async Task OnGetAsync()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            IQueryable<Mini> minis = from m in _context.Mini select m;

            Mini = await _context.Mini
                        .Include(m => m.MiniTags)
                        .Where(m => m.Status == Status.Pending)
                        .OrderBy(m => m.MiniTags.Count())
                        .AsNoTracking()
                        .ToListAsync();
        }
    }
}