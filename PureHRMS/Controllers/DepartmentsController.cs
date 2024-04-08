using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;

namespace PureHRMS.Controllers
{
    public class DepartmentsController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult InsertDepartment()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataAll = dc.tblDepartmentMasters.OrderByDescending(x => x.DepartmentId).ToList();
                ViewBag.DepartmentList = dataAll;
                return View();

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("InsertDepartment")]
        public ActionResult InsertDepartmentPost()
        {

            using (HRMSConn dc = new HRMSConn())
            {
                var dataAll = dc.tblDepartmentMasters.OrderByDescending(x => x.DepartmentId).ToList();
                ViewBag.DepartmentList = dataAll;

                tblDepartmentMaster dm = new tblDepartmentMaster();
                TryUpdateModel(dm);

                if (ModelState.IsValid)
                {
                    dc.tblDepartmentMasters.Add(dm);
                    dc.SaveChanges();
                    ModelState.Clear();

                    return RedirectToAction("InsertDepartment");
                }
                else
                {
                    return View();
                }


            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDepartment(string id,string deptname)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                try
                {
                    int Deptid = Convert.ToInt32(id);
                    var data = dc.tblDepartmentMasters.Where(x => x.DepartmentId == Deptid).SingleOrDefault();
                    data.DepartmentName = deptname;
                    dc.SaveChanges();
                    return RedirectToAction("InsertDepartment");

                }
                catch
                {
                    return RedirectToAction("InsertDepartment");

                }
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDepartment(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int Deptid = Convert.ToInt32(id);
                var data = dc.tblDepartmentMasters.Where(x => x.DepartmentId == Deptid).SingleOrDefault();
                dc.tblDepartmentMasters.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("InsertDepartment");
            }
        }
    }
}