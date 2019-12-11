using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        public List<Mini> MiniList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PageNumber { get; set; }

        public int ParsedPageNumber { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Creator = await _context
                .Set<Creator>()
                .Include(x => x.Sites)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Creator == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(Creator.ThingiverseURL))
            {
                MiniList = new List<Mini>();
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

                                MiniList.Add(NewMini);
                            }
                        }
                    }
                }
            }
            return Page();
        }
    }
}