using BancHubPayment.Models;
using System.Web.Mvc;

namespace BanchubPayment.Controllers
{
    public class StatusController : Controller
    {
        //
        // GET: /Status/        
        public ActionResult Status(StatusModel sm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.UserName = "Testuser";
                ViewBag.logo = "../images/utility.gif";
                return View(sm);
            }
            else
                return View(sm);
        }

    }
}
