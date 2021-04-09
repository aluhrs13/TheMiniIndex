using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class EditModel : PageModel
    {
        public EditModel(UserManager<IdentityUser> userManager, MiniIndexContext context, TelemetryClient telemetry)
        {
            _userManager = userManager;
            _context = context;
            _telemetry = telemetry;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;
        private readonly TelemetryClient _telemetry;

        public Mini Mini { get; set; }
        public List<Tag> UnusedTags { get; set; }
        public List<Tag> RecommendedTags { get; set; }
        public List<Tag> TargetedCreatureTags { get; set; }
        public string ShowHelp { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string showHelp = "")
        {
            ShowHelp = showHelp;
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            Mini = await _context.Mini
                .Include(m => m.MiniTags)
                    .ThenInclude(mt => mt.Tag)
                .Include(m => m.Creator)
                .Include(m => m.User)
                .Include(m => m.Sources)
                    .ThenInclude(s => s.Site)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Mini == null)
            {
                return NotFound();
            }

            _telemetry.TrackEvent("EditedMini", new Dictionary<string, string> { { "MiniId", Mini.ID.ToString() } });

            UnusedTags = _context
                .Tag
                .AsEnumerable()
                .Except(Mini.MiniTags.Where(m => (m.Status == Status.Approved || m.Status == Status.Pending)).Select(mt => mt.Tag))
                .OrderBy(m => m.Category.ToString())
                .ThenBy(m => m.TagName)
                .ToList();

            //TODO - Remove punctuation?
            //TODO - Get this to work PERFORMANTLY for creatures with more than 2 words in their name
            List<string> nameSplit = Mini.Name.ToUpperInvariant().Split(' ').ToList();

            //Doing this "normally" is an infinite loop.
            int listLength = nameSplit.Count - 1;
            for (int i = 0; i < listLength; i++)
            {
                nameSplit.Add(nameSplit.ElementAt(i) + " " + nameSplit.ElementAt(i+1));
            }

            RecommendedTags = UnusedTags
                .Where(t => nameSplit.Contains(t.TagName.ToUpperInvariant()))
                .ToList();

            //TODO - Things that are creatures to ignore before using this automatically: "Shadow"
            TargetedCreatureTags = RecommendedTags
                .Where(t => t.Category == TagCategory.CreatureName)
                .ToList();

            _telemetry.TrackEvent("SuperSmartTagSuggestions", new Dictionary<string, string> { { "MiniId", Mini.ID.ToString() }, { "MiniName", Mini.Name }, { "SuggestedTags", string.Join(", ", TargetedCreatureTags.Select(t=>t.TagName).ToList()) } });

            return Page();
        }
    }
}