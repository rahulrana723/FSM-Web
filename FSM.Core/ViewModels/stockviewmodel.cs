using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class StockViewModel
    {
        public Guid ID { get; set; }
        public string Label { get; set; }
        public string Material { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Decimal> Cost { get; set; }
        public Nullable<Guid> EmpId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> Available { get; set; }
        public Nullable<int> OTRW { get; set; }
    }
}
