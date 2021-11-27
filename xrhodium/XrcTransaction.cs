namespace WrappingServicesAudit
{
    public class XrcTransaction
    {
        
        public string txid { get; set; }
        public string hash { get; set; }
        public int version { get; set; }
        public int size { get; set; }
        public int vsize { get; set; }
        public int weight { get; set; }
        public int locktime { get; set; }
        public Vin[] vin { get; set; }
        public Vout[] vout { get; set; }

        public class Vin
        {
            public string txid { get; set; }
            public int vout { get; set; }
            public Scriptsig scriptSig { get; set; }
            public long sequence { get; set; }
        }

        public class Scriptsig
        {
            public string asm { get; set; }
            public string hex { get; set; }
        }

        public class Vout
        {
            public decimal value { get; set; }
            public int n { get; set; }
            public Scriptpubkey scriptPubKey { get; set; }
        }

        public class Scriptpubkey
        {
            public string asm { get; set; }
            public string hex { get; set; }
            public int reqSigs { get; set; }
            public string type { get; set; }
            public string[] addresses { get; set; }
        }

    }
}