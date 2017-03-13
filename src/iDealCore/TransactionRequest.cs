// <copyright file="TransactionRequest.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The Payment protocol initiates the exchange of messages of the actual iDEAL payment initiation.
    /// After the Consumer has chosen iDEAL as a payment method and has selected his bank, the Merchant sends a TransactionRequest to the Acquirer.
    /// Within the iDEAL standards this message is referred to as the AcquirerTransactionRequest.
    /// </summary>
    public class TransactionRequest : RequestBase
    {
#pragma warning disable SA1623 // PropertySummaryDocumentationMustMatchAccessors

        /// <summary>
        /// Valid Merchant URL (not necessarily beginning with http:// or https://) which must redirect the Consumer from the Issuer back to the Merchant
        /// website after authorisation of the transaction by the Consumer.
        /// Example: https://www.webshop.nl/processpayment
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The amount payable in euro.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the product(s) or services being paid for.
        /// This field must not contain characters that can lead to problems (for example those occurring in HTML editing codes). To prevent any
        /// possible errors most iDEAL systems will reject any description that contains HTML-tags and other such code.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Transaction.entranceCode is an 'authentication identifier' to facilitate continuation of the session between Merchant and Consumer,
        /// even if the existing session has been lost.It enables the Merchant to recognise the Consumer associated with a (completed) transaction.
        /// The Transaction.entranceCode is sent to the Merchant in the Redirect. The Transaction.entranceCode must have a minimum variation of 1
        /// million and should comprise letters and/or figures (maximum 40 positions).
        /// The Transaction.entranceCode is created by the Merchant and passed to the Issuer.
        /// </summary>
        public string EntranceCode { get; set; }

        /// <summary>
        /// Unique identification of the order within the Merchant’s system.
        /// Ultimately appears on the payment confirmation(Bank statement / account overview of the Consumer and Merchant).
        /// </summary>
        public string PurchaseId { get; set; }

        /// <summary>
        /// The XML message send by the Merchant to the Acquirer to initiate the payment.
        /// </summary>
        /// <param name="issuerId">The issuer id, for example NLABNA</param>
        /// <returns>Transaction result</returns>
        public async Task<TransactionResponse> Request(string issuerId)
        {
            var result = new TransactionResponse();

            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var sb = new StringBuilder();
            sb.Append("<AcquirerTrxReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Issuer>");
            sb.Append("<issuerID>" + issuerId + "</issuerID>");
            sb.Append("</Issuer>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + this.MerchantId + "</merchantID>");
            sb.Append("<subID>" + this.MerchantSubId + "</subID>");
            sb.Append("<merchantReturnURL>" + this.ReturnUrl + "</merchantReturnURL>");
            sb.Append("</Merchant>");
            sb.Append("<Transaction>");
            sb.Append("<purchaseID>" + this.PurchaseId + "</purchaseID>");
            sb.Append("<amount>" + this.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "</amount>");
            sb.Append("<currency>EUR</currency>");
            sb.Append("<expirationPeriod>PT1H</expirationPeriod>");
            sb.Append("<language>nl</language>");
            sb.Append("<description>" + this.Description + "</description>");
            sb.Append("<entranceCode>" + this.EntranceCode + "</entranceCode>");
            sb.Append("</Transaction>");
            sb.Append("</AcquirerTrxReq>");

            string digest = DigestValue(sb.ToString());
            string signature = this.SignXml(digest);

            sb.Clear();
            sb.Append("<AcquirerTrxReq xmlns=\"http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1\" version=\"3.3.1\">");
            sb.Append("<createDateTimestamp>" + timestamp + "</createDateTimestamp>");
            sb.Append("<Issuer>");
            sb.Append("<issuerID>" + issuerId + "</issuerID>");
            sb.Append("</Issuer>");
            sb.Append("<Merchant>");
            sb.Append("<merchantID>" + this.MerchantId + "</merchantID>");
            sb.Append("<subID>" + this.MerchantSubId + "</subID>");
            sb.Append("<merchantReturnURL>" + this.ReturnUrl + "</merchantReturnURL>");
            sb.Append("</Merchant>");
            sb.Append("<Transaction>");
            sb.Append("<purchaseID>" + this.PurchaseId + "</purchaseID>");
            sb.Append("<amount>" + this.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "</amount>");
            sb.Append("<currency>EUR</currency>");
            sb.Append("<expirationPeriod>PT1H</expirationPeriod>");
            sb.Append("<language>nl</language>");
            sb.Append("<description>" + this.Description + "</description>");
            sb.Append("<entranceCode>" + this.EntranceCode + "</entranceCode>");
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
            sb.Append("</AcquirerTrxReq>");

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

            result.IssuerAuthenticationURL = XmlNodeValue("issuerAuthenticationURL", data);
            result.TransactionID = XmlNodeValue("transactionID", data);
            result.TransactionCreateDateTimestamp = XmlNodeValue("transactionCreateDateTimestamp", data);
            result.PurchaseID = XmlNodeValue("purchaseID", data);

            return result;
        }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
    }
}
