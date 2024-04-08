using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult EmployeeDashboard( string msg)
        {
            if(msg==null)
            {
                ;
            }
            else
            {
                ViewBag.FoundError = msg;
            }
            if (Session["UserIdentfier"] == null)
            {
                return RedirectToAction("SecureUserLogin", "Login");
            }

            using (HRMSConn dc = new HRMSConn())
            {
                int userid = Convert.ToInt32(Session["UserIdentfier"]);
                ViewBag.Identifier = userid;
                var dataUser = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                ViewBag.EmployeeName = dataUser.FullName;
                var dataset = dc.tblNoticeMasters.OrderByDescending(x => x.NoticeId).ToList();
                ViewBag.Notices = dataset;

                var AlotedHours = dc.tblProjectAllotteds.Where(x => x.refEmployeeId == userid).OrderByDescending(x => x.AllottedId).ToList();
                List<SelectListItem> AllowedHoursList = new List<SelectListItem>();
                foreach (var liveprojects in AlotedHours)
                {
                    int pid = Convert.ToInt32(liveprojects.refProjectId);
                    var LivePrj = dc.tblProjectMasters.Where(z => z.ProjectId == pid && z.ProjectStatus != "Completed").ToList();
                    if (LivePrj.Count > 0)
                    {
                        AllowedHoursList.Add(new SelectListItem { Text = LivePrj[0].ProjectTitle, Value = liveprojects.AllowedHours.ToString() });
                    }
                }
                ViewBag.ProjectAllowtedHours = AllowedHoursList;

                ViewBag.IsSupAdmin = IsSupportAdmin();
                ViewBag.AdminUnread = CountNewAdminTickets().ToString();

                ViewBag.EmployeeUnread = CountNewployeeTickets().ToString();

                ///////RECENT 10 LEAVES LIST /////////////////


                CheckIfUSerHasChilds(userid.ToString());

                //////////////////////////////////////////////

                return View();

            }
        }
        public ActionResult EmployeeLogout()
        {
            if (Session["UserIdentfier"] == null)
            {
                return RedirectToAction("SecureUserLogin", "Login");
            }
            Session.Abandon();
            return RedirectToAction("SecureUserLogin", "Login");
        }
        [HttpGet]
        public ActionResult EmployeeChangePassword()
        {
            if (Session["UserIdentfier"] == null)
            {
                return RedirectToAction("SecureUserLogin", "Login");
            }
            return View();
        }
        [HttpPost]
        [ActionName("EmployeeChangePassword")]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeChangePasswordPost(string userOldPassword, string userNewPassword, string userNewPasswordRepeat)
        {
            if (Session["UserIdentfier"] == null)
            {
                return RedirectToAction("SecureUserLogin", "Login");
            }
            if (userOldPassword == "" || userNewPassword == "" || userNewPasswordRepeat == "" ||
                userOldPassword == null || userNewPassword == null || userNewPasswordRepeat == null
                )
            {
                ViewBag.FoundError = "All fields are required.";
            }
            else
            {
                int userIdentifier = Convert.ToInt32(Session["UserIdentfier"]);
                using (HRMSConn dc = new HRMSConn())
                {
                    var data = dc.tblUserMasters.Where(x => x.UserId == userIdentifier && x.Active == true).SingleOrDefault();
                    if (userNewPassword == userNewPasswordRepeat)
                    {
                        if (data.UserPassword == userOldPassword)
                        {
                            data.UserPassword = userNewPassword;
                            dc.SaveChanges();
                            ViewBag.FoundError = "Password changed successfully. ";
                        }
                        else
                        {
                            ViewBag.FoundError = "Your old password is incorrect. ";
                        }
                    }
                    else
                    {
                        ViewBag.FoundError = "Your Newpassword & Retype Newpassword Mismatched";
                    }



                }
            }

            return View();

        }

        public bool IsSupportAdmin()
        {
            bool sts = false;
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                var IsSupportAdmin = dc.tblSupportAdminManagers.Where(x => x.refEmployeeId == uid).ToList();
                if (IsSupportAdmin.Count > 0)
                {
                    sts= true;
                }
                return sts;
            }
        }
        public int CountNewAdminTickets()
        {
            int NewCount = 0;
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                var IsSupportAdmin = dc.tblSupportAdminManagers.Where(x => x.refEmployeeId == uid).ToList();
                if (IsSupportAdmin.Count == 0)
                {
                    NewCount = 0;
                }
                else
                {
                    int[] depts = new int[IsSupportAdmin.Count];
                    for (int z = 0; z < IsSupportAdmin.Count; z++)
                    {
                        depts[z] = Convert.ToInt32(IsSupportAdmin[z].refDepartmentId); // INSERTING ALL DEPARTMENTS ALLOWED TO THIS EMPLOYEE
                    }

                    var GetTickets = dc.tblSupportTickedMasters.OrderByDescending(x => x.Tid).Where(
                     x => depts.Contains((int)x.TDeptid)).ToList();


                    for (int i = 0; i < GetTickets.Count; i++)
                    {

                        int tid = Convert.ToInt32(GetTickets[i].Tid);

                        var dataRef = dc.tblTicketRefs.Where(x => x.AdminRead == false && x.refTId == tid).ToList();
                        if (dataRef.Count > 0)
                        {
                            NewCount = NewCount + 1;
                        }


                    }
                }


                return NewCount;
            }
        }
        public int CountNewployeeTickets()
        {
            int NewCount = 0;
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
               
                    

                var GetTickets = dc.tblSupportTickedMasters.OrderByDescending(x => x.Tid).Where( x=>x.UserInvolved==uid).ToList();


                for (int i = 0; i < GetTickets.Count; i++)
                {

                    int tid = Convert.ToInt32(GetTickets[i].Tid);

                    var dataRef = dc.tblTicketRefs.Where(x => x.UserRead == false && x.refTId == tid).ToList();
                    if (dataRef.Count > 0)
                    {
                        NewCount = NewCount + 1;
                    }



                }


                return NewCount;
            }
        }

        ////////////////ATTENDENCE MARK ////

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkSelfAttendanceIn()
        {
            using (HRMSConn dc=new HRMSConn())
            {
                int userid = Convert.ToInt32(Session["UserIdentfier"]);
                var IsAddancePresent = dc.tblAttendanceMasters.Where(x => x.refEuid == userid && x.AttendanceDate == System.DateTime.Today).ToList();
                if (IsAddancePresent.Count==0 || IsAddancePresent ==null)
                {

                    tblAttendanceMaster am = new tblAttendanceMaster();
                    am.AttandanceCode = "P";
                    am.refEuid = userid;
                    am.AttendanceDate = System.DateTime.Now;
                    am.ClockIn = System.DateTime.Now.TimeOfDay;
                    am.CreatedOn = System.DateTime.Now;
                    am.UpdatedOn = System.DateTime.Now;
                    dc.tblAttendanceMasters.Add(am);
                    dc.SaveChanges();
                    ViewBag.FoundError = "IN TIME makred successfully";
                    return RedirectToAction("EmployeeDashboard", new { msg = ViewBag.FoundError });
                  
                }
                else
                {
                    ViewBag.FoundError = "You already maked IN TIME";
                    return RedirectToAction("EmployeeDashboard", new { msg = ViewBag.FoundError });
                }
              

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkSelfAttendanceOut()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int userid = Convert.ToInt32(Session["UserIdentfier"]);
                var IsAddancePresent = dc.tblAttendanceMasters.Where(x => x.refEuid == userid && x.AttendanceDate == System.DateTime.Today).ToList();
                if (IsAddancePresent.Count == 0 || IsAddancePresent == null)
                {

                    ViewBag.FoundError = "Please mark IN TIME first";
                    return RedirectToAction("EmployeeDashboard", new { msg = ViewBag.FoundError });

                }
                else
                {
                    IsAddancePresent[0].ClockOut= System.DateTime.Now.TimeOfDay;
                    dc.SaveChanges();
                    ViewBag.FoundError = "OUT TIME marked successfully";
                    return RedirectToAction("EmployeeDashboard", new { msg = ViewBag.FoundError });
                }


            }
        }

        //TEAM LEAVES DASHBOARD FUNCTION
        public void CheckIfUSerHasChilds(string userid)
        {

            using (HRMSConn dc = new HRMSConn())
            {
                int employeeid = Convert.ToInt32(userid);
                var getchilds = dc.tblTeamMasters.Where(y => y.TeamParentUser == employeeid && y.TeamParentUser != y.TeamUsers).ToList();
                List<TeamLeavesDashboard> listLeaves = new List<TeamLeavesDashboard>();
                for (int i = 0; i < getchilds.Count; i++)
                {
                    int getChildUid = Convert.ToInt32(getchilds[i].TeamUsers);
                    DateTime Past30days = System.DateTime.Now.AddDays(-30); //ALL TO COMPARE TILL BACK 30 DAYS 
                    var dataLeavePending = dc.tblLeaveApplicationTracks.Where(x => x.refEuid == getChildUid && x.ApproveStatus == false && x.StartDate> Past30days).ToList();
                    if(dataLeavePending==null || dataLeavePending.Count==0)
                    {

                    }
                    else
                    {
                        var getuser = dc.tblUserMasters.Where(x => x.UserId == getChildUid).SingleOrDefault();
                        int DesigId = Convert.ToInt32(getuser.DesignationId);
                        int DeptId = Convert.ToInt32(getuser.DepartmentId);
                        string DesigName = dc.tblDesignationMasters.Where(x => x.DesignationId == DesigId).SingleOrDefault().DesignationName;
                        string DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == DeptId).SingleOrDefault().DepartmentName;
                        for (int j=0;j< dataLeavePending.Count;j++)
                        {                            
                            listLeaves.Add(
                                new TeamLeavesDashboard
                                {
                                    EUid = getChildUid,
                                    Designation = DesigName,
                                    EmpName = getuser.FullName,
                                    StartDate= dataLeavePending[j].StartDate,
                                    Days= dataLeavePending[j].NoOfDays.ToString(),
                                    Department= DeptName

                                }
                                );
                        }
                    }
                       
                    CheckIfUSerHasChilds(getChildUid.ToString());
                }

                ViewBag.DashTeamLeaves = listLeaves;
            }
        }
    }
}