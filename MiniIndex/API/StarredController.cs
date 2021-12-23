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
    public class StarredController : ControllerBase
    {

        public StarredController(
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

        // POST api/<StarredController>
        [HttpPost("{id}")]
        [Authorize]
        public async Task<IActionResult> Post(int id)
        {
            Starred newStarred = new Starred
            {
                Mini = await _context.Mini.FindAsync(id),
                User = await _userManager.GetUserAsync(User)
            };

            await _context.Starred.AddAsync(newStarred);
            await _context.SaveChangesAsync();
            return Ok("{}");
        }

        // DELETE api/<StarredController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            IdentityUser CurrentUser = await _userManager.GetUserAsync(User);
            Starred Starred = await _context.Starred.FindAsync(id, CurrentUser.Id);

            if (Starred != null)
            {
                _context.Starred.Remove(Starred);
                await _context.SaveChangesAsync();
                return Ok("{}");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
