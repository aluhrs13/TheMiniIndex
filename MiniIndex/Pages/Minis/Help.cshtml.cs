using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class HelpModel : PageModel
    {
        public HelpModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;
        private static Random rnd = new Random();

        public IList<Mini> Mini { get; set; }

        public async Task<IActionResult> OnGetAsync(bool beta=false)
        {
            IQueryable<Mini> minis = from m in _context.Mini select m;

            if (beta == true)
            {
                Mini = await _context.Mini
                            .Include(m => m.MiniTags)
                            .Where(m => m.Status == Status.Approved)
                            .ToListAsync();

                return Redirect("https://beta.theminiindex.com/minis/"+ Mini[rnd.Next(Mini.Count)].ID);

            }

            Mini = await _context.Mini
                            .Include(m => m.MiniTags)
                            .Where(m => m.Status == Status.Pending)
                            .ToListAsync();

            return RedirectToPage("./Edit", new { id = Mini[rnd.Next(Mini.Count)].ID, showHelp = "tag" });
        }
    }
}