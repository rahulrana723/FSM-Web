using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
  public  class OTRWSearchViewmodel
    {
        [Key]
        public Guid ID { get; set; }
        public string Detail { get; set; }
        public string Type { get; set; }
        public Nullable<int> Typeid { get; set; }
        public string Label { get; set;  }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<Guid> StockID { get; set; }
        public string UnitMeasure { get; set; }
        public string OTRWName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<Guid> OTRWID { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> JobId { get; set; }
    }
}
