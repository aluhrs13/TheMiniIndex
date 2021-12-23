using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Tags
{
    public class BrowseModel : PageModel
    {
        public BrowseModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;
        public IList<IGrouping<Tag, MiniTag>> Tag { get; set; }


        public async Task OnGetAsync()
        {
            Tag = _context.MiniTag
                .AsNoTracking().TagWith("Browse Tags")
                .Include(t => t.Tag)
                .AsEnumerable()
                .GroupBy(t => t.Tag)
                .OrderByDescending(t => t.Key.Category)
                .ThenBy(t => t.Key.TagName)
                .ToList();
        }
    }
}