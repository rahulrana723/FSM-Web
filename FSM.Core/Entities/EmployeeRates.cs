using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class EmployeeRates
    {
        [Key]
        public Guid RateId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string EID { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeType { get; set; }
        public string MobileNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal BaseRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ActualRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable< DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable< DateTime> ModifiedDate { get; set; }
    }
}
