using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FSM.Web.FSMConstant;
using System.Web.Mvc;
using System.ComponentModel;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeJobsViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EmployeeJobId { get; set; }
        public string CustomerName { get; set; }
        public List<CustomerCoName> CustomerCOLastName { get; set; }
        public List<SiteDetail> LstCustomerSiteDetail { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        [DisplayName("Job Type")]
        public Nullable<Constant.JobType> JobType { get; set; }
        public Nullable<Constant.WorkType> WorkType { get; set; }
        public Nullable<Constant.AccountJobType> AccountJobType { get; set; }
        public Nullable<Constant.JobStatus> Status { get; set; }
        [Required]
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<Constant.PrefTimeOfDay> PreferTime { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "*Estimated hours should be greater than 0.")]
        public Nullable<int> EstimatedHours { get; set; }

        [AllowHtml]
        public string JobNotes { get; set; }
        [AllowHtml]
        public string OperationNotes { get; set; }
        public Nullable<Guid> AssignTo { get; set; }
        public Nullable<Guid> BookedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public List<EmployeeList> Employees { get; set; }
        public HttpPostedFileBase attachment { get; set; }
        public string BookedByName { get; set; }
        public List<OTRWEmployee> OTRWList { get; set; }
        public List<string> GetUserRoles { get; set; }
        public string CustomerLastName { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public Nullable<Constant.InvoiceStatus> InvoiceStatus { get; set; }
        public int PageSize { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public List<Joblist> JobList { get; set; }
        public Nullable<Guid> LinkedJobId { get; set; }
        public Nullable<Guid> SupportjobSiteId { get; set; }
        public string StrataPlan { get; set; }
        public string StrataNumber { get; set; }
        public string SiteAddress { get; set; }
        public string AssignUser { get; set; }
        public string ContactName { get; set; }
        public bool? IsDelete { get; set; }
        public string SiteFileName { get; set; }

        public string DisplayJobType { get; set; }
        public string DisplayStatus { get; set; }

        public string InvoiceDetails { get; set; }
        public Nullable<DateTime> CompletionDate { get; set; }
    }

    public class Joblist
    {
        public Guid Jobid { get; set; }
        public String jobnumeric { get; set; }
    }
    public class OTRWEmployee
    {
        public String EmployeeName { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }
    }
    public class CustomerCoName
    {
        public String LastName { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
    }
    public class SiteDetail
    {
        public string SiteName { get; set; }
        public Nullable<Guid> SitesId { get; set; }
    }

    public class EmployeeList
    {
        public String Employeename { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }

    }
}