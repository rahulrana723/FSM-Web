using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JobStockListViewmodel
    {
        public Guid Id { get; set; }
        public Guid StockId { get; set; }
        public string StockName { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Guid JobId { get; set; }
    }
}