using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class PurchaseorderItemJob
    {
        [Key]
        public Guid ID { get; set; }
        [ForeignKey("PurchaseOrderByJob")]
        public Nullable<Guid> PurchaseOrderID { get; set; }
        public Nullable<Guid> StockID { get; set; }
        public string PurchaseItem { get; set; }
        public string UnitOfMeasure { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual PurchaseOrderByJob PurchaseOrderByJob { get; set; }
        public virtual Stock Stock { get; set; }
        public Nullable<bool> IsSyncedToMyob { get; set; }
    }
}
