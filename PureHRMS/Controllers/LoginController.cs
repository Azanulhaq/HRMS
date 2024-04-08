using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PureHRMS.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult SecureUserLogin()
        {
            if (Session["UserIdentfier"] != null)
            {
                return RedirectToAction("EmployeeDashboard", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [ActionName("SecureUserLogin")]
        [ValidateAntiForgeryToken]
        public ActionResult SecureUserLoginPost(string userName,string userPassword)
        {
            if (HttpContext.Session["UserIdentfier"] == null)
            {
                if (userName=="" || userPassword==""|| userName ==null || userPassword == null)
                {
                    ViewBag.FoundError = "Username & Passwords are required.";
                }
                else
                {
                    using (HRMSConn dc = new HRMSConn())
                    {
                        tblUserMaster um = new tblUserMaster();
                        var data = dc.tblUserMasters.Where(x => x.EmailId == userName & x.UserPassword == userPassword).ToList();
                        if (data.Count > 0)
                        {
                            if (data[0].Active == true)
                            {
                                Session["UserIdentfier"] = data[0].UserId;

                                return RedirectToAction("EmployeeDashboard", "Home");

                            }
                            else
                            {
                                ViewBag.FoundError = "Sorry, Your account has been suspended.";
                            }
                        }
                        else
                        {
                            ViewBag.FoundError = "Sorry, You are not registered with us.";
                        }

                    }
                }
               
            }
            else
            {
                return RedirectToAction("EmployeeDashboard", "Home");
            }
           
           
                return View();
        }
    }
}