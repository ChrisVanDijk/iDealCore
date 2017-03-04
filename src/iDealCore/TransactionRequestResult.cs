namespace iDealCore
{
    public class TransactionRequestResult
    {
        public bool IsError { get; set; }
        public ErrorResult Error { get; set; }

        public string IssuerAuthenticationURL { get; set; }
        public string TransactionID { get; set; }
        public string TransactionCreateDateTimestamp { get; set; }
        public string PurchaseID { get; set; }
    }
}
