﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using Newtonsoft.Json;

namespace MiniIndex.Pages.Creators
{
    public class DetailsModel : PageModel
    {
        public DetailsModel(MiniIndexContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;
        public Creator Creator { get; set; }
        public List<Mini> ThingiverseMiniList { get; set; }
        public List<Mini> AllCreatorsMinis { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PageNumber { get; set; }

        public int ParsedPageNumber { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            TelemetryClient telemetry = new TelemetryClient();

            if (id == null)
            {
                return NotFound();
            }

            Creator = await _context.Creator.FirstOrDefaultAsync(m => m.ID == id);

            if (Creator == null)
            {
                return NotFound();
            }

            telemetry.TrackEvent("ViewedCreator", new Dictionary<string, string> { { "CreatorId", Creator.ID.ToString() } });

            AllCreatorsMinis = new List<Mini>();
            AllCreatorsMinis = _context.Mini.Where(m => m.Creator.ID == Creator.ID).Where(m=>m.Status == Status.Approved).ToList();

            if (!String.IsNullOrEmpty(Creator.ThingiverseURL))
            {

                ThingiverseMiniList = new List<Mini>();
                using (HttpClient client = new HttpClient())
                {
                    if (String.IsNullOrEmpty(PageNumber))
                    {
                        ParsedPageNumber = 1;
                    }
                    else
                    {
                        ParsedPageNumber = Int32.Parse(PageNumber);
                    }

                    string ThingiverseUserName = Creator.ThingiverseURL.Split('/').Last();
                    HttpResponseMessage response = await client.GetAsync("https://api.thingiverse.com/users/" + ThingiverseUserName + "/things?access_token=" + _configuration["ThingiverseToken"] + "&page=" + ParsedPageNumber);
                    HttpContent responseContent = response.Content;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                        {
                            string result = await reader.ReadToEndAsync();
                            dynamic returnList = JsonConvert.DeserializeObject(result);

                            foreach (dynamic CurrentMini in returnList)
                            {
                                Mini NewMini = new Mini();
                                string CurrentLink = CurrentMini["public_url"].ToString();

                                if (_context.Mini.Any(m => m.Link == CurrentLink))
                                {
                                    NewMini = _context.Mini.FirstOrDefault(m => m.Link == CurrentLink);
                                }
                                else
                                {
                                    NewMini.Name = CurrentMini["name"].ToString();
                                    NewMini.Link = CurrentLink;
                                    NewMini.Thumbnail = CurrentMini["thumbnail"].ToString();
                                    NewMini.Status = Status.Unindexed;
                                }

                                ThingiverseMiniList.Add(NewMini);
                            }
                        }
                    }
                }
            }
            return Page();
        }
    }
}