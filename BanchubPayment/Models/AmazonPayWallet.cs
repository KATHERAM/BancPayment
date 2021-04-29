using AmazonPay;
using AmazonPay.Responses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanchubPayment.Models
{
  
        public class AmazonPayWallet
    {
        private static Client client = null;
        public AmazonPay AmazonPayWalletDetails()
        {

            //var Getuser = client.GetUserInfo(accesstoken);
            //dynamic UserDetails = JObject.Parse(Getuser);
            //var setorder = setResponse.GetJson();
            //dynamic Setorderdetails = JObject.Parse(setorder);
            AmazonPay WalletDetails = new AmazonPay
            {
            //    EFTUserId = 3333,
            //    EFTId = 8,
            //    PaymentTypeId = 6,
            //    WalletUserName = UserDetails.name,
            //    WalletId = UserDetails.user_id,
            //    WalletReferenceId = "W01-" + GenerateNewRandom() + "-" + GenerateNewRandom(),
            //    WalletRequestId = Setorderdetails.SetOrderReferenceDetailsResponse.ResponseMetadata.RequestId.ToString(),
            //    AuthorizationId = "",
            //    ClientReferenceId = "C01-" + GenerateNewRandom() + "-" + GenerateNewRandom(),
            //    TransactionType = "",
            //    TransactionDescription = "",
            //    TransactionAmount = Setorderdetails.SetOrderReferenceDetailsResponse.SetOrderReferenceDetailsResult.OrderReferenceDetails.OrderTotal.Amount.ToString(),
            //    Currency = Setorderdetails.SetOrderReferenceDetailsResponse.SetOrderReferenceDetailsResult.OrderReferenceDetails.OrderTotal.CurrencyCode.ToString()
            };
            return WalletDetails;
        }
        private string GenerateNewRandom()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            if (r.Distinct().Count() == 1)
            {
                r = GenerateNewRandom();
            }
            return r;
        }
       
        
    }
    public class AmazonPay
    {
        public int EFTUserId { set; get; }
        public int EFTId { set; get; }
        public int PaymentTypeId { set; get; }
        public string WalletUserName { set; get; }
        public string WalletId { set; get; }
        public string WalletReferenceId { set; get; }
        public string WalletRequestId { set; get; }
        public string AuthorizationId { set; get; }
        public string ClientReferenceId { set; get; }
        public string TransactionType { set; get; }
        public string TransactionDescription { set; get; }
        public decimal TransactionAmount { set; get; }
        public string Currency { set; get; }
        public DateTime TransactionBeginDateTime { set; get; }
        public DateTime TransactionEndDateTime { set; get; }
        public string InternalTransactionId { set; get; }
        public string TransactionStatus { set; get; }
        public string GUID { set; get; }
        public DateTime LastUpdatedDate { set; get; }
    }
}