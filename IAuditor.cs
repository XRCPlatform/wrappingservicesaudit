using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public interface IAuditor
    {
        Task<AuditResul> Audit(Order order);
    }
}