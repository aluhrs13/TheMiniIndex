using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniIndex.Models;
using Microsoft.AspNetCore.Authorization;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Tags
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly MiniIndexContext _context;

        public CreateModel(MiniIndexContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Tag Tag { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Tag.Add(Tag);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}