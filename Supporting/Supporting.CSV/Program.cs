using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace MiniIndex.CSV
{
    class Program
    {
        static string AutoCreateKey = "";

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Dictionary<string, int> DomainCount = new Dictionary<string, int>();
            int i = 0;

            HtmlWeb web = new HtmlWeb();

            string folder = "C:\\Users\\aluhr\\Downloads\\MMBest\\";
            string[] sheets = new string[] { "Bestiary.html" };//, "PC NPC Classes.html", "Uncategorized (Plz Send Help).html" };

            Dictionary<string, List<string>> TagLookup = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> RemoveList = new Dictionary<string, List<string>>();

            HtmlDocument htmlDoc1 = web.Load(@"C:\Users\aluhr\Downloads\MMBest\The Mini Index Import.html");
            HtmlNodeCollection rows1 = htmlDoc1.DocumentNode.SelectNodes("//tr");

            foreach (HtmlNode row in rows1)
            {
                if (row.Element("td") != null)
                {
                    KeyValuePair<string, List<string>> CurrentItem = new KeyValuePair<string, List<string>>(row.Element("td").InnerText.Replace("&#39;", "'").ToLowerInvariant(), new List<string>());
                    KeyValuePair<string, List<string>> CurrentRemoval = new KeyValuePair<string, List<string>>(row.Element("td").InnerText.Replace("&#39;", "'").ToLowerInvariant(), new List<string>());

                    foreach (var nNode in row.DescendantNodes())
                    {
                        if(nNode.NodeType == HtmlNodeType.Text && !nNode.XPath.Contains("th"))
                        {
                            if (nNode.InnerText.StartsWith("-"))
                            {
                                CurrentRemoval.Value.Add(nNode.InnerText.Replace("&#39;", "'").Remove(0, 1).ToLowerInvariant());
                            }
                            else
                            {
                                CurrentItem.Value.Add(nNode.InnerText.Replace("&#39;", "'").ToLowerInvariant());
                            }
                        }
                    }
                    CurrentItem.Value.RemoveAt(0);
                    TagLookup.Add(CurrentItem.Key, CurrentItem.Value.Distinct().ToList());
                    if(CurrentRemoval.Value.Count > 0)
                    {
                        RemoveList.Add(CurrentRemoval.Key, CurrentRemoval.Value.Distinct().ToList());
                    }
                }
            }


            string CurrentTag = "";
            foreach (string sheet in sheets)
            {
                HtmlDocument htmlDoc = web.Load(folder + sheet);
                HtmlNodeCollection rows = htmlDoc.DocumentNode.SelectNodes("//tr");

                foreach(HtmlNode row in rows)
                {
                    if (row.Element("td") != null)
                    {
                        //First non-th column is the tag name.
                        CurrentTag = row.Element("td").InnerText;
                        foreach (var nNode in row.DescendantNodes())
                        {
                            //Every link on that row is to that thing
                            if (nNode.Attributes.Contains("href"))
                            {
                                Uri linkURL = new Uri(nNode.GetAttributeValue("href", "https://not.real/"));
                                Console.WriteLine(CurrentTag+" - "+linkURL.ToString());
                                //TODO: Submit it

                                //TODO: If submission works, or it exists already add and remove tags as needed.
                                foreach (string tag in TagLookup[CurrentTag.ToLowerInvariant()])
                                {
                                    //TODO: Create the tag
                                }

                                //TODO: Iterate through remove list
                            }
                        }
                    }
                }

                //HtmlNodeCollection links = htmlDoc.DocumentNode.SelectNodes("//a");

                /*
                foreach (HtmlNode link in links)
                {
                    Uri linkURL = new Uri(link.GetAttributeValue("href", String.Empty));

                    if (!DomainCount.ContainsKey(linkURL.Host))
                    {
                        DomainCount.Add(linkURL.Host, 1);
                    }
                    else
                    {
                        DomainCount[linkURL.Host]++;
                    }


                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.theminiindex.com/api/minis/create?url=" + linkURL.ToString() + "&key=" + AutoCreateKey);
                    Console.WriteLine("Looking at - " + linkURL);

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            Console.WriteLine("\t Status - " + response.StatusCode);
                            Console.WriteLine("\t Mini URL - " + await reader.ReadToEndAsync());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\t Exception - " + ex.Message);
                    }

                    i++;
                    if (i >= 50)
                    {
                        i = 0;
                        Console.WriteLine("Waiting...");
                        System.Threading.Thread.Sleep(60000);
                    }
                }*/
            }


            foreach (KeyValuePair<string, int> Site in DomainCount)
            {
                Console.WriteLine(Site.Key + " - " + Site.Value);
            }
        }
    }
}
