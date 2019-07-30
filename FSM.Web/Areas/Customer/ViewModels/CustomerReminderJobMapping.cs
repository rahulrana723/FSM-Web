using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerReminderJobMapping
    {

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