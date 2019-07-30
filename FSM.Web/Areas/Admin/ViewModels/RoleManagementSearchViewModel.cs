using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class RoleManagementSearchViewModel
    {
        public int PageSize { get; set; }
        public string searchkeyword { get; set; }
    }
}