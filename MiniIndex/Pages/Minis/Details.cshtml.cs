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
        public bool IsStarred { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            Mini = await _context.Mini
                .AsNoTracking().TagWith("Viewwed Mini")
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
                IsStarred = _context.Starred.AsNoTracking().Any(m => m.Mini.ID == Mini.ID && m.User.Id == CurrentUser.Id);
            }
            else
            {
                IsStarred = false;
            }

            return Page();
        }
    }
}