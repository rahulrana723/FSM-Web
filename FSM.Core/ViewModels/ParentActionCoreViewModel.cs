using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class ParentActionCoreViewModel
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public Nullable<Guid> RoleId { get; set; }
        public int IsSelected { get; set; }
        public Nullable<Guid> ModuleId { get; set; }
        public string ModuleName { get; set; }
    }
}
