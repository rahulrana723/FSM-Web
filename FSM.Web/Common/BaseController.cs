using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Common
{
    public class BaseController : Controller
    {
        public List<string> GetUserRoles
        {
            get
            {
                return (List<string>)Session["FSMUserRoles"];
            }
            set { Session["FSMUserRoles"] = value; }
        }

        public string GetUserId
        {
            get
            {
                return Session["FSMUserId"].ToString();
            }
            set { Session["FSMUserId"] = value; }
        }

        public List<string> GetUserRoleId
        {
            get
            {
                return (List<string>)Session["FSMUserRoleId"];
            }
            set { Session["FSMUserRoleId"] = value; }
        }

        public string GetUserModule
        {
            get
            {
                return Session["FSMUserModule"] != null ? Session["FSMUserModule"].ToString() : string.Empty;
            }
            set { Session["FSMUserModule"] = value; }
        }

        public List<string> GetModuleListByRole
        {
            get
            {
                return Session["FSMUserModuleList"] != null ? (List<string>)Session["FSMUserModuleList"] : new List<string>();
            }
            set { Session["FSMUserModuleList"] = value; }
        }

        public string UserNotificationCount
        {
            get
            {
                return Session["FSMNotificationCount"] != null ? Session["FSMNotificationCount"].ToString() : string.Empty;
            }
            set { Session["FSMNotificationCount"] = value; }
        }

        public string GetUserName
        {
            get
            {
                return Session["FSMUserName"] != null ? Session["FSMUserName"].ToString() : string.Empty;
            }
            set { Session["FSMUserName"] = value; }
        }
    }
}