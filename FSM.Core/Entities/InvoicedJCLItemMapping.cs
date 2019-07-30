using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class InvoicedJCLItemMapping
    {
        [Key]
        public Guid ID { get; set; }
        public Nullable<Guid> JCLItemID { get; set; }
        public Nullable<Guid> ColorID { get; set; }
        public Nullable<Guid> SizeID { get; set; }
        public Nullable<Guid> ProductStyleID { get; set; }
        public Nullable<Guid> ExtraID { get; set; }
        public Nullable<Guid> JobID { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Decimal Price { get; set; }
        public Decimal TotalPrice { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime>CreateDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }

        public Nullable<int> OrderNo { get; set; }
    }
}
