using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace DigitalCreditBroker.Provider.SAP.HanaCloud
{
    public class ODataClient
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string CompanyCode { get; set; }

        public ODataClient()
        {

        }

        public ODataClient(string baseUrl, string apiKey, string companyCode)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            CompanyCode = companyCode;
        }

        public async Task<List<T>> GetAll<T>(string odata)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetStringAsync(UrlExtender(odata));
                var result = JsonConvert.DeserializeObject<ODataResponse<T>>(response);

                return result.D.Results;
            }
        }

        public async Task<T> Get<T>(string url, string key)
        {
            using (var client = CreateClient())
            {
                var response = await client.GetStringAsync($"{UrlExtender(url)}({key})");
                var result = JsonConvert.DeserializeObject<T>(response);

                return result;
            }
        }

        protected HttpClient CreateClient()
        {
            CheckSettings();
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(handler)
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

        private string UrlExtender(string url) => $"/s4hanacloud/sap/opu/odata/sap/{url}";
    }
}
