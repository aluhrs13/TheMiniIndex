using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniIndex.Models;

namespace MiniIndex
{
    public class CreateModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string mini { get; set; }

        public CreateModel(UserManager<IdentityUser> userManager, MiniIndex.Models.MiniIndexContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["MiniID"] = new SelectList(_context.Mini, "ID", "ID");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");

            if (!ModelState.IsValid || String.IsNullOrEmpty(mini))
            {
                return Page();
            }

            Starred newStarred = new Starred();
            newStarred.Mini = _context.Mini.Where(m => m.ID == Int32.Parse(mini)).First();
            newStarred.User = await _userManager.GetUserAsync(User);

            _context.Starred.Add(newStarred);
            await _context.SaveChangesAsync();

            return Page();
        }

        [BindProperty]
        public Starred Starred { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("./Index");
        }
    }
}
