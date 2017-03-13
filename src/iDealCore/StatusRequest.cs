// <copyright file="StatusRequest.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// To verify whether a transaction was successful the Merchant will start the Query protocol by sending a StatusRequest to the Acquirer.
    /// </summary>
    public class StatusRequest : RequestBase
    {
        /// <summary>
        /// Verify whether a transaction was successful
        /// </summary>
        /// <param name="transactionId">StatusRequestResult</param>
        /// <returns>Status result</returns>
        public async Task<StatusResponse> Request(string transactionId)
        {
            var result = new StatusResponse();

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var sb = new StringBuilder();
            sb.Append("<AcquirerStatusReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + this.MerchantId + "</merchantID>");
            sb.Append("<subID>" + this.MerchantSubId + "</subID>");
            sb.Append("</Merchant>");
            sb.Append("<Transaction>");
            sb.Append("<transactionID>" + transactionId + "</transactionID>");
            sb.Append("</Transaction>");
            sb.Append("</AcquirerStatusReq>");

            string digest = DigestValue(sb.ToString());
            var signature = this.SignXml(digest);

            sb.Clear();
            sb.Append("<AcquirerStatusReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + this.MerchantId + "</merchantID>");
            sb.Append("<subID>" + this.MerchantSubId + "</subID>");
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
            sb.Append("<KeyName>" + this.SigningCertificate.Thumbprint + "</KeyName>"); // == priv. cert. thumb
            sb.Append("</KeyInfo>");
            sb.Append("</Signature>");

            sb.Append("</AcquirerStatusReq>");

            string data = string.Empty;

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.ExpectContinue = false;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("iDEAL Connector Core v1.0");
                var response2 = client.PostAsync(this.IDealURL, new System.Net.Http.StringContent(sb.ToString(), Encoding.UTF8, "text/xml"));
                data = await response2.Result.Content.ReadAsStringAsync();
            }

            var errorCode = XmlNodeValue("errorCode", data);
            if (!string.IsNullOrEmpty(errorCode))
            {
                result.IsError = true;
                var errorResult = new ErrorResponse();
                errorResult.ErrorCode = errorCode;
                errorResult.ErrorMessage = XmlNodeValue("errorMessage", data);
                errorResult.ErrorDetail = XmlNodeValue("errorDetail", data);
                errorResult.SuggestedAction = XmlNodeValue("suggestedAction", data);
                errorResult.ConsumerMessage = XmlNodeValue("consumerMessage", data);
                result.Error = errorResult;

                return result;
            }

            if (this.ValidateSignature && !this.VerifySignature(data))
            {
                throw new Exception("Cannot verify signature!");
            }

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
