using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class UserMessageViewModel
    {
        public Nullable<Guid> LoggedInUser { get; set; }
        public Nullable<Guid> ToMessageUser { get; set; }
        public string Message { get; set; }
    }
}