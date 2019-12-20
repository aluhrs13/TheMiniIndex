using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Creators
{
    public class BrowseModel : PageModel
    {
        public BrowseModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;
        public IList<Creator> Creator { get; set; }
        public Dictionary<string, int> NumberMinis { get; set; }
        public IList<Mini> MiniList { get; set; }

        public async Task OnGetAsync()
        {
            NumberMinis = new Dictionary<string, int>();
            Creator = await _context.Set<Creator>().OrderBy(c => c.Name).ToListAsync();
            MiniList = await _context.Mini.ToListAsync();

            foreach (Creator curCreator in Creator)
            {
                NumberMinis.Add(curCreator.Name, MiniList.Where(m => m.Creator == curCreator).Count());
            }
        }
    }
}