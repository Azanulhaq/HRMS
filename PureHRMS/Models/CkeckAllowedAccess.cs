using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PureHRMS.Models
{
    public class CkeckAllowedAccess : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (AccessAllowedCheck(actionName, controllerName))
            {
                //HAVE PERMISSTION OF THIS  ACTION AND CONTROLLER
                ;
            }
            else
            {
                //DONT'T HAVE PERMISSTION OF THIS  ACTION AND CONTROLLER, SO SEND TO DASHBOARD
                if (HttpContext.Current.Session["UserIdentfier"] == null)
                {
                    filterContext.Result = new RedirectResult("/Login/SecureUserLogin/");
                }
                    
            }
        }
        private bool AccessAllowedCheck(string action, string controller)
        {
            // CHECK ACTION AND CONTROLLER FOR SPECFIC USER ID 
            // IF MATCHES RETURNS TRUE ELSE FALSE
            bool getaccessvar = false;
            if (HttpContext.Current.Session["UserIdentfier"] != null)
            {
                using (HRMSConn dc = new HRMSConn())
                {
                    int useridset = Convert.ToInt32(HttpContext.Current.Session["UserIdentfier"]);
                    var data = dc.tblPermissionMasters.Where(x => x.refUserId == useridset && x.Allowed==true).ToList();
                    for(int i=0;i<data.Count;i++)
                    {
                        int menuallowed = Convert.ToInt32(data[i].refMenuId);
                        var check = dc.tblMenuRelatedActions.Where(y => y.refMenuId == menuallowed).ToList();
                        if(check.Count>0)
                        {
                            for(int j=0;j<check.Count;j++)
                            {
                               if(check[j].ActionName == action && check[j].ControllerName== controller)
                                {
                                    getaccessvar= true;
                                    goto exitloops;
                                    
                                }
                            }
                        }
                    }

                }
                  
            }
            else
            {
                getaccessvar = false; ;
            }
            exitloops:
            return getaccessvar;
        }
    }
}