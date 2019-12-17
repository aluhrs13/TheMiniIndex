using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.Minis
{
    public class HelpModel : PageModel
    {
        public HelpModel (
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
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
