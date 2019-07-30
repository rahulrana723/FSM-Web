using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class AssetManagementSearchViewModel
    {
        public string searchkeyword { get; set; }
        public Constant.AssetAssignStatus AssetAssignStatus { get; set; }
        public int PageSize { get; set; }
    }
}