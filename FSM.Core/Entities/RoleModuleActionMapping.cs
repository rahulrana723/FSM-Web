using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class RoleModuleActionMapping
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("RoleModuleMapping")]
        public Nullable<Guid> RoleId { get; set; }
        [ForeignKey("Action_Master")]
        public Nullable<Guid> ActionMasterId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual RoleModuleMapping RoleModuleMapping { get; set; }
        public virtual Action_Master Action_Master { get; set; }
    }
}
