using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class UpdateOTRWNotesViewModel
    {
        public string estimated_hour { get; set; }
        public string Id { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }

        public string OTRWjobNotes { get; set; }
    }
}