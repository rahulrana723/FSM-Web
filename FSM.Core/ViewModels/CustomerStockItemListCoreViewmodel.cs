using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class CustomerStockItemListCoreViewmodel
    {
        public Guid Id { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<Decimal> Amount { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
