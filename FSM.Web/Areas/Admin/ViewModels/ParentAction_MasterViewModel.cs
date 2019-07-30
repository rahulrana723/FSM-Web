using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class ParentAction_MasterViewModel
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public Nullable<Guid> RoleId { get; set; }
        public bool IsSelected { get; set; }
        public Nullable<Guid> ModuleId { get; set; }
        public string ModuleName { get; set; }
    }
}