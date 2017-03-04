using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace iDealCore
{
    public class RequestBase
    {
        public X509Certificate2 SigningCertificate { get; set; }
        public X509Certificate2 IssuerCertificate { get; set; }

        public string MerchantId { get; set; }
        public string MerchantSubId { get; set; }

        public bool TestMode { get; set; } = false;
        public string IDealURL { get; set; } = "https://ideal.rabobank.nl/ideal/iDEALv3";

        public bool ValidateSignature { get; set; } = true;

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

            using (var rsa = SigningCertificate.GetRSAPrivateKey())
            {
                byte[] signatureBytes = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string signature = Convert.ToBase64String(signatureBytes);
                return signature;
            }
        }

        public bool VerifySignature(string data)
        {
            var startPos = data.IndexOf("<SignedInfo>");
            var endPos = data.IndexOf("</SignedInfo>");
            string signatureData = "<SignedInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\">" + data.Substring(startPos + 12, endPos - (startPos + 12)) + "</SignedInfo>";

            string signatureValue = XmlNodeValue("SignatureValue", data).Replace("\r", "").Replace("\n", "");

            var base64Signature = Convert.FromBase64String(signatureValue);
            signatureData = signatureData.Replace("/><SignatureMethod", "></CanonicalizationMethod><SignatureMethod");
            signatureData = signatureData.Replace("/><Reference", "></SignatureMethod><Reference");
            signatureData = signatureData.Replace("/></Transforms", "></Transform></Transforms");
            signatureData = signatureData.Replace("/><DigestValue", "></DigestMethod><DigestValue");

            using (var rsa = IssuerCertificate.GetRSAPublicKey())
            {
                return rsa.VerifyData(Encoding.ASCII.GetBytes(signatureData), base64Signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static string DigestValue(string xml)
        {
            using (var algorithm = SHA256.Create())
            {
                byte[] data = Encoding.ASCII.GetBytes(xml);
                byte[] hash = algorithm.ComputeHash(data);
                return Convert.ToBase64String(hash);
            }
        }

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

    }
}
