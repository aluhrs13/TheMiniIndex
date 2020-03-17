using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(MiniIndexContext context)
        {
            _context = context;
            MiniCount = 0;
            UntaggedMiniCount = 0;
        }

        private readonly MiniIndexContext _context;

        public int MiniCount { get; set; }
        public int UntaggedMiniCount { get; set; }
        public List<Mini> Mini { get; set; }

        public void OnGet()
        {
            Mini = _context.Set<Mini>()
                .Include(m => m.Creator)
                .Include(m => m.Sources)
                    .ThenInclude(s => s.Site)
                .Where(m => m.Status == Status.Approved)
                .OrderByDescending(m => m.ApprovedTime)
                    .ThenByDescending(m=>m.ID)
                .Take(4)
                .ToList();

            MiniCount = _context.Set<Mini>().Where(m => m.Status==Status.Approved).Count();
            UntaggedMiniCount = _context.Set<Mini>().Where(m => m.Status == Status.Pending).Count();
        }
    }
}