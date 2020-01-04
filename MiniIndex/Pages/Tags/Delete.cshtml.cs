using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.Tags
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;
        public bool DisableSubmit = false;
        public int MiniCount = 0;

        public DeleteModel(MiniIndex.Models.MiniIndexContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tag Tag { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Moderator"))
            {
                Tag = await _context.Tag.FirstOrDefaultAsync(m => m.ID == id);

                if (Tag == null)
                {
                    return NotFound();
                }

                List<Mini> TaggedMinis = _context.Mini.Where(m => m.MiniTags.Any(mt => mt.TagID == Tag.ID)).ToList();
                MiniCount = TaggedMinis.Count;

                if (MiniCount > 0)
                {
                    DisableSubmit = true;
                }

                return Page();
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Moderator"))
            {
                Tag = await _context.Tag.FindAsync(id);

                if (Tag != null)
                {
                    List<Mini> TaggedMinis = _context.Mini.Where(m => m.MiniTags.Any(mt => mt.TagID == Tag.ID)).ToList();
                    MiniCount = TaggedMinis.Count;

                    if (MiniCount > 0)
                    {
                        return NotFound();
                    }

                    _context.Tag.Remove(Tag);
                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("/Admin/CategoryManager");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
