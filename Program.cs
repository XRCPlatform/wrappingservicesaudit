using System;

namespace WrappingServicesAudit
{
    class Program
    {
        static void Main(string[] args)
        {
            BscanClient bscan = new BscanClient("EVI33RE8626MU3J2AEAB34RCBYWCTSX4EK", false);
            Auditor auditor = new Auditor(bscan);
            Cosigner cosigner = new Cosigner(auditor);
            cosigner.ProcessPendingTransactionsAsync().Wait();
        }
    }
}
