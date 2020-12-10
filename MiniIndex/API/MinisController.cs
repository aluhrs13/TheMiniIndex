using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Core.Submissions;
using MiniIndex.Minis;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/minis")]
    public class MinisController : Controller
    {
        public MinisController(
                MiniIndexContext context,
                IMapper mapper,
                IMediator mediator,
                IConfiguration configuration,
                TelemetryClient telemetry)
        {
            _apiKey = configuration["AutoCreateKey"];
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _telemetry = telemetry;
        }

        private readonly string _apiKey;
        private readonly MiniIndexContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;

        [EnableCors("SpecificOrigins")]
        [HttpGet("view")]
        public async Task<IActionResult> GetMiniAPI(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            List<Mini> RelatedMinis = new List<Mini> { };
            Mini mini = await _context.Mini
                                        .Include(m => m.MiniTags)
                                            .ThenInclude(mt => mt.Tag)
                                        .Include(m=>m.Creator)
                                        .FirstOrDefaultAsync(m => m.ID == id);

            if (mini == null)
            {
                return NotFound();
            }

            _telemetry.TrackEvent("ViewedMiniAPI", new Dictionary<string, string> { { "MiniId", mini.ID.ToString() } });

            //Find Related Minis. This is fairly hacky right now, but good for common cases.
            if (mini.MiniTags.Where(mt => mt.Status == Status.Approved).Any())
            {
                //First - Creature Name, the most straight-forward.
                IEnumerable<Tag> creatureTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.CreatureName).Select(mt => mt.Tag);

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
                IEnumerable<Tag> classTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.Class).Select(mt => mt.Tag);
                IEnumerable<Tag> raceTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.Race).Select(mt => mt.Tag);

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
                IEnumerable<Tag> creatureTypeTags = mini.MiniTags.Where(mt => mt.Status == Status.Approved).Where(mt => mt.Tag.Category == TagCategory.CreatureType).Select(mt => mt.Tag);

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
                    RelatedMinis = RelatedMinis.Concat(_context.Mini.Where(m => m.Creator == mini.Creator).ToList()).ToList();
                }
            }

            //Filter down just to approved and recent Minis.
            RelatedMinis = RelatedMinis.Distinct()
                                .Where(m => m.Status == Status.Approved)
                                .Where(m => m.ID != mini.ID)
                                .OrderByDescending(m => m.ApprovedTime).ToList();

            return Ok(new
                        {
                            ID = mini.ID,
                            Name = mini.Name,
                            Status = mini.Status,
                            Thumbnail = mini.Thumbnail,
                            Link = mini.Link,
                            Creator = new { name = mini.Creator.Name, id = mini.Creator.ID },
                            Tags = mini.MiniTags.Select(mt=> new { TagName = mt.Tag.TagName, Category = mt.Tag.Category, Status = mt.Status }),
                            RelatedMinis = RelatedMinis.Select(rm=>new { id = rm.ID, Name = rm.Name, Thumbnail = rm.Thumbnail, Link = rm.Link, Creator = new { Name = rm.Creator.Name, Id = rm.Creator.ID } }).Take(5)
                        });
        }

        [EnableCors("SpecificOrigins")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchMinisAPI(
            [FromQuery]MiniSearchModel search = null,
            [FromQuery]int pageSize = 21,
            [FromQuery]int pageIndex = 1,
            [FromQuery]int creatorId = 0)
        {
            //Mild hack - There's some case where pageIndex is hitting 0 and I can't tell how/why. (GitHub #182)
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            Creator creatorInfo = new Creator();

            if(creatorId > 0)
            {
                creatorInfo = await _context.Mini
                                .Include(m => m.Creator)
                                    .ThenInclude(c => c.Sites)
                                .Select(m => m.Creator)
                                .FirstOrDefaultAsync(c => c.ID == creatorId);
            }

            //I really don't want people manually setting massive page sizes, so hardcoding this for now.
            pageSize = 21;

            PageInfo pagingInfo = new PageInfo(pageSize, pageIndex);

            MiniSearchRequest searchRequest = new MiniSearchRequest { PageInfo = pagingInfo, Creator = creatorInfo };
            _mapper.Map(search).Over(searchRequest);
            PaginatedList<Mini> searchResult = await _mediator.Send(searchRequest);

            _telemetry.TrackEvent("NewMiniSearchAPI", new Dictionary<string, string> {
                                                            { "SearchString", searchRequest.SearchString },
                                                            { "HadResults", searchResult.Count>0 ? "True" : "False" },
                                                            { "PageIndex", searchRequest.PageInfo.PageIndex.ToString()}
                                                        });

            return Ok(searchResult.Select(m=> new
                    {
                        ID = m.ID,
                        Name = m.Name,
                        Status = m.Status,
                        Creator = new { name = m.Creator.Name, id = m.Creator.ID },
                        Thumbnail = m.Thumbnail,
                        Link = m.Link
                    })
                );
        }

        [HttpGet("check")]
        public async Task<IActionResult> FindExistingMini(Uri url)
        {
            if (url == null)
            {
                return BadRequest();
            }

            Mini mini = await _context.Mini.FirstOrDefaultAsync(m => m.Link == url.ToString() && m.Status == Status.Approved);

            if (mini == null)
            {
                return NotFound();
            }

            //TODO: look at using UrlHelper or LinkGenerator for this
            return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
        }

        [HttpGet("tagList")]
        public async Task<IActionResult> FindMinisTags(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Mini mini = await _context.Mini
                                .Include(m => m.MiniTags)
                                    .ThenInclude(mt => mt.Tag)
                                .FirstOrDefaultAsync(m => m.ID == id);

            var tagList = JsonSerializer.Serialize(mini.MiniTags
                .Where(m => m.Status == Status.Approved || m.Status == Status.Pending)
                .Select(mt => new
                {
                    ID = mt.Tag.ID,
                    TagName = mt.Tag.TagName,
                    Category = mt.Tag.Category.ToString(),
                    Status = mt.Status.ToString()
                })
                .ToList()
                .OrderBy(i => i.Status));

            if (mini == null)
            {
                return NotFound();
            }

            return Ok(tagList);
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateMini(Uri url, string key)
        {
            if (url == null || key != _apiKey)
            {
                return BadRequest();
            }

            IdentityUser user = await _context.Users.FirstAsync(u => u.Email.Contains("admin@theminiindex.com"));
            Mini mini = await _mediator.Send(new MiniSubmissionRequest(url, user));

            if (mini != null)
            {
                //TODO: look at using UrlHelper or LinkGenerator for this
                return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
            }
            else
            {
                return new StatusCodeResult(501);
            }
        }
    }
}