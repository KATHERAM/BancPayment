using BanchubPayment.Models;
using BancHubPayment.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BanchubPayment.Controllers
{
    public class AuthenticateController : Controller
    {
        //
        // GET: /Authenticate/
        [HttpPost]
        public ActionResult Index(Authenticate auth)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //AuthenticateModel authModel = null;
                    
                    //var val1 = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(auth.ClientId.ToLower()));                    
                    //val1 = new string(val1.ToCharArray().Reverse().ToArray());                    
                    //var val = DataEncryption.DecryptStringAES(auth.Param, val1);                    
                    //if (val.Length > 0)
                    //{
                    //    var dict = HttpUtility.ParseQueryString(val);
                    //    string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
                    //    authModel = JsonConvert.DeserializeObject<AuthenticateModel>(json);
                    //}                    
                    return View(auth);

                    //auth.ClientId = DecodeParam(auth.ClientId);
                    //auth.AchType = DecodeParam(auth.AchType);
                    //auth.AuthToken = DecodeParam(auth.AuthToken);
                    //auth.Apikey = DecodeParam(auth.Apikey);
                    //auth.Currency = DecodeParam(auth.Currency);
                    //auth.Description = DecodeParam(auth.Description);
                    ////auth.Email = DecodeParam(auth.Email);
                    //auth.PaymentOptions = DecodeParam(auth.PaymentOptions);
                    ////auth.Phon = DecodeParam(auth.Phone);
                    //auth.ReferenceId = DecodeParam(auth.ReferenceId);
                    //auth.Url = DecodeParam(auth.Url);

                    //return View(auth);
                }
                else
                    return new RedirectResult("../Error");
            }
            catch
            {
                return new RedirectResult("../Error");                
            }
        }

        //private string DecodeParam(string val)
        //{
        //    if (string.IsNullOrEmpty(val))
        //        return null;

        //    StringWriter writer = new StringWriter();
        //    Server.UrlDecode(val, writer);
        //    return writer.ToString();
        //}

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Authenticate(Authenticate auth)
        {
            string token = "";

            AuthenticateModel authModel = null;

            var val1 = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(auth.ClientId.ToLower()));
            val1 = new string(val1.ToCharArray().Reverse().ToArray());
            var val = DataEncryption.DecryptStringAES(auth.Param, val1);
            if (val.Length > 0)
            {
                var dict = HttpUtility.ParseQueryString(val);
                string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
                var url = dict.GetValues("Url").GetValue(0);                
                var referenceId = dict.GetValues("ReferenceId").GetValue(0);
                try
                {
                    authModel = JsonConvert.DeserializeObject<AuthenticateModel>(json);
                }
                catch(JsonException)
                {
                    return Json(new { success = false, responseText = "The request has been failed.", returnUrl = url + "?transactionid=&status=FAILED&error_description=The request has been failed. Please try again&referenceid=" + referenceId }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                TempData.Clear();
                if(TryValidateModel(authModel))
                //if (ModelState.IsValid)
                {
                    decimal amt = 0;

                    try
                    {
                        if (!decimal.TryParse(authModel.Amount.ToString(), out amt))
                            amt = 0;
                    }
                    catch
                    {
                        amt = 0;
                    }

                    if (string.IsNullOrEmpty(authModel.ClientId) || string.IsNullOrEmpty(authModel.Apikey) || string.IsNullOrEmpty(authModel.ReferenceId) || 
                        (amt == 0 && !authModel.AuthorizeOnly) || string.IsNullOrEmpty(authModel.Currency) || string.IsNullOrEmpty(authModel.Url))
                    {
                        return Json(new { success = false, responseText = "The request has been failed.", returnUrl = authModel.Url + "?transactionid=&status=FAILED&error_description=The requested input param(s) data is not valid. Please try again&referenceid=" + authModel.ReferenceId }, JsonRequestBehavior.AllowGet);
                    }
                    
                    //Autheticate the user using the BancHUB API
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    PaymentService.BancHUBPaymentServiceClient clnt = new PaymentService.BancHUBPaymentServiceClient();
                    clnt.Open();

                    token = clnt.Authorize(new PaymentService.Userdetails { username = auth.ClientId, apikey = authModel.Apikey });
                    if (token == "Invalid user details." || token == "Required user/apikey details.")
                        return Json(new { success = false, responseText = "Invalid user details.", returnUrl = authModel.Url + "?transactionid=&status=FAILED&error_description=Invalid user details.&referenceid=" + authModel.ReferenceId }, JsonRequestBehavior.AllowGet);

                    authModel.AuthToken = token;
                    TempData["auth"] = authModel;
                    //TempData.Keep();
                    //Session["auth"] = auth;
                    //return this.RedirectToAction("Payment", "Process", auth);
                }
                else
                    return Json(new { success = false, responseText = "The request has been failed.", returnUrl = authModel.Url + "?transactionid=&status=FAILED&error_description=The request has been failed. Please try again&referenceid=" + authModel.ReferenceId }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { success = false, responseText = "The request has been failed.", returnUrl = authModel.Url + "?transactionid=&status=FAILED&error_description=The request has been failed. Please try again&referenceid=" + authModel.ReferenceId }, JsonRequestBehavior.AllowGet);
            }

            //return Json(new { success = true, responseText= auth }, JsonRequestBehavior.AllowGet);
             return Json(new { success = true, responseText = token }, JsonRequestBehavior.AllowGet);
        }

    }
}
