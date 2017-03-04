﻿using iDealCore;
using System;
using System.Security.Cryptography.X509Certificates;

namespace iDeal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            X509Certificate2 signingCertificate = new X509Certificate2("private.p12", "Password", X509KeyStorageFlags.Exportable);
            X509Certificate2 issuerCertificate = new X509Certificate2("idealcheckout.cer");

            string merchantId = "123456789";
            string idealUrl = "https://www.ideal-checkout.nl:443/simulator/";
            string returlUrl = "http://localhost:7878/idealreturn";

            var idealLib = new DirectoryRequest();
            idealLib.SigningCertificate = signingCertificate;
            idealLib.IssuerCertificate = issuerCertificate;
            idealLib.MerchantId = merchantId;
            idealLib.MerchantSubId = "0";
            idealLib.IDealURL = idealUrl;
            var issuerResult = idealLib.GetIssuers().Result;

            if (issuerResult.IsError)
            {
                Console.WriteLine(issuerResult.Error.ErrorMessage);
                Console.WriteLine(issuerResult.Error.ConsumerMessage);
                Console.ReadKey();
                return;
            }

            foreach (var bank in issuerResult.Issuers)
            {
                Console.WriteLine(bank.IssuerId + " -> " + bank.IssuerName);
            }

            var tranactionLib = new TransactionRequest();
            tranactionLib.SigningCertificate = signingCertificate;
            tranactionLib.IssuerCertificate = issuerCertificate;
            tranactionLib.MerchantId = merchantId;
            tranactionLib.MerchantSubId = "0";
            tranactionLib.IDealURL = idealUrl;
            tranactionLib.ReturnUrl = returlUrl;
            tranactionLib.Amount = 2.95M;
            tranactionLib.Description = "Test Order";
            tranactionLib.EntranceCode = Guid.NewGuid().ToString();
            tranactionLib.OrderId = "12332365";

            var request = tranactionLib.Request(issuerResult.Issuers[0].IssuerId).Result;

            if (request.IsError)
            {
                Console.WriteLine(request.Error.ErrorMessage);
                Console.WriteLine(request.Error.ConsumerMessage);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(request.IssuerAuthenticationURL);
            // System.IO.File.WriteAllText(@"c:\temp\url.txt", request.IssuerAuthenticationURL.Replace("&amp;", "&"));
            Console.WriteLine("Please goto above url and after payment etc. press any key");

            Console.ReadKey();

            var statusLib = new StatusRequest();
            statusLib.SigningCertificate = signingCertificate;
            statusLib.IssuerCertificate = issuerCertificate;
            statusLib.MerchantId = merchantId;
            statusLib.MerchantSubId = "0";
            statusLib.IDealURL = idealUrl;

            var status = statusLib.Request(request.TransactionID).Result;

            if (status.IsError)
            {
                Console.WriteLine(status.Error.ErrorMessage);
                Console.WriteLine(status.Error.ConsumerMessage);
                Console.ReadKey();
                return;
            }

            Console.WriteLine(status.Status);
            Console.WriteLine(status.ConsumerName);
            Console.WriteLine(status.ConsumerIBAN);
            Console.WriteLine(status.ConsumerBIC);
            Console.WriteLine(status.Currency);
            Console.WriteLine(status.Amount);

            Console.ReadKey();
        }
    }
}
