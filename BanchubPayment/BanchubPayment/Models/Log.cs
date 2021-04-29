using System;
using System.IO;
using System.Web.Hosting;

namespace BanchubPayment.Models
{
    public class Log
    {
        public void ErrorLog(string modName, string functionName, string errDesc)
        {
            try
            {
                lock (this)
                {
                    StreamWriter objWriter;
                    string sPath = HostingEnvironment.ApplicationPhysicalPath + "\\" + "GenericError.txt";
                    //sPath = AppDomain.CurrentDomain.RelativeSearchPath + "\\" + "GenericError.txt"; 
                    // AppDomain.CurrentDomain.BaseDirectory 
                    //const string sPath = @"F:\\GenericError.txt";
                    using (objWriter = new StreamWriter(sPath, true))
                    {
                        objWriter.WriteLine(string.Format("{0:ddMMyyyy hh:mm:ss}", DateTime.Now) + "-" + modName + "-" + functionName + "-" + errDesc);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

}