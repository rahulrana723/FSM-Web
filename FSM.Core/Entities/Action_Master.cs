using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class Action_Master
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Module_Master")]
        public Nullable<Guid> ModuleId { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public string Controller { get; set; }
        [ForeignKey("ParentAction_Master")]
        public Nullable<Guid> ParentActionMasterId { get; set; }
        public virtual Module_Master Module_Master { get; set; }
        public virtual ParentAction_Master ParentAction_Master { get; set; }
    }
}
