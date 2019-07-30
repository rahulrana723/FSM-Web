using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
   public class JobStockListCoreViewModel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> Jobid { get; set; }
        public Guid StockId { get; set; }
        public string Label { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}
