using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSM.Core.Entities
{
    public class RoleParentActionMapping
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("AspNetRoles")]
        public Nullable<Guid> RoleId { get; set; }
        [ForeignKey("ParentAction_Master")]
        public Nullable<Guid> ParentActionMasterId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual AspNetRoles AspNetRoles { get; set; }
        public virtual ParentAction_Master ParentAction_Master { get; set; }
    }
}
