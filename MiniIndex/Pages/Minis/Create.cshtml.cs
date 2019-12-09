using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;
        public SelectList CreatorSL { get; set; }
        [BindProperty]
        public Mini Mini { get; set; }
        [BindProperty(SupportsGet = true)]
        public string URL { get; set; }

        public CreateModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                MiniIndexContext context,
                IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Mini = new Mini();
            int ret = -1;

            //No URL passed, render normal page.
            if (string.IsNullOrEmpty(URL))
            {
                return Page();
            }
            else
            {
                //Parse out the URL and process it
                string originalURL = URL;
                URL = URL.ToLower();
                URL = URL.Split("?")[0];

                //Add more conditions here for new sources. Move to switch statement...
                if (URL.Contains("thingiverse.com"))
                {
                    ret = await ParseThingiverse(Mini, URL);
                }else if (URL.Contains("shapeways.com"))
                {
                    ret = await ParseShapeways(Mini, URL);
                }else if (URL.Contains("gumroad.com"))
                {
                    ret = await ParseGumroad(Mini, originalURL);
                }
            }

            //Mini is already indexed, redirect to the existing page.
            if(ret > 0)
            {
                return RedirectToPage("./Details", new { id = ret });
            }

            //Invalid URL. Need to give a good error.
            return Page();
        }

        private async Task<int> ParseGumroad(Mini mini, string URL)
        {
            string parsedURL = "https://gumroad.com/products/"+URL.Split('#')[1]+"/display";

            //Initialize HTML Agility Pack variables
            string html = parsedURL;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            //Parse out the Thumbnail image
            HtmlNode ImageNode = htmlDoc.DocumentNode.Descendants("img").First();
            Mini.Thumbnail = ImageNode.GetAttributeValue("src", "");

            //Parse out the name
            HtmlNode TitleNode = htmlDoc.DocumentNode.Descendants("h1").First();
            Mini.Name = TitleNode.InnerText;

            Mini.Link = URL;

            //Parse out creator URL
            Creator creator = new Creator();
            Mini.Creator = creator;
            Mini.Creator.WebsiteURL = URL.Split('?')[0];
            Mini.Creator.Name = Mini.Creator.WebsiteURL.Split('/').Last();

            //Parse out cost
            HtmlNode CostNode = htmlDoc.DocumentNode.Descendants("strong").First();

            if (CostNode.InnerText == "$0+")
            {
                Mini.Cost = 0;
            }
            else
            {
                Mini.Cost = (int)Math.Ceiling(double.Parse(CostNode.InnerText.Substring(1)));
            }

            //Check if it exists
            if (_context.Mini.Any(m => m.Link == Mini.Link))
            {
                return _context.Mini.Where(m => m.Link == Mini.Link).FirstOrDefault().ID;
            }

            return -1;
        }

        private async Task<int> ParseShapeways(Mini mini, string URL)
        {
            //Initialize HTML Agility Pack variables
            string html = URL;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            //Parse out the Thumbnail image
            HtmlNode ImageDiv = htmlDoc.GetElementbyId("slideshow-big");
            HtmlNode ImageNode = ImageDiv.ChildNodes.Where(cn => cn.Name == "img").First();
            Mini.Thumbnail = ImageNode.GetAttributeValue("src", "");

            //Parse out the name
            string TitleString = "product-title-header";
            foreach (HtmlNode h1Node in htmlDoc.DocumentNode.SelectNodes("//body//h1[@data-coyote-locator='" + TitleString + "']"))
            {
                Mini.Name = h1Node.InnerText;
            }

            Mini.Link = URL;

            //Parse out creator URL
            Creator creator = new Creator();
            Mini.Creator = creator;
            string CreatorURLString = "view-profile";
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//body//a[@data-sw-tracking-link-id='" + CreatorURLString + "']"))
            {
                string RelativeShapewaysURL = node.Attributes.Where(att=>att.Name=="href").First().Value;
                Mini.Creator.ShapewaysURL = "https://www.shapeways.com" + RelativeShapewaysURL;
            }
            
            if (_context.Mini.Any(m => m.Link == Mini.Link))
            {
                return _context.Mini.Where(m => m.Link == Mini.Link).FirstOrDefault().ID;
            }
            
            return -1;
        }

        //TODO - Refactor this out for re-use purposes
        //If you change thumbnail logic, change it in FixThumbnail.cshtml.cs also
        public async Task<int> ParseThingiverse(Mini Mini, string URL)
        {
            HttpClient client = new HttpClient();
            string[] SplitURL = URL.Split(":");

            HttpResponseMessage response = await client.GetAsync("https://api.thingiverse.com/things/" + SplitURL.Last() + "/?access_token=" + _configuration["ThingiverseToken"]);
            HttpContent responseContent = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    string result = await reader.ReadToEndAsync();
                    JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);

                    Mini.Name = currentMini["name"].ToString();
                    Mini.Link = currentMini["public_url"].ToString();
                    Mini.Thumbnail = currentMini["default_image"]["url"].ToString();

                    if(Mini.Thumbnail.EndsWith(".stl") || Mini.Thumbnail.EndsWith(".obj"))
                    {
                        Mini.Thumbnail = currentMini["default_image"]["sizes"][4]["url"].ToString();
                    }

                    Creator creator = new Creator();
                    Mini.Creator = creator;
                    Mini.Creator.ThingiverseURL = currentMini["creator"]["public_url"].ToString();

                    if (_context.Mini.Any(m => m.Link == Mini.Link))
                    {
                        return _context.Mini.Where(m => m.Link == Mini.Link).FirstOrDefault().ID;
                    }
                }
            }
            return -1;
        }
        

        //TODO - Patreon currently disabled due to thumbnail expiring. Need to add caching of thumbnails somewhere to fix it.
        private async Task<int> ParsePatreon(Mini mini, string URL)
        {
            HttpClient client = new HttpClient();
            string[] SplitURL = URL.Split('/', '-');

            HttpResponseMessage response = await client.GetAsync("https://www.patreon.com/api/posts/"+SplitURL.Last());
            HttpContent responseContent = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    String result = await reader.ReadToEndAsync();
                    JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);

                    Mini.Name = currentMini["data"]["attributes"]["title"].ToString();
                    Mini.Link = currentMini["data"]["attributes"]["url"].ToString();

                    //TODO - Returned URL has a token time, may expire. Might have to rehost images.
                    Mini.Thumbnail = currentMini["data"]["attributes"]["image"]["large_url"].ToString();

                    Creator creator = new Creator();
                    Mini.Creator = creator;
                    Mini.Creator.PatreonURL = currentMini["included"][0]["attributes"]["url"].ToString();

                    Mini.Cost = 1;

                    if (_context.Mini.Any(m => m.Link == Mini.Link))
                    {
                        return _context.Mini.Where(m => m.Link == Mini.Link).FirstOrDefault().ID;
                    }
                }
            }
            return -1;
        }

        public Creator LastChanceFindCreator(string source, string URL)
        {
            Creator foundCreator = null;

            if(source == "Thingiverse" || source== "Shapeways" || source == "Patreon")
            {
                string URLName = URL.Split("/").Last();
                foundCreator = _context.Creator.Where(c => c.Name == URLName).FirstOrDefault();

                if (foundCreator == null)
                {
                    Mini.Creator.Name = URLName;
                }
                else
                {
                    Mini.Creator = foundCreator;
                }
            }

            if(source == "Gumroad")
            {
                string URLName = URL.Split('/').Last().Split('?')[0].Split('#')[0];
                foundCreator = _context.Creator.Where(c => c.Name == URLName).FirstOrDefault();

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Creator foundCreator = null;
            if (!string.IsNullOrEmpty(Mini.Creator.ThingiverseURL))
            {
                foundCreator = _context.Creator.Where(c => c.ThingiverseURL == Mini.Creator.ThingiverseURL).FirstOrDefault();

                if (foundCreator != null)
                {
                    Mini.Creator = foundCreator;
                }
                else
                {
                    foundCreator = LastChanceFindCreator("Thingiverse", Mini.Creator.ThingiverseURL);
                }
            }
            else if (!string.IsNullOrEmpty(Mini.Creator.ShapewaysURL))
            {
                foundCreator = _context.Creator.Where(c => c.ShapewaysURL == Mini.Creator.ShapewaysURL).FirstOrDefault();

                if (foundCreator != null)
                {
                    Mini.Creator = foundCreator;
                }
                else
                {
                    foundCreator = LastChanceFindCreator("Shapeways", Mini.Creator.ShapewaysURL);
                }
            }
            else if (!string.IsNullOrEmpty(Mini.Creator.PatreonURL))
            {
                foundCreator = _context.Creator.Where(c => c.PatreonURL == Mini.Creator.PatreonURL).FirstOrDefault();

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

            return RedirectToPage("./Details", new { id = _context.Mini.Where(m => m.Link == Mini.Link).FirstOrDefault().ID });
        }
    }
}