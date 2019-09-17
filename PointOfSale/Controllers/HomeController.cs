using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.Helpers;

namespace PointOfSale.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public JsonResult GetEncryptedData(string text)
        {
            return Json(MyExtension.Encrypt(text), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Dashboard()
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get("CookieUserInfo");
            if (cookie != null)
            {
                return this.View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}