using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PureHRMS.Models;
namespace PureHRMS.Controllers
{
    public class BasicController : Controller
    {
        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult EmployeeNoticesInsert()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataset = dc.tblNoticeMasters.OrderByDescending(x => x.NoticeId).ToList();
                ViewBag.Mylists = dataset;
                return View();
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EmployeeNoticesInsert")]
        public ActionResult EmployeeNoticesInsertPost(string noticetilte, string noticeDescp)
        {
            try
            {
                using (HRMSConn dc = new HRMSConn())
                {
                   
                    if (noticetilte == "" || noticetilte == null || noticeDescp == "" || noticeDescp == null)
                    {
                        //////////////////

                        ViewBag.FoundError = "Please enter title & description.";
                        var dataset = dc.tblNoticeMasters.OrderByDescending(x => x.NoticeId).ToList();
                        ViewBag.Mylists = dataset;
                        ///////////////////
                        return View();
                    }
                    else
                    {
                        tblNoticeMaster nm = new tblNoticeMaster();
                        nm.NoticeTitle = noticetilte;
                        nm.NoticeDescription = noticeDescp;
                        dc.tblNoticeMasters.Add(nm);
                        dc.SaveChanges();
                        ModelState.Clear();
                        return RedirectToAction("EmployeeNoticesInsert");
                    }
                
                }
            }
            catch
            {
                ViewBag.FoundError = "Sorry, some error occured.";
                return View();
            }


        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeNoticesUpdate(int id, string noticetilteEdit, string noticeDescpEdit)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataset = dc.tblNoticeMasters.Where(x => x.NoticeId == id).SingleOrDefault();
                dataset.NoticeTitle = noticetilteEdit;
                dataset.NoticeDescription = noticeDescpEdit;
                dc.SaveChanges();
                //////////////////
                var datasetnew = dc.tblNoticeMasters.OrderByDescending(x => x.NoticeId).ToList();
                ViewBag.Mylists = datasetnew;
                ///////////////////
                return RedirectToAction("EmployeeNoticesInsert");
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeNoticesDelete(int id)
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var dataset = dc.tblNoticeMasters.Where(x => x.NoticeId == id).SingleOrDefault();
                dc.tblNoticeMasters.Remove(dataset);
                dc.SaveChanges();
                //////////////////
                var datasetnew = dc.tblNoticeMasters.OrderByDescending(x => x.NoticeId).ToList();
                ViewBag.Mylists = datasetnew;
                ///////////////////
                return RedirectToAction("EmployeeNoticesInsert");
            }
        }

