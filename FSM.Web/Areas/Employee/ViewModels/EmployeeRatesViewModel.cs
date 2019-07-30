using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeRatesViewModel
    {
        [Key]
        public Guid RateId { get; set; }
        [Display(Name = "Employee ID")]
        public int EID { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Required]
        [Display(Name = "Employee Type")]
        public FSMConstant.Constant.EmployeeType EmployeeType { get; set; }
        [RegularExpression(@"^([0]|\+91[\-\s]?)?[789]\d{9}$", ErrorMessage = "Entered Mobile No is not valid.")]
        public string MobileNumber { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal BaseRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ActualRate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EffectiveDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        [NotMapped]
        [Display(Name = "Employee ID")]
        public List<string> EmployeeIDList { get; set; }
        public int EmployeeRateID { get; set; }
        public int PageSize { get; set; }
        public string Role { get; set; }
        public List<SelectListItem> EmployeeUserNameList { get; set; }
        public Guid CategoryId { get; set; }
        public Nullable<Guid> SubCategoryId { get; set; }
        public IEnumerable<SelectListItem> RateCategoryList { get; set; }
        public IEnumerable<SelectListItem> RateSubCategoryList { get; set; }
    }
}