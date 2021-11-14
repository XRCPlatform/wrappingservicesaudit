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
    public class WrappingServicesClient: WebApiClient
    {
        private string uri = "https://xrcapi.wrapping.services/listPending";
    
        public WrappingServicesClient(bool UseTor):base(UseTor)
        {
            
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
    }
}
