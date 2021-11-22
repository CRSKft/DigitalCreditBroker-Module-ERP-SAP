using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DigitalCreditBroker.Provider.SAP.BusinessOne
{
    public class ODataClient
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public ODataClient()
        {

        }

        public ODataClient(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
        }

        public async Task<List<T>> GetAll<T>(string odata)
        {
            using (var client = CreateClient())
            {
                var finalList = new List<T>();

                var response = await client.GetStringAsync(UrlExtender(odata));
                var result = JsonConvert.DeserializeObject<ODataResponse<T>>(response);

                finalList.AddRange(result.Value);

                while (!string.IsNullOrEmpty(result.NextLink))
                {
                    response = await client.GetStringAsync(UrlExtender(result.NextLink));
                    result = JsonConvert.DeserializeObject<ODataResponse<T>>(response);
                    finalList.AddRange(result.Value);
                }

                return finalList;
            }
        }

        public async Task<T> Get<T>(string url, string key)
        {
            using (var client = CreateClient())
            {
                var finalList = new List<T>();

                var response = await client.GetStringAsync($"{UrlExtender(url)}({key})");
                var result = JsonConvert.DeserializeObject<T>(response);

                return result;
            }
        }

        protected HttpClient CreateClient()
        {
            CheckSettings();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            //httpClient.DefaultRequestHeaders.Add("demoDb", "string");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("APIKey", ApiKey);
            return httpClient;
        }

        private void CheckSettings()
        {
            if (string.IsNullOrEmpty(ApiKey))
                throw new ArgumentNullException("API Key");

            if (string.IsNullOrEmpty(BaseUrl))
                throw new ArgumentNullException("Base URL");
        }

        private string UrlExtender(string url) => $"/sapb1/b1s/v2/{url}";
    }
}
