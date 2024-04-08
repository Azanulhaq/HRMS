using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class DocsController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult AddDoc(string msg)
        {
            if(msg!=null && msg!="")
            {
                ViewBag.FoundError = msg;
            }
            using (HRMSConn dc = new HRMSConn())
            {
                ViewBag.DocList = dc.tblCommonDocs.ToList();
                return View();

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("AddDoc")]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocPost(string docname, HttpPostedFileBase fileposted)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                ViewBag.DocList = dc.tblCommonDocs.ToList();

                //////////////
                if (docname == null || docname == "")
                {
                    ViewBag.FoundError = "Please enter file name";
                    return View();
                }
                //////
                if (fileposted == null)
                {
                    ViewBag.FoundError = "Please choose file to upload";
                    return View();
                }
                else
                {
                    string FilenameUploaded = "";
                    if (fileposted.ContentLength > 2500000)
                    {
                        ViewBag.FoundError = "Uploaded image is greater than 2 MB, please upload small file";
                        return View();
                    }
                    else
                    {
                       
                        string uniqueid = System.DateTime.Now.Year.ToString() +
                         System.DateTime.Now.Month.ToString() +
                         System.DateTime.Now.Day.ToString() +
                         System.DateTime.Now.Hour.ToString() +
                         System.DateTime.Now.Minute.ToString() +
                         System.DateTime.Now.Millisecond.ToString();
                        string ext = System.IO.Path.GetExtension(fileposted.FileName);
                        FilenameUploaded = "CommonDocs_" + uniqueid + ext;
                        fileposted.SaveAs(Server.MapPath("~/UploadedData/" + FilenameUploaded));
                        tblCommonDoc cd = new tblCommonDoc();
                        cd.DateAdded = System.DateTime.Now;
                        cd.DocFileName = FilenameUploaded;
                        cd.DocName = docname;
                        dc.tblCommonDocs.Add(cd);
                        dc.SaveChanges();
                        return RedirectToAction("AddDoc", new { msg= "Document uploaded successfully" });
                    }
                }
         
               
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoc(string id)
        {
            
            using (HRMSConn dc = new HRMSConn())
            {
                int docid = Convert.ToInt32(id);
                var data = dc.tblCommonDocs.Where(x=>x.DocId== docid).SingleOrDefault();
                if(data!=null)
                {
                    string fileTodelete = data.DocFileName;
                    if(System.IO.File.Exists(Server.MapPath("~/UploadedData/" + fileTodelete)))
                    {
                        System.IO.File.Delete(Server.MapPath("~/UploadedData/" + fileTodelete));
                    }
                    dc.tblCommonDocs.Remove(data);
                    dc.SaveChanges();
                    return RedirectToAction("AddDoc", new { msg = "Document deleted successfully" });
                }
                return RedirectToAction("AddDoc");

            }
        }

        
        public ActionResult DisplayDoc()
        {
        
            using (HRMSConn dc = new HRMSConn())
            {
                ViewBag.DocList = dc.tblCommonDocs.ToList();
                return View();

            }
        }

    }
}