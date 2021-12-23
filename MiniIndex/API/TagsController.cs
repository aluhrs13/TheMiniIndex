using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Tags;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiniIndex.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        public TagsController(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context,
                IMapper mapper,
                IMediator mediator,
                TelemetryClient telemetry)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _telemetry = telemetry;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;

        // GET: api/<TagsController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string search = null)
        {
            var tags = await _mediator.Send(new GetTagsRequest(search));
            return Ok(tags);
        }

        // PATCH api/<TagsController>/
        [HttpPatch]
        [Authorize(Roles ="Moderator")]
        public async Task<IActionResult> Patch([FromBody] Tag value)
        {
            Tag Tag = await _context.Tag.FindAsync(value.ID);

            if (Tag == null)
            {
                return NotFound();
            }

            if(value.Category != null)
            {
                Tag.Category = value.Category;
            }

            if(value.TagName != null)
            {
                Tag.TagName = value.TagName;
            }

            _context.Attach(Tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }

            return Ok("{}");
        }

        // DELETE api/<TagsController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Delete(int id)
        {
            //TODO: Delete MiniTags too? Maybe only work if there's no approved MiniTags
            Tag Tag = await _context.Tag.FindAsync(id);
            _context.Remove(Tag);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }

            return Ok("{}");
        }

        /*
         * 
         * 
         * 
         * PAIRS BELOW HERE
         * 
         * 
         * 
         */

        // GET api/<TagsController>/5/Pairs
        [HttpGet("{id}/Pairs")]
        public async Task<IActionResult> Get(int id)
        {
            Tag selectedTag = await _context.Tag.FindAsync(id);
                
            List<TagPair> pairs = _context.TagPair.AsNoTracking()
                    .Where(tp => tp.Tag1 ==selectedTag || tp.Tag2 == selectedTag)
                    .Include(tp=>tp.Tag2)
                    .Include(tp=>tp.Tag1)
                    .ToList();

            return Ok(pairs);
        }

        // POST api/<TagsController>/5/Pairs/6?type=<type>
        [HttpPost("{tag1}/Pairs/{tag2}")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> PairPost(int tag1, int tag2, [FromQuery] int type)
        {
            Tag Tag1 = await _context.Tag.FindAsync(tag1);
            Tag Tag2 = await _context.Tag.FindAsync(tag2);
            PairType pairType = (PairType)type;

            //This is a hack. 99 means "child" from the tag manager, so we're swapping them.
            //It's a dumb hack, but I'm tired.
            if (type == 99)
            {
                Tag1 = await _context.Tag.FindAsync(tag2);
                Tag2 = await _context.Tag.FindAsync(tag1);
                pairType = PairType.Parent;
            }

            if (Tag1 == null | Tag2 == null)
            {
                return NotFound();
            }

            TagPair newPair = new TagPair()
            {
                Tag1 = Tag1,
                Tag2 = Tag2,
                Type = pairType
            };

            if (await _context.TagPair.AnyAsync(tp => (tp.Tag1 == Tag1 && tp.Tag2 == Tag2)))
            {
                return NotFound();
            }

            await _context.TagPair.AddAsync(newPair);

            await _context.SaveChangesAsync();
            return Ok("{}");
        }

        // DELETE api/Pairs/6
        [HttpDelete("/api/Pairs/{id}")]
        public async Task<IActionResult> PairDelete(int id)
        {
            TagPair TagPair = await _context.TagPair.FindAsync(id);

            _context.TagPair.Remove(TagPair);

            await _context.SaveChangesAsync();
            return Ok("{}");
        }
    }
}
