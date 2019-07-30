using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class RoleModuleMapping
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("AspNetRoles")]
        public Nullable<Guid> RoleId { get; set; }
        [ForeignKey("Module_Master")]
        public Nullable<Guid> ModuleId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual Module_Master Module_Master { get; set; }
        public virtual AspNetRoles AspNetRoles { get; set; }
    }
}
