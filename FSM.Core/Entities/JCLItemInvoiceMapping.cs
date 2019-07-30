using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class JCLItemInvoiceMapping
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("JCL")]
        public Guid JCLId { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual JCL JCL { get; set; }
    }
}
