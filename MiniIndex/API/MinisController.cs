using AgileObjects.AgileMapper;
using MediatR;
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

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/minis")]
    public class MinisController : Controller
    {
        public MinisController(
                MiniIndexContext context,
                IMapper mapper,
                IMediator mediator,
                IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _apiKey = configuration["AutoCreateKey"];
        }

        private readonly MiniIndexContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly string _apiKey;

        [EnableCors("SpecificOrigins")]
        [HttpGet("mini")]
        public async Task<IActionResult> GetMini(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Mini mini = await _context.Mini.FirstOrDefaultAsync(m=>m.ID == id);

            if (mini == null)
            {
                return NotFound();
            }

            return Ok(mini);
        }

        [EnableCors("SpecificOrigins")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchMinis(
            [FromQuery]MiniSearchModel search = null,
            [FromQuery]int pageSize = 21,
            [FromQuery]int pageIndex = 1)
        {
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

            return Ok(searchResult.Select(m=> new
                    {
                        ID = m.ID,
                        Name = m.Name,
                        Status = m.Status,
                        Creator = m.Creator.Name,
                        Thumbnail = m.Thumbnail,
                        Link = m.Link
                    })
                );
        }

        [HttpGet("check")]
        public async Task<IActionResult> FindExistingMini(Uri url)
        {
            if (url == null)
            {
                return BadRequest();
            }

            Mini mini = await _context.Mini.FirstOrDefaultAsync(m => m.Link == url.ToString() && m.Status == Status.Approved);

            if (mini == null)
            {
                return NotFound();
            }

            //TODO: look at using UrlHelper or LinkGenerator for this
            return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
        }

        [HttpGet("tagList")]
        public async Task<IActionResult> FindMinisTags(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Mini mini = await _context.Mini
                                .Include(m => m.MiniTags)
                                    .ThenInclude(mt => mt.Tag)
                                .FirstOrDefaultAsync(m => m.ID == id);

            var tagList = JsonSerializer.Serialize(mini.MiniTags
                .Where(m => m.Status == Status.Approved || m.Status == Status.Pending)
                .Select(mt => new
                {
                    ID = mt.Tag.ID,
                    TagName = mt.Tag.TagName,
                    Category = mt.Tag.Category.ToString(),
                    Status = mt.Status.ToString()
                })
                .ToList()
                .OrderBy(i => i.Status));

            if (mini == null)
            {
                return NotFound();
            }

            return Ok(tagList);
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateMini(Uri url, string key)
        {
            if (url == null || key != _apiKey)
            {
                return BadRequest();
            }

            IdentityUser user = await _context.Users.FirstAsync(u => u.Email.Contains("admin@theminiindex.com"));
            Mini mini = await _mediator.Send(new MiniSubmissionRequest(url, user));

            if (mini != null)
            {
                //TODO: look at using UrlHelper or LinkGenerator for this
                return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
            }
            else
            {
                return new StatusCodeResult(501);
            }
        }
    }
}