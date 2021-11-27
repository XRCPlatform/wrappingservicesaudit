using Newtonsoft.Json;

namespace WrappingServicesAudit.xrhodium
{
    public class SignedTransaction
    {
        [JsonProperty(PropertyName = "transactionHex")]
        public string TransactionHex { get; set; }
    }
}
