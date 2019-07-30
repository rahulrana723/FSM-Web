using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class GetContactLogListViewModel
    {
        public Guid ContactLogId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public string JobId { get; set; }
        public int? JobNo { get; set; }
        public Nullable<DateTime> LogDate { get; set; }
        public Nullable<DateTime> RecontactDate { get; set; }
        public string ContactLogNote { get; set; }

    }
}