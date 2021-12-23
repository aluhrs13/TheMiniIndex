using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IndexModel(UserManager<IdentityUser> userManager, MiniIndexContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;
        public IList<Starred> Starred { get; set; }

        public async Task OnGetAsync()
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);

            Starred = await _context.Starred.AsNoTracking()
                .Include(s => s.Mini)
                .Include(s => s.User)
                .Where(s => s.User == CurrentUser)
                .ToListAsync();
        }
    }
}