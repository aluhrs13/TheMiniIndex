using AgileObjects.AgileMapper;
using Lib.AspNetCore.ServerTiming;
using Lib.AspNetCore.ServerTiming.Http.Headers;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Minis
{
    [Route("/Minis")] 
    public class MinisController : Controller
    {
        public MinisController(MiniIndexContext context, IMapper mapper, IMediator mediator, TelemetryClient telemetry, IServerTiming serverTiming)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _telemetry = telemetry;
            _serverTiming = serverTiming;
        }

        private readonly MiniIndexContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;
        private readonly IServerTiming _serverTiming;



        [HttpGet("")]
        public async Task<IActionResult> BrowseMinis(
            [FromQuery]MiniSearchModel search = null,
            [FromQuery]int pageSize = 21,
            [FromQuery]int pageIndex = 1)
        {
            DateTime startTime = DateTime.Now;
            //Mild hack - There's some case where pageIndex is hitting 0 and I can't tell how/why. (GitHub #182)
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            //I really don't want people manually setting massive page sizes, so hardcoding this for now.
            pageSize = 21;

            PageInfo pagingInfo = new PageInfo(pageSize, pageIndex);

            MiniSearchRequest searchRequest = new MiniSearchRequest { PageInfo = pagingInfo };
            _mapper.Map(search).Over(searchRequest);
            PaginatedList<Mini> searchResult = await _mediator.Send(searchRequest);

            SearchSupportingInfo searchInfo = await _mediator.Send(new GetSearchInfoRequest());

            var allTags = searchInfo.Tags
                .OrderBy(t => t.Category.ToString())
                .ThenBy(t => t.TagName)
                .ToList();

            search.TagOptions = new SelectList(allTags, "TagName", "TagName", null, "Category");

            BrowseModel model = new BrowseModel(search, searchResult);

            TimeSpan timeSpent = DateTime.Now - startTime;
            _serverTiming.Metrics.Add(new ServerTimingMetric("Query", (decimal)timeSpent.TotalMilliseconds, "LINQ Query"));
            _telemetry.TrackEvent("NewMiniSearch", new Dictionary<string, string> {
                                                            { "SearchString", searchRequest.SearchString },
                                                            { "Tags", String.Join(",", searchRequest.Tags) },
                                                            { "FreeOnly", searchRequest.FreeOnly.ToString() },
                                                            { "HadResults", searchResult.Count>0 ? "True" : "False" },
                                                            { "PageIndex", searchRequest.PageInfo.PageIndex.ToString()}
                                                        });

            return View("BrowseMinis", model);
        }
    }
}