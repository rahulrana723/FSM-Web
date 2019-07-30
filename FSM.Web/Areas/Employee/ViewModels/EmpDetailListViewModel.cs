using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmpDetailListViewModel
    {
        public string Name { get; set; }
        public string TFN { get; set; }
        public string UserName { get; set; }
        public int EID { get; set; }
        public string Role { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public Guid EmployeeId { get; set; }
        public string Message { get; set; }
        public string LastDateSent { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid LoggedUserId { get; set; }
        public Nullable<int> UnReadMsg { get; set; }
        public Nullable<int> OTRWOrder { get; set; }
    }
}