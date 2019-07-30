using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class Vacation
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("EmployeeDetail")]
        public Nullable<Guid> EmployeeId { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<int> Hours { get; set; }
        public string Reason { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> LeaveType { get; set; }
        [ForeignKey("RoastedOff")]
        public Nullable<Guid> RoastedOffId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual EmployeeDetail EmployeeDetail { get; set; }
        public virtual RoastedOff RoastedOff { get; set; }
    }
}
