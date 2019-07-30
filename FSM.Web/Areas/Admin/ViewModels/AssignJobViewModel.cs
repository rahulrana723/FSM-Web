using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class AssignJobViewModel
    {
        public Guid JobId { get; set; }
        public string AssignedDate { get; set; }
        public Nullable<Guid> AssignedTo { get; set; }
        public Nullable<Guid> CurrentResourceId { get; set; }
        public List<SelectListItem> OTRWList { get; set; }
        [DisplayName("AssignedTo")]
        public Nullable<Guid> Assigned_To { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}