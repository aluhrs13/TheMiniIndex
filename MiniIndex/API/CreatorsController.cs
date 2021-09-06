using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CreatorsController : ControllerBase
    {
        public CreatorsController(
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

        // GET: api/<CreatorsController>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int pageSize = 21,
            [FromQuery] int pageIndex = 1)
        {
            //TODO: Use Mediatr and Pagination classes
            //TODO: Telemetry
            List<Creator> countQuery = await _context.Set<Mini>()
                .Include(m => m.Creator).ThenInclude(c => c.Sites)
                .Select(m => m.Creator)
                .ToListAsync();

            Dictionary<Creator, int> CreatorCounts = new Dictionary<Creator, int>();

            if (pageIndex > 1)
            {
                CreatorCounts = countQuery
                    .GroupBy(x => x)
                    .OrderByDescending(x => x.Count())
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToDictionary(k => k.Key, v => v.Count());
            }
            else
            {
                CreatorCounts = countQuery
                    .GroupBy(x => x)
                    .OrderByDescending(x => x.Count())
                    .Take(pageSize)
                    .ToDictionary(k => k.Key, v => v.Count());
            }


            return Ok(CreatorCounts.Select(k => new
            {
                ID = k.Key.ID,
                Name = k.Key.Name,
                MiniCount = k.Value,
                SourceSites = k.Key.Sites.Select(ss => new
                {
                    SiteName = ss.SiteName,
                    URL = ss.CreatorPageUri
                })
            }
            ));
        }
    }
}
