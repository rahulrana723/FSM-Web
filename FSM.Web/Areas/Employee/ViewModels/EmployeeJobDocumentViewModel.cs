using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeJobDocumentViewModel
    {
        [Key]
        public Nullable<Guid> Id { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public string DocName { get; set; }
        public string SaveDocName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifyDate { get; set; }
        public Nullable<Guid> ModifyBy { get; set; }
    }
}