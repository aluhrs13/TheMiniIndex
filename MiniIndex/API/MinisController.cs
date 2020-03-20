using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Core.Submissions;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Threading.Tasks;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/minis")]
    public class MinisController : Controller
    {
        public MinisController(
                MiniIndexContext context,
                IMediator mediator,
                IConfiguration configuration)
        {
            _context = context;
            _mediator = mediator;
            _apiKey = configuration["AutoCreateKey"];
        }

        private readonly MiniIndexContext _context;
        private readonly IMediator _mediator;
        private readonly string _apiKey;
        private object configuration;

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

        [HttpGet("create")]
        public async Task<IActionResult> CreateMini(Uri url, string key)
        {
            if(url == null || key != _apiKey){
                return BadRequest();
            }

            IdentityUser user = await _context.Users.FirstAsync(u=>u.Email.Contains("admin@theminiindex.com"));
            Mini mini = await _mediator.Send(new MiniSubmissionRequest(url, user));

            if (mini != null)
            {
                return Ok($"https://www.theminiindex.com/Minis/Details?id={mini.ID}");
            }
            else
            {
                return new StatusCodeResult(501);
            }
            
            //return Ok($"https://localhost:44386/Minis/Details?id={mini.ID}");

        }
    }
}