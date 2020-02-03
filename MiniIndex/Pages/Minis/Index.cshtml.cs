using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class IndexModel : PageModel
    {
        public IndexModel(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context,
                TelemetryClient telemetry)
        {
            _userManager = userManager;
            _context = context;
            _telemetry = telemetry;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;
        private readonly TelemetryClient _telemetry;

        [BindProperty(SupportsGet = true)]
        public string[] SearchString { get; set; }

        public SelectList TagsList { get; set; }
        public PaginatedList<Mini> Mini { get; set; }
        public List<Tag> Tags { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool FreeOnly { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            IQueryable<Mini> minis = _context.Mini;

            if (FreeOnly)
            {
                minis = minis.Where(m => m.Cost == 0);
            }

            if (SearchString is null || !SearchString.Any())
            {
                SearchString = HttpContext.Request.Query
                    .Where(x => x.Key.Contains("SearchString"))
                    .SelectMany(x => x.Value)
                    .Where(x => !String.IsNullOrWhiteSpace(x))
                    .ToArray();
            }

            if (SearchString.Any())
            {
                foreach (string tag in SearchString)
                {
                    minis = minis.Where(t => t.MiniTags.Any(mt => mt.Tag.TagName == tag));

                    if (pageIndex == null || pageIndex == 1)
                    {
                        _telemetry.TrackEvent("SearchedMinis", new Dictionary<string, string> { { "SearchString", tag } });
                    }
                }
            }
            else
            {
                _telemetry.TrackEvent("SearchedMinis", new Dictionary<string, string> { { "SearchString", "" } });
            }

            //If the user is logged in, we should show them their submitted minis too even if they aren't approved.
            int pageSize = 21;
            if (user != null)
            {
                minis = minis.Where(m => m.Status == Status.Approved || m.User == user);
            }
            else
            {
                minis = minis.Where(m => m.Status == Status.Approved);
            }

            int miniCount = await minis.CountAsync();

            minis = minis
                .Include(m => m.Creator)
                .Include(m => m.Sources)
                    .ThenInclude(s => s.Site)
                .OrderByDescending(m => m.ID)
                .AsNoTracking();

            Mini = await PaginatedList.CreateAsync(minis, miniCount, pageIndex ?? 1, pageSize);

            List<Tag> allTags = await _context
                .Tag
                .AsNoTracking()
                .ToListAsync();

            Tags = allTags
                .OrderBy(m => m.Category.ToString())
                .ThenBy(m => m.TagName)
                .ToList();

            TagsList = new SelectList(allTags, "TagName", "TagName", null, "Category");
        }
    }
}