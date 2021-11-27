using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WrappingServicesAudit.xrhodium;

namespace WrappingServicesAudit
{
    public class XRhodiumClient : WebApiClient, IxRhodiumClient
    {
        private readonly string serverUri;
        private readonly string authorization;
        public XRhodiumClient(string serverUri, string username, string password, bool UseTor) : base(UseTor)
        {
            this.serverUri = serverUri;
            this.authorization = GetAuthorizationHeader(username, password);
        }

        private string GetAuthorizationHeader(string username, string password)
        {
            string header = $"{username}:{password}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
        }
  

        public async Task<XrcTransaction> DecodeRawHex(string hex)
        {
            string body = $"[{{\"method\": \"decoderawtransaction\",\"params\": [\"{hex}\"], \"id\":{System.DateTimeOffset.UnixEpoch.ToUnixTimeMilliseconds()}}}]";
            var response = await ProcessRequest(body).ConfigureAwait(false);
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var decodedResult = JsonConvert.DeserializeObject<List<DecodeRawXrcTransactionResponse>>(responseString);
                return decodedResult.FirstOrDefault().result;
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {serverUri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }

        private async Task<HttpResponseMessage> ProcessRequest(string body)
        {
            var httpClient = GetHttpClient(serverUri);
            var request = new HttpRequestMessage(HttpMethod.Post, serverUri);
            request.Headers.Add("Auhorization", $"Bearer {authorization}");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            return await httpClient.SendAsync(request).ConfigureAwait(false);
        }

        public async Task<string> SignWithMultisig(string hex, string password)
        {
            string body = $"[{{\"method\": \"fundandsignmultisigtransaction\",\"params\": [\"\",\"{hex}\",\"{password}\"], \"id\":{System.DateTimeOffset.UnixEpoch.ToUnixTimeMilliseconds()}}}]";
            var response = await ProcessRequest(body).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                if (!responseString.Contains("error"))
                {
                    var decodedResult = JsonConvert.DeserializeObject<List<SignedTransaction>>(responseString);
                    return decodedResult.FirstOrDefault().TransactionHex;
                }
                else
                {
                    throw new Exception(responseString);
                } 
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {serverUri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }
    }
}
