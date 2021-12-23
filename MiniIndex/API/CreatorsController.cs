using AgileObjects.AgileMapper;
using Hangfire;
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
using System.Net.Http;
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
        TelemetryClient telemetry,
        IRecurringJobManager recurringJobManager,
        HttpClient httpClient)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _telemetry = telemetry;
            _recurringJobManager = recurringJobManager;
            _httpClient = httpClient;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly HttpClient _httpClient;
        // GET: api/<CreatorsController>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int pageSize = 21,
            [FromQuery] int pageIndex = 1)
        {
            //TODO: Use Mediatr and Pagination classes
            //TODO: Telemetry
            List<Creator> countQuery = await _context.Mini.AsNoTracking().TagWith("Creator API List")
                                                .Include(m => m.Creator)
                                                    .ThenInclude(c => c.Sites)
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

        [HttpGet("{id}/Scan")]
        public async Task<IActionResult> AddScan(int id)
        {
            List<SourceSite> sites = await _context.Set<SourceSite>().AsNoTracking().TagWith("Creator Scan API")
                                            .Where(x => x.Creator.ID == id)
                                            .ToListAsync();

            foreach(SourceSite site in sites)
            {
                _recurringJobManager.AddOrUpdate(site.ID.ToString(), () => _httpClient.GetAsync("http://miniindexprofileparser.azurewebsites.net/api/ProfileParser?code=<CODE LOGIC HERE>&url="+site.CreatorPageUri), Cron.Weekly, null);
            }

            return Ok();
        }
    }
}
