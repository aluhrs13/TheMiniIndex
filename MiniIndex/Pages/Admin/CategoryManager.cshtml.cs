using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class CategoryManagerModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndex.Models.MiniIndexContext _context;
        public IList<Tag> Tag { get; set; }

        public CategoryManagerModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                MiniIndex.Models.MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                Tag = await _context.Tag
                    .OrderBy(t=>t.Category)
                    .ToListAsync();
            }
        }
    }
}
