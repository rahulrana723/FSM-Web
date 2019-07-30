using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class InvoiceStockJCLItemCoreViewModel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> Jobid { get; set; }
        public Guid StockId { get; set; }
        public string Items { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> AmountAUD { get; set; }
        public Nullable<decimal> AmountGSTAUD { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
