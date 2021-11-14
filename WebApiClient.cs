using MihaZupan;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WrappingServicesAudit
{
    public class WebApiClient
    {
        public WebApiClient(bool UseTor)
        {
            useTor = UseTor;
        }

        private HttpClient _httpClient = null;
        private readonly bool useTor;

        internal HttpClient GetHttpClient(string uri)
        {
            if (_httpClient == null)
            {
                var handler = new HttpClientHandler { };
                if (useTor)
                {
                    handler = new HttpClientHandler
                    {
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