using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class PurchaseOrderVM
    {
        public Guid JobId { get; set; }
        public Nullable<int> JobKey { get; set; }
        public Nullable<int> JobType { get; set; }
    }
}