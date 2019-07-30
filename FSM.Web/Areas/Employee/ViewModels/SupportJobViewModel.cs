using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class SupportJobViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public Nullable<Constant.JobType> JobType { get; set; }
        public Nullable<DateTime> BookedDate { get; set; }
        public Guid SupportJobId { get; set; }
    }
}