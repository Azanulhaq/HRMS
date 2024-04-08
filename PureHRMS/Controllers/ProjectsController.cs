using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;

namespace PureHRMS.Controllers
{
    public class ProjectsController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult InsertProject(string setmsg)
        {
            if(setmsg!=null && setmsg!="")
            {
                ViewBag.FoundError = setmsg;
            }
            return View();
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("InsertProject")]
        [ValidateAntiForgeryToken]       
        public ActionResult InsertProjectPost()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                tblProjectMaster pm = new tblProjectMaster();               
                pm.Createdby = Convert.ToInt32(Session["UserIdentfier"]);
                pm.CreatedOn = System.DateTime.Now;
                pm.ProjectStatus = "In Progress";
                TryUpdateModel(pm);
                if (ModelState.IsValid)
                {                  
                    dc.tblProjectMasters.Add(pm);
                    dc.SaveChanges();
                    return RedirectToAction("InsertProject",new { setmsg= "Project Adedd Successfully with Project ID: " + pm.ProjectId.ToString() });
                }
                else
                {
                    return View();
                }
                

            }
        }

        [CkeckAllowedAccess]
        public ActionResult AllProjectList()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblProjectMasters.OrderByDescending(x=>x.ProjectId).ToList();
                return View(data);
            }
        }

        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult EditProject(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int projectId = Convert.ToInt32(id);
                var data = dc.tblProjectMasters.Where(x => x.ProjectId== projectId).SingleOrDefault();
                return View(data);
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("EditProject")]
        [ValidateAntiForgeryToken]        
        public ActionResult EditProjectPost(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {

                int projectId = Convert.ToInt32(id);
                var data = dc.tblProjectMasters.Where(x => x.ProjectId == projectId).SingleOrDefault();

                TryUpdateModel(data);
                if (ModelState.IsValid)
                {
                    dc.SaveChanges();
                    ViewBag.FoundError = "Project updated successfully";
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
        public ActionResult DeleteProject(string id)
        {
            using ( HRMSConn dc=new HRMSConn())
            {
                int prjid = Convert.ToInt32(id);
                var data = dc.tblProjectMasters.Where(x => x.ProjectId == prjid).SingleOrDefault();
                dc.tblProjectMasters.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("AllProjectList");
            }

               
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatusProject(string id)
        {
            int pid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblProjectMasters.Where(x => x.ProjectId == pid).SingleOrDefault();
                if (data.ProjectStatus == "In Progress")
                {
                    data.ProjectStatus = "Completed";
                }
                else
                {
                    data.ProjectStatus = "In Progress";
                }
                dc.SaveChanges();
                return RedirectToAction("AllProjectList");
            }
        }
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult ProjectTeamAllocation(string id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int prjid = Convert.ToInt32(id);
                var data = dc.tblProjectMasters.Where(x => x.ProjectId == prjid).SingleOrDefault();
                ViewBag.ProjectID = data.ProjectId;
                ViewBag.ProjectTitle = data.ProjectTitle;

                // MAKING DROPDOWNLIST //////
                var dataUsers = dc.tblUserMasters.Where(x=>x.Active==true && x.IsWebMaster!=true).ToList();

                List<SelectListItem> EmployeeListAll = new List<SelectListItem>();
                EmployeeListAll.Add(new SelectListItem { Text = "-Please Select-", Value = "0" });

                for (int j=0;j< dataUsers.Count;j++)
                {
                    int DesigID =Convert.ToInt32(dataUsers[j].DesignationId);
                    var DesignationOfUsers = dc.tblDesignationMasters.Where(x => x.DesignationId == DesigID).SingleOrDefault();
                    int DeptID = Convert.ToInt32(dataUsers[j].DepartmentId); ;
                    var DepartmentOfUsers = dc.tblDepartmentMasters.Where(x => x.DepartmentId == DeptID).SingleOrDefault();
                    EmployeeListAll.Add(new SelectListItem
                    {
                        Text = dataUsers[j].FullName+ "---" + dataUsers[j].EmpId + "---" +
                        DesignationOfUsers.DesignationName + "---" + DepartmentOfUsers.DepartmentName,
                        Value = dataUsers[j].UserId.ToString()
                    });
                }
              
                ViewBag.AllEmpList = EmployeeListAll;

                ////////

                ViewBag.AllotedList = dc.tblProjectAllotteds.Where(x => x.refProjectId == prjid).ToList();
                ViewBag.CountAllotedList = dc.tblProjectAllotteds.Where(x => x.refProjectId == prjid).ToList().Count;

                //////////////////
                return View();
            }


        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("ProjectTeamAllocation")]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectTeamAllocationPost(string id, int EmpId)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                int prjid = Convert.ToInt32(id);
                var data = dc.tblProjectMasters.Where(x => x.ProjectId == prjid).SingleOrDefault();
                ViewBag.ProjectID = data.ProjectId;
                ViewBag.ProjectTitle = data.ProjectTitle;

                // MAKING DROPDOWNLIST //////
                var dataUsers = dc.tblUserMasters.Where(x => x.Active == true && x.IsWebMaster!=true).ToList();

                List<SelectListItem> EmployeeListAll = new List<SelectListItem>();
                EmployeeListAll.Add(new SelectListItem { Text = "-Please Select-", Value = "0" });

                for (int j = 0; j < dataUsers.Count; j++)
                {
                    int DesigID = Convert.ToInt32(dataUsers[j].DesignationId);
                    var DesignationOfUsers = dc.tblDesignationMasters.Where(x => x.DesignationId == DesigID).SingleOrDefault();
                    int DeptID = Convert.ToInt32(dataUsers[j].DepartmentId); ;
                    var DepartmentOfUsers = dc.tblDepartmentMasters.Where(x => x.DepartmentId == DeptID).SingleOrDefault();
                    EmployeeListAll.Add(new SelectListItem
                    {
                        Text = dataUsers[j].FullName + "---" + dataUsers[j].EmpId + "---" +
                        DesignationOfUsers.DesignationName + "---" + DepartmentOfUsers.DepartmentName,
                        Value = dataUsers[j].UserId.ToString()
                    });
                }

                ViewBag.AllEmpList = EmployeeListAll;

                ////////
                
                ViewBag.AllotedList = dc.tblProjectAllotteds.Where(x => x.refProjectId == prjid).ToList();
                ViewBag.CountAllotedList = dc.tblProjectAllotteds.Where(x => x.refProjectId == prjid).ToList().Count;

                //////////////////
                if (EmpId==0)
                {
                    ViewBag.FoundError = "Please select employee";
                    return View();
                }
                else
                {
                    ///
                    var chkduplicate = dc.tblProjectAllotteds.Where(x => x.refProjectId == prjid && x.refEmployeeId == EmpId).ToList();
                    if(chkduplicate.Count>0)
                    {
                        ViewBag.FoundError = "Project already allotted to employee";
                        return View();
                    }
                    ///
                    tblProjectAllotted pa = new tblProjectAllotted();
                    pa.refEmployeeId = EmpId;
                    pa.refProjectId = Convert.ToInt32(prjid);
                    TryUpdateModel(pa);
                    if (ModelState.IsValid)
                    {
                        dc.tblProjectAllotteds.Add(pa);
                        dc.SaveChanges();
                        return RedirectToAction("ProjectTeamAllocation", new { id = prjid });

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
        public ActionResult DeleteProjectAllocation(string prjid, string delid)
        {
            int allid = Convert.ToInt32(delid);
            int pid = Convert.ToInt32(prjid);
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblProjectAllotteds.OrderByDescending(x => x.AllottedId).Where(x=>x.AllottedId== allid).SingleOrDefault();
                dc.tblProjectAllotteds.Remove(data);
                dc.SaveChanges();
                return RedirectToAction("ProjectTeamAllocation",new { id = pid });
            }
        }
        [CkeckAllowedAccess]
        public ActionResult ProjectFinancials(string id)
        {
            ViewBag.ProjectId = id;

            using (HRMSConn dc=new HRMSConn())
            {
                double ProjectEstCost =0;
                double ProjectActualcost = 0;
                string ProjectStatus = "";

                string created = "";
                int projectId = Convert.ToInt32(id);
                var data = dc.tblProjectMasters.Where(x => x.ProjectId == projectId).SingleOrDefault();
                ViewBag.ProjectTitle = data.ProjectTitle;
                ViewBag.ProjectStatus = data.ProjectStatus;
                ViewBag.StartDate = Convert.ToDateTime(data.StartDate).ToShortDateString();
                ViewBag.EstEndDate = Convert.ToDateTime(data.EstEndDate).ToShortDateString();
                if(data.ActEndDate!=null)
                {
                    ViewBag.EndDate = Convert.ToDateTime(data.ActEndDate).ToShortDateString();
                }
                else
                {
                    ViewBag.EndDate = "";
                }
               
                int uid = Convert.ToInt32(data.Createdby);
                var dataCreated = dc.tblUserMasters.Where(x => x.UserId == uid).ToList();
                if(dataCreated.Count>0)
                {
                    created = dataCreated[0].FullName + " Empoyee ID: " + dataCreated[0].EmpId;
                }
                ViewBag.CreatedBy = created;
                ViewBag.CreatedOn = Convert.ToDateTime(data.CreatedOn).ToShortDateString();
                ViewBag.Notes = data.ProjectNotes;

                ///TIMESHEET ENTRIES///
                var AllProjectTimeLine = dc.tblTimeSheetManagers.Where(x => x.refTimeProjectId== projectId).OrderByDescending(x=>x.TimeId).ToList();              

                List<ProjectComments> CommentList = new List<ProjectComments>();
                for(int h=0;h< AllProjectTimeLine.Count;h++)
                {
                    int CommentsUserId = Convert.ToInt32(AllProjectTimeLine[h].refTimeEmpId);
                    var SingleUser = dc.tblUserMasters.Where(x => x.UserId == CommentsUserId).SingleOrDefault();
                    CommentList.Add(
                        new ProjectComments
                        {
                            Date = AllProjectTimeLine[h].TimeDate,
                            Hours = AllProjectTimeLine[h].TimeHours.ToString(),
                            Comments = AllProjectTimeLine[h].Comments.ToString(),
                            Employee = SingleUser.FullName + "--" + SingleUser.EmpId
                        }
                        );

                }

                ViewBag.TimeSheetAll = CommentList;

                //////////////////// TEAM MEMBERS /////////////////
                List<ProjectAdvanceTeam> TeamList = new List<ProjectAdvanceTeam>();

                var getTeam = dc.tblProjectAllotteds.Where(z => z.refProjectId == projectId).ToList();
                if(getTeam.Count>0)
                {
                  for(int i=0;i<getTeam.Count;i++)
                    {
                        try
                        {
                            

                            int userid = Convert.ToInt32(getTeam[i].refEmployeeId);
                            var DataHoursWorked = dc.tblTimeSheetManagers.Where(c => c.refTimeEmpId == userid && c.refTimeProjectId == projectId).ToList();
                            
                           double hoursWorkedOnproject = 0;

                            for(int j=0;j< DataHoursWorked.Count;j++)
                            {
                                hoursWorkedOnproject = hoursWorkedOnproject + Convert.ToDouble(DataHoursWorked[j].TimeHours);
                            }

                            hoursWorkedOnproject = Math.Round(hoursWorkedOnproject, 2);
                           
                            var DataUsers = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                            int desigId= Convert.ToInt32(DataUsers.DesignationId);
                            int deptId = Convert.ToInt32(DataUsers.DepartmentId);

                            double costAllowed = Math.Round((Convert.ToDouble(getTeam[i].AllowedHours) * Convert.ToDouble(getTeam[i].PerHourRate)), 2) ;
                            double ActualCostCal = Math.Round((Convert.ToDouble(hoursWorkedOnproject) * Convert.ToDouble(getTeam[i].PerHourRate)), 2);

                            ////PROFIT-LOSS CAL////
                            double AmoutDiffrent = 0;
                            string ResultStr = "";
                            if(ActualCostCal> costAllowed)
                            {
                                ///LOSS///
                                AmoutDiffrent = Math.Round((ActualCostCal - costAllowed),2);
                                ResultStr =  " Loss Of "+ AmoutDiffrent;
                            }
                            else
                            {
                                ///PROFIT
                                 AmoutDiffrent = Math.Round((costAllowed - ActualCostCal),2);
                                ResultStr =" Profit Of "+ AmoutDiffrent;

                            }
                            /////
                            var DataDesignations = dc.tblDesignationMasters.Where(x => x.DesignationId == desigId).SingleOrDefault();
                            var DataDepartmants = dc.tblDepartmentMasters.Where(x => x.DepartmentId == deptId).SingleOrDefault();
                            TeamList.Add(new ProjectAdvanceTeam
                            {
                                Name = DataUsers.FullName,
                                Designation = DataDesignations.DesignationName,
                                Department = DataDepartmants.DepartmentName,
                                HourRate = getTeam[i].PerHourRate.ToString(),
                                AllowedHours = getTeam[i].AllowedHours.ToString(),
                                AllowedCost = costAllowed.ToString(),
                                HoursWorked= hoursWorkedOnproject.ToString(),
                                ActualCost= ActualCostCal.ToString(),
                                Results= ResultStr
                            });
                            /////////////GLOBAL COSTS STS+ATS //////////
                            ProjectEstCost = Math.Round((ProjectEstCost + costAllowed), 2);
                            ProjectActualcost = Math.Round((ProjectActualcost + ActualCostCal), 2);
                            /////////////////////////
                        }
                        catch
                        {
                            ;
                        }
                       

                    }
                    ////

                    double ProjectCostDiff = 0;                  

                    if (ProjectActualcost > ProjectEstCost)
                    {
                        ///LOSS///
                        ProjectCostDiff = Math.Round((ProjectActualcost - ProjectEstCost), 2);
                        ProjectStatus = "Loss Of "+ ProjectCostDiff ;
                    }
                    else
                    {
                        ///PROFIT
                        ProjectCostDiff = Math.Round((ProjectEstCost - ProjectActualcost), 2);
                        ProjectStatus = "Profit Of " + ProjectCostDiff;

                    }
                }

                ViewBag.ProjectEstCost = ProjectEstCost.ToString();
                ViewBag.ProjectAC = ProjectActualcost.ToString();
                ViewBag.FinancialSts = ProjectStatus;
                ViewBag.TeamStats = TeamList;
                ////////////////
                return View();
            }
           
        }
    }
}