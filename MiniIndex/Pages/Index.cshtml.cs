using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;
        public List<Mini> Mini { get; set; }
        public int MiniCount=0;


        public IndexModel(MiniIndex.Models.MiniIndexContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            var minis = from m in _context.Mini select m;

            Mini = minis
                .Include(m => m.Creator)
                .Where(m=>m.Status==Status.Approved)
                .OrderByDescending(m => m.ID)
                .Take(4)
                .ToList<Mini>();

            MiniCount = minis.Count();

        }
    }
}
