using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
using System.Transactions;
namespace PureHRMS.Controllers
{
    public class EmployeeController : Controller
    {
        bool sts = true;
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult InsertEmployee()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataDepts = dc.tblDepartmentMasters.ToList();
                ViewBag.DeptList = dataDepts;

                List<string> listQual = new List<string>();

                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXml(Server.MapPath("~/OtherFiles/XMLs/Qualificatios.xml"));
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    listQual.Add(dr[0].ToString());
                }

                ViewBag.QualiList = listQual;

                return View();
            }
        }
        public JsonResult GetDesignations(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int deptid = Convert.ToInt32(id);
                var dataDesig = dc.tblDesignationMasters.Where(x => x.RefDepartmentId == deptid).ToList();
                return Json(new SelectList(dataDesig, "DesignationId", "DesignationName"));
            }
        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("InsertEmployee")]      
        [ValidateAntiForgeryToken]
        public ActionResult InsertEmployeePost(HttpPostedFileBase fileposted, string EmailId, string FullName)
        {

            using (HRMSConn dc = new HRMSConn())
            {

                //////////////////////////////////////////////////


                var dataDepts = dc.tblDepartmentMasters.ToList();
                ViewBag.DeptList = dataDepts;

                List<string> listQual = new List<string>();

                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXml(Server.MapPath("~/OtherFiles/XMLs/Qualificatios.xml"));
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    listQual.Add(dr[0].ToString());
                }

                ViewBag.QualiList = listQual;

                ///////////////////////////////////////




                string filename = "";
                if (fileposted != null)
                {

                    if (fileposted.ContentLength > 2500000)
                    {
                        ViewBag.FoundError = "Uploaded image is greater than 2 MB, upload small image";
                        return View();
                    }
                    else
                    {
                        CommonFunctions cf = new CommonFunctions();
                        string uniqueid = System.DateTime.Now.Year.ToString() +
                            System.DateTime.Now.Month.ToString() +
                            System.DateTime.Now.Day.ToString() +
                            System.DateTime.Now.Hour.ToString() +
                            System.DateTime.Now.Minute.ToString() +
                            System.DateTime.Now.Millisecond.ToString();
                        string ext = System.IO.Path.GetExtension(fileposted.FileName);
                        filename = CommonFunctions.GenerateSlug(FullName) + "_" + uniqueid + ext;
                        fileposted.SaveAs(Server.MapPath("~/UploadedData/EmployeeImages/" + filename));
                    }
                }
                var dataCheck = dc.tblUserMasters.Where(x => x.EmailId == EmailId).ToList();
                if (dataCheck.Count > 0)
                {
                    ViewBag.FoundError = "This email already exists. Please other new email.";
                    return View();

                }

                tblUserMaster um = new tblUserMaster();
                TryUpdateModel(um);
                if (ModelState.IsValid)
                {
                    um.IsWebMaster = false;
                    um.ImagePath = filename;
                    um.UserPassword = EmailId;
                    um.UserType = "EMployee";
                    um.Active = true;
                    dc.tblUserMasters.Add(um);
                    dc.SaveChanges();
                    return View("_Success");
                }
                else
                {

                    return View();
                }

            }
        }

        [CkeckAllowedAccess]
        public ActionResult UpdateEmployee()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                //DO NOT SHOW WEB MASTER HERE
                var data = dc.tblUserMasters.OrderByDescending(x => x.UserId).Where(x => x.IsWebMaster != true).ToList();
                return View(data);

            }
        }

        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult EditEmployee(string id)
        {
            int userid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
                ////IF WEB MASTER DON'T CHANGE ANYTHING ////

                var WebMaster = dc.tblUserMasters.Where(z => z.UserId == userid).SingleOrDefault();
                if (WebMaster != null)
                {
                    if (WebMaster.IsWebMaster == true)
                    {
                        // DON'T ALLOW UPDATE OF WEB MASTER

                        return RedirectToAction("UpdateEmployee");

                    }
                    else
                    {
                        // ALLOW PROCESSING OF BELOW CODE
                    }
                }

                //////////////////////////////
                var dataDepts = dc.tblDepartmentMasters.ToList();
                ViewBag.DeptList = dataDepts;

                List<string> listQual = new List<string>();

                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXml(Server.MapPath("~/OtherFiles/XMLs/Qualificatios.xml"));
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    listQual.Add(dr[0].ToString());
                }

                ViewBag.QualiList = listQual;

                var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();

                int deptid = Convert.ToInt32(data.DepartmentId);

                var dataDesig = dc.tblDesignationMasters.Where(x => x.RefDepartmentId == deptid).ToList();
                ViewBag.DesigList = dataDesig;

                return View(data);

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("EditEmployee")]      
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployeePost(string id, HttpPostedFileBase fileposted, string EmailId, string FullName)
        {
            int userid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {

                ////IF WEB MASTER DON'T CHANGE ANYTHING ////

                var WebMaster = dc.tblUserMasters.Where(z => z.UserId == userid).SingleOrDefault();
                if (WebMaster != null)
                {
                    if (WebMaster.IsWebMaster == true)
                    {
                        // DON'T ALLOW UPDATE OF WEB MASTER

                        return RedirectToAction("UpdateEmployee");

                    }
                    else
                    {
                        // ALLOW PROCESSING OF BELOW CODE
                    }
                }

                //////////////////////////////
                var dataDepts = dc.tblDepartmentMasters.ToList();
                ViewBag.DeptList = dataDepts;

                List<string> listQual = new List<string>();

                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXml(Server.MapPath("~/OtherFiles/XMLs/Qualificatios.xml"));
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    listQual.Add(dr[0].ToString());
                }

                ViewBag.QualiList = listQual;

                var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                if (data.EmailId != EmailId)
                {
                    var dataCheck = dc.tblUserMasters.Where(x => x.EmailId == EmailId).ToList();
                    if (dataCheck.Count > 0)
                    {
                        ViewBag.FoundError = "This email already exists. Please other new email.";
                        return View();

                    }
                }

                int deptid = Convert.ToInt32(data.DepartmentId);
                var dataDesig = dc.tblDesignationMasters.Where(x => x.RefDepartmentId == deptid).ToList();
                ViewBag.DesigList = dataDesig;



                string filename = "";
                if (fileposted != null)
                {

                    if (fileposted.ContentLength > 2500000)
                    {
                        ViewBag.FoundError = "Uploaded image is greater than 2 MB, upload small image";
                        return View();
                    }
                    else
                    {
                        CommonFunctions cf = new CommonFunctions();
                        string uniqueid = System.DateTime.Now.Year.ToString() +
                            System.DateTime.Now.Month.ToString() +
                            System.DateTime.Now.Day.ToString() +
                            System.DateTime.Now.Hour.ToString() +
                            System.DateTime.Now.Minute.ToString() +
                            System.DateTime.Now.Millisecond.ToString();
                        string ext = System.IO.Path.GetExtension(fileposted.FileName);
                        filename = CommonFunctions.GenerateSlug(FullName) + "_" + uniqueid + ext;
                        string imgname = data.ImagePath;
                        if (System.IO.File.Exists(Server.MapPath("~/UploadedData/EmployeeImages/" + imgname)))
                        {
                            System.IO.File.Delete(Server.MapPath("~/UploadedData/EmployeeImages/" + imgname));
                        }
                        fileposted.SaveAs(Server.MapPath("~/UploadedData/EmployeeImages/" + filename));
                    }
                }
                else
                {
                    filename = data.ImagePath;
                }

                TryUpdateModel(data);

                if (ModelState.IsValid)
                {
                    data.ImagePath = filename;

                    dc.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dc.SaveChanges();
                    ViewBag.FoundError = "Employee updated successfully.";
                    return View(data);
                }
                else
                {
                    return View(data);
                }

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEmployee(string id)
        {
            int userid = Convert.ToInt32(id);


            using (HRMSConn dc = new HRMSConn())
            {
                ////IF WEB MASTER DON'T CHANGE ANYTHING ////

                var WebMaster = dc.tblUserMasters.Where(z => z.UserId == userid).SingleOrDefault();
                if (WebMaster != null)
                {
                    if (WebMaster.IsWebMaster == true)
                    {
                        // DON'T ALLOW UPDATE OF WEB MASTER

                        return RedirectToAction("UpdateEmployee");

                    }
                    else
                    {
                        // ALLOW PROCESSING OF BELOW CODE
                    }
                }

                //////////////////////////////
                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();

                        if (CheckIfUSerHasChilds(data.UserId.ToString()))
                        {
                            //USER HAS CHILDS SO YOU CAN DISABLE OR DELETE IT, TILL IT HAS CHILDS
                            ;
                        }
                        else
                        {

                            dc.tblUserMasters.Remove(data);
                            dc.SaveChanges();

                            /////DELETE ITS ENTRY FROM TEAM TABLE-- IF USER IS DISABLE IT'S ENTRY WILL NOT BE THERE IN TEAM TABLE///
                            var delTeamEntry = dc.tblTeamMasters.Where(y => y.TeamUsers == userid).SingleOrDefault();
                            if (delTeamEntry != null)
                            {
                                dc.tblTeamMasters.Remove(delTeamEntry);
                                dc.SaveChanges();
                            }

                            //////

                            string imgname = data.ImagePath;
                            if (System.IO.File.Exists(Server.MapPath("~/UploadedData/EmployeeImages/" + imgname)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/UploadedData/EmployeeImages/" + imgname));
                            }


                            trans.Complete();
                            trans.Dispose();
                        }


                        return RedirectToAction("UpdateEmployee");

                    }
                    catch
                    {
                        trans.Dispose();
                        // SOME ERROR WHILE PROCESSING REQUEST
                        return RedirectToAction("UpdateEmployee");
                    }
                }


            }

        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisableEmployee(string id)
        {
            int userid = Convert.ToInt32(id);


            using (HRMSConn dc = new HRMSConn())
            {
                ////IF WEB MASTER DON'T CHANGE ANYTHING ////

                var WebMaster = dc.tblUserMasters.Where(z => z.UserId == userid).SingleOrDefault();
                if (WebMaster != null)
                {
                    if (WebMaster.IsWebMaster == true)
                    {
                        // DON'T ALLOW UPDATE OF WEB MASTER

                        return RedirectToAction("UpdateEmployee");

                    }
                    else
                    {
                        // ALLOW PROCESSING OF BELOW CODE
                    }
                }

                //////////////////////////////

                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                        if (CheckIfUSerHasChilds(data.UserId.ToString()))
                        {
                            //USER HAS CHILDS SO YOU CAN DISABLE OR DELETE IT, TILL IT HAS CHILDS
                            ;
                        }
                        else
                        {
                            data.Active = false;
                            dc.SaveChanges();

                            /////DELETE ITS ENTRY FROM TEAM TABLE--IT COULD BE NULL IF USER DOESN'T HAVE ANY TEAM///
                            var delTeamEntry = dc.tblTeamMasters.Where(y => y.TeamUsers == userid).SingleOrDefault();
                            if (delTeamEntry != null)
                            {
                                dc.tblTeamMasters.Remove(delTeamEntry);
                                dc.SaveChanges();
                            }

                            /////


                            trans.Complete();
                            trans.Dispose();
                        }
                        return RedirectToAction("UpdateEmployee");

                    }
                    catch
                    {
                        trans.Dispose();
                        //SOME ERROR WHILE PROCESSING REQUEST
                        return RedirectToAction("UpdateEmployee");
                    }
                }

            }

        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnableEmployee(string id)
        {
            int userid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                data.Active = true;
                dc.SaveChanges();
                return RedirectToAction("UpdateEmployee");
            }
        }
        [CkeckAllowedAccess]
        public ActionResult EmployeeTeamView()
        {
            return View();
        }
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult ManageTeam()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataParents = (from p in dc.tblUserMasters.Where(x => x.Active == true)
                                   select new
                                   {
                                       UserIdentifier = p.UserId,
                                       UserDisplay = p.FullName + "---" + p.EmpId + "---" + p.EmailId
                                   }).ToList();
                dataParents.Insert(0, new { UserIdentifier = 0, UserDisplay = "-Please Select-" });
                ViewBag.AllUsersParents = dataParents;
                ///// REMOVE WEB MASTER FROM CHILDS////
                var dataChilds = (from p in dc.tblUserMasters.Where(x => x.Active == true && x.IsWebMaster != true)
                                  select new
                                  {
                                      UserIdentifier = p.UserId,
                                      UserDisplay = p.FullName + "---" + p.EmpId + "---" + p.EmailId
                                  }).ToList();
                dataChilds.Insert(0, new { UserIdentifier = 0, UserDisplay = "-Please Select-" });
                ViewBag.AllUsersChilds = dataChilds;
                ////////////////////////
                return View();


            }
        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("ManageTeam")]
        [ValidateAntiForgeryToken]
        public ActionResult ManageTeamPost(int ParentDDL, int ChildtDDL)
        {
            using (HRMSConn dc = new HRMSConn())
            {

                if (ParentDDL == ChildtDDL)
                {
                    ViewBag.FoundError = "Parent & child can't be same.";
                }
                else
                {


                    if (ParentDDL == 0 || ChildtDDL == 0)
                    {
                        ViewBag.FoundError = "Please select both parent & child members from the lists.";
                    }
                    else
                    {
                        bool haschilds = CheckParents(ParentDDL, ChildtDDL);
                        if (haschilds)
                        {
                            var getEntryIfAny = dc.tblTeamMasters.Where(z => z.TeamUsers == ChildtDDL).ToList();
                            if (getEntryIfAny.Count > 0)
                            {
                                getEntryIfAny[0].TeamParentUser = ParentDDL;
                                dc.SaveChanges();
                                ViewBag.FoundError = "Team updated successfully.";
                                ModelState.Clear();
                            }
                            else
                            {
                                tblTeamMaster tm = new tblTeamMaster();
                                tm.TeamParentUser = ParentDDL;
                                tm.TeamUsers = ChildtDDL;
                                dc.tblTeamMasters.Add(tm);
                                dc.SaveChanges();
                                ViewBag.FoundError = "Team updated successfully.";
                                ModelState.Clear();
                            }
                        }
                        else
                        {
                            ViewBag.FoundError = "You can assign parent to the child in the same team chain.";
                            ModelState.Clear();
                        }




                    }

                }
                ///////////////////
                var dataParents = (from p in dc.tblUserMasters.Where(x => x.Active == true)
                                   select new
                                   {
                                       UserIdentifier = p.UserId,
                                       UserDisplay = p.FullName + "---" + p.EmpId + "---" + p.EmailId
                                   }).ToList();
                dataParents.Insert(0, new { UserIdentifier = 0, UserDisplay = "-Please Select-" });
                ViewBag.AllUsersParents = dataParents;
                ///// REMOVE WEB MASTER FROM CHILDS////
                var dataChilds = (from p in dc.tblUserMasters.Where(x => x.Active == true && x.IsWebMaster != true)
                                  select new
                                  {
                                      UserIdentifier = p.UserId,
                                      UserDisplay = p.FullName + "---" + p.EmpId + "---" + p.EmailId
                                  }).ToList();
                dataChilds.Insert(0, new { UserIdentifier = 0, UserDisplay = "-Please Select-" });
                ViewBag.AllUsersChilds = dataChilds;
                ////////////////////////
                return View();
            }
        }
        [CkeckAllowedAccess]
        public ActionResult EmployeePrintView(string id)
        {
            int userid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                int desigId = Convert.ToInt32(data.DesignationId);
                int deprtId = Convert.ToInt32(data.DepartmentId);
                ViewBag.Desig = dc.tblDesignationMasters.Where(x => x.DesignationId == desigId).SingleOrDefault().DesignationName;
                ViewBag.Dept = dc.tblDepartmentMasters.Where(x => x.DepartmentId == deprtId).SingleOrDefault().DepartmentName;
                return View(data);

            }
        }
        public bool CheckParents(int parentid, int childid)
        {
            //CHECK - BECAUSE WE CAN ATTACH PARENT TO ITS CHILD IN SAME CHAIN

            using (HRMSConn dc = new HRMSConn())
            {

                var getparent = dc.tblTeamMasters.Where(y => y.TeamUsers == parentid && y.TeamParentUser != y.TeamUsers).SingleOrDefault();

                if (getparent != null)
                {
                    if (getparent.TeamParentUser == childid)
                    {
                        sts = false;
                    }
                    int newparent = Convert.ToInt32(getparent.TeamParentUser);
                    CheckParents(newparent, childid);
                }
                else
                {
                    //IT IS THE ROOT NODE
                }

            }
            return sts;
        }

        public bool CheckIfUSerHasChilds(string userid)
        {

            using (HRMSConn dc = new HRMSConn())
            {
                int employeeid = Convert.ToInt32(userid);
                var getchilds = dc.tblTeamMasters.Where(y => y.TeamParentUser == employeeid && y.TeamParentUser != y.TeamUsers).ToList();
                if (getchilds.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        //////// LEAVE MANAGEMENT ///

        [CkeckAllowedAccess]
        public ActionResult ManageLeaves(string id, string msg)
        {
            if (msg == null)
            {
                ;
            }
            else
            {
                ViewBag.FoundError = msg;
            }
            int UserUid = Convert.ToInt32(id);
            ViewBag.UserIdentifier = UserUid;
            using (HRMSConn dc = new HRMSConn())
            {

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
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CkeckAllowedAccess]
        public ActionResult AddEmployeeLeaves(string id, string ADDLeaveDDL, string AddLeaveCount)
        {
            int UserUid = Convert.ToInt32(id);
            //FIRST CHEK - INSERT OR UPDATE
            if (ADDLeaveDDL == null || ADDLeaveDDL == "")
            {

                return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "Please select leave type" });
            }
            else
            {
                using (HRMSConn dc = new HRMSConn())
                {
                    int leaveID = Convert.ToInt32(ADDLeaveDDL);

                    var EmpLeaves = dc.tblLeaveBalances.Where(x => x.refEmpUid == UserUid && x.refLeaveID == leaveID).SingleOrDefault();
                    decimal NewLeaves = 0;
                    if (EmpLeaves == null)
                    {
                        //THESE LEAVES NOT PRESENT  FOR THIS EMPLOYEE
                        tblLeaveBalance lb = new tblLeaveBalance();
                        lb.LeaveBalance = Convert.ToDecimal(AddLeaveCount);
                        lb.refEmpUid = UserUid;
                        lb.refLeaveID = leaveID;
                        dc.tblLeaveBalances.Add(lb);
                        dc.SaveChanges();
                        return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "Leaves added successfully" });

                    }
                    else
                    {
                        decimal OldLEaves = Convert.ToDecimal(EmpLeaves.LeaveBalance);

                        NewLeaves = Math.Round(OldLEaves + Convert.ToDecimal(AddLeaveCount), 1);
                        EmpLeaves.LeaveBalance = NewLeaves;
                        dc.SaveChanges();
                        return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "Leaves updated successfully" });

                    }



                }


            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CkeckAllowedAccess]
        public ActionResult RemoveEmployeeLeaves(string id, string RemoveLeaveDDL, string RemoveLeaveCount)
        {
            int UserUid = Convert.ToInt32(id);
            //FIRST CHEK - INSERT OR UPDATE
            if (RemoveLeaveDDL == null || RemoveLeaveDDL == "")
            {

                return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "Please select leave type" });
            }
            else
            {
                using (HRMSConn dc = new HRMSConn())
                {
                    int leaveID = Convert.ToInt32(RemoveLeaveDDL);

                    var EmpLeaves = dc.tblLeaveBalances.Where(x => x.refEmpUid == UserUid && x.refLeaveID == leaveID).SingleOrDefault();
                    decimal NewLeaves = 0;
                    if (EmpLeaves == null)
                    {

                        return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "No leaves to remove" });

                    }
                    else
                    {
                        decimal OldLEaves = Convert.ToDecimal(EmpLeaves.LeaveBalance);
                        decimal LeavesToRemove = Convert.ToDecimal(RemoveLeaveCount);
                        if (OldLEaves >= LeavesToRemove)
                        {
                            NewLeaves = Math.Round(OldLEaves - LeavesToRemove, 1);
                            EmpLeaves.LeaveBalance = NewLeaves;
                            dc.SaveChanges();
                            return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "Leaves removed successfully" });
                        }
                        else
                        {
                            return RedirectToAction("ManageLeaves", new { id = UserUid, msg = "Please select less leaves to remove" });
                        }



                    }



                }


            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CkeckAllowedAccess]
        public ActionResult ApproveEmployeeLeaveAdmin(string Uid, string LTid)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int Euid = Convert.ToInt32(Uid);
                int LPTid = Convert.ToInt32(LTid);
                var data = dc.tblLeaveApplicationTracks.Where(x => x.BLId == LPTid).SingleOrDefault();
                if (data.ApproveStatus == false)
                {
                    data.ApproveStatus = true;
                    data.ApprovedNotes = "Dated:- " + System.DateTime.Now.ToString() + " Approved By :- UID-" + Session["UserIdentfier"].ToString();
                    dc.SaveChanges();
                }
                return RedirectToAction("ManageLeaves", new { id = Euid, msg = "Leave approved successfully" });
            }

        }

    }
}