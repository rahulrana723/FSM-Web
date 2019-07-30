using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class OTRWStock
    {

        [Key]
        public Guid ID { get; set; }
        [ForeignKey("Stock")]
        public Nullable<Guid> StockID { get; set; }
        public string UnitMeasure { get; set; }
        public int? Quantity { get; set; }
        public Nullable<Guid> OTRWID { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual Stock Stock { get; set; }
    }
}
