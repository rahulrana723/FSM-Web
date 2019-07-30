using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerContactsViewModel
    {
        public Guid ContactId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
       // [Required(ErrorMessage = "Site name is required !")]
        public string SiteId { get; set; }
        public Nullable<Constant.HomeAddressTitle> Title { get; set; }
       // [Required(ErrorMessage = "Contacts type is required !")]
        [DisplayName("Contact Type")]
        public Nullable<Constant.ContactsType> ContactsType { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "First name is required !")]
        [DisplayName("First name")]
        //[RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Invalid First name")]
        public string FirstName { get; set; }
        [StringLength(50)]
        //[Required(ErrorMessage = "Last name is required !")]
        [DisplayName("Last name")]
       // [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Invalid Last name")]
        public string LastName { get; set; }
        [DisplayName("Mobile")]
       // [RegularExpression("^\\+?([0-9]{2})\\)?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid mobile no")]
      ///  [MaxLength(10, ErrorMessage = "Mobile no max length is 10")]
        public string PhoneNo1 { get; set; }
        [DisplayName("LandLine")]
       // [RegularExpression("^\\+?([0-9]{2})\\)?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid landLine no")]
       // [MaxLength(10, ErrorMessage = "LandLine no max length is 10")]
        public string PhoneNo2 { get; set; }
        [DisplayName("Alternate")]
       // [RegularExpression("^\\+?([0-9]{2})\\)?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid alternate no")]
        //[MaxLength(10, ErrorMessage = "Alternate no max length is 10")]
        public string PhoneNo3 { get; set; }
        [DisplayName("Notes")]
        public string Spare1 { get; set; }
        [DisplayName("Spare 2")]
       // [MaxLength(50, ErrorMessage = "Spare 2 max length is 10")]
        public string Spare2 { get; set; }
        [DisplayName("Spare 3")]
       // [MaxLength(50, ErrorMessage = "Spare 3 max length is 10")]
        public string Spare3 { get; set; }
        public int PageSize { get; set; }
        [DisplayName("Email")]
        //[Required(ErrorMessage = "Email Id is required !")]
      //  [RegularExpression(".+@.+\\..+", ErrorMessage = "Please enter correct email Id")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "Email Id is invalid !")]
        public string EmailId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public bool ContactConfirmation { get; set; }
        public bool IsBillingContact { get; set; }
        public bool IsContact { get; set; }
        public bool IsStrataManager { get; set; }
        public bool IsRealEstate { get; set; }
        public string PageNum { get; set; }
        //public string ContactsType { get; set; }
        public string HideAddContacts { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public string JobId { get; set; }
        public string InvoiceId { get; set; }
        public string SiteAddress { get; set; }
        public string DisplayContactsType { get; set; }
        public string UserName { get; set; }

        [DisplayName("Default contact for all")]

        public bool DefaultContact { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string CustomerFileName { get; set; }
    }
}