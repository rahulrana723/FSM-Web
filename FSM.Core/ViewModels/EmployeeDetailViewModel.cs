using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class EmployeeDetailViewModelCore
    {
        public string Name { get; set; }
        public string TFN { get; set; }
        public string UserName { get; set; }
        public string EID { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public string LastDateSent { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Nullable<int> UnReadMsg { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public bool IsDelete { get; set; }
    }

    public class EmployeeDetailViewModelDashboard
    {
        public string Name { get; set; }
        public string TFN { get; set; }
        public string UserName { get; set; }
        public string EID { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public string LastDateSent { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Nullable<int> UnReadMsg { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public bool IsDelete { get; set; }
        public Guid RoleId { get; set; }
        public Nullable<int> OTRWOrder { get;set;}
    }
}
