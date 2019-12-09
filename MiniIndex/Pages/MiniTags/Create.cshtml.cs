using System;
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
    public class CreateModel : PageModel
    {
        public CreateModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        [BindProperty(SupportsGet = true)]
        public string mini { get; set; }

        [BindProperty(SupportsGet = true)]
        public string tag { get; set; }

        [BindProperty(SupportsGet = true)]
        public string tagName { get; set; }

        [BindProperty]
        public MiniTag MiniTag { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(mini))
            {
                return Page();
            }

            if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(tagName))
            {
                return Page();
            }

            MiniTag newMiniTag = new MiniTag();
            Tag newTag = new Tag();

            if (!string.IsNullOrEmpty(tagName))
            {
                //Need to make a new tag, go by name.
                Tag newNewTag = new Tag
                {
                    TagName = tagName
                };

                if (!_context.Tag.Any(m => m.TagName == newNewTag.TagName))
                {
                    _context.Tag.Add(newNewTag);
                    await _context.SaveChangesAsync();
                }

                newTag = _context.Tag.Where(t => t.TagName == tagName)
                            .Include(m => m.MiniTags)
                            .First();

                newMiniTag.TagID = newTag.ID;
            }
            else
            {
                //Don't need to make a new tag, go by ID.
                newTag = _context.Tag.Where(t => t.ID == Int32.Parse(tag))
                                .Include(m => m.MiniTags)
                                .First();

                newMiniTag.TagID = Int32.Parse(tag);
            }
            newMiniTag.Tag = newTag;
            newMiniTag.MiniID = Int32.Parse(mini);

            Mini newMini = _context.Mini.Where(m => m.ID == newMiniTag.MiniID)
                .Include(m => m.Creator)
                .Include(m => m.MiniTags)
                    .ThenInclude(mt => mt.Tag)
                .First();

            newMiniTag.Mini = newMini;
            newTag.MiniTags.Add(newMiniTag);

            if (!newMini.MiniTags.Where(mt => mt.Tag.TagName == newTag.TagName).Any())
            {
                newMini.MiniTags.Add(newMiniTag);
                _context.MiniTag.Add(newMiniTag);
                await _context.SaveChangesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("./Index");
        }
    }
}