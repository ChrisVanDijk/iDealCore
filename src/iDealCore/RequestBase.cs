// <copyright file="RequestBase.cs" company="CVD Software">
// Copyright © Chris van Dijk. Licensed under the MIT License
// </copyright>

namespace IDealCore
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    /// <summary>
    /// Base class for DirectoryRequest, TransactionRequest and StatusRequest
    /// </summary>
    public class RequestBase
    {
#pragma warning disable SA1623 // PropertySummaryDocumentationMustMatchAccessors

        /// <summary>
        /// The certificate used to sign the request to the Issuer
        /// </summary>
        public X509Certificate2 SigningCertificate { get; set; }

        /// <summary>
        /// The certificate used to check the issuer response
        /// </summary>
        public X509Certificate2 IssuerCertificate { get; set; }

        /// <summary>
        /// MerchantID as supplied to the Merchant by the Acquirer.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Merchant subID, as supplied to the Merchant by the Acquirer, if the Merchant has requested to use this.
        /// </summary>
        public string MerchantSubId { get; set; }

        /// <summary>
        /// Issuer endpoint, example: https://ideal.rabobank.nl/ideal/iDEALv3
        /// </summary>
        public string IDealURL { get; set; } = "https://ideal.rabobank.nl/ideal/iDEALv3";

        /// <summary>
        /// Validate issuer response. Default true.
        /// </summary>
        public bool ValidateSignature { get; set; } = true;

        /// <summary>
        /// Create SHA256 hash
        /// </summary>
        /// <param name="xml">Data</param>
        /// <returns>SHA256 Hash</returns>
        public static string DigestValue(string xml)
        {
            using (var algorithm = SHA256.Create())
            {
                byte[] data = Encoding.ASCII.GetBytes(xml);
                byte[] hash = algorithm.ComputeHash(data);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Parse XML node
        /// </summary>
        /// <param name="key">Nodename to find</param>
        /// <param name="xml">XML data</param>
        /// <param name="startPos">Start position, default 0</param>
        /// <returns>The XML Node</returns>
        public static string XmlNodeValue(string key, string xml, int startPos = 0)
        {
            int begin = xml.IndexOf("<" + key + ">", startPos);

            if (begin < 0)
            {
                return string.Empty;
            }

            begin += key.Length + 2;
            int end = xml.IndexOf("</" + key + ">", startPos);

            if (end < 0)
            {
                return string.Empty;
            }

            string result = xml.Substring(begin, end - begin);
            return result;
        }

        /// <summary>
        /// Sign the XML for the request
        /// </summary>
        /// <param name="digestValue">SHA256 hash</param>
        /// <returns>Signature</returns>
        public string SignXml(string digestValue)
        {
            var sb = new StringBuilder();
            sb.Append("<SignedInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\">");
            sb.Append("<CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod>");
            sb.Append("<SignatureMethod Algorithm=\"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256\"></SignatureMethod>");
            sb.Append("<Reference URI=\"\">");
            sb.Append("<Transforms>");
            sb.Append("<Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\"></Transform>");
            sb.Append("</Transforms>");
            sb.Append("<DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha256\"></DigestMethod>");
            sb.Append("<DigestValue>" + digestValue + "</DigestValue>");
            sb.Append("</Reference>");
            sb.Append("</SignedInfo>");

            byte[] data = Encoding.ASCII.GetBytes(sb.ToString());

            using (var rsa = this.SigningCertificate.GetRSAPrivateKey())
            {
                byte[] signatureBytes = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string signature = Convert.ToBase64String(signatureBytes);
                return signature;
            }
        }

        /// <summary>
        /// Verify response signature
        /// </summary>
        /// <param name="data">Data to verify</param>
        /// <returns>True if the signature is correct</returns>
        public bool VerifySignature(string data)
        {
            var startPos = data.IndexOf("<SignedInfo>");
            var endPos = data.IndexOf("</SignedInfo>");
            string signatureData = "<SignedInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\">" + data.Substring(startPos + 12, endPos - (startPos + 12)) + "</SignedInfo>";

            string signatureValue = XmlNodeValue("SignatureValue", data).Replace("\r", string.Empty).Replace("\n", string.Empty);

            var base64Signature = Convert.FromBase64String(signatureValue);
            signatureData = signatureData.Replace("/><SignatureMethod", "></CanonicalizationMethod><SignatureMethod");
            signatureData = signatureData.Replace("/><Reference", "></SignatureMethod><Reference");
            signatureData = signatureData.Replace("/></Transforms", "></Transform></Transforms");
            signatureData = signatureData.Replace("/><DigestValue", "></DigestMethod><DigestValue");

            using (var rsa = this.IssuerCertificate.GetRSAPublicKey())
            {
                return rsa.VerifyData(Encoding.ASCII.GetBytes(signatureData), base64Signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }

#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors
}
