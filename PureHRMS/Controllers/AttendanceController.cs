using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;

namespace PureHRMS.Controllers
{
    public class AttendanceController : Controller
    {
        [CkeckAllowedAccess]
        public ActionResult SelectAttandanceDate(string msg)
        {
            if (msg == null || msg == "")
            {

            }
            else
            {
                ViewBag.FoundError = msg;
            }
            return View();
        }

        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult MarkAttendanceNewAdmin(string AttDate)
        {
            

            if (AttDate == null || AttDate == "")
            {
                return RedirectToAction("SelectAttandanceDate", new { msg = "Please select date" });
            }
            else
            {
                ViewBag.DateSelected = AttDate;
                DateTime dt = new DateTime();
                dt = Convert.ToDateTime(AttDate);
                /////////////////
                List<MarkAdminAttendance> UsersActiveList = new List<MarkAdminAttendance>();
                using (HRMSConn dc = new HRMSConn())
                {
                   


                    List<string> AcodesList = new List<string>();

                    System.Data.DataSet ds = new System.Data.DataSet();
                    ds.ReadXml(Server.MapPath("~/OtherFiles/XMLs/AttCodes.xml"));
                    foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                    {
                        AcodesList.Add(dr[0].ToString());
                    }

                    ViewBag.ListCode = AcodesList;

                    //////////////////////
                    var dataUsers = dc.tblUserMasters.Where(x => x.Active == true && x.IsWebMaster != true).OrderByDescending(x => x.UserId).ToList();

                    for (int i = 0; i < dataUsers.Count; i++)
                    {
                        int uid = Convert.ToInt32(dataUsers[i].UserId);
                        string LeavesBalanceUser = "";
                        var LeaveBal = dc.tblLeaveBalances.Where(x => x.refEmpUid == uid).OrderByDescending(x => x.LBId).ToList();
                        for (int j = 0; j < LeaveBal.Count; j++)
                        {
                            int lvid = Convert.ToInt32(LeaveBal[j].refLeaveID);
                            string LeaveName = dc.tblLeaveMasters.Where(z => z.LId == lvid).SingleOrDefault().LeaveName;
                            LeavesBalanceUser = LeavesBalanceUser + "<b>" + LeaveName + "</b> : " + LeaveBal[j].LeaveBalance + "<br/>";
                        }
                        // IF ATTENDACE PRESENT THAN THAT TIME ELSE DEFAULT TIME
                        var dataInOut = dc.tblAttendanceMasters.Where(x => x.refEuid == uid && x.AttendanceDate == dt).SingleOrDefault();
                        TimeSpan? InVal = null, OutVal = null;
                        string AttCode = null;
                        if (dataInOut != null)
                        {
                            if (dataInOut.ClockIn == null)
                            {
                                var GetTimeFronSet = dc.tblSettingsMasters.ToList();
                                string InTimeGet = "";
                                if (GetTimeFronSet.Count>0)
                                {
                                    if(GetTimeFronSet[0].InTime!=null)
                                    {
                                         InTimeGet = GetTimeFronSet[0].InTime.ToString();
                                    }
                                    else
                                    {
                                        InTimeGet = "09:00";
                                    }
                                    
                                }
                                else
                                {
                                    InTimeGet = "09:00";
                                }
                                
                                InVal = TimeSpan.Parse(InTimeGet); //DEFAULT TIME
                            }
                            else
                            {
                                InVal = dataInOut.ClockIn;

                            }
                            if (dataInOut.ClockOut == null)
                            {
                                var GetTimeFronSet = dc.tblSettingsMasters.ToList();
                                string OutTimeGet = "";
                                if (GetTimeFronSet.Count > 0)
                                {
                                    if (GetTimeFronSet[0].OutTime != null)
                                    {
                                        OutTimeGet = GetTimeFronSet[0].OutTime.ToString();
                                    }
                                    else
                                    {
                                        OutTimeGet = "18:00";
                                    }

                                }
                                else
                                {
                                    OutTimeGet = "18:00";
                                }
                                OutVal = TimeSpan.Parse(OutTimeGet);  //DEFAULT TIME
                            }
                            else
                            {
                                OutVal = dataInOut.ClockOut;
                            }
                            if (dataInOut.AttandanceCode == null)
                            {
                                AttCode = "P";
                            }
                            else
                            {
                                AttCode = dataInOut.AttandanceCode;
                            }
                        }
                        else
                        {
                            var GetTimeFronSet = dc.tblSettingsMasters.ToList();
                            string OutTimeGet = "";
                            if (GetTimeFronSet.Count > 0)
                            {
                                if (GetTimeFronSet[0].OutTime != null)
                                {
                                    OutTimeGet = GetTimeFronSet[0].OutTime.ToString();
                                }
                                else
                                {
                                    OutTimeGet = "18:00";
                                }

                            }
                            else
                            {
                                OutTimeGet = "18:00";
                            }
                            string InTimeGet = "";
                            if (GetTimeFronSet.Count > 0)
                            {
                                if (GetTimeFronSet[0].InTime != null)
                                {
                                    InTimeGet = GetTimeFronSet[0].InTime.ToString();
                                }
                                else
                                {
                                    InTimeGet = "09:00";
                                }

                            }
                            else
                            {
                                InTimeGet = "09:00";
                            }

                            InVal = TimeSpan.Parse(InTimeGet);  //DEFAULT TIME

                            OutVal = TimeSpan.Parse(OutTimeGet);  //DEFAULT TIME
                        }

                        //GET TODAYS APPROVED LEAVES LIST EMPs 
                       
                        var dataLeaveChk = dc.tblLeaveApplicationTracks.Where(x => x.refEuid == uid && x.ApproveStatus==true).ToList();
                        //CHECK LEAVE FOR SELECTED DATE
                        for(int k=0;k< dataLeaveChk.Count;k++)
                        {
                            DateTime? SD = dataLeaveChk[k].StartDate;
                            int Days = Convert.ToInt32(Math.Round(Convert.ToDouble(dataLeaveChk[k].NoOfDays), 0, MidpointRounding.AwayFromZero));
                            DateTime? ED =Convert.ToDateTime(dataLeaveChk[k].StartDate).AddDays(Days).AddDays(-1);
                            if(SD<= dt && ED>= dt)
                            {
                                LeavesBalanceUser = LeavesBalanceUser + " <span class='label label-danger' > On Leave -<br/> &nbsp; " + Convert.ToDateTime(SD).ToShortDateString() +" - " + Convert.ToDateTime(ED).ToShortDateString() + "&nbsp; <br/>&nbsp;  Total Days : " + dataLeaveChk[k].NoOfDays + " </span> ";
                            }
                        }
                      
                        
                        /////////////////////////////////////////

                        UsersActiveList.Add(
                 new MarkAdminAttendance
                 {
                     Euid = dataUsers[i].UserId,
                     AC= AttCode,
                     EmpId = dataUsers[i].EmpId,
                     EmployeeName = dataUsers[i].FullName,
                     InTime = InVal.ToString().Substring(0, 5), //COVERTED TO STRING FOR BETTER DISPLAY
                     OutTime = OutVal.ToString().Substring(0, 5), //COVERTED TO STRING FOR BETTER DISPLAY
                     PendingLeaves = LeavesBalanceUser


                 }
                      );
                    }
                 
                }


                return View(UsersActiveList);
            }

        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("MarkAttendanceNewAdmin")]
        public ActionResult MarkAttendanceNewAdminPost(List<MarkAdminAttendance> listmy, string AttDate)
        {
            if (listmy == null)
            {

                return RedirectToAction("SelectAttandanceDate", new { msg = "No employees found" });
            }
            if (AttDate == null || AttDate == "")
            {
               
                return RedirectToAction("SelectAttandanceDate", new { msg = "Please select date" });
            }
            else
            {
                DateTime dt = new DateTime();
                dt = Convert.ToDateTime(AttDate);
                using (HRMSConn dc = new HRMSConn())
                {
                    ViewBag.DateSelected = AttDate;
                   
                    TryUpdateModel(listmy);
                  
                    //////////////// INSERT OR UPDATE DATA HERE ROW BY ROW ///////

                    for (int z = 0; z < listmy.Count; z++)
                    {
                        int userid = listmy[z].Euid;
                        var getSingleRowAtt = dc.tblAttendanceMasters.Where(x => x.refEuid == userid && x.AttendanceDate == dt).SingleOrDefault();
                        if (getSingleRowAtt == null)
                        {
                            tblAttendanceMaster tam = new tblAttendanceMaster();
                            tam.AttandanceCode = listmy[z].AC.ToString();
                            tam.AttendanceDate = dt;
                            tam.ClockIn = TimeSpan.Parse(listmy[z].InTime);
                            tam.ClockOut = TimeSpan.Parse(listmy[z].OutTime);
                            tam.CreatedOn = System.DateTime.Now;
                            tam.refEuid = userid;
                            dc.tblAttendanceMasters.Add(tam);
                        }
                        else
                        {
                            getSingleRowAtt.AttandanceCode = listmy[z].AC.ToString();
                            getSingleRowAtt.ClockIn = TimeSpan.Parse(listmy[z].InTime);
                            getSingleRowAtt.ClockOut = TimeSpan.Parse(listmy[z].OutTime);
                            getSingleRowAtt.UpdatedOn = DateTime.Now;
                        }
                        dc.SaveChanges();
                    }

                    ///////////////////////////////////////////////////////////////

                }


                return RedirectToAction("MarkAttendanceNewAdmin",new { AttDate = dt.ToShortDateString() });
            }
        }

