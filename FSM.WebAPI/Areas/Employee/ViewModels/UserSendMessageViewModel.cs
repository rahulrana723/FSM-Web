using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class UserSendMessageViewModel
    {
        public Guid ID { get; set; }
        public Nullable<Guid> MessageThreadID { get; set; }
        public Nullable<Guid> From_Id { get; set; }
        public Nullable<Guid> To_Id { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public Nullable<Boolean> IsMessageRead { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}