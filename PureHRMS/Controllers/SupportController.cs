using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class SupportController : Controller
    {
        [HttpGet]
        public ActionResult ComposeTicket()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                ViewBag.DepartmentList = dc.tblDepartmentMasters.ToList();
                return View();

            }
        }
        [HttpPost]
        [ActionName("ComposeTicket")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ComposeTicketPost(string descrip,string DeptId, HttpPostedFileBase fileposted)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                ViewBag.DepartmentList = dc.tblDepartmentMasters.ToList();

                //////////////
                if (DeptId == null || DeptId == "")
                {
                    ViewBag.FoundError = "Please select department";
                    return View();
                }
                using (TransactionScope trns = new TransactionScope())
                {
                    try
                    {
                        CommonFunctions cf = new CommonFunctions();
                        tblSupportTickedMaster sm = new tblSupportTickedMaster();
                        sm.TDeptid = Convert.ToInt32(DeptId);
                        sm.TStatus = "Open";
                        sm.UserInvolved = Convert.ToInt32(Session["UserIdentfier"]);
                        TryUpdateModel(sm);
                        if (ModelState.IsValid)
                        {
                            string FilenameUploaded = "";

                            if (fileposted != null)
                            {

                                if (fileposted.ContentLength > 2500000)
                                {
                                    ViewBag.FoundError = "Uploaded image is greater than 2 MB, upload small image";
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
                                    FilenameUploaded = CommonFunctions.GenerateSlug(fileposted.FileName) + "_" + uniqueid + ext;
                                    fileposted.SaveAs(Server.MapPath("~/UploadedData/TicketFiles/" + FilenameUploaded));
                                }
                            }

                            dc.tblSupportTickedMasters.Add(sm);
                            dc.SaveChanges();
                            tblTicketRef tref = new tblTicketRef();
                            tref.AdminRead = false;
                            tref.UserRead = true;
                            tref.CreatedBy = Convert.ToInt32(Session["UserIdentfier"]);
                            tref.refTId = sm.Tid;
                            tref.TDate = System.DateTime.Now;
                            tref.TFile = FilenameUploaded;
                            tref.TText = Microsoft.Security.Application.Sanitizer.GetSafeHtml(descrip);
                            tref.UserInvolved = Convert.ToInt32(Session["UserIdentfier"]);
                            dc.tblTicketRefs.Add(tref);
                            dc.SaveChanges();
                            ViewBag.FoundError = "Ticket raised successfully";
                            ModelState.Clear();
                            trns.Complete();
                            trns.Dispose();
                        }
                        else
                        {
                            trns.Dispose();
                        }

                    }
                    catch
                    {
                        ViewBag.FoundError = "Sorry, we found errors";
                        trns.Dispose();
                    }

                }
                return View();
            }
        }

        public ActionResult TicketHistory()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                var data = dc.tblSupportTickedMasters.OrderByDescending(x => x.Tid).Where(x => x.UserInvolved == uid).ToList();
                List<MyTicketList> TicketList = new List<MyTicketList>();
                for (int i = 0; i < data.Count; i++)
                {
                    bool readsts = true;
                    int tid = Convert.ToInt32(data[i].Tid);
                    int did = Convert.ToInt32(data[i].TDeptid);
                    var dataRef = dc.tblTicketRefs.Where(x => x.UserRead == false && x.refTId == tid && x.UserInvolved == uid).ToList();
                    string deptname = dc.tblDepartmentMasters.Where(x => x.DepartmentId == did).SingleOrDefault().DepartmentName.ToString();
                    if (dataRef.Count > 0)
                    {
                        readsts = false;
                    }

                    TicketList.Add(
                        new MyTicketList
                        {
                            ID = data[i].Tid,
                            Read = readsts,
                            Subject = data[i].TSubject,
                            Department = deptname

                        }
                        );
                }
                return View(TicketList);

            }
        }

        [HttpGet]
        public ActionResult ReplyOnTicket(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                int tid = Convert.ToInt32(id);
                var data = dc.tblSupportTickedMasters.Where(x => x.Tid == tid && x.UserInvolved == uid).SingleOrDefault();
                int departid = Convert.ToInt32(data.TDeptid);
                ViewBag.TicketId = data.Tid;
                ViewBag.TicketSub = data.TSubject;
                ViewBag.DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == departid).SingleOrDefault().DepartmentName;

                var datatickets = dc.tblTicketRefs.Where(z => z.refTId == tid).OrderByDescending(z => z.STId).ToList();
                datatickets.ForEach(x => x.UserRead = true);
                dc.SaveChanges();

                List<ReplyTicketHistory> TicketList = new List<ReplyTicketHistory>();
                for (int i = 0; i < datatickets.Count; i++)
                {
                    int uidcreated = Convert.ToInt32(datatickets[i].CreatedBy);
                    var username = dc.tblUserMasters.Where(x => x.UserId == uidcreated).SingleOrDefault();
                    TicketList.Add(
                        new ReplyTicketHistory
                        {
                            Text = datatickets[i].TText,
                            Date = datatickets[i].TDate,
                            CreatedBy = username.FullName + " - " + username.EmpId,
                            AttachedFile = datatickets[i].TFile
                        }
                        );

                }
                ViewBag.TicketList = TicketList;

                return View();

            }
        }
        [HttpPost]
        [ActionName("ReplyOnTicket")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ReplyOnTicketPost(string descrip, string id, HttpPostedFileBase fileposted)
        {
            using (HRMSConn dc = new HRMSConn())
            {

                ///////////////////////////

                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                int tid = Convert.ToInt32(id);
                var data = dc.tblSupportTickedMasters.Where(x => x.Tid == tid && x.UserInvolved == uid).SingleOrDefault();
                int departid = Convert.ToInt32(data.TDeptid);
                ViewBag.TicketId = data.Tid;
                ViewBag.TicketSub = data.TSubject;
                ViewBag.DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == departid).SingleOrDefault().DepartmentName;

                ////////////////////////////////

                string FilenameUploaded = "";

                if (fileposted != null)
                {

                    if (fileposted.ContentLength > 2500000)
                    {
                        ViewBag.FoundError = "Uploaded image is greater than 2 MB, upload small image";
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
                        FilenameUploaded = CommonFunctions.GenerateSlug(fileposted.FileName) + "_" + uniqueid + ext;
                        fileposted.SaveAs(Server.MapPath("~/UploadedData/TicketFiles/" + FilenameUploaded));
                    }
                }

                //////////////////////////////////////////

                tblTicketRef tref = new tblTicketRef();
                tref.refTId = tid;
                tref.AdminRead = false;
                tref.UserRead = true;
                tref.UserInvolved = uid;
                tref.CreatedBy = uid;
                tref.TDate = System.DateTime.Now;
                tref.TFile = FilenameUploaded;
                tref.TText = Microsoft.Security.Application.Sanitizer.GetSafeHtml(descrip);
                dc.tblTicketRefs.Add(tref);
                dc.SaveChanges();
                ModelState.Clear();
                ViewBag.FoundError = "Ticket replied successfully";

                ///////////////////////////////////

                var datatickets = dc.tblTicketRefs.Where(z => z.refTId == tid).OrderByDescending(z => z.STId).ToList();
                List<ReplyTicketHistory> TicketList = new List<ReplyTicketHistory>();
                for (int i = 0; i < datatickets.Count; i++)
                {
                    int uidcreated = Convert.ToInt32(datatickets[i].CreatedBy);
                    var username = dc.tblUserMasters.Where(x => x.UserId == uidcreated).SingleOrDefault();
                    TicketList.Add(
                        new ReplyTicketHistory
                        {
                            Text = datatickets[i].TText,
                            Date = datatickets[i].TDate,
                            CreatedBy = username.FullName + "/ " + username.EmpId,
                            AttachedFile = datatickets[i].TFile
                        }
                        );

                }
                ViewBag.TicketList = TicketList;

                ///////////////////////////
                return View();

            }
        }

        [HttpGet]
        public ActionResult ManageSupportAdmin()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                ViewBag.DepartmentList = dc.tblDepartmentMasters.ToList();
                var dataEmployees = (from p in dc.tblUserMasters.Where(x => x.Active == true)
                                     select new
                                     {
                                         UserIdentifier = p.UserId,
                                         UserDisplay = p.FullName + "---" + p.EmpId + "---" + p.EmailId
                                     }).ToList();
                dataEmployees.Insert(0, new { UserIdentifier = 0, UserDisplay = "-Please Select-" });
                ViewBag.EmployeeList = dataEmployees;
                /////////////////
                var suppal = dc.tblSupportAdminManagers.OrderByDescending(x => x.SMId).ToList();
                List<SupportAdminList> AL = new List<SupportAdminList>();
                for (int j = 0; j < suppal.Count; j++)
                {
                    int DeptInnerId = Convert.ToInt32(suppal[j].refDepartmentId);
                    int EmpInnerId = Convert.ToInt32(suppal[j].refEmployeeId);
                    string DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == DeptInnerId).SingleOrDefault().DepartmentName;
                    var userdata = dc.tblUserMasters.Where(x => x.UserId == EmpInnerId).SingleOrDefault();
                    string EmpName = userdata.FullName + "---" + userdata.EmpId + "---" + userdata.EmailId;
                    AL.Add(
                        new SupportAdminList
                        {
                            ID = suppal[j].SMId,
                            Employee = EmpName,
                            Department = DeptName

                        }
                        );
                }
                ViewBag.SuppAL = AL;
                ///////////////////////
                return View();
            }


        }
        [HttpPost]
        [ActionName("ManageSupportAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult ManageSupportAdminPost(string DeptId, string empDDL)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                //////////////////////
                ViewBag.DepartmentList = dc.tblDepartmentMasters.ToList();
                var dataEmployees = (from p in dc.tblUserMasters.Where(x => x.Active == true)
                                     select new
                                     {
                                         UserIdentifier = p.UserId,
                                         UserDisplay = p.FullName + "---" + p.EmpId + "---" + p.EmailId
                                     }).ToList();
                dataEmployees.Insert(0, new { UserIdentifier = 0, UserDisplay = "-Please Select-" });
                ViewBag.EmployeeList = dataEmployees;

                /////////////////
                var suppal = dc.tblSupportAdminManagers.ToList();
                List<SupportAdminList> AL = new List<SupportAdminList>();
                for (int j = 0; j < suppal.Count; j++)
                {
                    int DeptInnerId = Convert.ToInt32(suppal[j].refDepartmentId);
                    int EmpInnerId = Convert.ToInt32(suppal[j].refEmployeeId);
                    string DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == DeptInnerId).SingleOrDefault().DepartmentName;
                    var userdata = dc.tblUserMasters.Where(x => x.UserId == EmpInnerId).SingleOrDefault();
                    string EmpName = userdata.FullName + "---" + userdata.EmpId + "---" + userdata.EmailId;
                    AL.Add(
                        new SupportAdminList
                        {
                            ID = suppal[j].SMId,
                            Employee = EmpName,
                            Department = DeptName

                        }
                        );
                }
                ViewBag.SuppAL = AL;
                ///////////////////////

                if (DeptId == null || DeptId == "")
                {
                    ViewBag.FoundError = "Please select department";
                    return View();
                }
                if (empDDL == null || empDDL == "" || empDDL == "0")
                {
                    ViewBag.FoundError = "Please select employee";
                    return View();
                }

                /////////////////////

                int DeparttID = Convert.ToInt32(DeptId);
                int UserId = Convert.ToInt32(empDDL);
                var data = dc.tblSupportAdminManagers.Where(x => x.refDepartmentId == DeparttID && x.refEmployeeId == UserId).ToList();

                if (data.Count > 0)
                {
                    ViewBag.FoundError = "This admin already exists";

                }
                else
                {
                    tblSupportAdminManager sam = new tblSupportAdminManager();
                    sam.refDepartmentId = DeparttID;
                    sam.refEmployeeId = UserId;
                    dc.tblSupportAdminManagers.Add(sam);
                    dc.SaveChanges();
                    ViewBag.FoundError = "Support admin added successfully";
                    ModelState.Clear();
                    /////////////////
                    var suppalnew = dc.tblSupportAdminManagers.OrderByDescending(x => x.SMId).ToList();
                    List<SupportAdminList> ALnew = new List<SupportAdminList>();
                    for (int z = 0; z < suppalnew.Count; z++)
                    {
                        int DeptInnerIdnew = Convert.ToInt32(suppalnew[z].refDepartmentId);
                        int EmpInnerIdnew = Convert.ToInt32(suppalnew[z].refEmployeeId);
                        string DeptNameNew = dc.tblDepartmentMasters.Where(x => x.DepartmentId == DeptInnerIdnew).SingleOrDefault().DepartmentName;
                        var UserdataNew = dc.tblUserMasters.Where(x => x.UserId == EmpInnerIdnew).SingleOrDefault();
                        string EmpNameNew = UserdataNew.FullName + "---" + UserdataNew.EmpId + "---" + UserdataNew.EmailId;
                        ALnew.Add(
                            new SupportAdminList
                            {
                                ID = suppalnew[z].SMId,
                                Employee = EmpNameNew,
                                Department = DeptNameNew

                            }
                            );
                    }
                    ViewBag.SuppAL = ALnew;
                    ///////////////////////
                }

                /////////////////////////////////////

                return View();
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSupportAdmin(int id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblSupportAdminManagers.Where(x => x.SMId == id).SingleOrDefault();
                dc.tblSupportAdminManagers.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("ManageSupportAdmin");
            }
        }
        
        public ActionResult TicketListAdmin()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                var IsSupportAdmin = dc.tblSupportAdminManagers.Where(x => x.refEmployeeId == uid).ToList();
                if (IsSupportAdmin.Count == 0)
                {
                    return RedirectToAction("EmployeeDashboard", "Home");
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


                    List<MyTicketList> TicketList = new List<MyTicketList>();
                    for (int i = 0; i < GetTickets.Count; i++)
                    {
                        bool readsts = true;

                        int tid = Convert.ToInt32(GetTickets[i].Tid);

                        int did = Convert.ToInt32(GetTickets[i].TDeptid);

                        var dataRef = dc.tblTicketRefs.Where(x => x.AdminRead == false && x.refTId == tid).ToList();
                        string deptname = dc.tblDepartmentMasters.Where(x => x.DepartmentId == did).SingleOrDefault().DepartmentName.ToString();
                        if (dataRef.Count > 0)
                        {
                            readsts = false;
                        }

                        TicketList.Add(
                            new MyTicketList
                            {
                                ID = GetTickets[i].Tid,
                                Read = readsts,
                                Subject = GetTickets[i].TSubject,
                                Department = deptname

                            }
                            );
                    }
                    return View(TicketList);
                }



            }
        }

        [HttpGet]
        public ActionResult ReplyOnTicketAdmin(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                int TktId = Convert.ToInt32(id);
                var IsSupportAdmin = dc.tblSupportAdminManagers.Where(x => x.refEmployeeId == uid).ToList();
                if (IsSupportAdmin.Count == 0)
                {
                    return RedirectToAction("EmployeeDashboard", "Home");
                }
                else
                {
                    int[] depts = new int[IsSupportAdmin.Count];
                    for (int z = 0; z < IsSupportAdmin.Count; z++)
                    {
                        depts[z] = Convert.ToInt32(IsSupportAdmin[z].refDepartmentId); // INSERTING ALL DEPARTMENTS ALLOWED TO THIS EMPLOYEE
                    }
                    var GetTickets = dc.tblSupportTickedMasters.OrderByDescending(x => x.Tid).Where(
                     x => depts.Contains((int)x.TDeptid) && x.Tid == TktId).SingleOrDefault();


                    ViewBag.TicketId = GetTickets.Tid;
                    ViewBag.TicketSub = GetTickets.TSubject;
                    int departid = Convert.ToInt32(GetTickets.TDeptid);
                    ViewBag.DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == departid).SingleOrDefault().DepartmentName;
                    var datatickets = dc.tblTicketRefs.Where(z => z.refTId == TktId).OrderByDescending(z => z.STId).ToList();
                    datatickets.ForEach(x => x.AdminRead = true);
                    dc.SaveChanges();

                    ////////////////////

                    List<ReplyTicketHistory> TicketList = new List<ReplyTicketHistory>();
                    for (int i = 0; i < datatickets.Count; i++)
                    {
                        int uidcreated = Convert.ToInt32(datatickets[i].CreatedBy);
                        var username = dc.tblUserMasters.Where(x => x.UserId == uidcreated).SingleOrDefault();
                        TicketList.Add(
                            new ReplyTicketHistory
                            {
                                Text = datatickets[i].TText,
                                Date = datatickets[i].TDate,
                                CreatedBy = username.FullName + " - " + username.EmpId,
                                AttachedFile = datatickets[i].TFile
                            }
                            );

                    }
                    ViewBag.TicketList = TicketList;

                    return View();
                }




            }
        }

        [HttpPost]
        [ActionName("ReplyOnTicketAdmin")]      
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ReplyOnTicketAdminPost(string id ,  HttpPostedFileBase fileposted, string descrip)
        {
            ///////////////////////////
            using (HRMSConn dc = new HRMSConn())
            {
                int uid = Convert.ToInt32(Session["UserIdentfier"]);
                int tid = Convert.ToInt32(id);
                var data = dc.tblSupportTickedMasters.Where(x => x.Tid == tid).SingleOrDefault();
                int UserInv = Convert.ToInt32(data.UserInvolved);
                int departid = Convert.ToInt32(data.TDeptid);
                ViewBag.TicketId = data.Tid;
                ViewBag.TicketSub = data.TSubject;
                ViewBag.DeptName = dc.tblDepartmentMasters.Where(x => x.DepartmentId == departid).SingleOrDefault().DepartmentName;

                ////////////////////////////////

                string FilenameUploaded = "";

                if (fileposted != null)
                {

                    if (fileposted.ContentLength > 2500000)
                    {
                        ViewBag.FoundError = "Uploaded image is greater than 2 MB, upload small image";
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
                        FilenameUploaded = CommonFunctions.GenerateSlug(fileposted.FileName) + "_" + uniqueid + ext;
                        fileposted.SaveAs(Server.MapPath("~/UploadedData/TicketFiles/" + FilenameUploaded));
                    }
                }

                //////////////////////////////////////////

                tblTicketRef tref = new tblTicketRef();
                tref.refTId = tid;
                tref.AdminRead = true;
                tref.UserRead = false;
                tref.UserInvolved = UserInv;
                tref.CreatedBy = uid;
                tref.TDate = System.DateTime.Now;
                tref.TFile = FilenameUploaded;
                tref.TText = Microsoft.Security.Application.Sanitizer.GetSafeHtml(descrip);
                dc.tblTicketRefs.Add(tref);
                dc.SaveChanges();
                ModelState.Clear();
                ViewBag.FoundError = "Ticket replied successfully";

                ///////////////////////////////////

                var datatickets = dc.tblTicketRefs.Where(z => z.refTId == tid).OrderByDescending(z => z.STId).ToList();
                List<ReplyTicketHistory> TicketList = new List<ReplyTicketHistory>();
                for (int i = 0; i < datatickets.Count; i++)
                {
                    int uidcreated = Convert.ToInt32(datatickets[i].CreatedBy);
                    var username = dc.tblUserMasters.Where(x => x.UserId == uidcreated).SingleOrDefault();
                    TicketList.Add(
                        new ReplyTicketHistory
                        {
                            Text = datatickets[i].TText,
                            Date = datatickets[i].TDate,
                            CreatedBy = username.FullName + "/ " + username.EmpId,
                            AttachedFile = datatickets[i].TFile
                        }
                        );

                }
                ViewBag.TicketList = TicketList;

                ///////////////////////////
                return View();
            }
               
        }

      
    }
}