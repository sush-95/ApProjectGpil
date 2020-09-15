using AccountPaybleWeb.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static AccountPaybleWeb.Manager.ViewModelClass;

namespace AccountPaybleWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string uname, string pwd)
        {
            string Controller = string.Empty, action = string.Empty;
            try
            {
               
                LoginModel obj = MVCManager.CheckLogin(uname.Trim(), pwd.Trim());
                if (obj != null)
                {
                    Session["User"] = obj;
                    return RedirectToAction("Home", "AP");
                }
                else
                {
                    TempData["error"] = "Wrong Credential.";
                    return RedirectToAction("Login");
                }
               
            }
            catch (Exception ex)
            {
                TempData["error"] = "Something went wrong.";
                return RedirectToAction("Login");
            }
           
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}