using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class TimeSheetController : Controller
    {
        bool sts = false;
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult AddTimeSheetDetail(string TimeDate)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                DateTime? dtchange = null;
                
              
                int empid = Convert.ToInt32(Session["UserIdentfier"]);
                var data = dc.tblProjectAllotteds.Where(x => x.refEmployeeId == empid).ToList();
                List<SelectListItem> projectlist = new List<SelectListItem>();
                projectlist.Add(new SelectListItem { Text = "-Please Select-", Value = "0" });
                foreach (var item in data)
                {
                    int pid = Convert.ToInt32(item.refProjectId);
                    var getpro = dc.tblProjectMasters.Where(x => x.ProjectId == pid && x.ProjectStatus != "Completed").ToList();
                    if (getpro.Count > 0)
                    {
                        int projectID = Convert.ToInt32(getpro[0].ProjectId);
                        projectlist.Add(new SelectListItem { Text = getpro[0].ProjectTitle, Value = projectID.ToString() });
                    }
                    else
                    {
                        ;
                    }
                }
                ViewBag.AllProjectsAssociats = projectlist;
                if (TimeDate == null || TimeDate == "" )
                {
                    dtchange = null;
                }
                else
                {
                    dtchange = Convert.ToDateTime(Convert.ToDateTime(TimeDate).ToShortDateString());
                }

                var oldentries = dc.tblTimeSheetManagers.Where(x => x.refTimeEmpId == empid && x.TimeDate == dtchange).ToList();
                ViewBag.OldEntries = oldentries;
            }
            return View();
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("AddTimeSheetDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult AddTimeSheetDetailPost(int ddlProjects, string TimeDate)
        {
            using (HRMSConn dc = new HRMSConn())
            {
               

                DateTime? dtchange = null;
                int empid = Convert.ToInt32(Session["UserIdentfier"]);
                var data = dc.tblProjectAllotteds.Where(x => x.refEmployeeId == empid).ToList();
                List<SelectListItem> projectlist = new List<SelectListItem>();
                projectlist.Add(new SelectListItem { Text = "-Please Select-", Value = "0" });
                foreach (var item in data)
                {
                    int pid = Convert.ToInt32(item.refProjectId);
                    var getpro = dc.tblProjectMasters.Where(x => x.ProjectId == pid && x.ProjectStatus != "Completed").ToList();
                    if (getpro.Count > 0)
                    {
                        int projectID = Convert.ToInt32(getpro[0].ProjectId);
                        projectlist.Add(new SelectListItem { Text = getpro[0].ProjectTitle, Value = projectID.ToString() });
                    }
                    else
                    {
                        ;
                    }
                }

                ViewBag.AllProjectsAssociats = projectlist;
                if (TimeDate == null || TimeDate == "")
                {
                    ViewBag.FoundError = "Please select date";
                    return View();
                }
                dtchange = Convert.ToDateTime(Convert.ToDateTime(TimeDate).ToShortDateString());
                var oldentries = dc.tblTimeSheetManagers.Where(x => x.refTimeEmpId == empid && x.TimeDate == dtchange).ToList();
                ViewBag.OldEntries = oldentries;
                if (oldentries.Count==0)
                {
                    ;
                }
                else
                {
                    DateTime creadted =Convert.ToDateTime(oldentries[0].CreatedOn);
                    DateTime dtToday =System.DateTime.Now;
                    TimeSpan difference = dtToday - creadted;
                    if(difference.Days<=2)
                    {
                        ;
                    }
                    else
                    {
                        ViewBag.FoundError = "You are not allowed to add any comments after two days.";
                        return View();
                    }
                }
                
                if (ddlProjects == 0)
                {
                    ViewBag.FoundError = "Please select project";                   
                    
                    return View();
                }
                else
                {
                  

                    tblTimeSheetManager tm = new tblTimeSheetManager();
                    tm.CreatedOn = System.DateTime.Now;
                    tm.refTimeEmpId = empid;
                    tm.refTimeProjectId = ddlProjects;
                    TryUpdateModel(tm);
                    if (ModelState.IsValid)
                    {
                        dc.tblTimeSheetManagers.Add(tm);
                        dc.SaveChanges();
                        ModelState.Clear();
                        ///

                        var oldentriesposted = dc.tblTimeSheetManagers.Where(x => x.refTimeEmpId == empid && x.TimeDate == dtchange).ToList();
                        ViewBag.OldEntries = oldentriesposted;
                        ///
                        DateTime dtnew = Convert.ToDateTime(dtchange);
                        string QueryDate = dtnew.ToShortDateString();
                        return RedirectToAction("AddTimeSheetDetail",new { TimeDate= QueryDate });
                    }
                    else
                    {
                        return View();
                    }
                }



            }


        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTimeshhetDetail(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int tid = Convert.ToInt32(id);                
                var data = dc.tblTimeSheetManagers.Where(x => x.TimeId == tid).SingleOrDefault();
                DateTime dtnew = Convert.ToDateTime(data.TimeDate);
                string QueryDate = dtnew.ToShortDateString();
                dc.tblTimeSheetManagers.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("AddTimeSheetDetail", new { TimeDate = QueryDate });
            }
        }

        [CkeckAllowedAccess]
        public ActionResult TimeSheetTeamMember(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int eid = Convert.ToInt32(id);
                var dataUser = dc.tblUserMasters.Where(x => x.UserId == eid).SingleOrDefault();
                ViewBag.EmployeeID = dataUser.EmpId;
                    ViewBag.Identifier = Session["UserIdentfier"].ToString();
                ViewBag.EmployeeName = dataUser.FullName;
                bool sts = CheckAcessForTimeSheetDisplay(Session["UserIdentfier"].ToString(), eid);
                if(sts)
                {
                    var data = dc.tblTimeSheetManagers.Where(x => x.refTimeEmpId == eid).OrderByDescending(x=>x.TimeId).ToList();

                    return View(data);
                }
                else
                {
                    return Redirect("/Home/EmployeeDashboard/");
                }
               

            }
        }
        public bool CheckAcessForTimeSheetDisplay(string loginid, int usertoassign)
        {

            using (HRMSConn dc = new HRMSConn())
            {
                int employeeid = Convert.ToInt32(loginid);
                var getchilds = dc.tblTeamMasters.Where(y => y.TeamParentUser == employeeid && y.TeamParentUser != y.TeamUsers).ToList();
                for (int i = 0; i < getchilds.Count; i++)
                {
                    if (getchilds[i].TeamUsers == usertoassign)
                    {
                        sts = true;
                    }
                    CheckAcessForTimeSheetDisplay(getchilds[i].TeamUsers.ToString(), usertoassign);
                }
            }
            return sts;
        }
    }
}