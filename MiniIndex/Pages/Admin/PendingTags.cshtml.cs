using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class PendingTagsModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<MiniTag> UserTags { get; set; }

        public PendingTagsModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            UserTags = _context.MiniTag
                .Include(mt => mt.Tagger)
                .Include(mt => mt.Mini)
                .Include(mt => mt.Tag)
                .Where(mt => mt.Mini.Status == Status.Approved)
                .Where(mt => mt.Status == Status.Pending)
                .Where(mt => mt.Tagger != null)
                .ToList();
        }
    }
}
