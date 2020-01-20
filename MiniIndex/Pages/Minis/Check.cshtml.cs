using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Minis
{
    public class CheckModel : PageModel
    {

        public CheckModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public int MiniID;

        public async Task<IActionResult> OnGetAsync(string URL)
        {
            if (URL == null)
            {
                return BadRequest();
            }

            //TODO - Make this a bit smarter, becaue it can be hung up by basic stuff like different capitalization.
            Mini mini = await _context.Mini
                .FirstOrDefaultAsync(m => m.Link == URL && m.Status == Status.Approved);

            if(mini == null)
            {
                return NotFound();
            }

            MiniID = mini.ID;
            
            return Page();
        }
    }
}