using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class EmployeeRateCoreViewModel
    {
        public Guid RateId { get; set; }
        public string EID { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<int> EmployeeType { get; set; }
        public string MobileNumber { get; set; }
        public decimal BaseRate { get; set; }
        public decimal ActualRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string Role { get; set; }
    }
}