        [CkeckAllowedAccess]
        public ActionResult MyAttendance(string AttDate)
        {
            using (HRMSConn dc=new HRMSConn())
            {
                ViewBag.DateSet = null;
                if (AttDate == null || AttDate == "" || (!AttDate.Contains('-')))
                {
                    ViewBag.DateSet = AttDate;
                    var DataAtt = dc.tblAttendanceMasters.Where(x => x.AId == 0).ToList(); // NO LIST
                    return View(DataAtt);

                }
                else
                {
                    ViewBag.DateSet = AttDate;
                    int Euid = Convert.ToInt32(Session["UserIdentfier"]);
                    string[] arr = AttDate.Split('-');
                    int SalMonth = Convert.ToInt32(arr[0]);
                    int SalYear = Convert.ToInt32(arr[1]);
                    int Todaldays = DateTime.DaysInMonth(SalYear, SalMonth);
                    string StartDt = SalMonth + "/" + "01/" + SalYear;
                    string EndDt = SalMonth + "/" + Todaldays.ToString()+"/"+ SalYear;
                  
                    DateTime StartDate = Convert.ToDateTime(Convert.ToDateTime(StartDt).ToShortDateString());
                    DateTime EndDate = Convert.ToDateTime(Convert.ToDateTime(EndDt).ToShortDateString());

                    var DataAtt = dc.tblAttendanceMasters.Where(x => x.refEuid == Euid && (x.AttendanceDate >= StartDate 
                    && x.AttendanceDate <= EndDate)).OrderBy(x=>x.AttendanceDate).ToList();

                    return View(DataAtt);
                }

                  
            }
          
        }
    }


}
