using EmailLibrary;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace BanchubPayment.Models
{
    public class SetEmail
    {
        private static readonly string MODULE = "PaymentProcess-Email";
        public bool Send(string customer,string receiverName,string tranDate,string amount,string orderId,string currency,string emailId,ref string status)
        {
            try
            {
                var path =ConfigurationManager.AppSettings["EmailTemplet"];
                var emailUrl = ConfigurationManager.AppSettings["emailUrl"];
                var key = ConfigurationManager.AppSettings["emailToken"];
                var auth = ConfigurationManager.AppSettings["emailAuth"];

                string messageBody = string.Empty;

                var filename = customer + "-body.html";
                var filePath = System.Web.HttpContext.Current.Server.MapPath(path);

                var validFile= CanonicalCombine(filePath,filename);
                
                //if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(Path.Combine(path, customer + "-body.html"))))
                if(File.Exists(validFile))
                {
                    //Get message body
                    //messageBody = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(Path.Combine(path, customer + "-body.html")));
                    messageBody = File.ReadAllText(validFile);
                    messageBody = messageBody.Replace("#name", receiverName);
                    messageBody = messageBody.Replace("#date", tranDate);
                    messageBody = messageBody.Replace("#amount", currency + ' ' + amount);
                }
                else if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(Path.Combine(path, "Default.html"))))
                {
                    //Get message body
                    messageBody = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(Path.Combine(path, "Default.html")));
                    messageBody = messageBody.Replace("#name", receiverName);
                    messageBody = messageBody.Replace("#customer", customer);
                    messageBody = messageBody.Replace("#date", tranDate);
                    messageBody = messageBody.Replace("#amount", currency + ' ' + amount);
                }
                //Get attachment
                List<Attachment> att = null;
                var invoiceFileName = customer + "-invoice.pdf";               

                var invoiceFile = CanonicalCombine(filePath, invoiceFileName);                               
                
                //if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(Path.Combine(path, customer + "-invoice.pdf"))))
                if(File.Exists(invoiceFile))
                {
                    //string file = ImprintPdf(Path.Combine(path, customer + "-invoice.pdf"), orderId, tranDate, currency + ' ' + amount);
                    string file = ImprintPdf(invoiceFile, orderId, tranDate, currency + ' ' + amount);

                    if (!string.IsNullOrEmpty(file))
                    {
                        att = new List<Attachment>
                                {
                                    new Attachment() { content = file, fileName = customer + "-invoice.pdf" }
                                };
                    }
                }

                status = EmailLibraryClass.SendMail(emailId
                                               , ""
                                               , ""
                                               , customer + " payment confirmation"
                                               , messageBody
                                               , auth
                                               , key
                                               , emailUrl, att);
                if (status.ToUpper().Equals("OK"))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "CreditPayment", ex.Message);
                return false;
            }
        }

        private string ImprintPdf(string pdfPath, string orderId, string date, string amount)
        {
            try
            {
                PdfReader reader = null;

                //var tempPath = Path.GetTempFileName().Replace(".tmp", ".pdf"); 
                var tempPath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf";

                lock (this)
                {
                    //reader = new PdfReader(File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath(pdfPath)));
                    reader = new PdfReader(File.ReadAllBytes(pdfPath));
                }

                List<string> _lstFields = null;
                _lstFields = reader.AcroFields.Fields.Keys.OfType<string>().ToList();

                PdfStamper formFiller = new PdfStamper(reader, new FileStream(tempPath, FileMode.Create));

                AcroFields formFields = formFiller.AcroFields;
                formFields.GenerateAppearances = true;

                if (formFields != null && formFields.Fields.Count > 0)
                {
                    foreach (string fieldKey in formFields.Fields.Keys)
                    {
                        if (fieldKey == "orderid")
                        {
                            formFields.SetField(fieldKey, orderId);
                        }
                        else if (fieldKey == "amount")
                        {
                            formFields.SetField(fieldKey, amount);
                        }
                        else if (fieldKey == "date")
                        {
                            formFields.SetField(fieldKey, date);
                        }

                    }
                }
                formFiller.FormFlattening = true;
                formFiller.Close();

                byte[] imageArray = File.ReadAllBytes(tempPath);
                File.Delete(tempPath);
                return Convert.ToBase64String(imageArray);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.ErrorLog(MODULE, "CreditPayment", ex.Message);
                return null;
            }            
        }
        
        public string CanonicalCombine(string basePath, string path)
        {
            if (path.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
                return null;

            string filePath = Path.Combine(basePath, path);

            if (filePath.IndexOfAny(Path.GetInvalidPathChars()) > -1)
                return null;

            if (!filePath.StartsWith(basePath))
                return null;            

            return filePath;
        }
    }
}