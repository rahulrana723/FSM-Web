using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class InvoiceAssignToMappingViewModel
    {
        public Guid Id { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public Guid InvoiceId { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
       
    }
}