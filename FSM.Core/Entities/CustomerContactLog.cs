using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class CustomerContactLog
    {
        [Key]
        public Guid CustomerContactId { get; set; }
        [ForeignKey("CustomerGeneralInfo")]
        public Guid CustomerGeneralInfoId { get; set; }
        [DisplayName("Customer Id")]
        public string CustomerId { get; set; }
        [DisplayName("Job Id")]
        public string JobId { get; set; }
        [DisplayName("Log Date")]
        public Nullable<DateTime> LogDate { get; set; }
        [DisplayName("ReContact Date")]
        public Nullable<DateTime> ReContactDate { get; set; }
        public string Note { get; set; }
        public Nullable<bool> IsReminder { get; set; }
        public Nullable<bool> IsScheduler { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
    }
}
