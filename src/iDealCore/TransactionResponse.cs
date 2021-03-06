﻿// <copyright file="TransactionResponse.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    /// <summary>
    /// If everything goes well the Acquirer will reply to the TransactionRequest with the TransactionResponse.
    /// </summary>
    public class TransactionResponse
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
        /// The complete Issuer URL to which the Consumer shall be redirected by the Merchant for authentication and authorisation of the transaction.
        /// </summary>
        public string IssuerAuthenticationURL { get; set; }

        /// <summary>
        /// Unique 16-digit number within iDEAL.
        /// The number consists of the acquirerID(first four positions) and a unique number generated by the Acquirer(12 positions).
        /// Ultimately appears on payment confirmation(bank statement or account overview of the Consumer and Merchant).
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Date and time at which the transaction was first registered by the Acquirer.This time can be used by Merchant, Acquiring bank and
        /// Issuing bank for reporting on the transaction.
        /// </summary>
        public string TransactionCreateDateTimestamp { get; set; }

        /// <summary>
        /// Unique identification of the order within the Merchant’s system.
        /// Ultimately appears on the payment confirmation(Bank statement /
        /// account overview of the Consumer and Merchant).
        /// This field has the same value as in the TransactionRequest.
        /// </summary>
        public string PurchaseID { get; set; }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
    }
}
