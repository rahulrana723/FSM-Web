using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class ScheduleReminder
    {
        public Guid Id { get; set; }
        public Nullable<Guid> CustomerContactLogId { get; set; }
        public string Email { get; set; }
        public string EmailTemplate { get; set; }
        public string Phone { get; set; }
        public string PhoneTemplate { get; set; }
        public Nullable<DateTime> ScheduleDate { get; set; }
        public bool Schedule { get; set; }
        public int? FromEmail { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}
