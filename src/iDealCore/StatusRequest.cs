using System;
using System.Text;
using System.Threading.Tasks;

namespace iDealCore
{
    public class StatusRequest : RequestBase
    {
        public async Task<StatusRequestResult> Request(string transactionId)
        {
            var result = new StatusRequestResult();

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var sb = new StringBuilder();
            sb.Append("<AcquirerStatusReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + MerchantId + "</merchantID>");
            sb.Append("<subID>" + MerchantSubId + "</subID>");
            sb.Append("</Merchant>");
            sb.Append("<Transaction>");
            sb.Append("<transactionID>" + transactionId + "</transactionID>");
            sb.Append("</Transaction>");
            sb.Append("</AcquirerStatusReq>");

            string digest = DigestValue(sb.ToString());
            var signature = SignXml(digest);

            sb.Clear();
            sb.Append("<AcquirerStatusReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + MerchantId + "</merchantID>");
            sb.Append("<subID>" + MerchantSubId + "</subID>");
            sb.Append("</Merchant>");
            sb.Append("<Transaction>");
            sb.Append("<transactionID>" + transactionId + "</transactionID>");
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

            sb.Append("</AcquirerStatusReq>");

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

            //Console.WriteLine(data);

            result.TransactionID = XmlNodeValue("transactionID", data);
            result.StatusDateTimestamp = XmlNodeValue("statusDateTimestamp", data);
            result.Status = XmlNodeValue("status", data);

            result.Currency = XmlNodeValue("currency", data);
            string amount = XmlNodeValue("amount", data);
            if (!string.IsNullOrEmpty(amount))
            {
                result.Amount = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture);
            }

            result.ConsumerName = XmlNodeValue("consumerName", data);
            result.ConsumerIBAN = XmlNodeValue("consumerIBAN", data);
            result.ConsumerBIC = XmlNodeValue("consumerBIC", data);

            return result;
        }
    }
}
