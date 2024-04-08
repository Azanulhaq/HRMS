using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using PureHRMS.Models;

namespace PureHRMS.Controllers
{
    public class DesignationsController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult InsertDesignation()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataDepts = dc.tblDepartmentMasters.ToList();
                ViewBag.DeptList = dataDepts;
                return View();
            }

        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("InsertDesignation")]
        public ActionResult InsertDesignationPost()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                tblDesignationMaster dm = new tblDesignationMaster();
                TryUpdateModel(dm);
                if (ModelState.IsValid)
                {
                    dc.tblDesignationMasters.Add(dm);
                    dc.SaveChanges();
                    return RedirectToAction("InsertDesignation");

                }
                else
                {
                    var dataDepts = dc.tblDepartmentMasters.ToList();
                    ViewBag.DeptList = dataDepts;                  
                    return View();
                }

            }

        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDesignation(string id, string desigName, int RefDepartmentId)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                try
                {
                    int desigid = Convert.ToInt32(id);
                    int refdept = Convert.ToInt32(RefDepartmentId);
                    var data = dc.tblDesignationMasters.Where(x => x.DesignationId == desigid).SingleOrDefault();
                    data.DesignationName = desigName;
                    data.RefDepartmentId = refdept;
                    dc.SaveChanges();
                    return RedirectToAction("InsertDesignation");
                }
                catch
                {
                    return RedirectToAction("InsertDesignation");
                }
               
                
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDesignation(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int Desigtid = Convert.ToInt32(id);
                var data = dc.tblDesignationMasters.Where(x => x.DesignationId == Desigtid).SingleOrDefault();
                dc.tblDesignationMasters.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("InsertDesignation");
            }
        }
    }
}