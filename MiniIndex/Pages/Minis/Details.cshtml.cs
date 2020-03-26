using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class DetailsModel : PageModel
    {
        public DetailsModel(UserManager<IdentityUser> userManager, MiniIndexContext context, TelemetryClient telemetry)
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
        public bool IsStarred { get; set; }
        public string ShowHelp { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string showHelp="")
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

            _telemetry.TrackEvent("ViewedMini", new Dictionary<string, string> { { "MiniId", Mini.ID.ToString() } });

            if (User.Identity.IsAuthenticated)
            {
                IsStarred = _context.Set<Models.Starred>().Any(m => m.Mini.ID == Mini.ID && m.User.Id == CurrentUser.Id);
            }
            else
            {
                IsStarred = false;
            }

            //TODO - 99% confident this is wrong.
            UnusedTags = _context
                .Tag
                .AsEnumerable()
                .Except(Mini.MiniTags.Where(mt=>mt.Status==Status.Pending).Select(mt => mt.Tag))
                .OrderBy(m => m.Category.ToString())
                .ThenBy(m => m.TagName)
                .ToList();


            string[] nameSplit = Mini.Name.ToUpperInvariant().Split(' ');

            RecommendedTags = UnusedTags
                .Where(t => nameSplit.Contains(t.TagName.ToUpperInvariant()))
                .ToList();

            return Page();
        }
    }
}