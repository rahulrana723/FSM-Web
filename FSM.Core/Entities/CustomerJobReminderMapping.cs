using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
  public  class CustomerJobReminderMapping
    {
        [Key]
        public Guid ID { get; set; }

        public Nullable<Guid> ReminderId { get; set; }

        public Nullable<Guid> Jobid { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }

        public Nullable<DateTime> ModifiedDate { get; set; }

        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
    }
}
