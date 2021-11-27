namespace WrappingServicesAudit
{
    internal class ArgsParser
    {
        private string[] args;

        public ArgsParser(string[] args)
        {
            this.args = args;

            foreach (var item in args)
            {
                var nvp = item.Split('=',System.StringSplitOptions.RemoveEmptyEntries);
                if (nvp[0].Equals("BscanApiKey",System.StringComparison.InvariantCultureIgnoreCase))
                {
                    BscanApiKey = nvp[1];
                }
                if (nvp[0].Equals("XRhodiumWalletPassphrase", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    XRhodiumWalletPassphrase = nvp[1];
                }
                if (nvp[0].Equals("XRhodiumRpcUsername", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    XRhodiumRpcUsername = nvp[1];
                }
                if (nvp[0].Equals("XRhodiumRpcPassword", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    XRhodiumRpcPassword = nvp[1];
                }
                if (nvp[0].Equals("XRhodiumServer", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    XRhodiumServer = nvp[1];
                }
                if (nvp[0].Equals("XRhodiumPort", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    XRhodiumPort = int.Parse(nvp[1]);
                }
            }
        }

        public string BscanApiKey { get; internal set; }
        public string XRhodiumServer { get; internal set; }
        public int XRhodiumPort { get; internal set; }
        public string XRhodiumRpcUsername { get; internal set; }
        public string XRhodiumRpcPassword { get; internal set; }
        public string XRhodiumWalletPassphrase { get; internal set; }
    }
}