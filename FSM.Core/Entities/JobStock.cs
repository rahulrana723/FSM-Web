using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class JobStock
    {
        [Key]
        public Guid ID { get; set; }
        [ForeignKey("Stock")]
        public Guid StockID { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        //public virtual Stock Stock { get; set; }
        public virtual Stock Stock { get; set; }
        public Guid JobId { get; set; }
        public string AssignedFrom { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
