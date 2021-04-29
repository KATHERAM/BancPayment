using BanchubPayment.Models;
using BancHubPayment.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web.Mvc;
using AmazonPay;
using AmazonPay.Responses;
using AmazonPay.StandardPaymentRequests;
using Newtonsoft.Json.Linq;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Web;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace BanchubPayment.Controllers
{
    public class ProcessController : Controller
    {
        private static readonly string MODULE = "PaymentProcess";
        private string errorMessage = string.Empty;
        private static AmazonPayWallet ampaywallet = null;
       
        private static AmazonPay.CommonRequests.Configuration clientConfig = null;
        private static Client client = null;
        //
        // GET: /Payment/
        public ActionResult Index()
        {            
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]        
        public ActionResult Cancel(string info)
        {
            var payInfo = DataEncryption.DecryptString(info.Split('#').GetValue(0).ToString(),
                                                       info.Split('#').GetValue(1).ToString().ToCharArray()).Split('{');
            TempData.Remove("auth");
            try
            {   
                return new RedirectResult(payInfo[4] + "?transactionid=&status=FAILED&error_description=Payment cancelled by the user&referenceid=" + payInfo[1]+ "&payment_method=");
            }
            catch
            {                
                return new RedirectResult(payInfo[4] + "?transactionid=&status=FAILED&error_description=Payment cancelled by the user.&referenceid=" + payInfo[1] + "&payment_method=");
            }            
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Payment()
        {
            AuthenticateModel model = null;

            try
            {
                if (TempData["autosave"] != null)
                {
                    TempData["auth"] = null;
                }
                
                model = (AuthenticateModel)TempData["auth"];
                //model = JsonConvert.DeserializeObject<AuthenticateModel>(auth);
                ViewBag.style =Path.Combine(ConfigurationManager.AppSettings["UserTheme"],model.ClientId+"style.css");

                ViewBag.username = model.ClientId;
                ViewBag.orderId = model.ReferenceId;
                ViewBag.currency = model.Currency;
                ViewBag.amount = model.Amount;
                ViewBag.token = model.AuthToken;
                ViewBag.url = model.Url;

                //Added new viewbag params for enabling and disabling the payment options.
                ViewBag.defaultmethod = model.DefaultMethod.ToString();
                ViewBag.options = model.PaymentOptions;
                ViewBag.autosave = model.AutoSave;
                TempData["autosave"]= model.AutoSave;
                TempData["returl"] = model.Url;
                ViewBag.AuthorizeOnly = model.AuthorizeOnly;
                TempData["AuthorizeOnly"] = model.AuthorizeOnly;
                TempData["Amount"] = model.Amount;
                //ViewBag.DisableContactInfo = model.DisableContactInfo;
                //TempData["DisableContactInfo"] = model.DisableContactInfo;

                TempData.Keep();

                var info = string.Join("{", model.ClientId, model.ReferenceId, model.Currency, model.Amount.ToString(), 
                                            model.Url, model.Refund.ToString(), model.AchType, model.Email.ToString(),model.Description, 
                                            model.AutoSave.ToString(), model.AuthorizeOnly.ToString());
                var pass = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
                ViewBag.info = DataEncryption.EncryptString(info, pass.ToCharArray()) + "#" + pass;

                var col = new ModelCollection
                {
                    CreditCardModel = new CreditCardModel(),
                    DebitCardModel = new DebitCardModel(),
                    AchModel = new AchModel(),
                    AchCaModel =new AchCaModel()
                };
                
                if(model.AutoSave)
                {
                    col.CreditCardModel.SaveAccount = true;
                    col.DebitCardModel.SaveAccount = true;
                    col.AchCaModel.SaveAccount = true;
                    col.AchModel.SaveAccount = true;
                }

                return View(col);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "CreditPayment", ex.Message);

                TempData.Remove("auth");

                if (TempData["returl"] != null)
                {
                    var tempurl = TempData["returl"];
                    TempData.Remove("returl");
                    return Json(new
                    {
                        success = true,
                        redirectUrl = tempurl + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again.&referenceid=" + model.ReferenceId
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,                        
                    redirectUrl = model.Url
                }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Payment([Bind]ModelCollection col)
        {
            try
            {
                return ModelState.IsValid ? null : View(col);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "CreditPayment", ex.Message);

                ViewBag.result = ex.Message;
                ViewBag.resulttitle = "Error";
                ViewBag.type = "error";
                return View(col);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult CreditPayment([Bind]CreditCardModel cp)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var objClient = new PaymentService.BancHUBPaymentServiceClient();
            var payInfo = DataEncryption.DecryptString(Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(0).ToString(), 
                                                            Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(1).ToString().ToCharArray()).Split('{');
            PaymentService.Status st = null;
            PaymentService.TokenStatus token = null;
           // AuthenticateModel model = null;
            try
            {
                if (TempData["auth"] == null)
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = payInfo[4]
                                        + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=CreitCard"
                                         + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }
                //model = (AuthenticateModel)TempData["auth"];
                var sKey = Convert.ToString(Request.Params["hid3"]);

                if (cp.CreditCardHolderName != null)
                    cp.CreditCardHolderName = AESEncrytDecry.DecryptStringAES(cp.CreditCardHolderName, sKey);
                if (cp.CreditCardNumber != null)
                    cp.CreditCardNumber = AESEncrytDecry.DecryptStringAES(cp.CreditCardNumber,sKey);
                if (cp.CreditCvv != null)
                    cp.CreditCvv = AESEncrytDecry.DecryptStringAES(cp.CreditCvv,sKey);

                CardValidation cardValidation = new CardValidation()
                {
                    CardName = cp.CreditCardHolderName,
                    CardNumber = cp.CreditCardNumber,
                    CardCvv = cp.CreditCvv
                };


                if (TryValidateModel(cardValidation) && ModelState.IsValid)
                {

                    objClient.Open();

                    var header = new PaymentService.Header
                    {
                        username = payInfo[0],
                        session_token = Convert.ToString(Request.Params["hid2"])
                    };

                    #region Saving account details

                    if (cp.SaveAccount || payInfo[9].ToUpper().Equals("TRUE") || (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0))
                    {
                        PaymentService.CardTokenDetails cardToken = new PaymentService.CardTokenDetails()
                        {
                            cardholder_name = cp.CreditCardHolderName,
                            card_number = cp.CreditCardNumber.Replace("-", ""),
                            country_code = "",
                            expire_month = Convert.ToInt32(cp.CreditExpiryMonth),
                            expire_year = cp.CreditExpiryYear,
                            payment_cardtype = PaymentService.PaymentCardType.creditcard,
                            description = ""
                        };
                        token = objClient.GetCardToken(header, cardToken);

                        if (!token.status.Equals("SUCCESS"))
                        {
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                            + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=CreditCard"
                                            + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0)
                        {
                            TempData.Remove("auth");
                            Session.Abandon();
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                                + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=CreditCard"
                                                + "&token_id=" + token.token_id + "&last_four=" + token.display_number.Split('-').GetValue(1)
                                                + "&type=" + token.display_number.Split('-').GetValue(0) + "&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        token = new PaymentService.TokenStatus
                        {
                            token_id = "",
                            display_number = GetCardType.CardType(cp.CreditCardNumber) + "-XXXX XXXX XXXX " + cp.CreditCardNumber.Replace("-", "").Substring(cp.CreditCardNumber.Replace("-", "").Length - 4)
                        };
                    }

                    #endregion

                    #region Card payment

                    var cardDetails = new PaymentService.CardDetails
                    {
                        card_number = cp.CreditCardNumber.Replace("-", ""),
                        card_present = PaymentService.CardPresent.no,
                        card_type = GetCardType.CardType(cp.CreditCardNumber),
                        cardholder_name = cp.CreditCardHolderName,
                        cvv2 = cp.CreditCvv.ToString(),
                        expire_month = Convert.ToInt32(cp.CreditExpiryMonth),
                        expire_year = cp.CreditExpiryYear,
                        payment_cardtype = PaymentService.PaymentCardType.creditcard,
                        payment_intent = payInfo[10].ToUpper().Equals("TRUE") ? PaymentService.PaymentIntent.authorize : PaymentService.PaymentIntent.sale
                    };

                    var tran = new PaymentService.TransactionDetails
                    {
                        client_referid = payInfo[1],//model.referenceid,
                        currency = payInfo[2],// model.currency,
                        purchase_level = PaymentService.PurchaseLevel.L1,
                        total_amount = Convert.ToDecimal(payInfo[3]), //model.amount,
                        transaction_date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"),
                        transaction_description = payInfo[8]
                    };

                    var billing = new PaymentService.BillingAddress
                    {
                        billing_username = cp.CreditCardHolderName,
                        user_zip = cp.CreditZipCode,
                        user_city = "XXXXXX",
                        user_state = "XX",
                        user_address1 = "XX, XXXXXXXXXXXX",
                        //user_contact=cp.Phone,
                        user_contact = cp.PhoneCode.Split(' ').GetValue(0) + cp.Phone.Replace("-", ""),
                        user_email = cp.EmailId
                    };

                    st = objClient.CardPayment(header, cardDetails, tran, billing);

                    objClient.Close();

                    #endregion

                    #region Payment status and sending email 

                    if (string.IsNullOrEmpty(st.transaction_id) && string.IsNullOrEmpty(st.error_desc))
                    {
                        st.status = "FAILED";
                        if (string.IsNullOrEmpty(st.error_desc)) st.error_desc = "Transaction failed due to system error. Sorry for the inconvenience caused.";
                    }

                    if (payInfo[7].ToUpper().Equals("TRUE") && st.status.ToUpper().Equals("SUCCESS") && !string.IsNullOrEmpty(st.transaction_id))
                    {
                        SetEmail email = new SetEmail();
                        if (!email.Send(header.username, cardDetails.cardholder_name, Convert.ToDateTime(tran.transaction_date).ToShortDateString(),
                                    tran.total_amount.ToString("0.00"), tran.client_referid, tran.currency, billing.user_email, ref errorMessage))
                        {
                            try
                            {
                                Log log = new Log();
                                log.ErrorLog(MODULE, "CreditPayment", "Fail to deliver the notifcation to " + cardDetails.cardholder_name + " - " + billing.user_email + "::Reason -" + errorMessage);
                            }
                            catch { }
                            st.error_desc = "Fail to deliver the notification. Sorry for the inconvenience caused. You will receive the notification shortly.";
                        }
                    }

                    #endregion

                    TempData.Remove("auth");
                    Session.Abandon();
                    return Json(new
                    {
                        success = true,
                        responseText = "",
                        redirectUrl = payInfo[4] + "?transactionid=" + st.transaction_id + "&status=" + st.status
                                        + "&error_description=" + st.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=CreditCard"
                                        + "&token_id=" + token.token_id + "&last_four=" + token.display_number.Split('-').GetValue(1)
                                        + "&type=" + token.display_number.Split('-').GetValue(0) + "&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }
                ViewBag.requestType = "Credit";
                ViewBag.info = Convert.ToString(Request.Params["hid1"]);
                ViewBag.token = Convert.ToString(Request.Params["hid2"]);
                
                //Keep the temp data on post backs
                TempData.Keep();
                
                return PartialView("_CreditPayment", cp);
            }
            catch(Exception ex)
            {
                bool isNetAvailable = NetworkInterface.GetIsNetworkAvailable();
                string error_description = isNetAvailable?"System error. Please try again.": "Network error. Your ";

                Log log = new Log();
                log.ErrorLog(MODULE, "CreditPayment", ex.Message);
                try
                {
                    objClient.Close();
                }
                catch 
                {
                    objClient.Abort();
                }
                TempData.Remove("auth");
                Session.Abandon();
                return Json(new
                {
                    success = true,
                    redirectUrl = payInfo[4]
                                    + "?transactionid=&status=FAILED&error_description=System error. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=CreitCard"
                                     + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                }, JsonRequestBehavior.AllowGet);                
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult DebitPayment([Bind]DebitCardModel dp)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var objClient = new PaymentService.BancHUBPaymentServiceClient();
            var payInfo = DataEncryption.DecryptString(Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(0).ToString(),
                                                            Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(1).ToString().ToCharArray()).Split('{');
            try
            {
                if (TempData["auth"] == null)
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = payInfo[4]
                                        + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=CreitCard"
                                         + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }

                var sKey = Convert.ToString(Request.Params["hid3"]);

                if (dp.DebitCardHolderName != null)
                    dp.DebitCardHolderName = AESEncrytDecry.DecryptStringAES(dp.DebitCardHolderName, sKey);
                if (dp.DebitCardNumber != null)
                    dp.DebitCardNumber = AESEncrytDecry.DecryptStringAES(dp.DebitCardNumber, sKey);
                if (dp.DebitCvv != null)
                    dp.DebitCvv = AESEncrytDecry.DecryptStringAES(dp.DebitCvv, sKey);

                CardValidation cardValidation = new CardValidation()
                {
                    CardName = dp.DebitCardHolderName,
                    CardNumber = dp.DebitCardNumber,
                    CardCvv = dp.DebitCvv
                };

                if (TryValidateModel(cardValidation) && ModelState.IsValid)
                {
                    objClient.Open();

                    var header = new PaymentService.Header()
                    {
                        username = payInfo[0],
                        session_token = Convert.ToString(Request.Params["hid2"])
                    };

                    #region Saving account details

                    PaymentService.TokenStatus token = null;

                    if (dp.SaveAccount || payInfo[9].ToUpper().Equals("TRUE") || (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0))
                    {
                        PaymentService.CardTokenDetails cardToken = new PaymentService.CardTokenDetails()
                        {
                            cardholder_name = dp.DebitCardHolderName,
                            card_number = dp.DebitCardNumber.Replace("-", ""),
                            country_code = "",
                            expire_month = Convert.ToInt32(dp.DebitExpiryMonth),
                            expire_year = dp.DebitExpiryYear,
                            payment_cardtype = PaymentService.PaymentCardType.debitcard,
                            description = ""
                        };
                        token = objClient.GetCardToken(header, cardToken);

                        if (!token.status.Equals("SUCCESS"))
                        {
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                            + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=DebitCard"
                                             + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }

                        if (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0)
                        {
                            TempData.Remove("auth");
                            Session.Abandon();
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                                + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=DebitCard"
                                                + "&token_id=" + token.token_id + "&last_four=" + token.display_number.Split('-').GetValue(1)
                                                + "&type=" + token.display_number.Split('-').GetValue(0) + "&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        token = new PaymentService.TokenStatus
                        {
                            token_id = "",
                            display_number = GetCardType.CardType(dp.DebitCardNumber) + "-XXXX XXXX XXXX " + dp.DebitCardNumber.Replace("-", "").Substring(dp.DebitCardNumber.Replace("-", "").Length - 4)
                        };
                    }

                    #endregion

                    #region Card payment process

                    var cardDetails = new PaymentService.CardDetails
                    {
                        card_number = dp.DebitCardNumber.Replace("-", ""),
                        card_present = PaymentService.CardPresent.no,
                        card_type = "",
                        cardholder_name = dp.DebitCardHolderName,
                        cvv2 = dp.DebitCvv.ToString(),
                        expire_month = Convert.ToInt32(dp.DebitExpiryMonth),
                        expire_year = dp.DebitExpiryYear,
                        payment_cardtype = PaymentService.PaymentCardType.debitcard,
                        payment_intent = payInfo[10].ToUpper().Equals("TRUE") ? PaymentService.PaymentIntent.authorize : PaymentService.PaymentIntent.sale
                    };

                    var tran = new PaymentService.TransactionDetails
                    {
                        client_referid = payInfo[1],
                        currency = payInfo[2],
                        purchase_level = PaymentService.PurchaseLevel.L1,
                        total_amount = Convert.ToDecimal(payInfo[3]),
                        transaction_date = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"),
                        transaction_description = ""
                    };

                    var billing = new PaymentService.BillingAddress
                    {
                        billing_username = dp.DebitCardHolderName,
                        user_zip = dp.DebitZipCode,
                        user_city = "XXXXXX",
                        user_state = "XX",
                        user_address1 = "XX, XXXXXXXXXXXX",
                        user_contact = dp.PhoneCode.Split(' ').GetValue(0) + dp.Phone.Replace("-", ""),
                        user_email = dp.EmailId
                    };

                    var st = objClient.CardPayment(header, cardDetails, tran, billing);

                    objClient.Close();

                    #endregion

                    #region Payment status  

                    if (string.IsNullOrEmpty(st.transaction_id))
                    {
                        st.status = "FAILED";
                        if (string.IsNullOrEmpty(st.error_desc)) st.error_desc = "Transaction failed due to system error. Sorry for the inconvenience caused.";
                    }

                    if (payInfo[7].ToUpper().Equals("TRUE") && st.status.ToUpper().Equals("SUCCESS") && !string.IsNullOrEmpty(st.transaction_id))
                    {
                        SetEmail email = new SetEmail();
                        if (!email.Send(header.username, cardDetails.cardholder_name, Convert.ToDateTime(tran.transaction_date).ToShortDateString(),
                                        tran.total_amount.ToString("0.00"), tran.client_referid, tran.currency, billing.user_email, ref errorMessage))
                        {
                            try
                            {
                                Log log = new Log();
                                log.ErrorLog(MODULE, "CreditPayment", "Fail to deliver the notifcation to " + cardDetails.cardholder_name + " - " + billing.user_email + "::Reason -" + errorMessage);
                            }
                            catch
                            {

                            }
                            st.error_desc = "Fail to deliver the notification. Sorry for the inconvenience caused. You will receive the notification shortly.";
                        }
                    }

                    #endregion

                    TempData.Remove("auth");
                    Session.Abandon();
                    return Json(new
                    {
                        success = true,
                        responseText = "",
                        redirectUrl = payInfo[4] + "?transactionid=" + st.transaction_id + "&status=" + st.status + "&error_description="
                                                        + st.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=DebitCard"
                                                        + "&token_id=" + token.token_id + "&last_four=" + token.display_number.Split('-').GetValue(1)
                                                        + "&type=" + token.display_number.Split('-').GetValue(0) + "&currency=" + payInfo[2]
                                                        + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }
                ViewBag.requestType = "Debit";
                ViewBag.info = Convert.ToString(Request.Params["hid1"]);
                ViewBag.token = Convert.ToString(Request.Params["hid2"]);
                
                //Keep the temp data on post backs
                TempData.Keep();
                return PartialView("_DebitPayment",dp);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "DebitPayment", ex.Message);
                try
                {
                    objClient.Close();
                }
                catch
                {
                    objClient.Abort();
                }
                TempData.Remove("auth");
                Session.Abandon();
                return Json(new
                {
                    success = true,
                    redirectUrl = payInfo[4]
                                    + "?transactionid=&status=FAILED&error_description=System error. Please try again.&referenceid=" + payInfo[1] 
                                    + "&payment_method=DebitCard" + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                }, JsonRequestBehavior.AllowGet);
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult AchPayment([Bind]AchModel ach)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var objClient = new PaymentService.BancHUBPaymentServiceClient();
            var payInfo = DataEncryption.DecryptString(Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(0).ToString(),
                                                        Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(1).ToString().ToCharArray()).Split('{');
            try
            {
                if (TempData["auth"] == null)
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = payInfo[4]
                                        + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=CreitCard"
                                         + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }

                var sKey = Convert.ToString(Request.Params["hid3"]);

                if (ach.AccountName != null)
                    ach.AccountName = AESEncrytDecry.DecryptStringAES(ach.AccountName, sKey);
                if (ach.AccountNumber != null)
                    ach.AccountNumber = AESEncrytDecry.DecryptStringAES(ach.AccountNumber, sKey);
                if (ach.RoutingNumber != null)
                    ach.RoutingNumber = AESEncrytDecry.DecryptStringAES(ach.RoutingNumber, sKey);

                AccountValidation acctValidation = new AccountValidation()
                {
                    AccountName = ach.AccountName,
                    AccountNumber = ach.AccountNumber,
                    RoutingNumber = ach.RoutingNumber
                };

                if (TryValidateModel(acctValidation) && ModelState.IsValid)

                {
                    objClient.Open();

                    var header = new PaymentService.Header
                    {
                        username = payInfo[0],
                        session_token = Convert.ToString(Request.Params["hid2"])
                    };

                    #region Saving account details

                    PaymentService.TokenStatus token = null;

                    if (ach.SaveAccount || payInfo[9].ToUpper().Equals("TRUE") || (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0))
                    {
                        PaymentService.BankAccountTokenDetails accountToken = new PaymentService.BankAccountTokenDetails()
                        {
                            account_name = ach.AccountName,
                            account_number = ach.AccountNumber,
                            account_type = (PaymentService.AccountType)Enum.Parse(typeof(PaymentService.AccountType), ach.AccountType, true),
                            routing_number = ach.RoutingNumber,
                            country_code = "USA",
                            description = ""
                        };
                        token = objClient.GetBankAccountToken(header, accountToken);

                        if (!token.status.Equals("SUCCESS"))
                        {
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                            + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=ACH"
                                             + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0)
                        {
                            TempData.Remove("auth");
                            Session.Abandon();
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                                + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=ACH"
                                                + "&token_id=" + token.token_id + "&last_four=" + token.display_number
                                                + "&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        token = new PaymentService.TokenStatus
                        {
                            token_id = "",
                            display_number = string.Concat(Enumerable.Repeat("X", ach.AccountNumber.Length - 3))
                                                                + ach.AccountNumber.Substring(ach.AccountNumber.Length - 3)
                        };
                    }

                    #endregion

                    #region Payment process

                    var achAccount = new PaymentService.AchAccountDetails
                    {
                        account_name = ach.AccountName,
                        account_number = ach.AccountNumber,
                        account_type = (PaymentService.AccountType)Enum.Parse(typeof(PaymentService.AccountType), ach.AccountType, true),
                        client_referid = payInfo[1],
                        currency = payInfo[2],
                        transaction_amount = Convert.ToDecimal(payInfo[3]),
                        transaction_description = payInfo[8],
                        receiver_identification = "",
                        transaction_effectivedate = DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"),
                        transaction_type = payInfo[5].ToUpper().Equals("TRUE") ? PaymentService.TransactionType.CREDIT : PaymentService.TransactionType.DEBIT,
                        payment_intent = PaymentService.DepositAccountType.BUSINESS,
                        payment_type = PaymentService.PaymentType.S,
                        ach_type = string.IsNullOrEmpty(payInfo[6]) ? PaymentService.AchType.WEB : (PaymentService.AchType)Enum.Parse(typeof(PaymentService.AchType), payInfo[6]),
                        routing_number = ach.RoutingNumber
                    };

                    var billing = new PaymentService.BillingAddress
                    {
                        billing_username = ach.AccountName,
                        user_zip = ach.ZipCode,
                        user_city = "XXXXXX",
                        user_state = "XX",
                        user_address1 = "XX, XXXXXXXXXXXX",
                        user_contact = ach.PhoneCode.Split(' ').GetValue(0) + ach.Phone.Replace("-", ""),
                        user_email = ach.EmailId
                    };

                    var st = objClient.AchAccountPayment(header, achAccount, billing);

                    objClient.Close();

                    #endregion

                    #region Payment status

                    if (string.IsNullOrEmpty(st.transaction_id))
                    {
                        st.status = "FAILED";
                        if (string.IsNullOrEmpty(st.error_desc)) st.error_desc = "Transaction failed due to system error. Sorry for the inconvenience caused.";
                    }
                    else
                        st.error_desc = "Payment request received. Final status will be notified shortly.";

                    if (payInfo[7].ToUpper().Equals("TRUE") && st.status.ToUpper().Equals("SUBMITTED") && !string.IsNullOrEmpty(st.transaction_id))
                    {
                        SetEmail email = new SetEmail();
                        if (!email.Send(header.username, achAccount.account_name, Convert.ToDateTime(achAccount.transaction_effectivedate).ToShortDateString(),
                                    Convert.ToString(achAccount.transaction_amount), achAccount.client_referid, achAccount.currency, billing.user_email, ref errorMessage))
                        {
                            try
                            {
                                Log log = new Log();
                                log.ErrorLog(MODULE, "CreditPayment", "Fail to deliver the notifcation to " + achAccount.account_name + " - " + billing.user_email + "::Reason -" + errorMessage);

                            }
                            catch
                            {
                                //ignored
                            }
                            st.error_desc = "Fail to deliver the notification. Sorry for the inconvenience caused. You will receive the notification shortly.";
                        }
                    }

                    #endregion

                    TempData.Remove("auth");
                    Session.Abandon();
                    return Json(new
                    {
                        success = true,
                        responseText = "",
                        redirectUrl = payInfo[4] + "?transactionid=" + st.transaction_id + "&status=" + st.status
                                       + "&error_description=" + st.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=ACH"
                                       + "&token_id=" + token.token_id + "&last_four=" + token.display_number + "&type=&currency="
                                       + payInfo[2] + "&amount=" + payInfo[3]

                    }, JsonRequestBehavior.AllowGet);
                }

                ViewBag.requestType = "Ach";
                ViewBag.info = Convert.ToString(Request.Params["hid1"]);
                ViewBag.token = Convert.ToString(Request.Params["hid2"]);
                //Keep the temp data on post backs
                TempData.Keep();
                return PartialView("_AchPayment", ach);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "AchPayment", ex.Message);
                try
                {
                    objClient.Close();
                }
                catch
                {
                    objClient.Abort();
                }
                TempData.Remove("auth");
                Session.Abandon();
                return Json(new
                {
                    success = true,
                    redirectUrl = payInfo[4]
                                    + "?transactionid=&status=FAILED&error_description=System error. Please try again.&referenceid=" + payInfo[1] 
                                    + "&payment_method=ACH" + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult AchCaPayment([Bind]AchCaModel achCa)
        {
            var objClient = new PaymentService.BancHUBPaymentServiceClient();
            var payInfo = DataEncryption.DecryptString(Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(0).ToString(),
                                                                       Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(1).ToString().ToCharArray()).Split('{');
            try
            {
                if (TempData["auth"] == null)
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = payInfo[4]
                                        + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=CreitCard"
                                         + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }

                var sKey = Convert.ToString(Request.Params["hid3"]);
                if (achCa.CaAccountName != null)
                    achCa.CaAccountName = AESEncrytDecry.DecryptStringAES(achCa.CaAccountName, sKey);
                if (achCa.CaAccountNumber != null)
                    achCa.CaAccountNumber = AESEncrytDecry.DecryptStringAES(achCa.CaAccountNumber, sKey);
                if (achCa.BankTransitNumber != null)
                    achCa.BankTransitNumber = AESEncrytDecry.DecryptStringAES(achCa.BankTransitNumber, sKey);

                AccountValidation acctValidation = new AccountValidation()
                {
                    AccountName = achCa.CaAccountName,
                    AccountNumber = achCa.CaAccountNumber,
                    RoutingNumber = achCa.BankTransitNumber
                };

                if (TryValidateModel(acctValidation) && ModelState.IsValid)

                {
                    objClient.Open();

                    var header = new PaymentService.Header
                    {
                        username = payInfo[0],
                        session_token = Convert.ToString(Request.Params["hid2"])
                    };

                    #region Saving account details

                    PaymentService.TokenStatus token = null;

                    if (achCa.SaveAccount || payInfo[9].ToUpper().Equals("TRUE") || (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0))
                    {
                        PaymentService.BankAccountTokenDetails accountToken = new PaymentService.BankAccountTokenDetails()
                        {
                            account_name = achCa.CaAccountName,
                            account_number = achCa.CaAccountNumber,
                            account_type = (PaymentService.AccountType)Enum.Parse(typeof(PaymentService.AccountType), achCa.CaAccountType, true),
                            routing_number = achCa.BankTransitNumber,
                            country_code = "CAN",
                            description = ""
                        };
                        token = objClient.GetBankAccountToken(header, accountToken);

                        if (!token.status.Equals("SUCCESS"))
                        {
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                            + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=ACH"
                                            + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (payInfo[10].ToUpper().Equals("TRUE") && Convert.ToDecimal(payInfo[3]) <= 0)
                        {
                            TempData.Remove("auth");
                            Session.Abandon();
                            return Json(new
                            {
                                success = true,
                                responseText = "",
                                redirectUrl = payInfo[4] + "?transactionid=" + "&status=" + token.status
                                                + "&error_description=" + token.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=ACH"
                                                + "&token_id=" + token.token_id + "&last_four=" + token.display_number.Split('-').GetValue(1)
                                                + "&type=" + token.display_number.Split('-').GetValue(0) + "&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        token = new PaymentService.TokenStatus
                        {
                            token_id = "",
                            display_number = string.Concat(Enumerable.Repeat("X", achCa.CaAccountNumber.Length - 3))
                                                                + achCa.CaAccountNumber.Substring(achCa.CaAccountNumber.Length - 3)
                        };
                    }

                    #endregion

                    #region Payment process

                    var achAccount = new PaymentService.AchAccountDetails
                    {
                        account_name = achCa.CaAccountName,
                        account_number = achCa.CaAccountNumber,
                        account_type = (PaymentService.AccountType)Enum.Parse(typeof(PaymentService.AccountType), achCa.CaAccountType, true),
                        client_referid = payInfo[1],
                        currency = payInfo[2],
                        transaction_amount = Convert.ToDecimal(payInfo[3]),
                        transaction_description = payInfo[8],
                        receiver_identification = "",
                        transaction_effectivedate = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        transaction_type = payInfo[5].ToUpper().Equals("TRUE") ? PaymentService.TransactionType.CREDIT : PaymentService.TransactionType.DEBIT,
                        payment_intent = PaymentService.DepositAccountType.BUSINESS,
                        payment_type = PaymentService.PaymentType.S,
                        ach_type = string.IsNullOrEmpty(payInfo[6]) ? PaymentService.AchType.WEB : (PaymentService.AchType)Enum.Parse(typeof(PaymentService.AchType), payInfo[6]),
                        routing_number = achCa.BankTransitNumber
                    };

                    var billing = new PaymentService.BillingAddress
                    {
                        billing_username = achCa.CaAccountName,
                        user_zip = achCa.CaZipCode,
                        user_city = "XXXXXX",
                        user_state = "XX",
                        user_address1 = "XX, XXXXXXXXXXXX",
                        user_contact = achCa.CaPhoneCode.Split(' ').GetValue(0) + achCa.CaPhone.Replace("-", ""),
                        user_email = achCa.CaEmailId
                    };

                    var st = objClient.AchAccountPayment(header, achAccount, billing);

                    objClient.Close();

                    #endregion

                    #region Payment status

                    if (string.IsNullOrEmpty(st.transaction_id))
                    {
                        st.status = "FAILED";
                        if (string.IsNullOrEmpty(st.error_desc)) st.error_desc = "Transaction failed due to system error. Sorry for the inconvenience caused.";
                    }
                    else
                        st.error_desc = "Payment request received. Final status will be notified shortly.";

                    if (payInfo[7].ToUpper().Equals("TRUE") && st.status.ToUpper().Equals("SUBMITTED") && !string.IsNullOrEmpty(st.transaction_id))
                    {
                        SetEmail email = new SetEmail();
                        if (!email.Send(header.username, achAccount.account_name, Convert.ToDateTime(achAccount.transaction_effectivedate).ToShortDateString(),
                                    Convert.ToString(achAccount.transaction_amount), achAccount.client_referid, achAccount.currency, billing.user_email, ref errorMessage))

                        {
                            {
                                try
                                {
                                    Log log = new Log();
                                    log.ErrorLog(MODULE, "CreditPayment", "Fail to deliver the notifcation to " + achAccount.account_name + " - " + billing.user_email + "::Reason -" + errorMessage);
                                }
                                catch
                                {

                                }
                                st.error_desc = "Fail to deliver the notification. Sorry for the inconvenience caused. You will receive the notification shortly.";
                            }
                        }
                    }

                    #endregion

                    TempData.Remove("auth");
                    Session.Abandon();
                    return Json(new
                    {
                        success = true,
                        responseText = "",
                        redirectUrl = payInfo[4] + "?transactionid=" + st.transaction_id + "&status=" + st.status
                                       + "&error_description=" + st.error_desc + "&referenceid=" + payInfo[1] + "&payment_method=ACH"
                                        + "&token_id=" + token.token_id + "&last_four=" + token.display_number + "&type=&currency="
                                        + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }
                ViewBag.requestType = "AchCa";
                ViewBag.info = Convert.ToString(Request.Params["hid1"]);
                ViewBag.token = Convert.ToString(Request.Params["hid2"]);
                //Keep the temp data on post backs
                TempData.Keep();
                return PartialView("_AchCaPayment", achCa);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "AchCaPayment", ex.Message);
                try
                {
                    objClient.Close();
                }
                catch
                {
                    objClient.Abort();
                }
                TempData.Remove("auth");
                Session.Abandon();
                return Json(new
                {
                    success = true,                    
                    redirectUrl = payInfo[4]
                                    + "?transactionid=&status=FAILED&error_description=System error. Please try again.&referenceid=" + payInfo[1] 
                                    + "&payment_method=ACH" + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
       
        public ActionResult AmazonPayWallet()
        {
           
            clientConfig = new AmazonPay.CommonRequests.Configuration();

            clientConfig.WithAccessKey(ConfigurationManager.AppSettings["access_key"].ToString())
                .WithSecretKey(ConfigurationManager.AppSettings["secret_key"].ToString())
                .WithMerchantId(ConfigurationManager.AppSettings["merchant_id"].ToString())
                .WithClientId(ConfigurationManager.AppSettings["ClientId"].ToString())
                .WithSandbox(true)
                .WithRegion(Regions.supportedRegions.us)
                .WithCurrencyCode(Regions.currencyCode.USD);
            client = new Client(clientConfig);
            Session.Add("ClientObject", client);
            ViewBag.info = Request.Params["hid1"].ToString();
            Session["accesstoken"] = Request.Params["hid2"].ToString();
            return PartialView("_AmazonPayWallet.cshtml");
        }
      

        [HttpPost]
        public ActionResult AmazonPay()
        {
            var objClient = new PaymentService.BancHUBPaymentServiceClient();
            var payInfo = DataEncryption.DecryptString(Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(0).ToString(),
                                                                        Convert.ToString(Request.Params["hid1"]).Split('#').GetValue(1).ToString().ToCharArray()).Split('{');
            try
            {
                if (TempData["auth"] == null)
                {
                    return Json(new
                    {
                        success = true,
                        redirectUrl = payInfo[4]
                                        + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=AmazonPay"
                                         + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }
                var name = Request.Params["PayAuth"];

                if (name == "Pay")
                {
                   
                        string uniqueReferenceId = GenerateRandomUniqueString();
                        ChargeRequest Charge = new ChargeRequest();
                        Charge.WithMerchantId(ConfigurationManager.AppSettings["merchant_id"].ToString())
                            .WithAmazonReferenceId(Request.Params["hid2"])
                            .WithAmount(decimal.Parse(payInfo[3]))
                            .WithCaptureNow(true)
                            .WithInheritShippingAddress(true)
                            .WithChargeReferenceId(uniqueReferenceId)
                            .WithTransactionTimeout(20)
                            .WithChargeNote("Note")
                            .WithCurrencyCode(Regions.currencyCode.USD);

                        AuthorizeResponse chargeResponse = client.Charge(Charge);
                        if (!chargeResponse.GetSuccess())
                        {
                            string errorCode = chargeResponse.GetErrorCode();
                            string errorMessage = chargeResponse.GetErrorMessage();
                            var Authrepose = chargeResponse.GetJson();
                            return Json(new
                            {
                                success = true,
                                redirectUrl = payInfo[4]
                                      + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=AmazonPay"
                                       + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);


                        }
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                responseText = "",

                                redirectUrl = payInfo[4] + "?transactionid='346436'&status='Completed'&error_description='dgfdgf'&referenceid=" + payInfo[1] + "&payment_method=AMPAY&type=&currency="
                                                + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);

                        }
                    
                    
                }
                else if (name == "Authorize")
                {
                        SetOrderReferenceDetailsRequest setRequestParameters = new SetOrderReferenceDetailsRequest();
                    setRequestParameters.WithAmazonOrderReferenceId(Request.Params["hid2"].ToString())
                        .WithAmount(decimal.Parse(payInfo[3]))
                        .WithCurrencyCode(Regions.currencyCode.USD)
                        .WithSellerNote("Note");
                           

                        OrderReferenceDetailsResponse setResponse = client.SetOrderReferenceDetails(setRequestParameters);

                    if (!setResponse.GetSuccess())
                    {

                        return Json(new
                        {
                            success = true,
                            redirectUrl = payInfo[4]
                                           + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=AmazonPay"
                                            + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        string uniqueReferenceId = GenerateRandomUniqueString();

                        AuthorizeRequest authRequestParameters = new AuthorizeRequest();
                        authRequestParameters.WithAmazonOrderReferenceId(Request.Params["hid2"].ToString())
                            .WithAmount(decimal.Parse(payInfo[3]))
                            .WithCurrencyCode(Regions.currencyCode.USD)
                            .WithAuthorizationReferenceId(uniqueReferenceId)
                            .WithTransactionTimeout(0)
                            .WithCaptureNow(true)
                            .WithSellerAuthorizationNote("Note");

                        AuthorizeResponse authResponse = client.Authorize(authRequestParameters);

                        // Authorize was not a success Get the Error code and the Error message
                        if (!authResponse.GetSuccess())
                        {
                            string errorCode = authResponse.GetErrorCode();
                            string errorMessage = authResponse.GetErrorMessage();
                            var Authrepose = authResponse.GetJson();
                            return Json(new
                            {
                                success = true,
                                redirectUrl = payInfo[4]
                                      + "?transactionid=&status=FAILED&error_description=Bad request or the page was refreshed by the external resource. Please try again." + "&referenceid=" + payInfo[1] + "&payment_method=AmazonPay"
                                       + "&token_id=&last_four=&type=&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);


                        }
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                responseText = "",

                                redirectUrl = payInfo[4] + "?transactionid='sfds'&status='fds'&error_description='dgf'&referenceid=" + payInfo[1] + "&payment_method=AMPAY&type=&currency="
                                                + payInfo[2] + "&amount=" + payInfo[3]
                            }, JsonRequestBehavior.AllowGet);

                            //}
                        };
                    }                   
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                      

                       
                        redirectUrl = payInfo[4]
                                            + "?transactionid=&status=FAILED&error_description=System error. Please try again.&referenceid=" + payInfo[1]
                                            + "&payment_method=AMPAY" + "&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                    }, JsonRequestBehavior.AllowGet);
                }


            }
            catch(Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "AmazonPay", ex.Message);
                try
                {
                    objClient.Close();
                }
                catch
                {
                    objClient.Abort();
                }
                TempData.Remove("auth");
                Session.Abandon();
                return Json(new
                {
                    success = true,
                    redirectUrl = payInfo[4]
                                    + "?transactionid=&status=FAILED&error_description=System error. Please try again.&referenceid=" + payInfo[1]
                                    + "&payment_method=AMPAY" + "&currency=" + payInfo[2] + "&amount=" + payInfo[3]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        
      
        public string GenerateRandomUniqueString()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            GuidString = GuidString.Replace("/", "");
            return GuidString;
        }

    }
}