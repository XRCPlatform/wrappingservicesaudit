using Newtonsoft.Json;

namespace WrappingServicesAudit
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "userAddress")]
        public string UserAddress { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "confirmed")]
        public bool Confirmed { get; set; }

        [JsonProperty(PropertyName = "userPaidTo")]
        public string UserPaidTo { get; set; }

        [JsonProperty(PropertyName = "rawtx")]
        public string RawXRCTransaction { get; set; }

    }
}
