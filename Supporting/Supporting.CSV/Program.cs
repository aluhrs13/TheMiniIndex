using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace MiniIndex.CSV
{
    class Program
    {
        static string AutoCreateKey = "";

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            bool addThingiverse = false;
            Dictionary<string, int> DomainCount = new Dictionary<string, int>();
            int i = 0;


            if (args.Length == 1)
            {
                if(args[0] == "thingiverse")
                {
                    addThingiverse = true;
                }
            }

            HtmlWeb web = new HtmlWeb();

            string folder = "C:\\Users\\aluhr\\Downloads\\MMBest\\";
            string[] sheets = new string[]{ "Bestiary.html", "PC NPC Classes.html", "Uncategorized (Plz Send Help).html" };


            foreach(string sheet in sheets)
            {
                HtmlDocument htmlDoc = web.Load(folder + sheet);
                HtmlNodeCollection links = htmlDoc.DocumentNode.SelectNodes("//a");

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

                    if (linkURL.Host.Contains("thingiverse") && addThingiverse == false)
                    {
                        continue;
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
                }
            }


            foreach (KeyValuePair<string, int> Site in DomainCount)
            {
                Console.WriteLine(Site.Key + " - " + Site.Value);
            }
        }
    }
}
