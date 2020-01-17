using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MiniIndex.Core.Minis.Parsers.Thingiverse
{
    public class ThingiverseClient
    {
        public ThingiverseClient(HttpClient httpClient, IConfiguration configuration)
        {
            httpClient.BaseAddress = new Uri("https://api.thingiverse.com");
            _httpClient = httpClient;

            _apiKey = configuration["ThingiverseToken"];
        }

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public async Task<ThingiverseModel.Thing> GetThing(string thingId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"things/{thingId}/?access_token={_apiKey}");
            HttpContent responseContent = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    string result = await reader.ReadToEndAsync();
                    ThingiverseModel.Thing thing = JsonConvert.DeserializeObject<ThingiverseModel.Thing>(result);

                    return thing;
                }
            }

            return null;
        }
    }
}