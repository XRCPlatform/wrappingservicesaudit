using MihaZupan;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public class WrappingServicesClient : WebApiClient, IWrappingServicesClient
    {
        private string listPendingUri = "https://xrcapi.wrapping.services/listPending";
        private string postSigUri = "https://xrcapi.wrapping.services/postSig";
        private string sendTxUri = "https://xrcapi.wrapping.services/sendTx";

        public WrappingServicesClient(bool UseTor) : base(UseTor)
        {

        }

        public async Task<Order[]> GetPendingOrdersAsync()
        {

            var httpClient = GetHttpClient(listPendingUri);
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, listPendingUri))
                .ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var ordersJson = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(ordersJson);
                return orders.ToArray();
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {listPendingUri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }

        public async Task<bool> PostSig(Order order, string hex)
        {
            var httpClient = GetHttpClient(listPendingUri);
            var request = new HttpRequestMessage(HttpMethod.Post, postSigUri);
            //request.Headers.Add("Accept", "application/x-www-form-urlencoded");
            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                                    {
                                        { "orderId", order.OrderId },
                                        { "sig2", hex }
                                    });

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {postSigUri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }

        public async Task<bool> SendTx(string orderId)
        {
            dynamic body = new JObject();
            body.OrderId = orderId;
            var httpClient = GetHttpClient(listPendingUri);
            var request = new HttpRequestMessage(HttpMethod.Post, sendTxUri);
            //request.Headers.Add("Accept", "application/x-www-form-urlencoded");
            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                                    {
                                        { "orderId", orderId }
                                    });
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {sendTxUri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }
    }
}
