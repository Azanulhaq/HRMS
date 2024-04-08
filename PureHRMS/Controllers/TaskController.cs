using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;

namespace PureHRMS.Controllers
{
    public class TaskController : Controller
    {
        bool sts = false;
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult InsertTask(string id)
        {

            int UserIdToAssign = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
                /////CHECK THE PARENT-CHILD RELATIONSHIP/////
                bool sts = CheckAcessForInsert(Session["UserIdentfier"].ToString(), UserIdToAssign);
                ////////////////////////////////////////////////
                if (sts)
                {
                    var data = dc.tblUserMasters.Where(x => x.UserId == UserIdToAssign).SingleOrDefault();

                    ViewBag.EmployeeName = data.FullName;
                    ViewBag.EmployeeID = data.EmpId;
                    ViewBag.Identifier = data.UserId;
                    return View();
                }
                else
                {
                    return Redirect("/Home/EmployeeDashboard/");
                }
            }

          
        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("InsertTask")]
       [ValidateAntiForgeryToken]
        public ActionResult InsertTaskPost(string id)
        {
            int UserIdToAssign = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblUserMasters.Where(x => x.UserId == UserIdToAssign).SingleOrDefault();

                ViewBag.EmployeeName = data.FullName;
                ViewBag.EmployeeID = data.EmpId;
                ViewBag.Identifier = data.UserId;
                /////CHECK THE PARENT-CHILD RELATIONSHIP/////
                bool sts=CheckAcessForInsert(Session["UserIdentfier"].ToString(), UserIdToAssign);
                ////////////////////////////////////////////////
                if(sts)
                {
                   
                    tblTaskMaster tm = new tblTaskMaster();
                    TryUpdateModel(tm);
                    tm.FinishNotes = "Added : " + System.DateTime.Now.ToShortDateString();
                    tm.AssignTo = UserIdToAssign;
                    tm.AssignBy = Convert.ToInt32(Session["UserIdentfier"]);
                    if (ModelState.IsValid)
                    {
                        dc.tblTaskMasters.Add(tm);
                        dc.SaveChanges();
                        ViewBag.FoundError = "Task assigned successfully.";
                        ModelState.Clear();
                        return View();

                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return Redirect("/Home/EmployeeDashboard/");
                }
                


            }

        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTask(string id)
        {

            int delid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
               
                    var data = dc.tblTaskMasters.Where(x => x.TaskId == delid).SingleOrDefault();
                    dc.tblTaskMasters.Remove(data);
                    dc.SaveChanges();
                    return View("_Success");
               
            }


        }
        public bool CheckAcessForInsert(string loginid,int usertoassign)
        {
           
            using (HRMSConn dc = new HRMSConn())
            {
                int employeeid =Convert.ToInt32(loginid);
                var getchilds = dc.tblTeamMasters.Where(y => y.TeamParentUser == employeeid && y.TeamParentUser != y.TeamUsers).ToList();
                for(int i=0;i< getchilds.Count;i++)
                {
                    if (getchilds[i].TeamUsers == usertoassign)
                    {
                        sts = true;
                    }
                    CheckAcessForInsert(getchilds[i].TeamUsers.ToString(), usertoassign);
                }
            }
            return sts;
        }

       
    }
}