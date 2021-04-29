using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BanchubPayment.Models
{
    public static class Validation
    {
        public static bool isValidCardNumber(string cardNumber)
        {
            return Regex.IsMatch(cardNumber, @"^(\(\d{4}\)|^\d{4}[-]?)?\d{4}[-]?\d{4}[-]?\d{1,7}$");            
        }

        public static bool isValidCardHolderName(string cardHolderName)
        {
            return Regex.IsMatch(cardHolderName, @"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$");
        }

        public static bool isValidCardCvv(string cardCvv)
        {
            return Regex.IsMatch(cardCvv, @"^[0-9]{3,4}$");
        }
    }

    public class CardValidation
    {
        //[Required]
        [RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$", ErrorMessage = "Enter valid name")]
        public string CardName { get; set; }

        //[Required]
        [RegularExpression(@"^(\(\d{4}\)|^\d{4}[-]?)?\d{4}[-]?\d{4}[-]?\d{1,7}$", ErrorMessage = "Invalid card number.")]
        public string  CardNumber { get; set; }

        //[Required]
        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Enter valid CVV/CVV2.")]
        public string CardCvv { get; set; }
    }

    public class AccountValidation
    {
        //[Required]
        [RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$", ErrorMessage = "Enter valid name")]
        public string AccountName { get; set; }

        //[Required]        
        [RegularExpression(@"^[0-9]{8,17}$", ErrorMessage = "Enter valid account number.")]
        public string AccountNumber { get; set; }

        //[Required]        
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Enter valid routing number.")]
        public string RoutingNumber { get; set; }
    }
}