        [CkeckAllowedAccess]
        public ActionResult ViewUsersForPermissions()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblUserMasters.OrderByDescending(z=>z.UserId).Where(z=>z.Active==true) .ToList();
                ViewBag.Userlist = data;
                return View();
            }
        }



        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult PermissionsView(string userid)
        {
            ViewBag.UserIdentifier = userid;
            int id = Convert.ToInt32(userid);
            using (HRMSConn dc = new HRMSConn())
            {
             
                var data = dc.tblUserMasters.Where(x => x.UserId == id).SingleOrDefault();
                ViewBag.UserName = data.FullName.ToString();
                ViewBag.EmpId = data.EmpId;
                return View();
            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PermissionsViewPost(string id,string userid)
        {
            
            int uid = Convert.ToInt32(userid);
            int menuid = Convert.ToInt32(id);
            using (HRMSConn dc = new HRMSConn())
            {


                ////IF WEB MASTER DON'T CHANGE ANYTHING ////

                var WebMaster = dc.tblUserMasters.Where(z => z.UserId == uid).SingleOrDefault();
                if (WebMaster!=null)
                {
                    if(WebMaster.IsWebMaster==true)
                    {
                        // DON'T ALLOW UPDATE OF WEB MASTER

                        return RedirectToAction("PermissionsView", new { userid = uid });

                    }
                    else
                    {
                        // ALLOW PROCESSING OF BELOW CODE
                    }
                }

               //////////////////////////////

                    var data = dc.tblMenuMasters.Where(x => x.MenuId == menuid).SingleOrDefault();

                var permisssionsTblmid = dc.tblPermissionMasters.Where(z => z.refMenuId == menuid && z.refUserId== uid).ToList();
                if (permisssionsTblmid.Count>0)
                {
                    if (permisssionsTblmid[0].Allowed == true)
                    {
                        permisssionsTblmid[0].Allowed = false;
                    }
                    else
                    {
                        permisssionsTblmid[0].Allowed = true;
                    }
                    dc.SaveChanges();
                }
                else
                {
                    int parentid= Convert.ToInt32(data.ParentId);
                    tblPermissionMaster pm = new tblPermissionMaster();
                    pm.refMenuId = menuid;
                    pm.refUserId = uid;
                    pm.refMenuParentID = parentid;
                    pm.Allowed = true;
                    dc.tblPermissionMasters.Add(pm);
                    dc.SaveChanges();
                }
             


                var getchildsifany = dc.tblMenuMasters.Where(y => y.ParentId == menuid).ToList();
                if(getchildsifany.Count>0)
                {
                    for(int d=0;d<getchildsifany.Count;d++)
                    {
                        int menuidchild = Convert.ToInt32(getchildsifany[d].MenuId);
                        var permisssionsTblmidChild = dc.tblPermissionMasters.Where(z => z.refMenuId == menuidchild && z.refUserId == uid).ToList();
                        if (permisssionsTblmidChild.Count > 0)
                        {
                            if(permisssionsTblmidChild[0].Allowed ==true)
                            {
                                permisssionsTblmidChild[0].Allowed = false;
                            }
                            else
                            {
                                permisssionsTblmidChild[0].Allowed = true;
                            }
                            
                            dc.SaveChanges();
                        }
                        else
                        {
                            int parentidchild = Convert.ToInt32(getchildsifany[d].ParentId);
                            tblPermissionMaster pmchild = new tblPermissionMaster();
                            pmchild.refMenuId = menuidchild;
                            pmchild.refUserId = uid;
                            pmchild.refMenuParentID = parentidchild;
                            pmchild.Allowed = true;
                            dc.tblPermissionMasters.Add(pmchild);
                            dc.SaveChanges();
                        }
                    }
                 

                }
           
                return RedirectToAction("PermissionsView",new { userid = uid });
            }
        }

        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult PolicyUpdate()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblPolicyMasters.Take(1).SingleOrDefault();
                return View(data);

            }
        }
        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("PolicyUpdate")]
        [ValidateAntiForgeryToken]
        public ActionResult PolicyUpdatePost()
        {
            using (HRMSConn dc=new HRMSConn())
            {
                var data = dc.tblPolicyMasters.Take(1).SingleOrDefault();
                TryUpdateModel(data);
                data.PolicyText =Microsoft.Security.Application.Sanitizer.GetSafeHtml(data.PolicyText);
                data.LastModified = System.DateTime.Now;
                data.ModifiedBy =Convert.ToInt32(Session["UserIdentfier"].ToString());
                dc.SaveChanges();
                ViewBag.FoundError = "Policies Updated Sucessfully";
                return View(data);

            }
        }


        [CkeckAllowedAccess]
        public ActionResult PolicyDisplay()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblPolicyMasters.Take(1).SingleOrDefault();
                return View(data);

            }
        }

        [CkeckAllowedAccess]
        [HttpGet]
        public ActionResult ContactUpdate()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblCommonContacts.Take(1).SingleOrDefault();
                return View(data);

            }
        }

        [CkeckAllowedAccess]
        [HttpPost]
        [ActionName("ContactUpdate")]
        [ValidateAntiForgeryToken]        
        public ActionResult ContactUpdatePost()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblCommonContacts.Take(1).SingleOrDefault();
                TryUpdateModel(data);
                data.ContactText = Microsoft.Security.Application.Sanitizer.GetSafeHtml(data.ContactText);
                data.LastUpdated = System.DateTime.Now;
                data.UpdatedBy = Convert.ToInt32(Session["UserIdentfier"].ToString());
                dc.SaveChanges();
                ViewBag.FoundError = "Contacts Updated Sucessfully";
                return View(data);

            }
        }

        [CkeckAllowedAccess]
        public ActionResult ContactDisplay()
        {
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblCommonContacts.Take(1).SingleOrDefault();
                return View(data);

            }
        }
        [CkeckAllowedAccess]
        public ActionResult EmployeeMyProfile()
        {
            int userid = Convert.ToInt32(Session["UserIdentfier"].ToString());
            using (HRMSConn dc = new HRMSConn())
            {
                var data = dc.tblUserMasters.Where(x => x.UserId == userid).SingleOrDefault();
                int desigId = Convert.ToInt32(data.DesignationId);
                int deprtId = Convert.ToInt32(data.DepartmentId);
                var dataDesig = dc.tblDesignationMasters.Where(x => x.DesignationId == desigId).SingleOrDefault();
                if(dataDesig!=null)
                {
                    ViewBag.Desig = dataDesig.DesignationName;
                }
                var dataDept = dc.tblDepartmentMasters.Where(x => x.DepartmentId == deprtId).SingleOrDefault();
                if (dataDept != null)
                {
                    ViewBag.Dept = dataDept.DepartmentName;
                }

             
                return View(data);

            }
        }
    }
}