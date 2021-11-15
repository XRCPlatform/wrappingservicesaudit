using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            string body = $"[{{\"method\": \"decoderawtransaction\",\"params\": [\"{hex}\"], \"id\":{System.DateTimeOffset.UnixEpoch}\"}}]";
            var httpClient = GetHttpClient(serverUri);
            var request = new HttpRequestMessage(HttpMethod.Post, serverUri);
            request.Headers.Add("Auhorization", $"Bearer {authorization}");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                var decodedString = await response.Content.ReadAsStringAsync();
                var decodedResult = JsonConvert.DeserializeObject<DecodeRawXrcTransactionResult>(decodedString);
                return decodedResult.result;
            }
            else
            {
                throw new Exception($"Got http [{response.StatusCode}] calling {serverUri}, message {response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()}");
            }
        }

        public void SignWithMultisig(string hex)
        {
            throw new NotImplementedException();
        }
    }
}
