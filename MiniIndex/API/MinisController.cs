using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Core.Submissions;
using MiniIndex.Minis;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.API
{
    [Route("api/[controller]")]
    [EnableCors("SpecificOrigins")]
    [ApiController]
    public class MinisController : ControllerBase
    {
        public MinisController(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context,
                IMapper mapper,
                IMediator mediator,
                TelemetryClient telemetry,
                IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _telemetry = telemetry;
            _configuration = configuration;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;
        private readonly IConfiguration _configuration;

        // GET: api/<MinisController>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(
            [FromQuery] MiniSearchModel search = null,
            [FromQuery] int pageSize = 21,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int creatorId = 0)
        {
            //TODO: Move creator into MiniSearchModel?
            Creator creatorInfo = new Creator();

            if (creatorId > 0)
            {
                creatorInfo = await _context.Mini.AsNoTracking().TagWith("Minis API Search")
                                .Include(m => m.Creator)
                                .Select(m => m.Creator)
                                .FirstOrDefaultAsync(c => c.ID == creatorId);
            }

            PageInfo pagingInfo = new PageInfo(pageSize, pageIndex);

            MiniSearchRequest searchRequest = new MiniSearchRequest { PageInfo = pagingInfo, Creator = creatorInfo };
            _mapper.Map(search).Over(searchRequest);
            PaginatedList<Mini> searchResult = await _mediator.Send(searchRequest);

            _telemetry.TrackEvent("MiniSearchAPI", new Dictionary<string, string> {
                                                            { "SearchString", searchRequest.SearchString },
                                                            { "Tags", String.Join(",", searchRequest.Tags) },
                                                            { "FreeOnly", searchRequest.FreeOnly.ToString() },
                                                            { "HadResults", searchResult.Count>0 ? "True" : "False" },
                                                            { "PageIndex", searchRequest.PageInfo.PageIndex.ToString()},
                                                            { "SortType", searchRequest.SortType}
                                                        });
            return Ok(
                searchResult.Select(
                    m => new
                        {
                            ID = m.ID,
                            Name = m.Name,
                            Status = m.Status.ToString(),
                            Creator = new { name = m.Creator.Name, id = m.Creator.ID },
                            Thumbnail = m.Thumbnail.Replace("miniindex.blob.core.windows.net", _configuration["CDNURL"] + ".azureedge.net"),
                            LinuxTime = m.ApprovedLinuxTime()
                        }
                    )
                );
        }

        // GET api/<MinisController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //TODO: Remove Link, propagate sources
            Mini mini = await _context.Mini.AsNoTracking().TagWith("Minis API View")
                                        .Include(m => m.Creator)
                                        .Include(m => m.MiniTags)
                                            .ThenInclude(mt=>mt.Tag)
                                        .FirstOrDefaultAsync(m => m.ID == id);

            _telemetry.TrackEvent("ViewedMiniAPI", new Dictionary<string, string> { { "MiniId", mini.ID.ToString() } });

            if (mini == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(mini);
            }
        }

        [HttpGet("{id}/Redirect")]
        public async Task<IActionResult> Redirect(int id)
        {
            Mini mini = await _context.Mini.AsNoTracking().TagWith("Minis API Redirect")
                                    .Include(m => m.Sources)
                                    .Include(m => m.Creator)
                                    .FirstOrDefaultAsync(m => m.ID == id);

            if (mini == null)
            {
                return NotFound();
            }

            MiniSourceSite Source = mini.Sources.FirstOrDefault();

            if (Source != null)
            {
                _telemetry.TrackEvent("MiniRedirect", new Dictionary<string, string> {
                        { "TargetHost", Source.Link.Host },
                        { "MiniID", id.ToString() },
                        { "CreatorID", mini.Creator.ID.ToString() }
                    });

                return Redirect(Source.Link.ToString());
            }
            else
            {
                _telemetry.TrackEvent("MiniRedirect", new Dictionary<string, string> {
                        { "TargetHost", new Uri(mini.Link).Host },
                        { "MiniID", id.ToString() },
                        { "CreatorID", mini.Creator.ID.ToString() }
                    });

                return Redirect(mini.Link.ToString());
            }
        }

        //TODO: Perf on this isn't good.
        [HttpGet("{id}/Related")]
        public async Task<IActionResult> Related(int id)
        {
            List<Mini> RelatedMinis = new List<Mini> { };
            Mini mini = await _context.Mini.TagWith("Minis API Related #1")
                            .Include(m => m.Creator)
                            .Include(m => m.MiniTags)
                                .ThenInclude(mt => mt.Tag)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(m => m.ID == id);

            //Find Related Minis. This is fairly hacky right now, but good for common cases.
            if (mini.MiniTags.Where(mt => mt.Status == Status.Approved).Any())
            {
                //First - Creature Name, the most straight-forward.
                IEnumerable<Tag> creatureTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.CreatureName).Select(mt => mt.Tag);

                if (creatureTags.Any())
                {
                    foreach (Tag creatureTag in creatureTags)
                    {
                        List<Mini> creatureRelatedMinis = _context.Mini.AsNoTracking().TagWith("Minis API Related #2")
                                                            .Include(m => m.Creator)
                                                            .Where(m => m.MiniTags.Where(mt => mt.Status == Status.Approved).Any(mt => mt.Tag == creatureTag))
                                                            .ToList();

                        RelatedMinis = RelatedMinis.Concat(creatureRelatedMinis).ToList();
                    }
                }

                //Second - Race + Class, pretty solid attempt but doesn't account for Gender which is mostly fine.
                IEnumerable<Tag> classTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.Class).Select(mt => mt.Tag);
                IEnumerable<Tag> raceTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.Race).Select(mt => mt.Tag);

                if (classTags.Any() && raceTags.Any())
                {
                    foreach (Tag classTag in classTags)
                    {
                        IEnumerable<Mini> classRelatedMinis = _context.Mini.AsNoTracking().TagWith("Minis API Related #3")
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
                IEnumerable<Tag> creatureTypeTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.CreatureType).Select(mt => mt.Tag);

                if (creatureTypeTags.Any())
                {
                    foreach (Tag creatureTypeTag in creatureTypeTags)
                    {
                        List<Mini> creatureRelatedMinis = _context.Mini.AsNoTracking().TagWith("Minis API Related #5")
                                                            .Include(m => m.Creator)
                                                            .Where(m => m.MiniTags.Where(mt => mt.Status == Status.Approved).Any(mt => mt.Tag == creatureTypeTag))
                                                            .ToList();

                        RelatedMinis = RelatedMinis.Concat(creatureRelatedMinis).ToList();
                    }
                }

                if (RelatedMinis.Count == 0)
                {
                    RelatedMinis = _context.Mini.AsNoTracking().TagWith("Minis API Related #5")
                                                .Include(m => m.Creator)                                            
                                                .Where(m => m.Creator == mini.Creator)
                                                .ToList();
                }
            }

            //Filter down just to approved and recent Minis.
            RelatedMinis = RelatedMinis.Distinct()
                                .Where(m => m.Status == Status.Approved)
                                .Where(m => m.ID != mini.ID)
                                .OrderByDescending(m => m.ApprovedTime)
                                .Take(6)
                                .ToList();
            
            return Ok(RelatedMinis);
        }

        [HttpGet("{id}/Tags")]
        public async Task<IActionResult> Tags(int id)
        {
            Mini selectedMini = await _context.Mini.AsNoTracking()
                            .Include(m => m.MiniTags)
                            .ThenInclude(mt => mt.Tag)
                            .FirstOrDefaultAsync(c => c.ID == id);

            if (selectedMini == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(
                    selectedMini.MiniTags.Select(
                        mt => new {
                            Status = mt.Status.ToString(),
                            mt.Tag.TagName,
                            mt.Tag.ID,
                            Category = mt.Tag.Category.ToString()
                        }
                    ).OrderBy(mt=>mt.Status).ThenBy(mt=>mt.TagName)
                );
            }
        }

        //TODO: Probably should [Authorize] this, but enabling programmatic case
        // PATCH api/<MinisController>
        [HttpPatch("{id}/FixThumbnail")]
        [Authorize]
        public async Task<IActionResult> FixThumbnail(int id)
        {
            Mini currentMini = await _context.Mini.FindAsync(id);
            Mini mini = await _mediator.Send(new MiniSubmissionRequest(new Uri(currentMini.Link), null, true));

            if (mini != null)
            {
                return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
            }
            else
            {
                return new StatusCodeResult(501);
            }
        }
        // POST api/<MinisController>

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string url)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                user = await _userManager.Users.FirstAsync(u => u.Email == "admin@theminiindex.com");
            }

            Mini mini = await _mediator.Send(new MiniSubmissionRequest(new Uri(url), user, false));

            if (mini != null)
            {
                return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
            }
            else
            {
                return new StatusCodeResult(501);
            }
        }

        // PATCH api/<MinisController>/
        [HttpPatch]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Patch([FromBody] Mini value)
        {
            Mini Mini = await _context.Mini
                            .Include(m => m.MiniTags)
                            .FirstOrDefaultAsync(m => m.ID == value.ID);

            if (Mini == null)
            {
                return NotFound();
            }

            if (value.Status == Status.Approved)
            {
                Mini.Status = Status.Approved;
                Mini.ApprovedTime = DateTime.Now;
                foreach (MiniTag mt in Mini.MiniTags)
                {
                    if (mt.Status == Status.Pending)
                    {
                        mt.Status = Status.Approved;
                        mt.ApprovedTime = DateTime.Now;
                        mt.LastModifiedTime = DateTime.Now;
                        _context.Attach(mt).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                Mini.Status = value.Status;
            }

            _context.Attach(Mini).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(Mini.ID);
        }
    }
}
