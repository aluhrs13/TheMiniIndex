using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.TagPairs
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly MiniIndexContext _context;

        public DeleteModel(MiniIndexContext context)
        {
            _context = context;
        }

        public TagPair TagPair { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (User.IsInRole("Moderator"))
            {
                TagPair = await _context.TagPair.FindAsync(id);

                _context.TagPair.Remove(TagPair);

                await _context.SaveChangesAsync();
                return Page();
            }
            return NotFound();
        }
    }
}
