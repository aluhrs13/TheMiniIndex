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
        public IList<Mini> RelatedMinis { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);
            RelatedMinis = new List<Mini> { };

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

            //Find Related Minis. This is fairly hacky right now, but good for common cases.
            if (Mini.MiniTags.Where(mt => mt.Status == Status.Approved).Any())
            {
                //First - Creature Name, the most straight-forward.
                IEnumerable<Tag> creatureTags = Mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.CreatureName).Select(mt => mt.Tag);

                if (creatureTags.Any())
                {
                    foreach (Tag creatureTag in creatureTags)
                    {
                        List<Mini> creatureRelatedMinis = _context.Mini
                                                            .Include(m => m.Creator)
                                                            .Where(m => m.MiniTags.Where(mt => mt.Status == Status.Approved).Any(mt => mt.Tag == creatureTag))
                                                            .ToList();

                        RelatedMinis = RelatedMinis.Concat(creatureRelatedMinis).ToList();
                    }
                }

                //Second - Race + Class, pretty solid attempt but doesn't account for Gender which is mostly fine.
                IEnumerable<Tag> classTags = Mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.Class).Select(mt => mt.Tag);
                IEnumerable<Tag> raceTags = Mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.Race).Select(mt => mt.Tag);

                if (classTags.Any() && raceTags.Any())
                {
                    foreach (Tag classTag in classTags)
                    {
                        IEnumerable<Mini> classRelatedMinis = _context.Mini
                                                                    .Include(m => m.Creator)
                                                                    .Include(m => m.MiniTags)
                                                                        .ThenInclude(mt => mt.Tag)
                                                                    .Where(m => m.MiniTags.Select(mt => mt.Tag).Contains(classTag));

                        foreach (Tag raceTag in raceTags)
                        {
                            List<Mini> raceClassRelatedMinis = classRelatedMinis.Where(m => m.MiniTags.Select(mt => mt.Tag).Contains(raceTag)).ToList();


                            RelatedMinis = RelatedMinis.Concat(raceClassRelatedMinis).ToList();
                        }
                    }
                }
            }

            //Finally, include things by CreatureType or this creator.
            //Only do this if we don't have any related already.
            if (RelatedMinis.Count == 0)
            {
                IEnumerable<Tag> creatureTypeTags = Mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.CreatureType).Select(mt => mt.Tag);

                if (creatureTypeTags.Any())
                {
                    foreach (Tag creatureTypeTag in creatureTypeTags)
                    {
                        List<Mini> creatureRelatedMinis = _context.Mini
                                                            .Include(m => m.Creator)
                                                            .Where(m => m.MiniTags.Where(mt => mt.Status == Status.Approved).Any(mt => mt.Tag == creatureTypeTag))
                                                            .ToList();

                        RelatedMinis = RelatedMinis.Concat(creatureRelatedMinis).ToList();
                    }
                }

                if (RelatedMinis.Count == 0)
                {
                    RelatedMinis = RelatedMinis.Concat(_context.Mini.Where(m => m.Creator == Mini.Creator).ToList()).ToList();
                }
            }

            //Filter down just to approved and recent Minis.
            RelatedMinis = RelatedMinis.Distinct()
                                .Where(m => m.Status == Status.Approved)
                                .OrderByDescending(m => m.ApprovedTime).ToList();

            if (User.Identity.IsAuthenticated)
            {
                IsStarred = _context.Set<Models.Starred>().Any(m => m.Mini.ID == Mini.ID && m.User.Id == CurrentUser.Id);
            }
            else
            {
                IsStarred = false;
            }

            return Page();
        }
    }
}