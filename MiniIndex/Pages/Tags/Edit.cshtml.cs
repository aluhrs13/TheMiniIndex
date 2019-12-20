using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Tags
{
    [Authorize]
    public class EditModel : PageModel
    {
        public EditModel(
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

                Tag = await _context.Tag.FirstOrDefaultAsync(m => m.ID == id);

                if (Enum.TryParse(category, out TagCategory newCategory))
                {
                    Tag.Category = newCategory;

                    _context.Attach(Tag).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                        return Page();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TagExists(Tag.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Moderator"))
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.Attach(Tag).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(Tag.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return NotFound();
            }
            return RedirectToPage("./Index");
        }

        private bool TagExists(int id)
        {
            return _context.Tag.Any(e => e.ID == id);
        }
    }
}