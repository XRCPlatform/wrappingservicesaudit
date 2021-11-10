using MihaZupan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public class WrappingServicesClient
    {
        
        private static HttpClient _httpClient = null;
        private string uri = "https://xrcapi.wrapping.services/listPending";
        private bool useTor;

        public WrappingServicesClient(bool UseTor)
        {
            useTor = UseTor;
        }

        public async Task<Order[]> GetPendingOrdersAsync()
        {
            
            var httpClient = GetHttpClient(uri);
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
            if (response.IsSuccessStatusCode)
            {
                var ordersJson = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(ordersJson);
                return orders.ToArray();
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {uri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
            
        }

        private HttpClient GetHttpClient(string uri)
        {
            if (_httpClient == null)
            {
                var handler = new HttpClientHandler { };
                if (useTor)
                {
                    handler = new HttpClientHandler { 
                        Proxy = new HttpToSocks5Proxy("127.0.0.1", 9050)
                    };
                }

                HttpClient httpClient = new HttpClient(handler);
                httpClient.BaseAddress = new Uri(uri);
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    Public = true
                };
                _httpClient = httpClient;
            }

            return _httpClient;
        }

    }
}
