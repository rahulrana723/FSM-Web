using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CustomerSiteJobViewModel
    {
        [Key]
        public Guid SiteDetailId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        [StringLength(20)]
       // [Required(ErrorMessage = "Unit is required!")]
        public string Unit { get; set; }
        [StringLength(20)]
        [DisplayName("Street Number")]
        [Required(ErrorMessage = "Street Number is required!")]
        public string Street { get; set; }
        [Required(ErrorMessage = "StreetName is required!")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for street name.")]
        [DisplayName("Street Name")]
        public string StreetName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Street Type is required!")]
        [DisplayName("Street Type")]
        public Constant.HomeAddressStreetType StreetType { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Suburb is required!")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for Suburb.")]
        public string Suburb { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "State is required!")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed State.")]
        public string State { get; set; }
        [Required(ErrorMessage = "Postal code is required!")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        [DisplayName("Postal Code")]
        public Nullable<int> PostalCode { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
    }
}