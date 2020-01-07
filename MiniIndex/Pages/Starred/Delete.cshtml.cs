using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Threading.Tasks;

namespace MiniIndex
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public DeleteModel(UserManager<IdentityUser> userManager, MiniIndexContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string mini { get; set; }

        [BindProperty]
        public Starred Starred { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(mini))
            {
                return Page();
            }

            int MiniID = Int32.Parse(mini);
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);

            Starred = await _context.Starred.FindAsync(MiniID, CurrentUser.Id);

            if (Starred != null)
            {
                _context.Starred.Remove(Starred);
                await _context.SaveChangesAsync();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Starred = await _context.Starred.FindAsync(id);

            if (Starred != null)
            {
                _context.Starred.Remove(Starred);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}