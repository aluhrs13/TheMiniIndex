using AgileObjects.AgileMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniIndex.Core.Minis.Search;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
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
            [FromQuery]MiniSearchModel search = null,
            [FromQuery]int pageSize = 20,
            [FromQuery]int pageIndex = 1)
        {
            PageInfo pagingInfo = new PageInfo(pageSize, pageIndex);

            MiniSearchRequest searchRequest = new MiniSearchRequest { PageInfo = pagingInfo };
            _mapper.Map(search).Over(searchRequest);

            PaginatedList<Mini> searchResult = await _mediator.Send(searchRequest);

            MiniSearchModel searchModel = search ?? new MiniSearchModel();

            BrowseModel model = new BrowseModel(searchModel, searchResult);

            return View("BrowseMinis", model);
        }
    }
}