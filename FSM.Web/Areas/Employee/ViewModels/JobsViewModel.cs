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
    public class JobsViewModel
    {
        public Guid Id { get; set; }
        [DisplayName("Customer")]
        public Guid CustomerGeneralInfoId { get; set; }
        [DisplayName("Job Id")]
        public Nullable<int> JobId { get; set; }
        [DisplayName("Job No")]
        public Nullable<int> JobNo { get; set; }
        [DisplayName("Customer Sites")]
        public Nullable<Guid> SiteId { get; set; }
        [DisplayName("Job Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Job type is required !")]
        public Constant.JobType JobType { get; set; }
        public Constant.JobCategory Category { get; set; }
        public string TimeSpent { get; set; }
        [DisplayName("OTRW Job Notes")]
        [AllowHtml]
        public string OTRWjobNotes { get; set; }
        public Nullable<Double> EstimatedTime { get; set; }
        public string StockRequired { get; set; }
        public Nullable<Double> TotalCost { get; set; }
        public List<SelectListItem> CustomerList { get; set; }
        [Required(ErrorMessage = "Customer name is required !")]
        public string CustomerInfoId { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        [Required(ErrorMessage = "Site name is required !")]

        public string tempSiteId { get; set; }
        //[Range(1, int.MaxValue, ErrorMessage = "Status is required !")]
        public Constant.JobStatus Status { get; set; }
        [DisplayName("Action")]
        public Constant.ActionStatus ActionStatus { get; set; }
        [DisplayName("Date Booked")]
        //[Required(ErrorMessage = "Date booked is required !")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> DateBooked { get; set; }
        [DisplayName("Prefer Time")]
        [Range(1, int.MaxValue, ErrorMessage = "Prefer Time is required ")]
        public Constant.PrefTimeOfDay PreferTime { get; set; }
        [Required(ErrorMessage = "Estimated hours is required !")]
        [DisplayName("Estimated Hours")]
        public Nullable<Double> EstimatedHours { get; set; }
        [DisplayName("Job Notes")]
        [AllowHtml]
        public string JobNotes { get; set; }
        [DisplayName("Operation Notes")]
        [AllowHtml]
        public string OperationNotes { get; set; }
        [DisplayName("Assign OTRW ")]
        public Nullable<Guid> AssignTo { get; set; }
        [DisplayName("Assign OTRW")]
        public List<Nullable<Guid>> AssignTo2 { get; set; }
        [DisplayName("Send Confirmations")]
        public Nullable<Constant.SendJobEmail> SendJobEmail { get; set; }
        [DisplayName("Send Confirmations")]
        public Nullable<Constant.ReSendJobEmail> ReSendJobEmail { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Work type is required !")]
        public Constant.WorkType WorkType { get; set; }
        [DisplayName("Compulsary Tasks")]
        public List<Guid?> TaskId { get; set; }
        public Nullable<Guid> Supervisor { get; set; }
        [DisplayName("OTRW Required")]
       // public Nullable<Constant.OTRWRequired> OTRWRequired { get; set; }
        public Nullable<int>  OTRWRequired { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        [DataType(DataType.Time)]
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public List<SelectListItem> OTRWList { get; set; }
        public List<SelectListItem> SuperVisorList { get; set; }
        public List<SelectListItem> CompulsaryList { get; set; }
        public string tempAssignTo { get; set; }
        public List<Nullable<Guid>> tempAssignTo2 { get; set; }

        public string MultipleAssignTo { get; set; }
        public List<SelectListItem> LinkJobList { get; set; }
        public string LinkJobId { get; set; }
        public List<HttpPostedFileBase> JobDocs { get; set; }

        [AllowHtml]
        public string Job_Notes { get; set; }
        [AllowHtml]
        public string OTRW_Notes { get; set; }
        [AllowHtml]
        public string Operation_Notes { get; set; }
        [DisplayName("Job Category")]
        public string JobCategory { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<Double> EstimatedHrsPerUser { get; set; }
        public string UserName { get; set; }
        public int? invoiceTypeCount { get; set; }
        public string InvoiceType { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public String CurrentJobtype { get; set; }

        public int CurrentJobStatus { get; set; }
        public bool? ChangeJobType { get; set; }
        public List<InvoiceData> InvoiceQuoteList { get; set; }
        //[DisplayName("Date & Time")]
        //public Nullable<DateTime> ModifiedTime { get; set; }
        //public string UserName { get; set; }

        [DisplayName("Work Order Number")]
        public string WorkOrderNumber { get; set; }
        public List<string> GetUserRoles { get; set; }
        public string ApproveStatus { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> OldDateBooked { get; set; }
        public Nullable<int> Contracted { get; set; }
        public Nullable<DateTime> ContractDueDate { get; set; }

        public JobPerFormanceViewModel jobPerFormanceViewModel { get; set; }
        public Nullable<int> NotificationType { get; set; }
        public List<AssignViewModel> AssignInfo { get; set; }
        public Boolean IsJobStart { get; set; }
        
    }
    public class AssignViewModel
    {
        public Guid AssignId { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}")]
        public Nullable<DateTime> AssignStartTime { get; set; }
        public List<SelectListItem> AssignOTRWList { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? AssignDateBooked { get; set; }

        public DateTime? DateBooked { get; set; }

        public string OTRWNotes { get; set; }
        public string mynotes { get; set; }

        //public Nullable<int> jobstatus { get; set; }
    }
        public class InvoiceData
    {
        public string InvoiceType { get; set; }
        public Nullable<Guid> Id { get; set; }
    }

    public class JobPerFormanceViewModel
    {
        public Nullable<Guid> JobId { get; set; }
        [DisplayName("Sale Income")]
        public Nullable<decimal> SaleIncome { get; set; }
        [DisplayName("Total Cost")]
        public Nullable<decimal> TotalCost { get; set; }
        [DisplayName("Labour Income")]
        public Nullable<decimal> LabourIncome { get; set; }
        [DisplayName("LIP Hours:")]
        public Nullable<decimal> LIPHR { get; set; }
        [DisplayName("Rev Hours:")]
        public Nullable<decimal> RevHours { get; set; }
        [DisplayName("JSPO Cost")]
        public Nullable<decimal> JSPOCost { get; set; }
        [DisplayName("Labour Cost")]
        public Nullable<decimal> LabourCost { get; set; }
        [DisplayName("LCPHR")]
        public Nullable<decimal> LCPHR { get; set; }
        [DisplayName("NRL Hours")]
        public Nullable<decimal> NRLHours { get; set; }
        [DisplayName("Stock Item")]
        public Nullable<decimal> StockItemCost { get; set; }
        [DisplayName("Labour Profit")]
        public Nullable<decimal> LabourProfit { get; set; }
        [DisplayName("LPPHR")]
        public Nullable<decimal> LPPHR { get; set; }
        public string DisplayLPPHR { get; set; }
        [DisplayName("Hours:")]
        public Nullable<decimal> Hours { get; set; }
        [DisplayName("Sales Bonus")]
        public Nullable<decimal> SalesBonus { get; set; }
    }
}