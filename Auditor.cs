using System.Threading.Tasks;

namespace WrappingServicesAudit
{
    public class Auditor : IAuditor
    {
        private readonly IBscanClient _bscanClient;
        private IWrappingServicesClient _wrappingServices;
        private IxRhodiumClient _xRhodium;

        public Auditor(IBscanClient bscanClient, IWrappingServicesClient wrappingServicesClient, IxRhodiumClient xRhodium) 
        {
            this._bscanClient = bscanClient;
            this._wrappingServices = wrappingServicesClient;
            this._xRhodium = xRhodium;
        }

        public async Task<AuditResul> Audit(Order order)
        {
            var balanceRule =  await AuditEnsureCurrentCoinsBurnedOnchain(order);
            if (balanceRule.Status == AuditStatus.Approved)
            {

                var xrc = await _xRhodium.DecodeRawHex(order.RawXRCTransaction);
                var outputs = xrc.vout;
                foreach (var item in outputs)
                {
                    var address = item.scriptPubKey.addresses[0];
                    if (address == order.UserAddress)
                    {
                        //decode raw xrc transaction and compare amount in xrc to wXRC amount they must match
                        //if failed retrun failed rule result
                        var xrcValueRule = await AuditXrcTransactionValueMatchesContractValue(order, item.value);
                        if (xrcValueRule.Status != AuditStatus.Approved)
                        {
                            return xrcValueRule;
                        }
                        // ensure that bsc address got exact amout of wXRC coins deposited on start
                        var startingBalanceRule =  await AuditStartingBalanceOnchain(order);
                        if (startingBalanceRule.Status != AuditStatus.Approved)
                        {
                            return startingBalanceRule;
                        }                            
                    }
                }
            }
            return balanceRule;
        }

        private async Task<AuditResul> AuditEnsureCurrentCoinsBurnedOnchain(Order order)
        {
            var balance = await _bscanClient.GetBalance(order.UserPaidTo);
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

        private async Task<AuditResul> AuditStartingBalanceOnchain(Order order)
        {
            var balance = await _bscanClient.GetEaliestBalance(order.UserPaidTo);
            if (!balance.Equals(new Money(order.Amount, MoneyUnit.XRC)))
            {
                var result = new AuditResul(order);
                result.Fail(new Rule("bsc address has unequal amount of wXRC based on contract."));
                return result;
            }
            else
            {
                return new AuditResul(order);
            }
        }

        private async Task<AuditResul> AuditXrcTransactionValueMatchesContractValue(Order order, decimal value)
        {
            var xrcValue = new Money(value, MoneyUnit.XRC);
            if (!xrcValue.Equals(new Money(order.Amount, MoneyUnit.XRC)))
            {
                var result = new AuditResul(order);
                result.Fail(new Rule("XRC Transaction transfers amount different to value of contract."));
                return result;
            }
            else
            {
                return new AuditResul(order);
            }
        }
    }
}
