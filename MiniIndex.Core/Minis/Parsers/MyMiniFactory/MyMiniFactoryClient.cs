using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MiniIndex.Core.Minis.Parsers.MyMiniFactory
{
    public class MyMiniFactoryClient
    {
        public MyMiniFactoryClient(HttpClient httpClient, IConfiguration configuration)
        {
            httpClient.BaseAddress = new Uri("https://www.myminifactory.com/api/v2/");
            _httpClient = httpClient;

            _apiKey = configuration["MyMiniFactoryToken"];
        }

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public async Task<MyMiniFactoryModel.RootObject> GetObject(string objectId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"objects/{objectId}?key={_apiKey}");
            HttpContent responseContent = response.Content;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    string result = await reader.ReadToEndAsync();
                    MyMiniFactoryModel.RootObject rootObject= JsonConvert.DeserializeObject<MyMiniFactoryModel.RootObject>(result);

                    return rootObject;
                }
            }

            return null;
        }
    }
}
