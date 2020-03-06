using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Minis
{
    [Route("dev/minis")]
    public class MinisController : Controller
    {
        public MinisController(IMapper mapper, IMediator mediator, TelemetryClient telemetry)
        {
            _mapper = mapper;
            _mediator = mediator;
            _telemetry = telemetry;
        }

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;


        [HttpGet("")]
        public async Task<IActionResult> BrowseMinis(
            [FromQuery]MiniSearchModel search = null,
            [FromQuery]int pageSize = 21,
            [FromQuery]int pageIndex = 1)
        {
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