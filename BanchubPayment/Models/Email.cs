using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace BanchubPayment.Models
{
    public class Email
	{
		//private readonly string moduleName = "SendEmail";

		private readonly string emailUrl = ConfigurationManager.AppSettings["emailUrl"];
		private readonly string emailToken = ConfigurationManager.AppSettings["emailToken"];
		private readonly string emailAuth = ConfigurationManager.AppSettings["emailAuth"];

		
		public string CC
		{
			get; set;
		}
        
		public string Port
		{
			get; set;
		}
        
		public string Note
		{
			get; set;
		}

		public string EmailId
		{
			get; set;
		}

		public string Subject
		{
			get; set;
		}

		public string header
		{
			get; set;
		}
        
		public bool SendEmail(string user, string merchant, string orderId, string transactionId, string remarks,string status, 
		                    string acNumber,string amount,ref string emailResult)
        {
			try
			{
				EmailProperties emailProperties = new EmailProperties
				{
					notificationMessage = SetBody(user, merchant, orderId, transactionId, remarks,status, acNumber,amount),
                    supportMailId = ConfigurationManager.AppSettings["FromAddress"],
					notificationSubject = Subject,
					notificationContentType = "text/html",
					notificationType = "EMAIL",
					recipientsAddress = new List<string> { EmailId }
				};

				string responseJson = "";
				var json = new JavaScriptSerializer().Serialize(emailProperties);

			    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpWebResponse response = ProcessWebRequest(emailUrl + emailToken, "POST", json);
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					responseJson = reader.ReadToEnd();
				}

				EmailResponse result = new EmailResponse();
				if (response.StatusCode == HttpStatusCode.OK)
					result = JsonConvert.DeserializeObject<EmailResponse>(responseJson);
				else
					result = JsonConvert.DeserializeObject<EmailResponse>(responseJson);

			    emailResult = result.responseMessage;

                return result.responseMessage.Contains("Success")? true:false;
			}
			catch
			{
				return false;
			}
		}

	    public string SetBody(string user, string merchant,string orderId,string transactionId,string remarks,string status,string acNumber,string amount)
	    {
	        try
	        {
	            string body = "<html><body><p>Dear" + " " + user + ",</p>";
	            body += "</br>&nbsp;&nbsp;&nbsp;&nbsp; Thanks for your payment using BancHUB service."
                        + "</br>&nbsp;&nbsp;&nbsp;&nbsp; Please find your transaction details below."
                        + "</br>&nbsp;&nbsp;&nbsp;&nbsp; <table style='width:50%;margin-left:5%'><tr><td style='font-weight:bold'>Order Id</td><td>" +orderId + "</td></tr>"
	                    + "<tr><td style = 'font-weight:bold'> Account Number </td><td>" + acNumber + "</td></tr>"
	                    + "<tr><td style = 'font-weight:bold'> Amount </td><td>" + amount + "</td></tr>"
                        + "<tr><td style = 'font-weight:bold'> Transaction Id </td><td>" + transactionId + "</td></tr>"
	                    + "<tr><td style = 'font-weight:bold'> Transaction Status </td><td>" + status + "</td></tr>"
                        + "<tr><td style = 'font-weight:bold'> Remarks </td><td>" + remarks + "</td></tr></table>"

	                    //+ "</br></br>&nbsp;&nbsp;&nbsp;&nbsp; If the payment not done by you, please contact your card issuer or bank for further action."
                        + "</br></br></br>Regards"
	                    + "</br>BancHUB</body></html>";


                return body;
	        }
	        catch
	        {
	            return "";
	        }
	    }

		private HttpWebResponse ProcessWebRequest(string url, string requestMethod, string json)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

			WebRequest request = WebRequest.Create(url);
			request.ContentType = "application/json";			
			request.Method = requestMethod;
			request.Headers.Add("Authorization", emailAuth);
                        
			// If required by the server, set the credentials.
			request.Proxy = WebProxy.GetDefaultProxy();
			request.Credentials = ConfigurationManager.AppSettings["isNetworkCredentials"].ToUpper().Equals("Y")
									? new NetworkCredential(ConfigurationManager.AppSettings["username"], ConfigurationManager.AppSettings["pass"]
															, ConfigurationManager.AppSettings["domain"])
									: CredentialCache.DefaultCredentials;

			using (var streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(json);
				streamWriter.Flush();
			}

			// Get the response.
			return (HttpWebResponse)request.GetResponse();
		}
	}

	public class EmailProperties
	{
		public string supportMailId
		{
			get; set;
		}

		public List<string> recipientsAddress
		{
			get; set;
		}

		public string notificationSubject
		{
			get; set;
		}

		public string notificationMessage
		{
			get; set;
		}

		public string notificationType
		{
			get; set;
		}

		public string notificationContentType
		{
			get; set;
		}

		public List<string> recipientsCCAddress
		{
			get; set;
		}

		public List<Attachments> attachments
		{
			get; set;
		}
	}

	public class Attachments
	{
		public string fileContent
		{
			get; set;
		}
		public string fileName
		{
			get; set;
		}

	}

	public class EmailResponse
	{
		public int responseCode
		{
			get; set;
		}

		public string responseMessage
		{
			get; set;
		}

		public string status
		{
			get; set;
		}

		public string notificationTitle
		{
			get; set;
		}

		public string notificationReportUUID
		{
			get; set;
		}

		public string responseOptionReportUUID
		{
			get; set;
		}

		public string notificationResponseStatus
		{
			get; set;
		}
	}
}