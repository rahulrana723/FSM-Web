using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class JCL
    {
        [Key]
        public Guid JCLId { get; set; }
        public Nullable<bool> HasBonus { get; set; }
        public Nullable<bool> ApplyBonus { get; set; }
        public string ItemName { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid BonusPerItem; Max 8 digits")]
        public Nullable<decimal> BonusPerItem { get; set; }
        public Nullable<int> Category { get; set; }
        public string Material { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid BonusMinimum; Max 8 digits")]
        public Nullable<decimal> BonusMinimum { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid Price; Max 8 digits")]
        public Nullable<decimal> Price { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid Per; Max 8 digits")]
        public Nullable<decimal> Per { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid GroupsOf; Max 8 digits")]
        public Nullable<decimal> GroupsOf { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid Minimum; Max 8 digits")]
        public Nullable<decimal> Minimum { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid DefaultQty; Max 8 digits")]
        public Nullable<int> DefaultQty { get; set; }
        public string Description { get; set; }
        public string GeneralQuestion { get; set; }
        public string DiagramQuestion { get; set; }
        public Nullable<bool> Diagram { get; set; }
        public Nullable<decimal> CallOutFree { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}
