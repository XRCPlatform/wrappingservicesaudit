using System;
using System.Collections.Generic;
using System.Text;

namespace WrappingServicesAudit
{

    public class DecodeRawXrcTransactionResponse
    {
        public XrcTransaction result { get; set; }
        public object error { get; set; }
        public int id { get; set; }
    }
}
