// <copyright file="DirectoryResponse.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    using System.Collections.Generic;

    /// <summary>
    /// The DirectoryRequest consists of an XML message that is sent to the Acquirer.
    /// </summary>
    public class DirectoryResponse
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
        /// Returns the issuers
        /// </summary>
        public List<Issuer> Issuers { get; set; }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
    }
}
