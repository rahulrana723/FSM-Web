using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class JobDocuments
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Jobs")]
        public Nullable<Guid> JobId { get; set; }
        public string DocName { get; set; }
        public string SaveDocName { get; set; }
        public string DocType { get; set; }
        public Jobs Jobs { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifyDate { get; set; }
        public Nullable<Guid> ModifyBy { get; set; }
    }
}
