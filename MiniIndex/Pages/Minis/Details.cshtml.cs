using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class DetailsModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;
        public Mini Mini { get; set; }
        public List<Tag> UnusedTags { get; set; }
        public List<Tag> MiscTags { get; set; }

        public DetailsModel(MiniIndex.Models.MiniIndexContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Mini = await _context.Mini
                .Include(m => m.MiniTags)
                    .ThenInclude(mt => mt.Tag)
                .Include(m => m.Creator)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);

            UnusedTags = _context
                .Tag
                .AsEnumerable()
                .Except(Mini.MiniTags.Select(mt => mt.Tag))
                .OrderBy(m => m.Category.ToString())
                .ThenBy(m => m.TagName)
                .ToList();

            if (Mini == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
