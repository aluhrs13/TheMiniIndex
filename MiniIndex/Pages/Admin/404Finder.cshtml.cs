using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using MiniIndex.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class _404FinderModel : PageModel
    {
        public _404FinderModel(MiniIndexContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            _telemetry = new TelemetryClient();
        }

        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetry;

        public IList<Mini> Mini { get; set; }
        public IList<Creator> Creator { get; set; }
        public List<Mini> MissingMinis { get; set; }
        public List<int> CheckedCreators { get; set; }

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                Mini = await _context.Mini
                        .Where(m => m.Status == Status.Approved)
                        .Include(m => m.Creator)
                        .AsNoTracking()
                        .ToListAsync();

                MissingMinis = new List<Mini>();
                CheckedCreators = new List<int>();

                using (HttpClient client = new HttpClient())
                {
                    foreach (Mini item in Mini)
                    {
                        var thingiverseSource = item.Creator.Sites.FirstOrDefault(s => s is ThingiverseSource);

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

                                    if (thingiverseSource.CreatorPageUri.ToString() != currentMini["creator"]["public_url"].ToString())
                                    {
                                        _telemetry.TrackEvent("Changing URL for " + item.Creator.Name + " from " + thingiverseSource.CreatorPageUri.ToString() + " to " + currentMini["creator"]["public_url"].ToString());

                                        item.Creator.Sites.Remove(thingiverseSource);
                                        item.Creator.Sites.Add(new ThingiverseSource(item.Creator, currentMini["creator"]["public_url"].ToString()));

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
}