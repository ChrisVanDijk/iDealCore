using System;
using System.Text;
using System.Threading.Tasks;

namespace iDealCore
{
    public class TransactionRequest : RequestBase
    {
        public string ReturnUrl { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string EntranceCode { get; set; }
        public string OrderId { get; set; }

        public async Task<TransactionRequestResult> Request(string issuerId)
        {
            var result = new TransactionRequestResult();

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var sb = new StringBuilder();
            sb.Append("<AcquirerTrxReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
			sb.Append("<createDateTimestamp>" + timestamp +  "</createDateTimestamp>");
			sb.Append("<Issuer>");
			sb.Append("<issuerID>"+ issuerId + "</issuerID>");
			sb.Append("</Issuer>");
			sb.Append("<Merchant>");
			sb.Append("<merchantID>"+ MerchantId + "</merchantID>");
            sb.Append("<subID>" + MerchantSubId + "</subID>");
            sb.Append("<merchantReturnURL>" + ReturnUrl + "</merchantReturnURL>");
			sb.Append("</Merchant>");
			sb.Append("<Transaction>");
            sb.Append("<purchaseID>" + OrderId + "</purchaseID>");
            sb.Append("<amount>" + Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "</amount>");
			sb.Append("<currency>EUR</currency>");
			sb.Append("<expirationPeriod>PT1H</expirationPeriod>");
			sb.Append("<language>nl</language>");
			sb.Append("<description>" + Description + "</description>");
            sb.Append("<entranceCode>" + EntranceCode + "</entranceCode>");
			sb.Append("</Transaction>");
            sb.Append("</AcquirerTrxReq>");

            string digest = DigestValue(sb.ToString());
            string signature = SignXml(digest);

            sb.Clear();
            sb.Append("<AcquirerTrxReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Issuer>");
            sb.Append("<issuerID>" + issuerId + "</issuerID>");
            sb.Append("</Issuer>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + MerchantId + "</merchantID>");
            sb.Append("<subID>" + MerchantSubId + "</subID>");
            sb.Append("<merchantReturnURL>" + ReturnUrl + "</merchantReturnURL>");
            sb.Append("</Merchant>");
            sb.Append("<Transaction>");
            sb.Append("<purchaseID>" + OrderId + "</purchaseID>");
            sb.Append("<amount>" + Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "</amount>");
            sb.Append("<currency>EUR</currency>");
            sb.Append("<expirationPeriod>PT1H</expirationPeriod>");
            sb.Append("<language>nl</language>");
            sb.Append("<description>" + Description + "</description>");
            sb.Append("<entranceCode>" + EntranceCode + "</entranceCode>");
            sb.Append("</Transaction>");
            sb.Append("<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">");
            sb.Append("<SignedInfo>");
            sb.Append("<CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod>");
            sb.Append("<SignatureMethod Algorithm=\"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256\"></SignatureMethod>");
            sb.Append("<Reference URI=\"\">");
            sb.Append("<Transforms>");
            sb.Append("<Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\"></Transform>");
            sb.Append("</Transforms>");
            sb.Append("<DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha256\"></DigestMethod>");
            sb.Append("<DigestValue>" + digest + "</DigestValue>");
            sb.Append("</Reference>");
            sb.Append("</SignedInfo>");
            sb.Append("<SignatureValue>" + signature + "</SignatureValue>");
            sb.Append("<KeyInfo>");
            sb.Append("<KeyName>" + SigningCertificate.Thumbprint + "</KeyName>"); // == priv. cert. thumb
            sb.Append("</KeyInfo>");
            sb.Append("</Signature>");
            sb.Append("</AcquirerTrxReq>");

            string data = string.Empty;

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.ExpectContinue = false;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("iDEAL Connector Core v1.0");
                var response2 = client.PostAsync(IDealURL, new System.Net.Http.StringContent(sb.ToString(), Encoding.UTF8, "text/xml"));
                data = await response2.Result.Content.ReadAsStringAsync();
            };

            var errorCode = XmlNodeValue("errorCode", data);
            if (!string.IsNullOrEmpty(errorCode))
            {
                result.IsError = true;
                var errorResult = new ErrorResult();
                errorResult.ErrorCode = errorCode;
                errorResult.ErrorMessage = XmlNodeValue("errorMessage", data);
                errorResult.ErrorDetail = XmlNodeValue("errorDetail", data);
                errorResult.SuggestedAction = XmlNodeValue("suggestedAction", data);
                errorResult.ConsumerMessage = XmlNodeValue("consumerMessage", data);
                result.Error = errorResult;

                return result;
            }

            if (ValidateSignature && !VerifySignature(data))
            {
                throw new Exception("Cannot verify signature!");
            }

            result.IssuerAuthenticationURL = XmlNodeValue("issuerAuthenticationURL", data);
            result.TransactionID = XmlNodeValue("transactionID", data);
            result.TransactionCreateDateTimestamp = XmlNodeValue("transactionCreateDateTimestamp", data);
            result.PurchaseID = XmlNodeValue("purchaseID", data);

            return result;
        }

    }
}
