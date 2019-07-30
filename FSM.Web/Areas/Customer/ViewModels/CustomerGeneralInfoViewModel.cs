using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FSM.Web.FSMConstant;
using System.ComponentModel;
using FSM.Core.Entities;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerGeneralInfoViewModel
    {
        [Key]
        public Guid CustomerGeneralInfoId { get; set; }
        public int CID { get; set; }
        public int CTId { get; set; }
        [StringLength(500)]
        [DisplayName("Customer Filename")]
        [Required(ErrorMessage = "File Name is required")]
        public string CustomerLastName { get; set; }
        [DisplayName("Customer Type")]
        [Required(ErrorMessage = "Customer type is required")]
        public Constant.CustomerType CustomerType { get; set; }
        [DisplayName("Pref Time Of Day")]
        public Constant.PrefTimeOfDay PrefTimeOfDay { get; set; }
        [StringLength(100)]
        [DisplayName("Strata Plan")]
        public string StrataPlan { get; set; }
        [StringLength(100)]
        [DisplayName("Strata Number")]
        public string StrataNumber { get; set; }
        public Constant.Frequency Frequency { get; set; }
        [DisplayName("Next Contact Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> NextContactDate { get; set; }
        [StringLength(50)]
        [DisplayName("Trading Name")]
        public string TradingName { get; set; }
        [DisplayName("Lead Type")]
        public Constant.LeadType LeadType { get; set; }
        [MaxLength(100, ErrorMessage = "PYV Length cannot be longer than 100 characters.")]
        public string PYV { get; set; }
        [MaxLength(500, ErrorMessage = "Explanation Length cannot be longer than 500 characters.")]
        [DisplayName("Explanation")]
        public string Explaination { get; set; }

        [MaxLength(50, ErrorMessage = "UnCon Length cannot be longer than 50 characters.")]
        public string UNCON { get; set; }
        [MaxLength(50, ErrorMessage = "Con Length cannot be longer than 50 characters.")]
        public string CON { get; set; }
        public string Customer { get; set; }
        public Constant.Terms Terms { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [DisplayName("Email Notification")]
        public bool EmailNotification { get; set; }
        public bool Photos { get; set; }
     //   public Nullable<int> Contracted { get; set; }
        public bool External { get; set; }
        public bool Solicited { get; set; }
        [DisplayName("Black Listed")]
        public Nullable<bool> BlackListed { get; set; }
        [StringLength(100)]
        [DisplayName("Black List Reason")]
        public string BlackListReason { get; set; }
        [DisplayName("Umbrella Group")]
        public bool UmbrellaGroup { get; set; }
        public string Note { get; set; }
        public string CustomerNotes { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string DisplayCustomerType { get; set; }
        public int CustomerSiteCount { get; set; }
        public string SiteFileName { get; set; }
        public Guid? SiteDetailId { get; set; }
        public string ViewAddressClass { get; set; }
        public ICollection<CustomerSiteDetail> CustomerSiteDetails { get; set; }
        public string UserName { get; set; }
    }



    public class CustomerSitesGeneralInfoViewModel
    {
        [Key]
        public Guid CustomerGeneralInfoId { get; set; }
        public int CID { get; set; }
        public int CTId { get; set; }
        [StringLength(500)]
        //[DisplayName("Customer Filename")]
        //[Required(ErrorMessage = "File Name is required")]
        public string CustomerLastName { get; set; }
        [DisplayName("Customer Type")]
        [Required(ErrorMessage = "Customer type is required")]
        public Constant.CustomerType CustomerType { get; set; }
        [DisplayName("Pref Time Of Day")]
        public Constant.PrefTimeOfDay PrefTimeOfDay { get; set; }
        [StringLength(100)]
        [DisplayName("Strata Plan")]
        public string StrataPlan { get; set; }
        [StringLength(100)]
        [DisplayName("Strata Number")]
        public string StrataNumber { get; set; }
        public Constant.Frequency Frequency { get; set; }
        [DisplayName("Next Contact Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> NextContactDate { get; set; }
        [StringLength(50)]
        [DisplayName("Trading Name")]
        public string TradingName { get; set; }
        [DisplayName("Lead Type")]
        public Constant.LeadType LeadType { get; set; }
        [MaxLength(100, ErrorMessage = "PYV Length cannot be longer than 100 characters.")]
        public string PYV { get; set; }
        [MaxLength(500, ErrorMessage = "Explanation Length cannot be longer than 500 characters.")]
        [DisplayName("Explanation")]
        public string Explaination { get; set; }

        [MaxLength(50, ErrorMessage = "UnCon Length cannot be longer than 50 characters.")]
        public string UNCON { get; set; }
        [MaxLength(50, ErrorMessage = "Con Length cannot be longer than 50 characters.")]
        public string CON { get; set; }
        public string Customer { get; set; }
        public Constant.Terms Terms { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [DisplayName("Email Notification")]
        public bool EmailNotification { get; set; }
        public bool Photos { get; set; }
        //   public Nullable<int> Contracted { get; set; }
        public bool External { get; set; }
        public bool Solicited { get; set; }
        [DisplayName("Black Listed")]
        public Nullable<bool> BlackListed { get; set; }
        [StringLength(100)]
        [DisplayName("Black List Reason")]
        public string BlackListReason { get; set; }
        [DisplayName("Umbrella Group")]
        public bool UmbrellaGroup { get; set; }
        public string Note { get; set; }

        [DisplayName("Customer Notes")]
        public string CustomerNotes { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string DisplayCustomerType { get; set; }
        public int CustomerSiteCount { get; set; }
        public string ViewAddressClass { get; set; }
        public ICollection<CustomerSiteDetail> CustomerSiteDetails { get; set; }
        public string UserName { get; set; }
       
        public Guid SiteDetailId { get; set; }
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
      
    
        public Nullable<Constant.Frequency> Contracted { get; set; }
        [DisplayName("Prefer Time Of Day")]
    
     
        public string filecount { get; set; }
        [DisplayName("File Name")]
        public string CustomerName { get; set; }
        public string Notes { get; set; }
        [DisplayName("Strata Plan")]
       
        public string SiteAddress { get; set; }
      
    }
}
