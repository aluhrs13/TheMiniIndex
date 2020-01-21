﻿using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Core.Tags;
using MiniIndex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniIndex.Minis
{
    [Route("dev/minis")]
    public class MinisController : Controller
    {
        public MinisController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        [HttpGet("")]
        public async Task<IActionResult> BrowseMinis(
            [FromQuery]SearchParametersModel search = null,
            [FromQuery]int pageSize = 20,
            [FromQuery]int pageIndex = 1)
        {
            PageInfo pagingInfo = new PageInfo(pageSize, pageIndex);

            MiniSearchRequest searchRequest = new MiniSearchRequest { PageInfo = pagingInfo };
            _mapper.Map(search).Over(searchRequest);

            Task<PaginatedList<Mini>> searchTask = _mediator.Send(searchRequest);
            Task<IEnumerable<Tag>> getTagsTask = _mediator.Send(new GetTagsRequest());

            await Task.WhenAll(searchTask, getTagsTask);

            SearchParametersModel searchModel = search ?? new SearchParametersModel();

            BrowseModel model = new BrowseModel(searchModel, searchTask.Result);

            return View("BrowseMinis", model);
        }
    }
}