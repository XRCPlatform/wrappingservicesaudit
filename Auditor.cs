using System;
using System.Collections.Generic;
using System.Text;

namespace WrappingServicesAudit
{
    public class Auditor
    {
        public static AuditResul Audit(Order order)
        {
            //https://api.bscscan.com/ check the bsc address was properly burned meaning present value is = 0
            //verify order amount matches incoming wXRC amount before it was burned
            throw new NotImplementedException("Audit not yet implemented");
        }
    }
}
