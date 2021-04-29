using System.Web.Mvc;

namespace BanchubPayment.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        [HandleError]
        public ActionResult Error()
        {
            return View();
        }

    }
}
