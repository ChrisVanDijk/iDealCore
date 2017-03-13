// <copyright file="ErrorResponse.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    /// <summary>
    /// If an error occurs while processing a DirectoryRequest, TransactionRequest or StatusRequest,
    /// for example because a request contains a invalid value, an ErrorResponse will be returned instead of the regular response.
    /// The ErrorResponse has the same structure for all three types of requests.
    /// </summary>
    public class ErrorResponse
    {
#pragma warning disable SA1623 // PropertySummaryDocumentationMustMatchAccessors

        /// <summary>
        /// Unique characteristic of the occurred error within the iDEAL system.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Descriptive text accompanying the ErrorCode.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Details of the error. As determined and described by the Acquirer.
        /// </summary>
        public string ErrorDetail { get; set; }

        /// <summary>
        /// Suggestions aimed at resolving the problem.
        /// </summary>
        public string SuggestedAction { get; set; }

        /// <summary>
        /// An Acquirer can include a (standardised) message here which the Merchant should show to the Consumer.
        /// </summary>
        public string ConsumerMessage { get; set; }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
    }
}
