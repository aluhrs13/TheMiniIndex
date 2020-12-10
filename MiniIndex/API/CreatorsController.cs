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
    [Route("api/creators")]
    public class CreatorsController : Controller
    {
        public CreatorsController(
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
        public async Task<IActionResult> GetCreatorAPI(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Creator creator = await _context.Set<Mini>()
                                        .Include(m => m.Creator).ThenInclude(c => c.Sites)
                                        .Select(m => m.Creator)
                                        .FirstOrDefaultAsync(c => c.ID == id);
            if (creator == null)
            {
                return NotFound();
            }

            _telemetry.TrackEvent("ViewedCreatorAPI", new Dictionary<string, string> { { "CreatorId", creator.ID.ToString() } });

            return Ok(new
            {
                ID = creator.ID,
                Name = creator.Name,
                SourceSites = creator.Sites.Select(ss => new
                {
                    SiteName = ss.SiteName,
                    URL = ss.CreatorPageUri
                })
            });
        }

        [EnableCors("SpecificOrigins")]
        [HttpGet("browse")]
        public async Task<IActionResult> BrowseCreatorsAPI(
            [FromQuery]int pageSize = 21,
            [FromQuery]int pageIndex = 1)
        {
            //Mild hack - There's some case where pageIndex is hitting 0 and I can't tell how/why. (GitHub #182)
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

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
                    .Skip(pageSize*pageIndex)
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


            return Ok(CreatorCounts.Select(k=>new
                { 
                    ID = k.Key.ID,
                    Name = k.Key.Name,
                    MiniCount = k.Value,
                    SourceSites = k.Key.Sites.Select(ss=> new
                    {
                        SiteName = ss.SiteName,
                        URL = ss.CreatorPageUri
                    })
                }
            ));
        }
    }
}