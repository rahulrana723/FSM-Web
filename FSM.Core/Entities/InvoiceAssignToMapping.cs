using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class InvoiceAssignToMapping
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Jobs")]
        public Guid JobId { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual Jobs Jobs { get; set; }
    }
}
