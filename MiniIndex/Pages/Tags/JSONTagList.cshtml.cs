using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.Tags
{
    public class JSONTagListModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;

        public JSONTagListModel(MiniIndex.Models.MiniIndexContext context)
        {
            _context = context;
        }

        public IList<Tag> Tag { get;set; }

        public async Task OnGetAsync()
        {
            Tag = await _context.Tag.ToListAsync();
        }
    }
}
