using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class AssetManagement
    {
        [Key]
        public Guid AssetManageID { get; set; }
        public Nullable<int> Type { get; set; }
        public string Identifier { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> DateAssigned { get; set; }
        public Nullable<Guid> AssignedTo { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}
