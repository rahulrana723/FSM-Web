using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class StockDetailViewModel
    {
        public Guid ID { get; set; }
        public string Label { get; set; }
        public string Material { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Available { get; set; }
    }
}