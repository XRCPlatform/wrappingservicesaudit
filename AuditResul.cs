using System.Collections.Generic;

namespace WrappingServicesAudit
{
    public class AuditResul
    {
        private AuditStatus status = AuditStatus.Pending;

        public AuditResul(Order order)
        {
            Failed = new List<Rule>();
            Order = order;
            status = AuditStatus.Approved;
        }

        public void Fail(Rule rule)
        {
            Failed.Add(rule);
            status = AuditStatus.Failed;
        }

        public List<Rule> Failed { get; }

        public Order Order { get; }

        public AuditStatus Status
        {
            get
            {
                return status;
            }
        }
    }
}