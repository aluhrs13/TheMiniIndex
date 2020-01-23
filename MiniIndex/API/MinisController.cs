using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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
        public MinisController(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

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
    }
}