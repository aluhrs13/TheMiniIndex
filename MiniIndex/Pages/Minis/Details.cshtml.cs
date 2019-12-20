using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class DetailsModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public Mini Mini { get; set; }
        public List<Tag> UnusedTags { get; set; }
        public List<Tag> MiscTags { get; set; }
        public List<Tag> WordMatchingTags { get; set; }
        public List<Tag> SimilarTags { get; set; }

        public DetailsModel(MiniIndexContext context)
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


            if (User.IsInRole("Moderator"))
            {
                string[] NameSplit = Mini.Name.ToLower().Split(' ');

                WordMatchingTags = _context.Tag
                    .Where(t => NameSplit.Contains(t.TagName.ToLower()))
                    .ToList();

                //TODO - I'm sure someone who knows LINQ could clean this up a lot.
                if (WordMatchingTags.Count > 0)
                {
                    List<Mini> SimilarMinis = _context.Mini
                                .Include(m => m.MiniTags)
                                    .ThenInclude(mt => mt.Tag)
                                    .AsEnumerable()
                                .Where(m => m.MiniTags.Select(mt => mt.Tag).Intersect(WordMatchingTags).Any())
                                .ToList();

                    List<MiniTag> SimilarMiniTags = SimilarMinis.SelectMany(m => m.MiniTags).ToList();
                    SimilarTags = SimilarMiniTags.GroupBy(mt => mt.Tag).Select(grp => grp.First().Tag).Except(Mini.MiniTags.Select(mt => mt.Tag)).ToList();
                }
                else
                {
                    SimilarTags = new List<Tag>();
                }
            }


            if (Mini == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
