using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace FSM.Web.Common

{
    public class RolePermissionsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var ActionName = filterContext.ActionDescriptor.ActionName;
            var ctrl = (BaseController)filterContext.Controller;
            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["Module"]))
            {
                ctrl.GetUserModule = filterContext.HttpContext.Request.QueryString["Module"];
            }
            string RoleId = ctrl.GetUserRoleId != null ? ctrl.GetUserRoleId[0] : string.Empty;
            string RoleName = ctrl.GetUserRoles != null ? ctrl.GetUserRoles[0] : string.Empty;
            var ModuleName = !string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["Module"]) ?
                              filterContext.HttpContext.Request.QueryString["Module"] : ctrl.GetUserModule;

            if (!ControllerName.Equals("Account", StringComparison.OrdinalIgnoreCase) &&
                !ControllerName.Equals("Manage", StringComparison.OrdinalIgnoreCase) &&
                !ControllerName.Equals("Dashboard", StringComparison.OrdinalIgnoreCase) &&
                !RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                !(ControllerName == "Employee" && (ActionName == "EmployeeProfile"||ActionName== "UploadFiles")))
            {
                var RoleManagement = new FMSRoleManagement(ModuleName, ControllerName, ActionName, Guid.Parse(RoleId));

                if (RoleManagement.HasModulePermission())
                {
                    if (!RoleManagement.HasActionPermission())
                    {
                        UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        string url = urlHelper.Action("UnAuthorize", "Error", new { area = "" });
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JavaScriptResult()
                            {
                                Script = "window.location = '" + url + "';"
                            };
                        }
                        else
                        {
                            filterContext.Result = new ViewResult
                            {
                                ViewName = "~/Views/Shared/UnAuthorize.cshtml",
                            };
                        }

                    }
                }
                else
                {
                    UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    string url = urlHelper.Action("UnAuthorize", "Error", new { area = "" });
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JavaScriptResult()
                        {
                            Script = "window.location = '" + url + "';"
                        };
                    }
                    else
                    {
                        filterContext.Result = new ViewResult
                        {
                            ViewName = "~/Views/Shared/UnAuthorize.cshtml",
                        };
                    }
                }

            }

            base.OnActionExecuting(filterContext);
        }
    }
}