using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class RescheduleViewModel
    {
        public string ResourceId { get; set; }

        [DisplayName("Reschedule date")]
        public string CalenderSelectDate { get; set; }
        [DisplayName("Assign OTRW")]
        public Nullable<Guid> AssignTo { get; set; }
        public List<SelectListItem> OTRWList { get; set; }
    }
}