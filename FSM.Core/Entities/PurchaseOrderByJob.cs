using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class PurchaseOrderByJob
    {
        public PurchaseOrderByJob()
        {
            this.PurchaseorderItemJobs = new HashSet<PurchaseorderItemJob>();
        }
        [Key]
        public Guid ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseOrderNo { get; set; }
        [ForeignKey("Supplier")]
        public Nullable<Guid> SupplierID { get; set; }
        public string Description { get; set; }
        public Decimal Cost { get; set; }
        [ForeignKey("Jobs")]
        public Nullable<Guid> JobID { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable<bool> IsApprove { get; set; }
        public Nullable<Guid> ApprovedBy { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual ICollection<PurchaseorderItemJob> PurchaseorderItemJobs { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Jobs Jobs { get; set; }
        public Nullable<bool> IsSyncedToMyob { get; set; }
    }
}
