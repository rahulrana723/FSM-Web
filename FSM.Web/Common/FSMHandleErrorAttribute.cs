using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace FSM.Web.Common
{
    public class FSMHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            // writing exceptions
            string path = HttpContext.Current.Server.MapPath("~/Exceptions/Exception_" +
                          DateTime.Now.ToString("ddMMyyyy") + ".txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine("Message :" + context.Exception.Message + "<br/>" + Environment.NewLine + "StackTrace :"
                    + context.Exception.StackTrace + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }

            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string url = urlHelper.Action("Index", "Error", new { area = "" });

            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.Result = new JavaScriptResult()
                {
                    Script = "window.location = '" + url + "';"
                };
            }
            else
            {
                HttpContext.Current.Request.GetOwinContext().Authentication.SignOut();
                HttpContext.Current.Session.Clear();

                // clear authentication cookie
                HttpCookie formcookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                formcookie.Expires = DateTime.Now.AddYears(-10);
                HttpContext.Current.Response.Cookies.Add(formcookie);

                // clear session cookie
                SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
                HttpCookie sessioncookie = new HttpCookie(sessionStateSection.CookieName);
                sessioncookie.Expires = DateTime.Now.AddYears(-10);
                HttpContext.Current.Response.Cookies.Add(sessioncookie);

                context.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml",
                };
            }
        }
    }
}