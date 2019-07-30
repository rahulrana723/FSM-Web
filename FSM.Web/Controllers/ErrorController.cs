using FSM.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace FSM.Web.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index()
        {
            HttpContext.Request.GetOwinContext().Authentication.SignOut();
            HttpContext.Session.Clear();

            // clear authentication cookie
            HttpCookie formcookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            formcookie.Expires = DateTime.Now.AddYears(-10);
            HttpContext.Response.Cookies.Add(formcookie);

            // clear session cookie
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie sessioncookie = new HttpCookie(sessionStateSection.CookieName);
            sessioncookie.Expires = DateTime.Now.AddYears(-10);
            HttpContext.Response.Cookies.Add(sessioncookie);

            return View();
        }

        public ActionResult UnAuthorize()
        {
            return View();
        }
    }
}