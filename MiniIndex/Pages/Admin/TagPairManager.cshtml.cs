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
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class TagPairManagerModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndexContext _context;
        public IList<TagPair> TagPairs { get; set; }

        public TagPairManagerModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                TagPairs = _context.TagPair
                                .Include(tp => tp.Tag1)
                                .Include(tp => tp.Tag2)
                                .ToList();
            }
        }
    }
}
