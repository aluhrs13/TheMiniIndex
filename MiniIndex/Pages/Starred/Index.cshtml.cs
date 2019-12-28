using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndex.Models.MiniIndexContext _context;

        public IndexModel(UserManager<IdentityUser> userManager, MiniIndex.Models.MiniIndexContext context)
        {
            _userManager = userManager;

            _context = context;
        }

        public IList<Starred> Starred { get;set; }

        public async Task OnGetAsync()
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);

            Starred = await _context.Starred
                .Include(s => s.Mini)
                .Include(s => s.User)
                .Where(s=>s.User== CurrentUser)
                .ToListAsync();
        }
    }
}
