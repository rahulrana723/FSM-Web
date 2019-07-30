using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class UserMessageViewModel
    {
        [Key]
        public Guid ID { get; set; }

        [ForeignKey("UserMessageThread")]
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
        public virtual UserMessageThread UserMessageThread { get; set; }
    }
}