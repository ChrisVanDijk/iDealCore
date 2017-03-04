namespace iDealCore
{
    public class ErrorResult
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetail { get; set; }
        public string SuggestedAction { get; set; }
        public string ConsumerMessage { get; set; }
    }
}
