using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MiniIndex.Core.Submissions;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context,
                IConfiguration configuration,
                IMediator mediator)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _mediator = mediator;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public SelectList CreatorSL { get; set; }

        [BindProperty]
        public Mini Mini { get; set; }

        [BindProperty(SupportsGet = true)]
        public string URL { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(URL))
            {
                return Page();
            }

            Mini mini = await _mediator.Send(new MiniSubmissionRequest(URL));

            if (mini is null)
            {
                Mini = new Mini();
                //TODO: proper error when mini submission could not be handled
                return Page();
            }

            return RedirectToPage("./Details", new { id = mini.ID });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Creator foundCreator = null;
            if (!String.IsNullOrEmpty(Mini.Creator.ThingiverseURL))
            {
                foundCreator = _context.Set<Creator>().FirstOrDefault(c => c.ThingiverseURL == Mini.Creator.ThingiverseURL);

                if (foundCreator != null)
                {
                    Mini.Creator = foundCreator;
                }
                else
                {
                    foundCreator = LastChanceFindCreator("Thingiverse", Mini.Creator.ThingiverseURL);
                }
            }
            else if (!String.IsNullOrEmpty(Mini.Creator.ShapewaysURL))
            {
                foundCreator = _context.Set<Creator>().FirstOrDefault(c => c.ShapewaysURL == Mini.Creator.ShapewaysURL);

                if (foundCreator != null)
                {
                    Mini.Creator = foundCreator;
                }
                else
                {
                    foundCreator = LastChanceFindCreator("Shapeways", Mini.Creator.ShapewaysURL);
                }
            }
            else if (!String.IsNullOrEmpty(Mini.Creator.PatreonURL))
            {
                foundCreator = _context.Set<Creator>().FirstOrDefault(c => c.PatreonURL == Mini.Creator.PatreonURL);

                if (foundCreator != null)
                {
                    Mini.Creator = foundCreator;
                }
                else
                {
                    foundCreator = LastChanceFindCreator("Patreon", Mini.Creator.PatreonURL);
                }
            }
            else
            {
                foundCreator = LastChanceFindCreator("Gumroad", Mini.Link);
            }

            Mini.User = await _userManager.GetUserAsync(User);

            _context.Mini.Add(Mini);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = _context.Mini.First(m => m.Link == Mini.Link).ID });
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