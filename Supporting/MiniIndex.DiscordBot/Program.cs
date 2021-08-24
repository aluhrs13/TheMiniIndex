using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;

namespace Supporting.DiscordBot
{
    class Program
    {
        static DiscordClient discord;
        static string AutoCreateKey = "";
        static string DiscordToken = "";

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = DiscordToken,
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async e =>
            {
                string messageText = e.Message.Content.ToLower();
                Console.WriteLine("Message Received - " + messageText);

                string[] splitMessage = messageText.Split(' ');
                foreach(string word in splitMessage)
                {
                    if (word.StartsWith("http"))
                    {
                        Console.WriteLine("Found link, indexing URL - " + word);

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.theminiindex.com/api/minis/create?url=" + word + "&key=" + AutoCreateKey);
                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:44386/api/minis/create?url=" + word + "&key=" + AutoCreateKey);

                        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            Console.WriteLine("Indexing status - " + response.StatusCode);
                            Console.WriteLine("Mini URL - " + await reader.ReadToEndAsync());
                        }
                    }
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}