using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class RoleManagementViewModel
    {
        public List<AspNetRolesViewModel> aspNetRolesViewModelList { get; set; }
        public RoleManagementSearchViewModel roleManagementSearchViewModel { get; set; }
    }
}