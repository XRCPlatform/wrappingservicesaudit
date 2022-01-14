using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public class Cosigner
    {
        private readonly IAuditor _auditor;
        private readonly IWrappingServicesClient _wrappingClient;
        private readonly IxRhodiumClient _xRhodium;

        public Cosigner(IAuditor auditor, IWrappingServicesClient wrappingClient, IxRhodiumClient xRhodiumlient)
        {
            this._auditor = auditor;
            this._wrappingClient = wrappingClient;
            this._xRhodium = xRhodiumlient;
        }

        public async Task ProcessPendingTransactionsAsync(string walletPassphrase)
        {
            var orders = await _wrappingClient.GetPendingOrdersAsync().ConfigureAwait(false);
            foreach (var order in orders)
            {
                
                try
                {
                    //if (order.OrderId.Contains("ba87be2"))
                    if (order.OrderId != null)// will never be 
                    {
                        Console.WriteLine($"Processing {order.OrderId}");
                        var audit = await _auditor.Audit(order);
                        if (audit.Status == AuditStatus.Approved)
                        {
                            var signed = await _xRhodium.SignWithMultisig(order.RawXRCTransaction, walletPassphrase);
                            await _wrappingClient.PostSig(order, signed);
                            await _wrappingClient.SendTx(order.OrderId);
                        }
                        else
                        {
                            foreach (var rule in audit.Failed)
                            {
                                Console.WriteLine($"{order.OrderId} failed {rule.Description}");
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping {order.OrderId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{order.OrderId} failed with exception {ex}");
                    
                }
            }
        }
    }
}
