using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public class Auditor : IAuditor
    {
        private readonly IBscanClient bscanClient;
        public Auditor(IBscanClient BscanClient)
        {
            bscanClient = BscanClient;
        }
        public async Task<AuditResul> Audit(Order order)
        {
            var balanceRule =  await AuditBalanceOnchain(order);
            if (balanceRule.Status == AuditStatus.Approved)
            {
                //decode raw xrc transaction and compare amount in xrc to wXRC amount they must match
                //if failed retrun failed rule result
            }

            return balanceRule;
        }

        private async Task<AuditResul> AuditBalanceOnchain(Order order)
        {
            var balance = await bscanClient.GetBalance(order.UserPaidTo);
            //https://api.bscscan.com/ check the bsc address was properly burned meaning present value is = 0
            //verify order amount matches incoming wXRC amount before it was burned
            if (!balance.Equals(new Money(0)))
            {
                var result = new AuditResul(order);
                result.Fail(new Rule("bsc address has more than 0 wXRC, meaning coin burn failed."));
                return result;
            }
            else
            {
                return new AuditResul(order);
            }
        }
    }
}
