using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerSiteDetailViewModel
    {
        [Key]
        public Guid SiteDetailId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }

        [DisplayName("Site File Name")]
        public string SiteFileName { get; set; }
        [StringLength(20)]
       // [Required(ErrorMessage = "Unit is required!")]
        public string Unit { get; set; }
        [StringLength(20)]
        [DisplayName("Street Number")]
        //[Required(ErrorMessage = "Street Number is required!")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Street Name is required!")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for street name.")]
        [DisplayName("Street Name")]
        public string StreetName { get; set; }
        //[Range(1, int.MaxValue, ErrorMessage = "Street Type is required!")]
        //[DisplayName("Street Type")]
        //public Constant.HomeAddressStreetType StreetType { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Suburb is required!")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for Suburb.")]
        public string Suburb { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "State is required!")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed State.")]
        public string State { get; set; }
        // [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        [DisplayName("Postal Code")]
        public Nullable<int> PostalCode { get; set; }
        [DisplayName("Mark As Preferred")]
        public bool MarkAsPreferred { get; set; }
        [DisplayName("Is Primary Address")]
        public bool IsPrimaryAddress { get; set; }
        //[Required(ErrorMessage = "Please Select Contact")]
        //public string ContactId { get; set; }

        public string StrataManagerId { get; set; }
        public string RealEstateId { get; set; }
        [DisplayName("Scheduled Price")]
        public string ScheduledPrice { get; set; }
        [DisplayName("Black Listed")]
        public bool BlackListed { get; set; }
        [StringLength(500)]
        [DisplayName("Black List Reason")]
        public string BlackListReason { get; set; }
        public Nullable<Constant.Frequency> Contracted { get; set; }
        [DisplayName("Prefer Time Of Day")]
        public Nullable<Constant.PrefTimeOfDay> PrefTimeOfDay { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string filecount { get; set; }
        [DisplayName("File Name")]
        public string CustomerName { get; set; }
        public string Notes { get; set; }
        [DisplayName("Strata Plan")]
        public string StrataPlan { get; set; }
        public string SiteAddress { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}