using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.MiniTags
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly MiniIndexContext _context;
        [BindProperty]
        public MiniTag MiniTag { get; set; }
        [BindProperty(SupportsGet = true)]
        public string mini { get; set; }
        [BindProperty(SupportsGet = true)]
        public string tag { get; set; }

        public DeleteModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(mini) || String.IsNullOrEmpty(tag))
            {
                return Page();
            }
            
            int TagID = Int32.Parse(tag);
            int MiniID = Int32.Parse(mini);

            MiniTag = await _context.MiniTag.FindAsync(MiniID, TagID);

            _context.MiniTag.Remove(MiniTag);
                await _context.SaveChangesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MiniTag = await _context.MiniTag.FindAsync(id);

            if (MiniTag != null)
            {
                _context.MiniTag.Remove(MiniTag);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
