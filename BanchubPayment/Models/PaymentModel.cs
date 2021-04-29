using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BancHubPayment.Models
{

    public class Authenticate
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ClientId { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Param { get; set; }
    }

    public class AuthenticateModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ClientId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Apikey { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string ReferenceId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 3)]
        public string Currency { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Url { get; set; }

        [StringLength(2000, MinimumLength = 1)]
        public string AuthToken { get; set; }

        [Required]
        public bool Refund { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string AchType { get; set; }

        public PaymentMethod DefaultMethod { get; set; }

        [StringLength(30, MinimumLength = 0)]
        public string PaymentOptions { get; set; }

        public bool Email { get; set; }

        [StringLength(20, MinimumLength = 0)]
        public string Description { get; set; }

        public bool AutoSave { get; set; }

        public bool AuthorizeOnly { get; set; }

        //public bool DisableContactInfo { get; set; }
        //public AuthenticateModel()
        //{
        //    AuthorizeOnly = false;
        //}
    }

    public enum PaymentMethod { CC=1,DC,BAC};

    public class StatusModel
    {
        [StringLength(30, MinimumLength = 0)]
        public string transactionId { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string status { get; set; }

        [StringLength(500, MinimumLength = 0)]
        public string remarks { get; set; }
    }

    public class CreditCardModel
    {
        public IList<SelectListItem> Months = new List<SelectListItem>() { 
                new SelectListItem() {Selected=true, Text="Month", Value="Month" },
                new SelectListItem() {Selected=false, Text="01",  Value="01" },
                new SelectListItem() {Selected=false, Text="02", Value="02" },
                new SelectListItem() {Selected=false, Text="03", Value="03" },
                new SelectListItem() {Selected=false, Text="04", Value="04" },
                new SelectListItem() {Selected=false, Text="05", Value="05" },
                new SelectListItem() {Selected=false, Text="06", Value="06" },
                new SelectListItem() {Selected=false, Text="07", Value="07" },
                new SelectListItem() {Selected=false, Text="08", Value="08" },
                new SelectListItem() {Selected=false, Text="09", Value="09" },
                new SelectListItem() {Selected=false, Text="10", Value="10" },
                new SelectListItem() {Selected=false, Text="11", Value="11" },
                new SelectListItem() {Selected=false, Text="12", Value="12" },
        };

        public IEnumerable<SelectListItem> Years
        {
            get
            {
                var objYear=new List<SelectListItem>();
                for (var i=0;i<21;i++)
                {
                    objYear.Add(i == 0
                        ? new SelectListItem() {Selected = true, Text = "Year", Value = "-1"}
                        : new SelectListItem()
                        {
                            Selected = true, Text = (DateTime.Today.Year + (i - 1)).ToString(),
                            Value = (DateTime.Today.Year + (i - 1)).ToString()
                        });
                }
                return objYear;
            }
        }
      
        [Required]
        [DataType(DataType.Text)]
        //[RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$",ErrorMessage="Enter valid name")]
        [Display(Name = "Name on the card")]
        public string CreditCardHolderName { get; set; }

        [Required]
        //[RegularExpression(@"^(\(\d{4}\)|^\d{4}[-]?)?\d{4}[-]?\d{4}[-]?\d{1,7}$",ErrorMessage="Invalid card number.")]
        [Display(Name = "Card number")]
        public string CreditCardNumber { get; set; }

        [Required]
        [Display(Name = "Expiry")]
        [RegularExpression(@"^(?:(?!Month).)*$", ErrorMessage = "Select valid month.")]
        public string CreditExpiryMonth { get; set; }

        [Required]
        [Display(Name = "Expiry")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Select valid year.")]
        public int CreditExpiryYear { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Enter valid CVV/CVV2.")]
        [Display(Name = "CVV")]
        public string CreditCvv { get; set; }

        [Required]
        [Display(Name = "Billing zip")]
        [RegularExpression(@"^[0-9]{5,6}$", ErrorMessage = "Enter valid billing zipcode.")]
        public string CreditZipCode { get; set; }

        [Required]
        [Display(Name = "Email")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be 6 to 100 characters")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid e-mail address")]        
        public string EmailId { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        //[StringLength(13, MinimumLength = 10, ErrorMessage = "Enter valid phone number with 10 digits.")]
        //[RegularExpression(@"^[0-9]{10,13}$", ErrorMessage = "Enter 10 digit valid phone number.")]
        [RegularExpression(@"^\d{3}[-]?\d{3}[-]?\d{4}$", ErrorMessage = "Enter 10 digit valid phone number.")]
        public string Phone { get; set; }

        [Required]        
        public string PhoneCode { get; set; }

        [Display(Name = "Save my account details")]
        public bool SaveAccount { get; set; }        

        //public IList<SelectListItem> STDCODE = new List<SelectListItem>() {
        //        new SelectListItem() {Selected=true, Text="Month", Value="Month" },
        //        new SelectListItem() {Selected=false, Text="01",  Value="01" },
        //        new SelectListItem() {Selected=false, Text="02", Value="02" },
        //        new SelectListItem() {Selected=false, Text="03", Value="03" },
        //        new SelectListItem() {Selected=false, Text="04", Value="04" },
        //        new SelectListItem() {Selected=false, Text="05", Value="05" },
        //        new SelectListItem() {Selected=false, Text="06", Value="06" },
        //        new SelectListItem() {Selected=false, Text="07", Value="07" },
        //        new SelectListItem() {Selected=false, Text="08", Value="08" },
        //        new SelectListItem() {Selected=false, Text="09", Value="09" },
        //        new SelectListItem() {Selected=false, Text="10", Value="10" },
        //        new SelectListItem() {Selected=false, Text="11", Value="11" },
        //        new SelectListItem() {Selected=false, Text="12", Value="12" },
        //};
    }

    public class DebitCardModel
    {
        public IList<SelectListItem> Months = new List<SelectListItem>() { 
                new SelectListItem() {Selected=true, Text="Month", Value="Month" },
                new SelectListItem() {Selected=false, Text="01",  Value="01" },
                new SelectListItem() {Selected=false, Text="02", Value="02" },
                new SelectListItem() {Selected=false, Text="03", Value="03" },
                new SelectListItem() {Selected=false, Text="04", Value="04" },
                new SelectListItem() {Selected=false, Text="05", Value="05" },
                new SelectListItem() {Selected=false, Text="06", Value="06" },
                new SelectListItem() {Selected=false, Text="07", Value="07" },
                new SelectListItem() {Selected=false, Text="08", Value="08" },
                new SelectListItem() {Selected=false, Text="09", Value="09" },
                new SelectListItem() {Selected=false, Text="10", Value="10" },
                new SelectListItem() {Selected=false, Text="11", Value="11" },
                new SelectListItem() {Selected=false, Text="12", Value="12" },
        };

        public IEnumerable<SelectListItem> Years
        {
            get
            {
                var objYear = new List<SelectListItem>();
                for (var i = 0; i < 21; i++)
                {
                    objYear.Add(i == 0
                        ? new SelectListItem() {Selected = true, Text = "Year", Value = "-1"}
                        : new SelectListItem()
                        {
                            Selected = true, Text = (DateTime.Today.Year + (i - 1)).ToString(),
                            Value = (DateTime.Today.Year + (i - 1)).ToString()
                        });
                }
                return objYear;
            }
        }

        [Required]
        [DataType(DataType.Text)]
        //[Range(5,50,ErrorMessage="Card holder name must be 5-50 chars")]
        //[RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$", ErrorMessage = "Enter valid name")]
        [Display(Name = "Name on the card")]
        public string DebitCardHolderName { get; set; }

        [Required]
        //[RegularExpression(@"^(\(\d{4}\)|^\d{4}[-]?)?\d{4}[-]?\d{4}[-]?\d{1,7}$", ErrorMessage = "Invalid card number.")]
        [Display(Name = "Card number")]
        public string DebitCardNumber { get; set; }

        [Required]
        [Display(Name = "Expiry")]
        [RegularExpression(@"^(?:(?!Month).)*$", ErrorMessage = "Select valid month.")]
        public string DebitExpiryMonth { get; set; }

        [Required]
        [Display(Name = "Expiry")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Select valid year.")]
        public int DebitExpiryYear { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Enter valid CVV/CVV2.")]
        [Display(Name = "CVV")]
        public string DebitCvv { get; set; }

        [Required]
        [Display(Name = "Billing zip")]
        [RegularExpression(@"^[0-9]{5,6}$", ErrorMessage = "Enter valid billing zipcode.")]
        public string DebitZipCode { get; set; }

        [Required]
        [Display(Name = "Email")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be 6 to 100 characters")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid e-mail address")]
        public string EmailId { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        //[StringLength(13, MinimumLength = 10, ErrorMessage = "Enter valid phone number with 10 digits.")]
        [RegularExpression(@"^\d{3}[-]?\d{3}[-]?\d{4}$$", ErrorMessage = "Enter 10 digit valid phone number.")]
        public string Phone { get; set; }       

        [Required]        
        public string PhoneCode { get; set; }

        [Display(Name = "Save my account details")]
        public bool SaveAccount { get; set; }
    }

    public class ModelCollection
    {
        public CreditCardModel CreditCardModel { get; set; }
        public DebitCardModel DebitCardModel { get; set; }
        public AchModel AchModel { get; set; }
        public AchCaModel AchCaModel { get; set; }

        //public AchWireModel AchWireModel { get; set; }
    }

    public class AchModel
    {
        public IList<SelectListItem> Type = new List<SelectListItem>() { 
                new SelectListItem() {Selected=true, Text="Select", Value="Select" },
                new SelectListItem() {Selected=false, Text="CHECKING",  Value="CHECKING" },
                new SelectListItem() {Selected=false, Text="SAVINGS", Value="SAVINGS" },
        };        

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Account name")]
        //[RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$", ErrorMessage = "Enter valid name")]
        public string AccountName { get; set; }

        [Required]
        [Display(Name = "Account number")]
        //[RegularExpression(@"^[0-9]{8,17}$", ErrorMessage = "Enter valid account number.")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Routing number")]
        //[RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Enter valid routing number.")]
        public string RoutingNumber { get; set; }

        [Required]
        [Display(Name = "Account type")]
        [RegularExpression(@"^(?:(?!Select).)*$",ErrorMessage="Select account type.")]
        public string AccountType { get; set; }

        [Required]
        [Display(Name = "Billing zip")]
        [RegularExpression(@"^[0-9]{5,6}$", ErrorMessage = "Enter valid billing zipcode (Length 5 or 6).")]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Email")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be 6 to 100 characters")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid e-mail address")]
        public string EmailId { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        //[StringLength(13, MinimumLength = 10, ErrorMessage = "Enter valid phone number with 10 digits.")]        
        [RegularExpression(@"^\d{3}[-]?\d{3}[-]?\d{4}$", ErrorMessage = "Enter 10 digit valid phone number.")]
        public string Phone { get; set; }
        
        [Required]        
        public string PhoneCode { get; set; }

        [Display(Name = "Save my account details")]
        public bool SaveAccount { get; set; }
    }

    public class AchCaModel
    {
        public IList<SelectListItem> CaType = new List<SelectListItem>() {
                new SelectListItem() {Selected=true, Text="Select", Value="Select" },
                new SelectListItem() {Selected=false, Text="DEMAND",  Value="DEMAND" },
                new SelectListItem() {Selected=false, Text="SAVINGS", Value="SAVINGS" },
        };

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Account name")]
        //[RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$", ErrorMessage = "Enter valid name")]
        public string CaAccountName { get; set; }

        [Required]
        [Display(Name = "Account number")]
        //[RegularExpression(@"^[0-9]{8,17}$", ErrorMessage = "Enter valid account number.")]
        public string CaAccountNumber { get; set; }

        [Required]
        [Display(Name = "Bank Transit number")]
        //[RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Enter valid bank transit number (Eg. 099912345 - bank number followed by branch number).")]
        public string BankTransitNumber { get; set; }

        [Required]
        [Display(Name = "Account type")]
        [RegularExpression(@"^(?:(?!Select).)*$", ErrorMessage = "Select account type.")]
        public string CaAccountType { get; set; }

        [Required]
        [Display(Name = "Billing zip")]
        [RegularExpression(@"^[0-9]{5,6}$", ErrorMessage = "Enter valid billing zipcode (Length 5 or 6).")]
        public string CaZipCode { get; set; }

        [Required]
        [Display(Name = "Email")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be 6 to 100 characters")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid e-mail address")]                             
        public string CaEmailId { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        //[StringLength(13, MinimumLength = 10, ErrorMessage = "Enter valid phone number with 10 digits.")]        
        [RegularExpression(@"^\d{3}[-]?\d{3}[-]?\d{4}$", ErrorMessage = "Enter 10 digit valid phone number.")]
        public string CaPhone { get; set; }

        [Required]        
        public string CaPhoneCode { get; set; }

        [Display(Name = "Save my account details")]
        public bool SaveAccount { get; set; }
    }

    public class AchWireModel
    {
        public IList<SelectListItem> Type = new List<SelectListItem>() { 
                new SelectListItem() {Selected=true, Text="Select", Value="Select" },
                new SelectListItem() {Selected=false, Text="CHECKING",  Value="CHECKING" },
                new SelectListItem() {Selected=false, Text="SAVINGS", Value="SAVINGS" },
        };

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Account name")]
        [RegularExpression(@"^[A-Za-z\s]{1,}[\.]{0,1}[A-Za-z\s]{0,}$", ErrorMessage = "Enter valid name")]
        public string WireAccountName { get; set; }

        [Required]
        [Display(Name = "Account number")]
        [RegularExpression(@"^[0-9]{9,17}$", ErrorMessage = "Enter valid account number.")]
        public int WireAccountNumber { get; set; }

        [Required]
        [Display(Name = "Routing number")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Enter valid routing number.")]
        public int WireRoutingNumber { get; set; }

        [Required]
        [Display(Name = "Account type")]
        [RegularExpression(@"^(?:(?!Select).)*$", ErrorMessage = "Select account type.")]
        public string WireAccountType { get; set; }

        [Required]
        [Display(Name = "Billing zip")]
        [RegularExpression(@"^[0-9]{5,6}$", ErrorMessage = "Enter valid billing zipcode.")]
        public string WireZipCode { get; set; }

        [Required]
        [Display(Name = "Email")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be 6 to 100 characters")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid e-mail address")]
        public string EmailId { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        //[StringLength(13, MinimumLength = 10, ErrorMessage = "Enter valid phone number with 10 digits.")]
        [RegularExpression(@"^\d{3}[-]?\d{3}[-]?\d{4}$", ErrorMessage = "Enter 10 digit valid phone number.")]
        public string Phone { get; set; }

        [Display(Name = "Save my account details")]
        public bool? SaveAccount { get; set; }
    }
}