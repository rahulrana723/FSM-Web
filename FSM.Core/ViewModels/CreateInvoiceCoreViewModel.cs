using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FSM.Core.ViewModels
{
    public class CreateInvoiceCoreViewModel
    {
        public string OTRWAssignName { get; set; }
        public Guid Id { get; set; }
        public Nullable<Guid> EmployeeJobId { get; set; }
        public string ContactName { get; set; }
        public string SiteUnit { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public string SiteStreetName { get; set; }
        public string SiteSuburb { get; set; }
        public string SiteStreetAddress { get; set; }
        public string SiteState { get; set; }
        public Nullable<int> SitePostalCode { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<Decimal> Price { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public Nullable<Guid> OTRWAssigned { get; set; }
        public string CustomerLastName { get; set; }
        public Nullable<int> BillingTitle { get; set; }
        public string CoLastName { get; set; }
        public string PhoneNo { get; set; }
        public string BillingUnit { get; set; }
        public string BillingStreetName { get; set; }
        public string BillingStreetAddress { get; set; }
        public string BillingSuburb { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingEmail { get; set; }
        public string DesriptionServicesPerformed { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public Nullable<Guid> ApprovedBy { get; set; }
        public Nullable<int> InvoiceStatus { get; set; }
        public Nullable<decimal> Paid { get; set; }
        public Nullable<decimal> Due { get; set; }
        public Nullable<int> SentStatus { get; set; }
        public Nullable<Guid> SupportJobId { get; set; }
        public int SupportJId { get; set; }
        public bool? CheckMeasure { get; set; }
        public String SupportCustName { get; set; }
        public Nullable<int> SupportjobType { get; set; }
        public Nullable<int> SupportjobStatus { get; set; }
        public Nullable<DateTime> SupportjobDateBooked { get; set; }
        public Nullable<Guid> SupportOTRW { get; set; }
        public DateTime? InvoiceSearchDate { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public int UnitMeasure { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public string SiteAddress { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<int> Type { get; set; }
        public string OTRWNotes { get; set; }
        public string OperationNotes { get; set; }
        public Nullable<int> CustomerType { get; set; }
        public string InvoiceType { get; set; }
        public string paidStatus { get; set; }

        public decimal? QuotePaidAmount { get; set; }
        public decimal? BalanceafterQuoteAmount { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string StrataPlan { get; set; }
        public string JobNotes { get; set; }

        public string DisplaySiteAddress { get; set; }
        public string DisplayBillingAddress { get; set; }
        public string WorkOrderNumber { get; set; }
        public int? TotalCount { get; set; }
    }
    public class GetInvoiceViewModel
    {
        public Nullable<Guid> ID { get; set; }
    }

    public class GetPurchaseorderViewModel
    {
        public Nullable<Guid> ID { get; set; }
        public Nullable<int> Purchaseorderno { get; set; }
        public string Name { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string Description { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public Nullable< bool> IsSyncedToMyob { get; set; }
        //public Nullable<Guid> SupplierID { get; set; }
        //
        //public Nullable<Guid> JobID { get; set; }
        //public Nullable<Guid> InvoiceId { get; set; }
        //public Nullable<bool> IsApprove { get; set; }
        //public Nullable<Guid> ApprovedBy { get; set; }
        //public Nullable<bool> IsDelete { get; set; }
        //public Nullable<Guid> CreatedBy { get; set; }
        //public Nullable<Guid> ModifiedBy { get; set; }

        //public Nullable<DateTime> ModifiedDate { get; set; }

    }
}
