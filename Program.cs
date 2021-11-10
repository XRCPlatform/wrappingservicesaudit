using System;

namespace WrappingServicesAudit
{
    class Program
    {
        static void Main(string[] args)
        {
            Cosigner cosigner = new Cosigner();
            cosigner.ProcessPendingTransactionsAsync().Wait();
        }
    }
}
