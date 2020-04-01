using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class CreatorManagerModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<IGrouping<Creator, Mini>> Creator { get; set; }

        public CreatorManagerModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Creator = _context.Mini
                        .Include(m => m.Creator)
                        .AsEnumerable()
                        .GroupBy(m => m.Creator)
                        .OrderBy(m=>m.Key.Name)
                        .ToList();
        }
    }
}
