using HtmlAgilityPack;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.PrusaPrinters
{
    public class PrusaPrintersParser : IParser
    {
        public PrusaPrintersParser()
        {
        }

        public string Site => "PrusaPrinters";

        public bool CanParse(Uri url)
        {
            bool isPPURL = url.Host.Replace("www.", "").Equals("prusaprinters.org", StringComparison.OrdinalIgnoreCase);

            if (!isPPURL)
            {
                return false;
            }

            bool ppFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.StartsWith("/prints/");

            return ppFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            //Split the LocalPath of url on '-' and grab the first value
            string id = url.LocalPath.Split('-').First().Split('/').Last();

            HttpClient client = new HttpClient();

            // Add a new Request Message
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.prusaprinters.org/graphql/");
            requestMessage.Headers.Add("ContentType", "application/json");

            string myContent = "{\"operationName\":\"PrintProfile\",\"variables\":{\"id\":\"30323\"},\"query\":\"query PrintProfile($id: ID!) {\n  print(id: $id) {\n    id\n    slug\n    name\n    user {\n      id\n      slug\n      email\n      donationLinks {\n        id\n        title\n        url\n        __typename\n      }\n      publicUsername\n      avatarFilePath\n      __typename\n    }\n    ratingAvg\n    myRating\n    ratingCount\n    content\n    category {\n      id\n      path {\n        id\n        name\n        __typename\n      }\n      __typename\n    }\n    modified\n    firstPublish\n    datePublished\n    hasModel\n    summary\n    shareCount\n    printDuration\n    numPieces\n    weight\n    nozzleDiameters\n    usedMaterial\n    layerHeights\n    materials {\n      name\n      __typename\n    }\n    dateFeatured\n    downloadCount\n    displayCount\n    pdfFilePath\n    commentCount\n    userGcodeCount\n    userGcodesCount\n    remixCount\n    canBeRated\n    inMyCollections\n    printer {\n      id\n      name\n      __typename\n    }\n    images {\n      id\n      filePath\n      __typename\n    }\n    tags {\n      name\n      id\n      __typename\n    }\n    thingiverseLink\n    filesType\n    foundInUserGcodes\n    remixParents {\n      ...remixParentDetail\n      __typename\n    }\n    __typename\n  }\n}\n\nfragment remixParentDetail on PrintRemixType {\n  id\n  parentPrintName\n  parentPrintAuthor {\n    id\n    slug\n    publicUsername\n    __typename\n  }\n  parentPrint {\n    id\n    name\n    slug\n    datePublished\n    images {\n      id\n      filePath\n      __typename\n    }\n    license {\n      id\n      name\n      disallowRemixing\n      __typename\n    }\n    __typename\n  }\n  __typename\n}\n\"}";

            requestMessage.Content = new StringContent(myContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(requestMessage);
            string responseString = await response.Content.ReadAsStringAsync();


            //Parse JSON

            //Pray?


            HtmlWeb web = new HtmlWeb();
            
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            string creatorName = htmlDoc.DocumentNode
                .SelectNodes("//a[@data-sw-tracking-link-id=\"view-profile\"]")
                .FirstOrDefault()
                .GetAttributeValue("data-sw-tracking-target-entity-id", null);

            Dictionary<string, string> miniProperties = htmlDoc.DocumentNode.SelectNodes("//div[@itemtype=\"http://schema.org/Product\"]/meta")
                .Select(node => new
                {
                    property = node.GetAttributeValue("itemprop", null),
                    content = node.GetAttributeValue("content", null)
                })
                .Where(node => !String.IsNullOrWhiteSpace(node.property))
                .ToDictionary(k => k.property, v => v.content);

            Creator creator = new Creator
            {
                Name = creatorName
            };
            PrusaPrintersSource source = new PrusaPrintersSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = System.Web.HttpUtility.HtmlDecode(miniProperties["name"]),
                Thumbnail = miniProperties["image"],
                Link = miniProperties["url"]
            };
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}




/*


Invoke-WebRequest -Uri "https://www.prusaprinters.org/graphql/" `
-Method "POST" `
-ContentType "application/json" `
-Body "{`"operationName`":`"PrintProfile`",`"variables`":{`"id`":`"30323`"},`"query`":`"query PrintProfile(`$id: ID!) {\n print(id: `$id) {\n id\n slug\n name\n user {\n id\n slug\n email\n donationLinks {\n id\n title\n url\n __typename\n      }\n publicUsername\n avatarFilePath\n __typename\n    }\n ratingAvg\n myRating\n ratingCount\n content\n category {\n id\n path {\n id\n name\n __typename\n      }\n __typename\n    }\n modified\n firstPublish\n datePublished\n hasModel\n summary\n shareCount\n printDuration\n numPieces\n weight\n nozzleDiameters\n usedMaterial\n layerHeights\n materials {\n name\n __typename\n    }\n dateFeatured\n downloadCount\n displayCount\n pdfFilePath\n commentCount\n userGcodeCount\n userGcodesCount\n remixCount\n canBeRated\n inMyCollections\n printer {\n id\n name\n __typename\n    }\n images {\n id\n filePath\n __typename\n    }\n tags {\n name\n id\n __typename\n    }\n thingiverseLink\n filesType\n foundInUserGcodes\n remixParents {\n...remixParentDetail\n __typename\n    }\n __typename\n  }\n}\n\nfragment remixParentDetail on PrintRemixType {\n id\n parentPrintName\n parentPrintAuthor {\n id\n slug\n publicUsername\n __typename\n  }\n parentPrint {\n id\n name\n slug\n datePublished\n images {\n id\n filePath\n __typename\n    }\n license {\n id\n name\n disallowRemixing\n __typename\n    }\n __typename\n  }\n __typename\n }\n`"}" 


Invoke-WebRequest -Uri "https://www.prusaprinters.org/graphql/" `
-Method "POST" `
-ContentType "application/json" `
-Body "{`"operationName`":`"PrintProfile`",`"variables`":{`"id`":`"30323`"},`"query`":`"query PrintProfile(`$id: ID!) {\n print(id: `$id) {\n id\n slug\n name\n }\n`"}" 



*/
