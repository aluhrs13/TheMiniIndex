using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using MiniIndex.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Creators
{
    public class DetailsModel : PageModel
    {
        public DetailsModel(MiniIndexContext context, IConfiguration configuration, TelemetryClient telemetry)
        {
            _context = context;
            _configuration = configuration;
            _telemetry = telemetry;
        }

        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetry;

        public string FunctionsCode { get; set; }

        public Creator Creator { get; set; }
        public List<Mini> ThingiverseMiniList { get; set; }
        public List<Mini> AllCreatorsMinis { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PageNumber { get; set; }

        public int ParsedPageNumber { get; set; }

        public string ThingiverseError { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            FunctionsCode = _configuration["FunctionsCode"];

            Creator = await _context
                .Set<Creator>()
                .AsNoTracking()
                .Include(x => x.Sites)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Creator == null)
            {
                return NotFound();
            }

            _telemetry.TrackEvent("ViewedCreator", new Dictionary<string, string> { { "CreatorId", Creator.ID.ToString() } });

            AllCreatorsMinis = _context.Mini.AsNoTracking().TagWith("Creator Mini List")
                                        .Where(m => m.Creator.ID == Creator.ID)
                                        .Where(m => (m.Status!=Status.Deleted && m.Status!=Status.Rejected))
                                        .OrderByDescending(m=>m.ID).ToList();

            return Page();
        }
    }
}