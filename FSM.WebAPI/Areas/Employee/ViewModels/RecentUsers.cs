using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class RecentUsers
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int UnReadMsg { get; set; }
        public Nullable<DateTime> Date { get; set; }
    }
}