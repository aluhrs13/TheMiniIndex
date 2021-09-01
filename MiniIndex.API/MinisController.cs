using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Core.Submissions;
using MiniIndex.Minis;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.API.Minis;
[Route("api/[controller]")]
[ApiController]
public class MinisController : ControllerBase
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

    // GET: api/<MinisController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<MinisController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        List<Mini> RelatedMinis = new List<Mini> { };
        Mini mini = await _context.Mini
                                    .Include(m => m.MiniTags)
                                        .ThenInclude(mt => mt.Tag)
                                    .Include(m => m.Creator)
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
            Tags = mini.MiniTags.Select(mt => new { TagName = mt.Tag.TagName, Category = mt.Tag.Category, Status = mt.Status }),
            RelatedMinis = RelatedMinis.Select(rm => new { id = rm.ID, Name = rm.Name, Thumbnail = rm.Thumbnail, Link = rm.Link, Creator = new { Name = rm.Creator.Name, Id = rm.Creator.ID } }).Take(5)
        });
    }

    // POST api/<MinisController>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] string value)
    {
        if (value == null)
        {
            return BadRequest();
        }

        IdentityUser user = await _context.Users.FirstAsync(u => u.Email.Contains("admin@theminiindex.com"));
        Mini mini = await _mediator.Send(new MiniSubmissionRequest(new System.Uri(value), user));

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

    // PUT api/<MinisController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<MinisController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
