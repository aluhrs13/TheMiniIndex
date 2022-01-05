using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniIndex.Core.Submissions;
using MiniIndex.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(
                UserManager<IdentityUser> userManager,
                IMediator mediator,
                TelemetryClient telemetry)
        {
            _userManager = userManager;
            _mediator = mediator;
            _telemetry = telemetry;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;

        public SelectList CreatorSL { get; set; }

        [BindProperty]
        public Mini Mini { get; set; }

        [BindProperty]
        public Uri URL { get; set; }
        
        public async Task<IActionResult> OnGetAsync(Uri PlaceholderURL)
        {
            URL = PlaceholderURL;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _telemetry.TrackEvent("CreatedMini", new Dictionary<string, string> { { "URL", URL.ToString() } });

            IdentityUser user = await _userManager.GetUserAsync(User);

            Mini mini = await _mediator.Send(new MiniSubmissionRequest(URL, user, false));

            //TODO: Display an error....
            if (mini is null)
            {
                return Page();
            }

            return RedirectToPage("./Edit", new { id = mini.ID, showHelp = "create" });
        }
    }
}