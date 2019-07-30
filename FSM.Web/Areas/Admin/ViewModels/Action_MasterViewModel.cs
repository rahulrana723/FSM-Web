using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class Action_MasterViewModel
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public Nullable<Guid> RoleId { get; set; }
        public Nullable<Guid> ActionMasterId { get; set; }
        public bool IsSelected { get; set; }
        public string ModuleName { get; set; }
    }
}