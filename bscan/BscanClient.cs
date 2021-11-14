using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WrappingServicesAudit.bscan;

namespace WrappingServicesAudit
{
    public class BscanClient : WebApiClient, IBscanClient
    {
        private readonly string apiKey;

        public BscanClient(string ApiKey, bool useTor) : base(useTor)
        {
            apiKey = ApiKey;
        }

        public async Task<Money> GetBalance(string Address)
        {
            string uri = $"https://api.bscscan.com/api?module=account&tag=latest&action=balance&address={Address}&apikey={apiKey}";
            var httpClient = GetHttpClient(uri);
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var balance = JsonConvert.DeserializeObject<GetBalanceResponse>(json);
                return Money.Parse(balance.Result);
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {uri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }
    }
}
