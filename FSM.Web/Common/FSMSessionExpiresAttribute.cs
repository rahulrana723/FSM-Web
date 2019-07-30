using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FSM.Web.Common
{
    public class FSMSessionExpiresAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookie1 = new HttpCookie("LoginPath1", filterContext.HttpContext.Request.Url.OriginalString);
            var Url = cookie1.Value;
            var request = new HttpRequest(null, Url, "");
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var values = routeData.Values;

            var controller = values["controller"]; //aaaa
            var action = values["action"]; //bbb

            if (HttpContext.Current.Session.Contents.Count == 0 &&
                filterContext.RouteData.Values["controller"].ToString() != "Account")
            {
                // saving cookie on session time out
                if ((controller).ToString() != "Error")
                {
                    HttpCookie cookie = new HttpCookie("LoginPath", filterContext.HttpContext.Request.Url.OriginalString);
                    filterContext.HttpContext.Response.Cookies.Add(cookie);
                }

                // clear authentication cookie
                HttpCookie formcookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                formcookie.Expires = DateTime.Now.AddYears(-10);
                HttpContext.Current.Response.Cookies.Add(formcookie);

                // clear session cookie
                SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
                HttpCookie sessioncookie = new HttpCookie(sessionStateSection.CookieName);
                sessioncookie.Expires = DateTime.Now.AddYears(-10);
                HttpContext.Current.Response.Cookies.Add(sessioncookie);

                UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                string url = urlHelper.Action("Login", "Account", new { area = "" });

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JavaScriptResult()
                    {
                        Script = "window.location = '" + url + "';"
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult(url);
                }
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}