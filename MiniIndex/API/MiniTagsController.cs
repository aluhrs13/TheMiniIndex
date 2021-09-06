using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Submissions;
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
    public class MiniTagsController : ControllerBase
    {

        public MiniTagsController(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context,
                IMapper mapper,
                IMediator mediator)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        // GET api/<MiniTagsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Mini selectedMini = await _context.Set<Mini>()
                                        .Include(m => m.MiniTags)
                                        .ThenInclude(mt => mt.Tag)
                                        .FirstOrDefaultAsync(c => c.ID == id);

            if (selectedMini == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(selectedMini.MiniTags.Select(mt=> new{ mt.Status, mt.Tag.TagName, mt.Tag.ID, mt.Tag.Category} ));
            }
        }

        // POST api/<MiniTagsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] MiniTag value)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            MiniTag newMT = await _mediator.Send(new MiniTagSubmissionRequest(value.Mini, value.Tag, user));

            //TODO: TBD -> some useful URL
            return Created("TBD", newMT.Mini.MiniTags.Select(mt=>mt.Tag.TagName));
        }

        // PATCH api/<MiniTagsController>/
        [HttpPatch]
        [Authorize(Roles="Moderator")]
        public async Task<IActionResult> Patch([FromBody] MiniTag value)
        {
            MiniTag MiniTag = await _context.MiniTag.FirstOrDefaultAsync(m => m.MiniID == value.MiniID && m.TagID == value.TagID);

            if (MiniTag == null)
            {
                return NotFound();
            }

            MiniTag.Status = value.Status;
            MiniTag.LastModifiedTime = DateTime.Now;
            _context.Attach(MiniTag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }

            return Ok();
       }

        // DELETE api/<MiniTagsController>/5
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] MiniTag value)
        {
            MiniTag MiniTag = await _context.MiniTag.FirstOrDefaultAsync(m => m.MiniID == value.MiniID && m.TagID == value.TagID);

            if (User.IsInRole("Moderator"))
            {
                _context.Remove(MiniTag);
            }
            else
            {
                MiniTag.Status = Status.Deleted;
                MiniTag.LastModifiedTime = DateTime.Now;
                _context.Attach(MiniTag).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }

            return Ok();
        }
    }
}