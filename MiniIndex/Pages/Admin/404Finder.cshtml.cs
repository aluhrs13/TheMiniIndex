using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiniIndex.Pages.Admin
{
    [Authorize]

    public class _404FinderModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;
        private readonly IConfiguration _configuration;
        private TelemetryClient telemetry = new TelemetryClient();


        public IList<Mini> Mini { get; set; }
        public IList<Creator> Creator { get; set; }
        public List<Mini> MissingMinis { get; set; }
        public List<int> CheckedCreators { get; set; }

        public _404FinderModel(MiniIndex.Models.MiniIndexContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                Mini = await _context.Mini
                        .Where(m=>m.Status==Status.Approved)
                        .Include(m => m.Creator)
                        .AsNoTracking()
                        .ToListAsync();

                MissingMinis = new List<Mini>();
                CheckedCreators = new List<int>();

                HttpClient client = new HttpClient();

                foreach (Mini item in Mini)
                {
                    HttpResponseMessage response = await client.GetAsync(item.Thumbnail);
                    HttpContent responseContent = response.Content;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        MissingMinis.Add(item);
                    }

                    if (item.Link.Contains("thingiverse") && !CheckedCreators.Contains(item.Creator.ID))
                    {
                        string[] SplitURL = item.Link.Split(":");

                        HttpResponseMessage response2 = await client.GetAsync("https://api.thingiverse.com/things/" + SplitURL.Last() + "/?access_token=" + _configuration["ThingiverseToken"]);
                        HttpContent responseContent2 = response2.Content;
                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            using (StreamReader reader = new StreamReader(await responseContent2.ReadAsStreamAsync()))
                            {
                                string result = await reader.ReadToEndAsync();
                                JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);

                                if (item.Creator.ThingiverseURL != currentMini["creator"]["public_url"].ToString())
                                {
                                    telemetry.TrackEvent("Changing URL for "+item.Creator.Name+" from " + item.Creator.ThingiverseURL + " to " + currentMini["creator"]["public_url"].ToString());
                                    item.Creator.ThingiverseURL = currentMini["creator"]["public_url"].ToString();
                                    _context.Attach(item.Creator).State = EntityState.Modified;
                                }
                            }
                            CheckedCreators.Add(item.Creator.ID);
                        }
                    }
                }

                await _context.SaveChangesAsync();

            }
        }
    }


}
