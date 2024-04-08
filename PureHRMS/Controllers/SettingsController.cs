using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class SettingsController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult GeneralSettingsUpdate(string strerr)
        {
            if(strerr!=null)
            {
                ViewBag.FoundError = strerr;
            }
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblSettingsMasters.SingleOrDefault();
                return View(data);
            }
                
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("GeneralSettingsUpdate")]
        [ValidateAntiForgeryToken]
        public ActionResult GeneralSettingsUpdatePost(HttpPostedFileBase comlogo)
        {
            using (HRMSConn dc=new HRMSConn())
            {
              try
                {
                    var data = dc.tblSettingsMasters.SingleOrDefault();
                    TryUpdateModel(data);
                    if (ModelState.IsValid)
                    {

                        string imgname = data.CompanyLogo;
                        string filename = "";
                        if (comlogo != null)
                        {

                            if (comlogo.ContentLength > 2500000)
                            {
                                ViewBag.FoundError = "Uploaded image is greater than 2 MB, upload small image";
                                return RedirectToAction("GeneralSettingsUpdate", new { strerr = ViewBag.FoundError });
                            }
                            else
                            {
                                string uniqueid = System.DateTime.Now.Year.ToString() +
                                    System.DateTime.Now.Month.ToString() +
                                    System.DateTime.Now.Day.ToString() +
                                    System.DateTime.Now.Hour.ToString() +
                                    System.DateTime.Now.Minute.ToString() +
                                    System.DateTime.Now.Millisecond.ToString();
                                string ext = System.IO.Path.GetExtension(comlogo.FileName);
                                filename = "Logo-" + uniqueid + ext;
                                imgname = data.CompanyLogo;
                                if (System.IO.File.Exists(Server.MapPath("~/UploadedData/CompanyLogo/" + imgname)))
                                {
                                    System.IO.File.Delete(Server.MapPath("~/UploadedData/CompanyLogo/" + imgname));
                                }
                                comlogo.SaveAs(Server.MapPath("~/UploadedData/CompanyLogo/" + filename));
                            }
                        }
                        else
                        {
                            filename = data.CompanyLogo;
                        }
                        data.CompanyLogo = filename;
                        dc.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dc.SaveChanges();

                        ViewBag.FoundError = "Data updated successfully";
                        return RedirectToAction("GeneralSettingsUpdate", new { strerr = ViewBag.FoundError });
                    }
                    else
                    {
                        return RedirectToAction("GeneralSettingsUpdate");
                    }
                }
                catch
                {
                    ViewBag.FoundError = "Sorry , some error found. Please fill fields carefully.";
                    return RedirectToAction("GeneralSettingsUpdate", new { strerr = ViewBag.FoundError });
                }


            }
               
          
           
        }
    }
}