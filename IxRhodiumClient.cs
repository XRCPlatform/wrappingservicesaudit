using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public interface IxRhodiumClient
    {
        public Task<XrcTransaction> DecodeRawHex(string hex);
        public void SignWithMultisig(string hex);
    }
}