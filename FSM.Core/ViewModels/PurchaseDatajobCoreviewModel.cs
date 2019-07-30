using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{public class PurchaseDatajobCoreviewModel
    {
        public string purchaseId { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string StockId { get; set; }
        public string PurchaseItem { get; set; }
        public string StockLabel { get; set; }
        public string UnitMeasure { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Cost { get; set; }
        public string JobId { get; set; }
        public string Description { get; set; }
        public string Supplierid { get; set; }
        public string AvailableQuatity { get; set; }
    }
}
