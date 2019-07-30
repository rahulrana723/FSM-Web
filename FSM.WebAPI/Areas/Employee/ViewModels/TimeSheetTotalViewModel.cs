using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class TimeSheetTotalViewModel
    {
        public  string JobDate { get; set; }
        public string Job { get; set; }
        public string Lunch { get; set; }
        public string Personal { get; set; }
        public string Travelling { get; set; }
    }
}