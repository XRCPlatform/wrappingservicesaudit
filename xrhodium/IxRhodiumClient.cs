using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public interface IxRhodiumClient
    {
        public Task<XrcTransaction> DecodeRawHex(string hex);
        public Task<string> SignWithMultisig(string hex, string password);
    }
}