using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : Controller
    {
        public TagsController(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public async Task<IEnumerable<string>> GetAllTags([FromQuery] string search = null)
        {
            IQueryable<Tag> tagsQuery = _context.Set<Tag>();

            if (!String.IsNullOrEmpty(search.Trim()))
            {
                tagsQuery = tagsQuery.Where(t => t.TagName.ToUpper().Contains(search.ToUpper()));
            }

            var tags = await tagsQuery
                .Select(t => t.TagName)
                .ToListAsync();

            return tags;
        }
    }
}