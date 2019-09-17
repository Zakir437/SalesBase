using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PointOfSale.Models;
using PointOfSale.ModelViews;

namespace PointOfSale.Controllers
{
    public class AccountController : Controller
    {
        PointOfSale_DBEntities db = new PointOfSale_DBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            try
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("CookieUserInfo");
                if (cookie == null)
                {
                    return this.View();
                }
                else
                {
                    return RedirectToAction("Index", "PointOfSale");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserLoginModelView model)
        {
            var aUser = db.Users.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);
            if (aUser != null)
            {
                HttpCookie cookie = new HttpCookie("CookieUserInfo");
                cookie.Values["UserName"] = aUser.FirstName + " " + aUser.LastName;
                cookie.Values["UserId"] = aUser.Id.ToString();
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "PointOfSale");
            }
            else
            {
                ViewBag.message = "Invalid Username or Password.";
            }
            return View();
        }
        public ActionResult LogOff()
        {
            try
            {
                var c = new HttpCookie("CookieUserInfo");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Login");
        }
    }
}