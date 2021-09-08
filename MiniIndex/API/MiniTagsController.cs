using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Submissions;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
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
                IMediator mediator)
        {
            _userManager = userManager;
            _context = context;
            _mediator = mediator;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMediator _mediator;

        // POST api/<MiniTagsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] MiniTag value)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            MiniTag newMT = await _mediator.Send(new MiniTagSubmissionRequest(value.Mini, value.Tag, user));
            return Ok();
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

        // DELETE api/<MiniTagsController>/
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] MiniTag value)
        {
            //TODO: Propagate deletion to paired tags. Possibly with a new state for "Autoadded"
            MiniTag MiniTag = await _context.MiniTag.FirstOrDefaultAsync(m => m.MiniID == value.Mini.ID && m.TagID == value.Tag.ID);


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