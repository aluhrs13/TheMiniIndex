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
    public class TagManagerModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndexContext _context;
        public List<TagListItem> Tag { get; set; }

        public TagManagerModel(
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
                Tag = await _context.Tag.TagWith("Tag Manager")
                    .AsNoTracking()
                    .Select(g => new TagListItem
                    {
                        Tag = g,
                        Count = g.MiniTags.Count
                    })
                    .OrderByDescending(m => m.Count)
                    .ToListAsync();
            }
        }
    }

    public class TagListItem
    {
        public Tag Tag { get; set; }
        public int Count { get; set; }
    }
}