using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MiniIndex.Core.Tags;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : Controller
    {
        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private readonly IMediator _mediator;

        public async Task<IEnumerable<string>> GetAllTags([FromQuery] string search = null)
        {
            var tags = await _mediator.Send(new GetTagsRequest(search));

            return tags;
        }
    }
}