using System.Collections.Generic;

namespace iDealCore
{
    public class DirectoryRequestResult
    {
        public bool IsError { get; set; }
        public ErrorResult Error { get; set; }

        public List<Issuer> Issuers { get; set; }
    }

    public class Issuer
    {
        public string IssuerId { get; set; }
        public string IssuerName { get; set; }
    }
}
