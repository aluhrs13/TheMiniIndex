using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using MiniIndex.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class FixThumbnailModel : PageModel
    {
        public FixThumbnailModel(MiniIndexContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        [BindProperty]
        public Mini Mini { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return NotFound();
        }

        private bool MiniExists(int id)
        {
            return _context.Mini.Any(e => e.ID == id);
        }
    }
}