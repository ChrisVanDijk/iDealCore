namespace iDealCore
{
    public class StatusRequestResult
    {
        public bool IsError { get; set; }
        public ErrorResult Error { get; set; }

        public string TransactionID { get; set; }
        public string Status { get; set; }
        public string StatusDateTimestamp { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public string ConsumerName { get; set; }
        public string ConsumerIBAN { get; set; }
        public string ConsumerBIC { get; set; }
    }
}
