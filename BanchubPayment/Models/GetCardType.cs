using System.Text.RegularExpressions;

namespace BanchubPayment.Models
{
    public static class GetCardType
    {
        private static string isVisa = "^4[0-9]{12}(?:[0-9]{3})?$";
        private static string isElectron = "^(4026|417500|4405|4508|4844|4913|4917)[0-9]+$";
        private static string isMaestro = "^(5018|5020|5038|5612|5893|6304|6759|6761|6762|6763|0604|6390)[0-9]+$";
        private static string isDankort = "^(5019)[0-9]+$";
        private static string isInterpayment = "^(636)[0-9]+$";
        private static string isUnionpay = "^(62|88)[0-9]+$";
        private static string isMasterCard = "^(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}$";
        private static string isAmex = "^3[47][0-9]{13}$";
        private static string isDiners = "^3(?:0[0-5]|[68][0-9])[0-9]{11}$";
        private static string isDiscover = "^6(?:011|5[0-9]{2})[0-9]{12}$";
        private static string isJCB = "^(?:2131|1800|35[0-9]{3})[0-9]{11}$";

        public static string CardType(string strCardNumber)
        {
            try
            {
                strCardNumber = strCardNumber.Replace("-", "").Replace(" ", "").Trim();

                if (Regex.IsMatch(strCardNumber, GetCardType.isVisa))
                    return "VISA";
                if (Regex.IsMatch(strCardNumber, GetCardType.isMasterCard))
                    return "MASTERCARD";
                if (Regex.IsMatch(strCardNumber, GetCardType.isAmex))
                    return "AMEX";
                if (Regex.IsMatch(strCardNumber, GetCardType.isDiners))
                    return "DINERS";
                if (Regex.IsMatch(strCardNumber, GetCardType.isDiscover))
                    return "DISCOVER";
                if (Regex.IsMatch(strCardNumber, GetCardType.isJCB))
                    return "JCB";
                if (Regex.IsMatch(strCardNumber, GetCardType.isElectron))
                    return "ELECTRON";
                if (Regex.IsMatch(strCardNumber, GetCardType.isMaestro))
                    return "MAESTRO";
                if (Regex.IsMatch(strCardNumber, GetCardType.isDankort))
                    return "DANKORT";
                if (Regex.IsMatch(strCardNumber, GetCardType.isInterpayment))
                    return "INTERPAYMENT";
                if (Regex.IsMatch(strCardNumber, GetCardType.isUnionpay))
                    return "UNIONPAY";
                if (strCardNumber.Substring(0, 3).ToUpper() == "DNE")
                    return "BANKCARD";
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}