using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Customer.ViewModels
{
    public class OTRWStockListViewModel
    {
        public Guid Id { get; set; }
        public Nullable<Guid> StockId { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string StockName { get; set; }
    }
}