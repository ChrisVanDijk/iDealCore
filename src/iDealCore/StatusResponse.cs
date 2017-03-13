// <copyright file="StatusResponse.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    /// <summary>
    /// This message communicates the status of the transaction (related to the transactionID which was sent in the StatusRequest) to the Merchant.
    /// If the status equals “Success” some additional fields with information about the Consumer are added to the message.
    /// If necessary, this information can be used to refund (part of) the transaction amount to the Consumer
    /// </summary>
    public class StatusResponse
    {
#pragma warning disable SA1623 // PropertySummaryDocumentationMustMatchAccessors

        /// <summary>
        /// Gets or sets a value indicating whether an error occured with the request
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Gets or sets a value with more information about the error
        /// </summary>
        public ErrorResponse Error { get; set; }

        /// <summary>
        /// Gets or sets unique 16-digit number within iDEAL
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Indicates whether the transaction has been successful or one of the following statuses:
        /// Success: Positive result; the payment is guaranteed.
        /// Cancelled: Negative result due to cancellation by Consumer; no payment has been made.
        /// Expired: Negative result due to expiration of the transaction; no payment has been made.
        /// Failure: Negative result due to other reasons; no payment has been made.
        /// Open: Final result not yet known). A new status request is necessary to obtain the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// If Status = Success, Cancelled, Expired or Failure
        /// This is the date and time at which the Issuer established the Transaction.
        /// </summary>
        public string StatusDateTimestamp { get; set; }

        /// <summary>
        /// Only included if Status = Success.
        /// The amount in euro guaranteed by the Acquirer to the Merchant (including decimal separator).
        /// The Merchant should verify that the value is equal to the value of amount in the TransactionRequest.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Only included if Status = Success.
        /// Currency of the amount guaranteed, expressed using the three-letter international currency code as per ISO 4217;
        /// Since iDEAL only supports Euro payments at this moment, the value should always be ‘EUR’.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Only included if Status = Success.
        /// Name of the Consumer according to the name of the account used for payment.
        /// In the exceptional case that the consumerName cannot be retrieved by the Issuer, this is filled with ‘N/A’.
        /// If governing law prevents Issuers outside the Netherlands from disclosing this information, field may be omitted.
        /// </summary>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Only included if Status = Success.
        /// The IBAN of the Consumer Bank account used for payment.
        /// If governing law prevents Issuers outside the Netherlands from disclosing this information, field may be omitted.
        /// </summary>
        public string ConsumerIBAN { get; set; }

        /// <summary>
        /// Only included if Status = Success. The IBAN of the Consumer Bank account used for payment.
        /// If governing law prevents Issuers outside the Netherlands from disclosing this information, field may be omitted.
        /// </summary>
        public string ConsumerBIC { get; set; }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
    }
}
