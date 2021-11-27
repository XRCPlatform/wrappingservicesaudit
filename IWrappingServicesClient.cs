using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public interface IWrappingServicesClient
    {
        Task<Order[]> GetPendingOrdersAsync();
        Task<bool> PostSig(Order order, string hex);
        Task<bool> SendTx(string orderId);
    }
}