using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class ActionByRoleViewModel
    {
        public Guid RoleParentActionMappingId { get; set; }
        public Guid ParentActionMasterId { get; set; }
        public string ParentActionMasterActionResult { get; set; }
        public string ParentActionMasterAction { get; set; }
        public string ActionMasterActionResult { get; set; }
        public string ActionMasterAction { get; set; }
    }
}
