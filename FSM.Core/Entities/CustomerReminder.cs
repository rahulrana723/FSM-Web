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
    public class CustomerReminder
    {
        [Key]
        public Guid ReminderId { get; set; }
        [DisplayName("Job Id")]
        public string JobId { get; set; }
        public string Note { get; set; }
        public int FromEmail { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModeifiedBy { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public string CustomerId { get; set; }
        public Nullable<DateTime> ReContactDate { get; set; }
        public Nullable<DateTime> ReminderDate { get; set; }
        public Nullable<bool> HasSMS { get; set; }
        public Nullable<bool> HasEmail { get; set; }
        public Nullable<int> MessageTypeId { get; set; }
        public Nullable<int> TemplateMessageId { get; set; }
        public Nullable<bool> IsDelete { get; set; }

    }
}
