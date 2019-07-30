using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
  public  class PurchaseorderItemsStock
    {
        [Key]
        public Guid ID { get; set; }
        public Guid StockID { get; set; }
        [ForeignKey("PurchaseOrderITem")]
        public Guid PurchaseOrderID { get; set; }
        public string UnitOfMeasure { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<Decimal> Price { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual PurchaseOrderByStock PurchaseOrderITem { get; set; }
    }
}
