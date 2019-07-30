using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerBillingAddressViewModel
    {
        [Key]
        public Guid BillingAddressId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        [DisplayName("Title")]
        public Nullable<Constant.Title> CustomerTitle { get; set; }
        [StringLength(50)]
       // [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
       // [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed for first name.")]
        public string FirstName { get; set; }
        [StringLength(50)]
        [DisplayName("Last Name")]
      //  [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only alphabets are allowed for last name.")]
      //  [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [DisplayName("Mobile No.")]
      //  [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "Digits between 10-13 valid for mobile no.")]

        //[Required(ErrorMessage = "Mobile No. is required")]
        public string PhoneNo1 { get; set; }
        [DisplayName("Landline No.")]
       // [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "Digits between 10-13 valid for landline no.")]
        public string PhoneNo2 { get; set; }
        [DisplayName("Alternate No.")]
     //   [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "Digits between 10-13 valid for alternate no.")]
        public string PhoneNo3 { get; set; }
        [DisplayName("Notes")]
        public string Spare1 { get; set; }
        [StringLength(50)]
        public string Spare2 { get; set; }
        [StringLength(50)]
        [DisplayName("Contact Position")]
        public string ContactPosition { get; set; }
        public bool PO { get; set; }
        [DisplayName("PO Box")]
        public string POAddress { get; set; }
        [StringLength(50)]
        [DisplayName("x C/O")]
        public string xCO { get; set; }
        [StringLength(50)]
        [DisplayName("Email Id")]
        [Required]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please enter correct email address")]
        public string EmailId { get; set; }
        [StringLength(20)]
        public string Unit { get; set; }
        [StringLength(20)]
        [DisplayName("Street Number")]
        //[Required(ErrorMessage = "Street Number is required")]
        public string StreetNo { get; set; }
        [StringLength(50)]
        //[Required(ErrorMessage = "Street Name is required")]
        [DisplayName("Street Name")]
        public string StreetName { get; set; }
        //[Required(ErrorMessage = "Street Type is required")]
        //[DisplayName("Street Type")]
        //public Nullable<Constant.HomeAddressStreetType> StreetType { get; set; }
        [StringLength(50)]
        public string Suburb { get; set; }
        [StringLength(50)]
        //[Required(ErrorMessage = "State name is required")]
        public string State { get; set; }
        //[Required(ErrorMessage = "Postal Code is required")]
        [StringLength(5)]
      //  [RegularExpression("^[0-9]{5}$", ErrorMessage = "Enter Digits 5 valid for postal code.")]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }
        public string BillingAddress { get; set; }
        public string Name { get; set; }
        [DisplayName("Strata Plan")]
        public string StrataPlan { get; set; }
        [DisplayName("Real Estate")]
        public string RealEstate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string UserName { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        [DisplayName("Default Address")]
        public bool IsDefault { get; set; }
    }
}