using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniIndex.Core.Submissions;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context,
                IMediator mediator)
        {
            _userManager = userManager;
            _context = context;
            _mediator = mediator;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;
        private readonly IMediator _mediator;

        public SelectList CreatorSL { get; set; }

        [BindProperty]
        public Mini Mini { get; set; }

        [BindProperty]
        public string URL { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IdentityUser user = await _userManager.GetUserAsync(User);

            Mini mini = await _mediator.Send(new MiniSubmissionRequest(URL, user));

            if (mini is null)
            {
                return Page();
            }

            return RedirectToPage("./Details", new { id = mini.ID });
        }

        private Creator LastChanceFindCreator(string source, string URL)
        {
            Creator foundCreator = null;

            if (source == "Thingiverse" || source == "Shapeways" || source == "Patreon")
            {
                string URLName = URL.Split("/").Last();
                foundCreator = _context.Set<Creator>().FirstOrDefault(c => c.Name == URLName);

                if (foundCreator == null)
                {
                    Mini.Creator.Name = URLName;
                }
                else
                {
                    Mini.Creator = foundCreator;
                }
            }

            if (source == "Gumroad")
            {
                string URLName = URL.Split('/').Last().Split('?')[0].Split('#')[0];
                foundCreator = _context.Set<Creator>().FirstOrDefault(c => c.Name == URLName);

                if (foundCreator == null)
                {
                    Mini.Creator.Name = URLName;
                    Mini.Creator.WebsiteURL = URL.Split('?')[0].Split('#')[0];
                }
                else
                {
                    Mini.Creator = foundCreator;
                }
            }

            return foundCreator;
        }
    }
}