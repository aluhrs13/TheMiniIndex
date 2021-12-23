using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Tags
{
    [Authorize]
    public class ManageModel : PageModel
    {
        public ManageModel(
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

        [BindProperty]
        public Tag Tag { get; set; }

        public IList<TagPair> TagPairs { get; set; }
        public int MiniCount { get; set; }
        public SelectList TagOptions{ get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string category)
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Moderator"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                Tag = await _context.Tag.FindAsync(id);

                MiniCount = _context.Mini.AsNoTracking().Where(m => m.MiniTags.Any(mt => mt.TagID == Tag.ID)).ToList().Count;

                TagPairs = _context.TagPair
                                    .AsNoTracking()
                                    .Where(tp => (tp.Tag1 == Tag || tp.Tag2 == Tag))
                                    .Include(tp => tp.Tag1)
                                    .Include(tp => tp.Tag2)
                                    .ToList();

                IList<Tag> allTags = _context.Tag.AsNoTracking().OrderBy(t => t.TagName).ToList();

                TagOptions = new SelectList(allTags, "ID", "TagName");

                return Page();

            }

            return NotFound();
        }
    }
}