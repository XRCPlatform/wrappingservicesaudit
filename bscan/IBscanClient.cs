using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public interface IBscanClient
    {
        Task<Money> GetBalance(string Address);
        Task<Money> GetEaliestBalance(string userPaidTo);
    }
}