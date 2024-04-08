using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class LeaveController : Controller
    {
        bool sts = false;
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult InsertLeave()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataAll = dc.tblLeaveMasters.OrderByDescending(x => x.LId).ToList();
                ViewBag.LeaveAll = dataAll;
               
                return View();

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("InsertLeave")]
        public ActionResult InsertLeavetPost()
        {

            using (HRMSConn dc = new HRMSConn())
            {
                var dataAll = dc.tblLeaveMasters.OrderByDescending(x => x.LId).ToList();
                ViewBag.LeaveAll = dataAll;

                tblLeaveMaster lm = new tblLeaveMaster();
                TryUpdateModel(lm);

                if (ModelState.IsValid)
                {
                    dc.tblLeaveMasters.Add(lm);
                    dc.SaveChanges();
                    ModelState.Clear();

                    return RedirectToAction("InsertLeave");
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
        public ActionResult UpdateLeave(string id, string LeaveName, string DDLLeaveDay)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                try
                {
                    int LeaveId = Convert.ToInt32(id);
                    var data = dc.tblLeaveMasters.Where(x => x.LId == LeaveId).SingleOrDefault();
                    data.LeaveName = LeaveName;
                    data.LeveValue = DDLLeaveDay;
                    dc.SaveChanges();
                    return RedirectToAction("InsertLeave");

                }
                catch
                {
                    return RedirectToAction("InsertLeave");

                }
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLeave(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int LeaveId = Convert.ToInt32(id);
                var data = dc.tblLeaveMasters.Where(x => x.LId == LeaveId).SingleOrDefault();
                dc.tblLeaveMasters.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("InsertLeave");
            }
        }


        //////  EMPLOYEE LEAVE  APPLY ///////////////////////
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult ApplyLeave(string msg)
        {
            int UserUid = Convert.ToInt32(Session["UserIdentfier"]);

            if (msg == null)
            {
                ;
            }
            else
            {
                ViewBag.FoundError = msg;
            }
                  LeaveDetails(UserUid);
                return View();
          
            
                

        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ApplyLeave")]
        public ActionResult ApplyLeavePost(string msg)
        {


            if (msg == null)
            {
                ;
            }
            else
            {
                ViewBag.FoundError = msg;
            }
            int UserUid = Convert.ToInt32(Session["UserIdentfier"]);
            tblLeaveApplicationTrack lat = new tblLeaveApplicationTrack();
            lat.ApproveStatus = false;
            lat.refEuid = UserUid; 
                  
            TryUpdateModel(lat);
           lat.ApplicantNotes = lat.ApplicantNotes + " , Applied On : " + System.DateTime.Now.ToShortDateString();
            if (ModelState.IsValid)
            {
                using (HRMSConn dc = new HRMSConn())
                {
                    dc.tblLeaveApplicationTracks.Add(lat);
                    dc.SaveChanges();
                    return RedirectToAction("ApplyLeave", new { msg = "Leave applied successfully" });

                }
            }
            else
            {
                LeaveDetails(UserUid);
                return View();
            }



        }

        public void LeaveDetails( int UserUidpass)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                //////////// LEAVE BALANCE HISTORY //////////////////
                int UserUid = UserUidpass;
                ViewBag.UserIdentifier = UserUid;
                var data = dc.tblUserMasters.Where(x => x.UserId == UserUid).SingleOrDefault();
                ViewBag.UserName = data.FullName.ToString();
                ViewBag.EmpId = data.EmpId;
                var dataLeaveMaster = dc.tblLeaveMasters.ToList();

                ViewBag.LeaveDLL = dataLeaveMaster;

                List<EmployeeLeaveDisplay> LeaveBalance = new List<EmployeeLeaveDisplay>();

                decimal TotalLeaves = 0;

                for (int i = 0; i < dataLeaveMaster.Count; i++)
                {
                    string LeaveCountbalance = "0";
                    int leaveID = Convert.ToInt32(dataLeaveMaster[i].LId);
                    var dataBal = dc.tblLeaveBalances.Where(x => x.refLeaveID == leaveID && x.refEmpUid == UserUid).SingleOrDefault();
                    if (dataBal == null)
                    {
                        LeaveCountbalance = "0.0";
                    }
                    else
                    {
                        ///CHECK DAYS FOR LEAVE AND MAKE IT MULTIPLYER
                        decimal LeaveMultiplyer = 0;
                        var LeaveMaster = dc.tblLeaveMasters.Where(x => x.LId == leaveID).SingleOrDefault();
                        LeaveMultiplyer = Convert.ToDecimal(LeaveMaster.LeveValue);
                        //////////////////////
                        decimal count = Math.Round((Convert.ToDecimal(dataBal.LeaveBalance) * LeaveMultiplyer), 1);
                        LeaveCountbalance = dataBal.LeaveBalance.ToString();
                        TotalLeaves = TotalLeaves + count;
                    }


                    LeaveBalance.Add(
                        new EmployeeLeaveDisplay
                        {
                            LeaveName = dataLeaveMaster[i].LeaveName,
                            LeaveType = dataLeaveMaster[i].LeveValue,
                            LeaveBalance = LeaveCountbalance
                        }
                        );
                }

                ViewBag.LeaveBal = LeaveBalance;
                ViewBag.TotalLeaveBal = Math.Round(TotalLeaves, 1);
                ViewBag.LeaveHistory = dc.tblLeaveApplicationTracks.Where(x => x.refEuid == UserUid).OrderByDescending(x => x.BLId).ToList();
                /////////////////////////////////////////////////////////////////////

            }
        }

        //////////////////////////APPROVE TEAM LEAVE //////////////////////////////
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult ApproveTeamLeave(string id)
        {
           
            int UserIdToApprove = Convert.ToInt32(id);
            /////CHECK THE PARENT-CHILD RELATIONSHIP/////
            bool stsget = CheckAcessForLeave(Session["UserIdentfier"].ToString(), UserIdToApprove);
            ////////////////////////////////////////////////
            if(stsget)
            {
                LeaveDetails(UserIdToApprove);
            }
            else
            {
                return Redirect("/Home/EmployeeDashboard/");
            }
           
            return View();
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("ApproveTeamLeave")]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveTeamLeavePost(string Uid, string LTid)
        {
            using (HRMSConn dc=new HRMSConn())
            {
                int Euid = Convert.ToInt32(Uid);
                int LPTid = Convert.ToInt32(LTid);
               var data= dc.tblLeaveApplicationTracks.Where(x => x.BLId == LPTid).SingleOrDefault();
                if(data.ApproveStatus==false)
                {
                    data.ApproveStatus = true;
                    data.ApprovedNotes = "Dated:- " + System.DateTime.Now.ToString() + " Approved By :- UID-" + Session["UserIdentfier"].ToString();
                    dc.SaveChanges();
                }
               
                return RedirectToAction("ApproveTeamLeave",new { id = Euid });
            }
               
        }

        public bool CheckAcessForLeave(string loginid, int usertoassign)
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
                    CheckAcessForLeave(getchilds[i].TeamUsers.ToString(), usertoassign);
                }
            }
            return sts;
        }

        //////////////////////////////////////////////////////////////////////////

    }
}