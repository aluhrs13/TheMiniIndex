using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.TagStream
{
    [Authorize]
    public class TagStreamModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<MiniTag> UserTags { get; set; }

        public TagStreamModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            UserTags = _context.MiniTag
                .Include(mt => mt.Tagger)
                .Include(mt => mt.Mini)
                .Include(mt => mt.Tag)
                .Where(mt => mt.Status == Status.Pending)
                .Where(mt => mt.Tagger != null)
                .ToList();

            UserTags = UserTags.ToList();
        }
    }
}
