using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
  public  class PurchaseOrderByStockCoreViewModel
    {
        public Guid ID { get; set; }
        public Guid supplierID { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifedDate { get; set; }
        public Nullable<int> PurchaseOrderNo { get; set; }
        public string Name { get; set; }
        public string StockLabel { get; set; }
        public string PurchaseOrderNoformated { get; set; }
    }
}
