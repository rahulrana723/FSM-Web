using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class InvoiceItems
    {
        [Key]
        public Guid Id { get; set; }

        public string Items { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<Decimal> Price { get; set; }
        public Nullable<Decimal> AmountAUD { get; set; }
        public Nullable<Decimal> AmountGSTAUD { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
