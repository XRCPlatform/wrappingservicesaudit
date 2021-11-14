using Newtonsoft.Json;

namespace WrappingServicesAudit.bscan
{
    public class GetBalanceResponse
    {

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }

    }
}
