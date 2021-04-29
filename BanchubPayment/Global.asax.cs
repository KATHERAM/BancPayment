using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace BanchubPayment
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    var frm = HttpContext.Current.Request.Form;

        //    if (frm.Count > 0 && HttpContext.Current.Request.CurrentExecutionFilePath == "/Authenticate")
        //    {

        //        string filename = Server.MapPath("~//") + "ErroLogs.txt";
        //        StreamWriter sw = new System.IO.StreamWriter(filename, true);
        //        sw.Write(DateTime.Now + "\n\r" + "First" + "\n\r\n\r");
        //        sw.Close();

        //        var collection = System.Web.HttpContext.Current.Request.Form;
        //        var propInfo = collection.GetType().GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        //        propInfo.SetValue(collection, false, new object[] { });
        //        //collection.Add("email1", "mymail@domain.com");

        //        var val1 = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(collection["ClientId"]));
        //        val1 = new string(val1.ToCharArray().Reverse().ToArray());

        //        var val = DecryptStringAES(collection["Input"], val1);
        //        collection.Remove("Input");
        //        collection.Remove("ClientId");
        //        if (val.Length > 0)
        //        {
        //            var param = val.Split('&');

        //            foreach (string p in param)
        //            {
        //                var tmp = p.Split(new[] { '=' }, 2);
        //                collection.Add(@tmp[0].ToString(), Uri.EscapeDataString(@tmp[1]));
        //            }
        //        }
        //    }

        //}

        //protected static string DecryptStringAES(string cipherText, string skey = null)
        //{

        //    //var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
        //    //var iv = Encoding.UTF8.GetBytes("8080808080808080");
        //    skey = skey.PadLeft(16, '0');
        //    var keybytes = Encoding.UTF8.GetBytes(skey.Substring(0, 16));
        //    var iv = Encoding.UTF8.GetBytes(skey.Substring(0, 16));

        //    var encrypted = Convert.FromBase64String(cipherText);
        //    var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
        //    return string.Format(decriptedFromJavascript);
        //}

        //private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        //{
        //    // Check arguments.
        //    if (cipherText == null || cipherText.Length <= 0)
        //    {
        //        throw new ArgumentNullException("cipherText");
        //    }
        //    if (key == null || key.Length <= 0)
        //    {
        //        throw new ArgumentNullException("key");
        //    }
        //    if (iv == null || iv.Length <= 0)
        //    {
        //        throw new ArgumentNullException("key");
        //    }

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    string plaintext = null;

        //    // Create an RijndaelManaged object
        //    // with the specified key and IV.
        //    using (var rijAlg = new RijndaelManaged())
        //    {
        //        //Settings
        //        rijAlg.Mode = CipherMode.CBC;
        //        rijAlg.Padding = PaddingMode.PKCS7;
        //        rijAlg.FeedbackSize = 128;

        //        rijAlg.Key = key;
        //        rijAlg.IV = iv;

        //        // Create a decrytor to perform the stream transform.
        //        var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
        //        try
        //        {
        //            // Create the streams used for decryption.
        //            using (var msDecrypt = new MemoryStream(cipherText))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {

        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        // Read the decrypted bytes from the decrypting stream
        //                        // and place them in a string.
        //                        plaintext = srDecrypt.ReadToEnd();

        //                    }

        //                }
        //            }
        //        }
        //        catch
        //        {
        //            plaintext = "keyError";
        //        }
        //    }

        //    return plaintext;
        //}
    }
}