namespace WrappingServicesAudit
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new ArgsParser(args);

            BscanClient bscan = new BscanClient(settings.BscanApiKey, false);
            XRhodiumClient xRhodium = new XRhodiumClient($"http://{settings.XRhodiumServer}:{settings.XRhodiumPort}/", settings.XRhodiumRpcUsername, settings.XRhodiumRpcPassword, false);
            var ws = new WrappingServicesClient(false);
            Auditor auditor = new Auditor(bscan, ws, xRhodium);
            Cosigner cosigner = new Cosigner(auditor, ws, xRhodium);
            cosigner.ProcessPendingTransactionsAsync(settings.XRhodiumWalletPassphrase).Wait();
        }
    }
}
