using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
using System.Transactions;
namespace PureHRMS.Controllers
{
    public class PayrollController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult ManagePayrollBasic(string id, string msg)
        {
            if (msg != null || msg != "")
            {
                ViewBag.FoundError = msg;
            }
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

                        return RedirectToAction("UpdateEmployee", "Employee");

                    }
                    else
                    {
                        // ALLOW PROCESSING OF BELOW CODE
                    }
                }

                //////////////////////////////
                var user = dc.tblUserMasters.Where(z => z.UserId == userid).SingleOrDefault();
                ViewBag.UserIdentifier = id;
                ViewBag.UserName = user.FullName;
                ViewBag.UserEmpID = user.EmpId;
                var data = dc.tblSalaryMasters.Where(x => x.refEID == userid).SingleOrDefault();
                return View(data);
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("ManagePayrollBasic")]
        [ValidateAntiForgeryToken]
        public ActionResult ManagePayrollBasicPost(string id)
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

                        return RedirectToAction("UpdateEmployee", "Employee");

                    }
                    else
                    {
                        ; // ALLOW PROCESSING OF BELOW CODE
                    }
                }

                //////////////////////////////
                var user = dc.tblUserMasters.Where(z => z.UserId == userid).SingleOrDefault();
                ViewBag.UserIdentifier = id;
                ViewBag.UserName = user.FullName;
                ViewBag.UserEmpID = user.EmpId;
                /////////////////////////////////////

                var data = dc.tblSalaryMasters.Where(x => x.refEID == userid).SingleOrDefault();

                if (data == null)
                {
                    tblSalaryMaster sm = new tblSalaryMaster();
                    sm.refEID = userid;
                    TryUpdateModel(sm);
                    if (ModelState.IsValid)
                    {
                        dc.tblSalaryMasters.Add(sm);
                        dc.SaveChanges();
                        return RedirectToAction("ManagePayrollBasic", new { msg = "Data Added Successfully" });
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    TryUpdateModel(data);
                    if (ModelState.IsValid)
                    {
                        dc.SaveChanges();
                        return RedirectToAction("ManagePayrollBasic", new { msg = "Data Saved Successfully" });
                    }
                    else
                    {
                        return View();
                    }
                }

            }
        }

        [CkeckAllowedAccess]
        public ActionResult MonthlySalarySelect(string msg)
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
        public ActionResult GenerateMonthlySalary(string SalDate)
        {

            if (SalDate == null || SalDate == "" || (!SalDate.Contains('-')))
            {
                return RedirectToAction("MonthlySalarySelect", new { msg = "Please select month & year properly" });
            }
            else
            {
                using (HRMSConn dc = new HRMSConn())
                {
                    string[] arr = SalDate.Split('-');
                    int SalMonth = Convert.ToInt32(arr[0]);
                    int SalYear = Convert.ToInt32(arr[1]);
                    ViewBag.MY = arr[0] + "-" + arr[1];

                    var dataSYALRDY = dc.tblMonthySals.Where(x => x.ForMonth == SalMonth && x.ForYear == SalYear).ToList();
                    if(dataSYALRDY.Count>0)
                    {
                        return RedirectToAction("MonthlySalarySelect", new { msg = "Salaries already generated for this month & year" });
                    }
                 
                    List<MonthlySalGenerator> SalList = new List<MonthlySalGenerator>();

                    var dataUsers = dc.tblUserMasters.Where(x => x.Active == true && x.IsWebMaster!=true).ToList();

                    for (int i = 0; i < dataUsers.Count; i++)
                    {
                        int Euid = Convert.ToInt32(dataUsers[i].UserId);
                        string EName = dataUsers[i].FullName;
                        string EmpID = dataUsers[i].EmpId;
                        var getBasicSalData = dc.tblSalaryMasters.Where(x => x.refEID == Euid).SingleOrDefault();


                        if (getBasicSalData == null)
                        {
                            ;
                        }
                        else
                        {
                            int NetSalaryAmountCal = (Convert.ToInt32(getBasicSalData.BasicSalary) +
                         Convert.ToInt32(getBasicSalData.HRA) +
                         Convert.ToInt32(getBasicSalData.SpecialAllowance) +
                         Convert.ToInt32(getBasicSalData.ConveyanceAllowance) +
                         Convert.ToInt32(getBasicSalData.MealAllowance) +
                          Convert.ToInt32(getBasicSalData.TelephoneAllowance) +
                          Convert.ToInt32(getBasicSalData.OtherAllowance)) -
                          (Convert.ToInt32(getBasicSalData.PF) + Convert.ToInt32(getBasicSalData.IncomeTax) + Convert.ToInt32(getBasicSalData.OtherDeduction));


                            DateTime dtSD = Convert.ToDateTime(SalMonth + "/01/" + SalYear);
                            int totaldays = DateTime.DaysInMonth(SalYear, SalMonth);
                            DateTime dtED = Convert.ToDateTime(+SalMonth + "/" + totaldays + "/" + SalYear);
                            int FullNonPayDays = dc.tblAttendanceMasters.Where(x => x.refEuid == Euid && (x.AttendanceDate >= dtSD && x.AttendanceDate <= dtED) && (x.AttandanceCode == "FDLWP" || x.AttandanceCode == "A")).ToList().Count;
                            int HalfNonPayDays = dc.tblAttendanceMasters.Where(x => x.refEuid == Euid && (x.AttendanceDate >= dtSD && x.AttendanceDate <= dtED) && x.AttandanceCode == "HDLWP").ToList().Count;

                            decimal workingdaysCal = totaldays;

                            decimal NonWorkingDays = Math.Round((Convert.ToDecimal(FullNonPayDays) + (Convert.ToDecimal(0.5) * Convert.ToDecimal(HalfNonPayDays))), 1);

                            decimal DaysWorkedCal = workingdaysCal - NonWorkingDays;

                            ////////////DEDUCTION FOR APBSENT IF ANY //////////////

                            int OneDaySal = NetSalaryAmountCal / Convert.ToInt32(workingdaysCal);

                            int DeductionAbs = Convert.ToInt32(Convert.ToDecimal(OneDaySal) * NonWorkingDays);

                            //////////////////////////////////////////////
                            int NetPayAmtCal = NetSalaryAmountCal - DeductionAbs;
                            ////////////////////////////


                            SalList.Add(
                                new MonthlySalGenerator
                                {
                                    Euid = Euid,
                                    DaysWorked = DaysWorkedCal,
                                    EmpID = EmpID,
                                    Name = EName,
                                    NetSalaryAmount = NetSalaryAmountCal,
                                    NetPayAmt = NetPayAmtCal,
                                    WorkingDays = workingdaysCal,
                                    FinalSalAmt = NetPayAmtCal,
                                    MoreAddition = 0,
                                    MoreAdditionNotes = "",
                                    MoreDeductions = 0,
                                    MoreDeductionsNotes = ""
                                }
                                );
                        }
                    }

                    return View(SalList);
                }

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GenerateMonthlySalary")]
        public ActionResult GenerateMonthlySalaryPost(List<MonthlySalGenerator> SalListGen ,string SalDate)
        {
            if (SalDate == null || SalDate == "" || (!SalDate.Contains('-')))
            {
                return RedirectToAction("MonthlySalarySelect", new { msg = "Please select month & year properly" });
            }
            else
            {
                string[] arr = SalDate.Split('-');
                int SalMonth = Convert.ToInt32(arr[0]);
                int SalYear = Convert.ToInt32(arr[1]);
                ViewBag.MY = arr[0] + "-" + arr[1];
                using (HRMSConn dc = new HRMSConn())
                {                   

                    var dataSYALRDY = dc.tblMonthySals.Where(x => x.ForMonth == SalMonth && x.ForYear == SalYear).ToList();

                    if (dataSYALRDY.Count > 0)
                    {
                        return RedirectToAction("MonthlySalarySelect", new { msg = "Salaries already generated for this month & year" });
                    }
                }
              
                    using (TransactionScope trans = new TransactionScope())
                    {
                    try
                    {
                        using (HRMSConn dc = new HRMSConn())
                        {


                            ////////////////
                            // LOOP HERE FOR INSERT//
                            ///////////////       

                            for(int i=0;i< SalListGen.Count;i++)
                            {
                                int Userid = Convert.ToInt32(SalListGen[i].Euid);

                                var dataBSdetails = dc.tblSalaryMasters.Where(x => x.refEID == Userid).SingleOrDefault();

                                tblMonthySal ms = new tblMonthySal();
                                ms.refEid = Userid;
                                ms.NetSalAmt = SalListGen[i].NetSalaryAmount;
                                ms.WorkingDays= SalListGen[i].WorkingDays;
                                ms.DaysWorked = SalListGen[i].DaysWorked;
                                ms.NetPaySalAmt = SalListGen[i].NetPayAmt;
                                ms.MoreDeduction = SalListGen[i].MoreDeductions;
                                ms.MoreDeductionNotes = SalListGen[i].MoreDeductionsNotes;
                                ms.MoreAddition = SalListGen[i].MoreAddition;
                                ms.MoreAdditionNotes = SalListGen[i].MoreAdditionNotes;
                                ms.FinalSalAmt = (Convert.ToInt32(SalListGen[i].NetPayAmt) + Convert.ToInt32(SalListGen[i].MoreAddition))- Convert.ToInt32(SalListGen[i].MoreDeductions);
                                ms.ForMonth = SalMonth;
                                ms.ForYear = SalYear;
                                ms.CreadtedOn =System.DateTime.Now;
                                ms.CreatedBy = Convert.ToInt32(Session["UserIdentfier"]);
                                ms.BasicSal = dataBSdetails.BasicSalary;
                                ms.HRA = dataBSdetails.HRA;
                                ms.SpecialAllowance = dataBSdetails.SpecialAllowance;
                                ms.ConveyanceAllowance = dataBSdetails.ConveyanceAllowance;
                                ms.MealAllowance = dataBSdetails.MealAllowance;
                                ms.TelephoneAllowance = dataBSdetails.TelephoneAllowance;
                                ms.OtherAllowance = dataBSdetails.OtherAllowance;
                                ms.OtherAllowanceNotes = dataBSdetails.OtherAllowanceNotes;
                                ms.PF = dataBSdetails.PF;
                                ms.IncomeTax = dataBSdetails.IncomeTax;
                                ms.OtherDeduction = dataBSdetails.OtherDeduction;
                                ms.OtherDeductionNotes = dataBSdetails.OtherDeductionNotes;
                                dc.tblMonthySals.Add(ms);
                                dc.SaveChanges();
                            }
                            
                            //////////////////////
                            ///LOOP END////
                            //////////////////////
                            trans.Complete();
                            trans.Dispose();
                            return View("_Success");
                        }
                    }
                    catch
                    {
                        trans.Dispose();
                        return RedirectToAction("MonthlySalarySelect", new { msg = "Sorry, we found some error" });
                    }

                    }
               
               
            }
            
        }

        [CkeckAllowedAccess]
        public ActionResult SalSlips(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int Euid = Convert.ToInt32(id);

                var Userdata = dc.tblUserMasters.Where(x => x.UserId == Euid).SingleOrDefault();
                ViewBag.UserName = Userdata.FullName.ToString();
                ViewBag.EmpId = Userdata.EmpId;
                ViewBag.UserIdentifier = Euid;

                var data = dc.tblMonthySals.Where(x => x.refEid == Euid).OrderByDescending(x=>x.MSId).ToList();
                return View(data);
            }

        }

        [CkeckAllowedAccess]
        public ActionResult PrintSalSlip(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int MSID = Convert.ToInt32(id);

                var dataSal = dc.tblMonthySals.Where(x => x.MSId == MSID).SingleOrDefault();
                int userId = Convert.ToInt32(dataSal.refEid);
                try
                {
                    
                    var dataUser = dc.tblUserMasters.Where(x => x.UserId == userId).SingleOrDefault();
                    int designID = Convert.ToInt32(dataUser.DesignationId);

                    ViewBag.EN = dataUser.FullName;
                    ViewBag.Eid = dataUser.EmpId;
                    ViewBag.Designation = dc.tblDesignationMasters.Where(x => x.DesignationId == designID).SingleOrDefault().DesignationName;
                    ViewBag.WkgDays = dataSal.WorkingDays;
                    ViewBag.WkdDays = dataSal.DaysWorked;
                    ViewBag.MY = dataSal.ForMonth + "/" + dataSal.ForYear;
                    ViewBag.NS = dataSal.NetSalAmt;
                    ViewBag.FSal = dataSal.FinalSalAmt;

                    ///CALCULATING LWPs///                
                    int OneDaySal = Convert.ToInt32(dataSal.NetSalAmt) / Convert.ToInt32(dataSal.WorkingDays);
                    decimal NonWorkingDays = Math.Round((Convert.ToDecimal(dataSal.WorkingDays) - Convert.ToDecimal(dataSal.DaysWorked)), 1);
                    int DeductionAbs = Convert.ToInt32(Convert.ToDecimal(OneDaySal) * NonWorkingDays);
                    /////////////////

                    ViewBag.LWD = NonWorkingDays + " Days of absentism so  " + DeductionAbs + " Amount of deduction.";
                    ViewBag.PF = dataSal.PF;
                    ViewBag.IT = dataSal.IncomeTax;
                    ViewBag.Dedc = dataSal.MoreDeduction + ", " + dataSal.MoreDeductionNotes;
                    ViewBag.Add = dataSal.MoreAddition + ", " + dataSal.MoreAdditionNotes;
                    var genset = dc.tblSettingsMasters.ToList();
                    ViewBag.NameOfCom = genset[0].CompanyNameSal;
                    ViewBag.Instructions = genset[0].CompanyInstructionForSal;
                    ViewBag.LogoName = genset[0].CompanyLogo;
                    ViewBag.AddCon = genset[0].AddressContactDetails;
                }
                catch
                {
                    return RedirectToAction("SalSlips", new  { id= userId });
                }
             

                return View();
            }

        }
        [CkeckAllowedAccess]
        public ActionResult MySalSlips()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int Euid = Convert.ToInt32(Session["UserIdentfier"]);

                var Userdata = dc.tblUserMasters.Where(x => x.UserId == Euid).SingleOrDefault();
                ViewBag.UserName = Userdata.FullName.ToString();
                ViewBag.EmpId = Userdata.EmpId;
                ViewBag.UserIdentifier = Euid;

                var data = dc.tblMonthySals.Where(x => x.refEid == Euid).OrderByDescending(x => x.MSId).ToList();
                return View(data);
            }

        }
        [CkeckAllowedAccess]
        public ActionResult PrintMySalSlip(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int Euid = Convert.ToInt32(Session["UserIdentfier"]);
                try
                {
                   
                    int MSID = Convert.ToInt32(id);

                    var dataSal = dc.tblMonthySals.Where(x => x.MSId == MSID).SingleOrDefault();
                    var dataUser = dc.tblUserMasters.Where(x => x.UserId == Euid).SingleOrDefault();
                    int designID = Convert.ToInt32(dataUser.DesignationId);

                    ViewBag.EN = dataUser.FullName;
                    ViewBag.Eid = dataUser.EmpId;
                    ViewBag.Designation = dc.tblDesignationMasters.Where(x => x.DesignationId == designID).SingleOrDefault().DesignationName;
                    ViewBag.WkgDays = dataSal.WorkingDays;
                    ViewBag.WkdDays = dataSal.DaysWorked;
                    ViewBag.MY = dataSal.ForMonth + "/" + dataSal.ForYear;
                    ViewBag.NS = dataSal.NetSalAmt;
                    ViewBag.FSal = dataSal.FinalSalAmt;

                    ///CALCULATING LWPs///                
                    int OneDaySal = Convert.ToInt32(dataSal.NetSalAmt) / Convert.ToInt32(dataSal.WorkingDays);
                    decimal NonWorkingDays = Math.Round((Convert.ToDecimal(dataSal.WorkingDays) - Convert.ToDecimal(dataSal.DaysWorked)), 1);
                    int DeductionAbs = Convert.ToInt32(Convert.ToDecimal(OneDaySal) * NonWorkingDays);
                    /////////////////

                    ViewBag.LWD = NonWorkingDays + " Days of absentism so  " + DeductionAbs + " Amount of deduction.";
                    ViewBag.PF = dataSal.PF;
                    ViewBag.IT = dataSal.IncomeTax;
                    ViewBag.Dedc = dataSal.MoreDeduction + ", " + dataSal.MoreDeductionNotes;
                    ViewBag.Add = dataSal.MoreAddition + ", " + dataSal.MoreAdditionNotes;
                    var genset = dc.tblSettingsMasters.ToList();
                    ViewBag.NameOfCom = genset[0].CompanyNameSal;
                    ViewBag.Instructions = genset[0].CompanyInstructionForSal;
                    ViewBag.LogoName = genset[0].CompanyLogo;
                    ViewBag.AddCon = genset[0].AddressContactDetails;
                }
                catch
                {
                    return RedirectToAction("MySalSlips");
                }
               

                return View();
            }

        }

    }
}