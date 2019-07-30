using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class ManageRolesViewModel
    {
        public List<Module_MasterViewModel> ModulesList { get; set; }
        public List<ParentAction_MasterViewModel> ActionsList { get; set; }
        public string RoleId { get; set; }

    }
}