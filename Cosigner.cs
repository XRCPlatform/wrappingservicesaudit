using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public class Cosigner
    {
        private readonly IAuditor auditor;
        public Cosigner(IAuditor auditor)
        {
            this.auditor = auditor;
        }

        public async Task ProcessPendingTransactionsAsync()
        {
            WrappingServicesClient wrappingClient = new WrappingServicesClient(false);
            var orders = await wrappingClient.GetPendingOrdersAsync().ConfigureAwait(false);

            foreach (var order in orders)
            {
                var audit = await auditor.Audit(order);
                if (audit.Status == AuditStatus.Approved)
                {
                    //sign  transaction over blockore client
                    //post transaction back to W.S
                }
                else
                {
                    //what log?
                }
            }
        }
    }
}
