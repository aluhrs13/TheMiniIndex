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
            if (User.IsInRole("Moderator"))
            {
                if (Id == null)
                {
                    return NotFound();
                }

                Mini = await _context.Mini
                    .FirstOrDefaultAsync(m => m.ID == Id);

                if (Mini == null)
                {
                    return NotFound();
                }

                //Fix Thumbnail
                if (Mini.Link.Contains("thingiverse"))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string[] SplitURL = Mini.Link.Split(":");

                        HttpResponseMessage response = await client.GetAsync("https://api.thingiverse.com/things/" + SplitURL.Last() + "/?access_token=" + _configuration["ThingiverseToken"]);
                        HttpContent responseContent = response.Content;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                            {
                                string result = await reader.ReadToEndAsync();
                                JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);
                                if (!currentMini["default_image"]["url"].ToString().EndsWith(".stl") && !currentMini["default_image"]["url"].ToString().EndsWith(".obj"))
                                {
                                    Mini.Thumbnail = currentMini["default_image"]["url"].ToString();
                                    _context.Attach(Mini).State = EntityState.Modified;
                                }
                                else
                                {
                                    Mini.Thumbnail = currentMini["default_image"]["sizes"][4]["url"].ToString();
                                    _context.Attach(Mini).State = EntityState.Modified;
                                }
                            }
                        }
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MiniExists(Mini.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToPage("/Minis/Details", new { id = Mini.ID });
            }
            else
            {
                return NotFound();
            }
        }

        private bool MiniExists(int id)
        {
            return _context.Mini.Any(e => e.ID == id);
        }
    }
}