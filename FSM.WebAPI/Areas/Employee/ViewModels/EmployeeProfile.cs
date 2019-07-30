using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class EmployeeProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SignaturePicture { get; set; }
        public string ProfilePicture { get; set; }
        public string HomeAddressMobile { get; set; }
        public string EID { get; set; }
        public Guid EmployeeId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmergencyFirstName { get; set; }
        public string EmergencyLastName { get; set; }
        public Guid Role { get; set; }
        public string IsActive { get; set; }
        public string ViewInvoice { get; set; }
        public string MakeInvoice { get; set; }
        public string ApproveInvoice { get; set; }
        public string SendInvoice { get; set; }
        public string MYOB { get; set; }
        public string TFN { get; set; }
        public string BusinessName { get; set; }
        public string HourlyRate { get; set; }
        public string Mobile { get; set; }
        public int?[] WorkType { get; set; }
        public int? UnreadMsgCount { get; set; }
    }
}