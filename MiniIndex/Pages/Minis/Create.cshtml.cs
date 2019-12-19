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
        public CreateModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                MiniIndexContext context,
                IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly MiniIndexContext _context;
        private readonly IConfiguration _configuration;
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

            Uri uri = new Uri(URL);
            Mini mini = null;

            //TODO: switch is nice, but polymorphism is nicer. Refactor this.
            switch (uri.Host.Replace("www.", ""))
            {
                case "thingiverse.com":
                    mini = await ParseThingiverse(URL);
                    break;

                case "shapeways.com":
                    mini = await ParseShapeways(URL);
                    break;

                case "gumroad.com":
                    mini = await ParseGumroad(URL);
                    break;

                default:
                    //valid URL, but not currently supported
                    //TODO: log when this happens?
                    break;
            }

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

        //TODO - Refactor this out for re-use purposes
        //If you change thumbnail logic, change it in FixThumbnail.cshtml.cs also
        private async Task<Mini> ParseThingiverse(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                string[] SplitURL = URL.Split(":");

                HttpResponseMessage response = await client.GetAsync("https://api.thingiverse.com/things/" + SplitURL.Last() + "/?access_token=" + _configuration["ThingiverseToken"]);
                HttpContent responseContent = response.Content;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();
                        JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);

                        Creator creator = new Creator
                        {
                            ThingiverseURL = currentMini["creator"]["public_url"].ToString()
                        };

                        Mini mini = new Mini()
                        {
                            Creator = creator,
                            Name = currentMini["name"].ToString(),
                            Link = currentMini["public_url"].ToString(),
                            Thumbnail = currentMini["default_image"]["url"].ToString()
                        };

                        if (Mini.Thumbnail.EndsWith(".stl") || Mini.Thumbnail.EndsWith(".obj"))
                        {
                            Mini.Thumbnail = currentMini["default_image"]["sizes"][4]["url"].ToString();
                        }

                        if (_context.Mini.Any(m => m.Link == Mini.Link))
                        {
                            return _context.Mini.First(m => m.Link == Mini.Link);
                        }
                    }
                }
                return null;
            }
        }

        private async Task<Mini> ParseGumroad(string URL)
        {
            string parsedURL = "https://gumroad.com/products/" + URL.Split('#')[1] + "/display";

            //Initialize HTML Agility Pack variables
            string html = parsedURL;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            //Parse out creator URL
            Creator creator = new Creator
            {
                WebsiteURL = URL.Split('?')[0],
                Name = Mini.Creator.WebsiteURL.Split('/').Last()
            };

            HtmlNode titleNode = htmlDoc.DocumentNode.Descendants("h1").First();
            HtmlNode imageNode = htmlDoc.DocumentNode.Descendants("img").First();
            HtmlNode costNode = htmlDoc.DocumentNode.Descendants("strong").First();

            Mini mini = new Mini
            {
                Creator = creator,
                Thumbnail = imageNode.GetAttributeValue("src", ""),
                Name = titleNode.InnerText,
                Link = URL,
                Cost = (costNode.InnerText == "$0+") ? 0 : (int)Math.Ceiling(Double.Parse(costNode.InnerText.Substring(1)))
            };

            //Check if it exists
            if (_context.Mini.Any(m => m.Link == Mini.Link))
            {
                return _context.Mini.First(m => m.Link == Mini.Link);
            }

            return null;
        }

        private async Task<Mini> ParseShapeways(string URL)
        {
            string html = URL;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            HtmlNode imageDiv = htmlDoc.GetElementbyId("slideshow-big");
            HtmlNode imageNode = imageDiv.ChildNodes.Where(cn => cn.Name == "img").First();

            string titleString = "product-title-header";
            HtmlNode nameNode = htmlDoc.DocumentNode.SelectNodes("//body//h1[@data-coyote-locator='" + titleString + "']").Last();

            string creatorURLString = "view-profile";
            HtmlNode creatorUrlNode = htmlDoc.DocumentNode.SelectNodes("//body//a[@data-sw-tracking-link-id='" + creatorURLString + "']").Last();
            string RelativeShapewaysURL = creatorUrlNode.Attributes.Where(att => att.Name == "href").First().Value;

            Creator creator = new Creator
            {
                ShapewaysURL = $"https://www.shapeways.com{RelativeShapewaysURL}"
            };

            var mini = new Mini()
            {
                Creator = creator,
                Name = nameNode.InnerText,
                Thumbnail = imageNode.GetAttributeValue("src", ""),
                Link = URL,
            };

            if (_context.Mini.Any(m => m.Link == Mini.Link))
            {
                return _context.Mini.First(m => m.Link == Mini.Link);
            }

            return null;
        }

        //TODO - Patreon currently disabled due to thumbnail expiring. Need to add caching of thumbnails somewhere to fix it.
        private async Task<Mini> ParsePatreon(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                string[] SplitURL = URL.Split('/', '-');

                HttpResponseMessage response = await client.GetAsync("https://www.patreon.com/api/posts/" + SplitURL.Last());
                HttpContent responseContent = response.Content;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();
                        JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);

                        Creator creator = new Creator
                        {
                            PatreonURL = currentMini["included"][0]["attributes"]["url"].ToString()
                        };

                        var mini = new Mini
                        {
                            Creator = creator,
                            Name = currentMini["data"]["attributes"]["title"].ToString(),
                            Link = currentMini["data"]["attributes"]["url"].ToString(),
                            Thumbnail = currentMini["data"]["attributes"]["image"]["large_url"].ToString(),
                            Cost = 1
                        };

                        if (_context.Mini.Any(m => m.Link == Mini.Link))
                        {
                            return _context.Mini.First(m => m.Link == Mini.Link);
                        }
                    }
                }
                return null;
            }
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