using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CreateInvoiceViewModel
    {
        public Guid Id { get; set; }
        public Nullable<Guid> EmployeeJobId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        [DisplayName("Contact")]
        [StringLength(50)]
        //[Required(ErrorMessage = "Contact name  required")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for contact name.")]
        public string ContactName { get; set; }
        public string PreparedBy { get; set; }

        [DisplayName("Unit")]
        [StringLength(20)]
        //[Required(ErrorMessage = "Unit required")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for site unit.")]
        public string SiteUnit { get; set; }

        [DisplayName("Street name")]
        [StringLength(50)]
        [Required(ErrorMessage = "Street name  required")]
        public string SiteStreetName { get; set; }

        // [Required(ErrorMessage = "Site street address required")]
        [DisplayName("Street address")]
        [StringLength(100)]
        public string SiteStreetAddress { get; set; }
        [DisplayName("Suburb")]
        [Required(ErrorMessage = "Site suburb required")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for site suburb.")]
        public string SiteSuburb { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Site state required")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for site state.")]
        public string SiteState { get; set; }

        [Required(ErrorMessage = "Postal code is  required")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        [DisplayName("Postal Code")]
        public Nullable<int> SitePostalCode { get; set; }

        public Nullable<int> JobId { get; set; }
        public List<SelectListItem> JCLItemList { get; set; }
        public List<SelectListItem> JCLColorList { get; set; }
        public List<SelectListItem> JCLSizeList { get; set; }
        public List<SelectListItem> JCLProductList { get; set; }
        public Nullable<int> JobNo { get; set; }
        [DisplayName("Job type")]
        public Nullable<Constant.JobType> JobType { get; set; }
        [DisplayName("Booked date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> DateBooked { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PriceWithGst { get; set; }
        public Nullable<decimal> BalanceDue { get; set; }
        public Nullable<int> AmountPay { get; set; }
        public decimal? DepositRequired { get; set; }
        [DisplayName("Status")]
        public Nullable<Constant.InvoiceJobStatus> JobStatus { get; set; }
        [DisplayName("OTRW assigned")]
        public Nullable<Guid> OTRWAssigned { get; set; }

        [DisplayName("Assign OTRW")]
        public List<Nullable<Guid>> OTRWAssignedList { get; set; }

        [DisplayName("Customer Filename")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for file name.")]
        public string CustomerLastName { get; set; }
        public Nullable<Guid> BillingAddressId { get; set; }
        [DisplayName("Title")]
        public Nullable<Constant.Title> BillingTitle { get; set; }
        [DisplayName("Customer Filename")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for file name.")]
        public string CoLastName { get; set; }
        [DisplayName("Phone No ")]
        // [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "Digits between 10-13 valid for phone no.")]
        public string PhoneNo { get; set; }
        [DisplayName("Unit")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for unit.")]
        public string BillingUnit { get; set; }
        [DisplayName("Street name")]
        [StringLength(50)]
        public string BillingStreetName { get; set; }
        [DisplayName("Street address")]
        [StringLength(100)]
        public string BillingStreetAddress { get; set; }
        [DisplayName("Suburb")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for billing suburb.")]
        public string BillingSuburb { get; set; }
        [DisplayName("State")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for billing state.")]
        [StringLength(50)]
        public string BillingState { get; set; }
        [DisplayName("Postal code")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        public string BillingPostalCode { get; set; }
        [DisplayName("Email Address")]
        public string BillingEmail { get; set; }
        [DisplayName("Description of the services performed")]
        public string DesriptionServicesPerformed { get; set; }
        [DisplayName("Invoice date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvcDate { get; set; }
        [DisplayName("Invoice no.")]
        public Nullable<int> InvoiceNo { get; set; }
        [DisplayName("Approved by")]
        public Nullable<Guid> ApprovedBy { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        [DisplayName("Check Measure")]
        public bool CheckMeasure { get; set; }
        public Nullable<int> Type { get; set; }
        public string DisplayType { get; set; }
        public string ApproveStatus { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<Constant.JobType> CurrentJobType { get; set; }
        [DisplayName("Status")]
        public Nullable<Constant.InvoiceStatus> InvoiceStatus { get; set; }
        public List<OTRWEmployeeInvoice> OTRWList { get; set; }
        public List<SelectListItem> OTRWListselect { get; set; }

        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for paid.")]
        public Nullable<decimal> Paid { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for due.")]
        public Nullable<decimal> Due { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public int PageSize { get; set; }
        [DisplayName("Sent Status")]
        public Nullable<int> SentStatus { get; set; }
        [DisplayName("Reason if Not Sent")]
        public string ReasonNotSent { get; set; }
        public Nullable<Guid> SupportJobId { get; set; }
        [DisplayName("Job Id")]
        public int SupportJId { get; set; }
        [DisplayName("C/O name")]
        public String SupportCustName { get; set; }
        [DisplayName("Job type")]
        public Nullable<Constant.JobType> SupportjobType { get; set; }
        [DisplayName("Status")]
        public Nullable<Constant.JobStatus> SupportjobStatus { get; set; }
        [DisplayName("Booked date")]
        public Nullable<DateTime> SupportjobDateBooked { get; set; }
        [DisplayName("OTRW assigned")]
        public Nullable<Guid> SupportOTRW { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public Nullable<Decimal> SubTotal { get; set; }
        public Nullable<Decimal> Total { get; set; }
        public int UnitMeasure { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public string SiteAddress { get; set; }
        public string UserName { get; set; }
        public string InvoiceType { get; set; }
        public string DisplaySentStatus { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public List<SelectListItem> BillingAddressList { get; set; }
        [DisplayName("OTRW Notes")]
        [AllowHtml]
        public string OTRWNotes { get; set; }
        [DisplayName("Amount Pay")]
        public Nullable<Constant.AmountPaid> AmountPaid { get; set; }

        [DisplayName("Operations Notes")]
        public string OperationNotes { get; set; }

        public string SiteFileName { get; set; }

        public string OtrwAssignedName { get; set; }

        public string paidStatus { get; set; }

        public decimal? BalanceafterQuoteAmount { get; set; }
        public decimal? QuotePaidAmount { get; set; }
        public decimal? GST { get; set; }

        [DisplayName("Strata Plan")]
        public string StrataPlan { get; set; }

        public string TradeName { get; set; }
        public string DisplayBillingAddress { get; set; }
        public string DisplaysiteAddress { get; set; }
        [DisplayName("Job Notes")]
        public string JobNotes { get; set; }


        public string StreetNumber { get; set; }
    }


    public class QuoteMaterialViewModel
    {
        [DisplayName("Date Quote Accepted")]
        public Nullable<DateTime> QuoteAcceptedDate { get; set; }
        [DisplayName("Job Quoted By")]
        public string QuotedBy { get; set; }
        [DisplayName("Customer Name")]
        public String CustomerName { get; set; }
        [DisplayName("Site Address")]
        public string SiteAddress { get; set; }
        public List<JcLViewModel> JCLInfo { get; set; }

        public Nullable<DateTime> JobDateBooked { get; set; }
        public Nullable<Decimal> DepositRequested { get; set; }

        public string JobAssignedto { get; set; }
        public Nullable<DateTime> DateDepositPaid { get; set; }

        public Nullable<Decimal> Totalpaid { get; set; }
    }
    public class OTRWEmployeeInvoice
    {
        public String EmployeeName { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }
    }

    public class MaterialViewModel
    {
        public Guid Id { get; set; }
        public Nullable<Guid> EmployeeJobId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        [DisplayName("Contact")]
        [StringLength(50)]
        //[Required(ErrorMessage = "Contact name  required")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for contact name.")]
        public string ContactName { get; set; }
        public string PreparedBy { get; set; }

        [DisplayName("Unit")]
        [StringLength(20)]
        //[Required(ErrorMessage = "Unit required")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for site unit.")]
        public string SiteUnit { get; set; }

        [DisplayName("Street name")]
        [StringLength(50)]
        [Required(ErrorMessage = "Street name  required")]
        public string SiteStreetName { get; set; }

        // [Required(ErrorMessage = "Site street address required")]
        [DisplayName("Street address")]
        [StringLength(100)]
        public string SiteStreetAddress { get; set; }
        [DisplayName("Suburb")]
        [Required(ErrorMessage = "Site suburb required")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for site suburb.")]
        public string SiteSuburb { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "Site state required")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for site state.")]
        public string SiteState { get; set; }

        [Required(ErrorMessage = "Postal code is  required")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        [DisplayName("Postal Code")]
        public Nullable<int> SitePostalCode { get; set; }

        public Nullable<int> JobId { get; set; }
        public List<SelectListItem> JCLItemList { get; set; }
        public List<SelectListItem> JCLColorList { get; set; }
        public List<SelectListItem> JCLSizeList { get; set; }
        public List<SelectListItem> JCLProductList { get; set; }
        public Nullable<int> JobNo { get; set; }
        [DisplayName("Job type")]
        public Nullable<Constant.JobType> JobType { get; set; }
        [DisplayName("Booked date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> DateBooked { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PriceWithGst { get; set; }
        public Nullable<decimal> BalanceDue { get; set; }
        public Nullable<int> AmountPay { get; set; }
        public decimal? DepositRequired { get; set; }
        [DisplayName("Status")]
        public Nullable<Constant.InvoiceJobStatus> JobStatus { get; set; }
        [DisplayName("OTRW assigned")]
        public Nullable<Guid> OTRWAssigned { get; set; }

        [DisplayName("Assign OTRW")]
        public List<Nullable<Guid>> OTRWAssignedList { get; set; }

        [DisplayName("Customer Filename")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for file name.")]
        public string CustomerLastName { get; set; }
        public Nullable<Guid> BillingAddressId { get; set; }
        [DisplayName("Title")]
        public Nullable<Constant.Title> BillingTitle { get; set; }
        [DisplayName("Customer Filename")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for file name.")]
        public string CoLastName { get; set; }
        [DisplayName("Phone No ")]
        // [RegularExpression("^[0-9]{10,13}$", ErrorMessage = "Digits between 10-13 valid for phone no.")]
        public string PhoneNo { get; set; }
        [DisplayName("Unit")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for unit.")]
        public string BillingUnit { get; set; }
        [DisplayName("Street name")]
        [StringLength(50)]
        public string BillingStreetName { get; set; }
        [DisplayName("Street address")]
        [StringLength(100)]
        public string BillingStreetAddress { get; set; }
        [DisplayName("Suburb")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for billing suburb.")]
        public string BillingSuburb { get; set; }
        [DisplayName("State")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for billing state.")]
        [StringLength(50)]
        public string BillingState { get; set; }
        [DisplayName("Postal code")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        public string BillingPostalCode { get; set; }
        [DisplayName("Email Address")]
        public string BillingEmail { get; set; }
        [DisplayName("Description of the services performed")]
        public string DesriptionServicesPerformed { get; set; }
        [DisplayName("Invoice date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvcDate { get; set; }
        [DisplayName("Invoice no.")]
        public Nullable<int> InvoiceNo { get; set; }
        [DisplayName("Approved by")]
        public Nullable<Guid> ApprovedBy { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        [DisplayName("Check Measure")]
        public bool CheckMeasure { get; set; }
        public Nullable<int> Type { get; set; }
        public string DisplayType { get; set; }
        public string ApproveStatus { get; set; }
        public string WorkOrderNumber { get; set; }
        public Nullable<Constant.JobType> CurrentJobType { get; set; }
        [DisplayName("Status")]
        public Nullable<Constant.InvoiceStatus> InvoiceStatus { get; set; }
        public List<OTRWEmployeeInvoice> OTRWList { get; set; }
        public List<SelectListItem> OTRWListselect { get; set; }

        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for paid.")]
        public Nullable<decimal> Paid { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for due.")]
        public Nullable<decimal> Due { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public int PageSize { get; set; }
        [DisplayName("Sent Status")]
        public Nullable<int> SentStatus { get; set; }
        [DisplayName("Reason if Not Sent")]
        public string ReasonNotSent { get; set; }
        public Nullable<Guid> SupportJobId { get; set; }
        [DisplayName("Job Id")]
        public int SupportJId { get; set; }
        [DisplayName("C/O name")]
        public String SupportCustName { get; set; }
        [DisplayName("Job type")]
        public Nullable<Constant.JobType> SupportjobType { get; set; }
        [DisplayName("Status")]
        public Nullable<Constant.JobStatus> SupportjobStatus { get; set; }
        [DisplayName("Booked date")]
        public Nullable<DateTime> SupportjobDateBooked { get; set; }
        [DisplayName("OTRW assigned")]
        public Nullable<Guid> SupportOTRW { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public Nullable<Decimal> SubTotal { get; set; }
        public Nullable<Decimal> Total { get; set; }
        public int UnitMeasure { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public string SiteAddress { get; set; }
        public string UserName { get; set; }
        public string InvoiceType { get; set; }
        public string DisplaySentStatus { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public List<SelectListItem> BillingAddressList { get; set; }
        [DisplayName("OTRW Notes")]
        [AllowHtml]
        public string OTRWNotes { get; set; }
        [DisplayName("Amount Pay")]
        public Nullable<Constant.AmountPaid> AmountPaid { get; set; }

        [DisplayName("Operations Notes")]
        public string OperationNotes { get; set; }

        public string SiteFileName { get; set; }

        public string OtrwAssignedName { get; set; }

        public string paidStatus { get; set; }

        public decimal? BalanceafterQuoteAmount { get; set; }
        public decimal? QuotePaidAmount { get; set; }
        public decimal? GST { get; set; }

        [DisplayName("Strata Plan")]
        public string StrataPlan { get; set; }

        public string TradeName { get; set; }
        public string DisplayBillingAddress { get; set; }
        public string DisplaysiteAddress { get; set; }
        [DisplayName("Job Notes")]
        public string JobNotes { get; set; }

        public string keyword { get; set; }
        public string StreetNumber { get; set; }
    }
}