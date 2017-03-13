// <copyright file="Issuer.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    using System.Collections.Generic;

    /// <summary>
    /// Information issuer
    /// </summary>
    public class Issuer
    {
#pragma warning disable SA1623 // PropertySummaryDocumentationMustMatchAccessors

        /// <summary>
        /// Bank Identifier Code (BIC) of the iDEAL Issuer.
        /// </summary>
        public string IssuerId { get; set; }

        /// <summary>
        /// The name of the Issuer (as this should be displayed to the Consumer in the Merchant’s Issuer list).
        /// </summary>
        public string IssuerName { get; set; }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
    }
}
