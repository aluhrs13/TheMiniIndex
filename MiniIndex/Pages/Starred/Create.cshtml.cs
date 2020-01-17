using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(UserManager<IdentityUser> userManager, MiniIndexContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public int? mini { get; set; }

        [BindProperty]
        public Starred Starred { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid || !mini.HasValue)
            {
                return Page();
            }

            Starred newStarred = new Starred
            {
                Mini = _context.Mini.Where(m => m.ID == mini).First(),
                User = await _userManager.GetUserAsync(User)
            };

            _context.Set<Starred>().Add(newStarred);
            await _context.SaveChangesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("./Index");
        }
    }
}