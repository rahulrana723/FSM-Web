using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class Invoice
    {
        [Key]
        public Guid Id { get; set; }
        public Nullable<Guid> EmployeeJobId { get; set; }
        public Nullable<Guid> CustomerGeneralInfoId { get; set; }
        public string ContactName { get; set; }
        public string SiteUnit { get; set; }
        public string SiteStreetName { get; set; }
        public string SiteStreetAddress { get; set; }
        public string SiteSuburb { get; set; }
        public string SiteState { get; set; }
        public Nullable<int> SitePostalCode { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public Nullable<int> AmountPaid { get; set; }
        public Nullable<Guid> OTRWAssigned { get; set; }
        public string CustomerLastName { get; set; }
        public Nullable<int> BillingTitle { get; set; }
        public string CoLastName { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<Guid> BillingAddressId { get; set; }
        public string BillingUnit { get; set; }
        public string BillingStreetName { get; set; }
        public string BillingStreetAddress { get; set; }
        public string BillingSuburb { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingEmail { get; set; }
        public string DesriptionServicesPerformed { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Nullable<int> InvoiceNo { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<Guid> ApprovedBy { get; set; }
        public Nullable<int> InvoiceStatus { get; set; }
        public Nullable<decimal> Paid { get; set; }
        public Nullable<decimal> Due { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<int> SentStatus { get; set; }
        public string ReasonNotSent { get; set; }
        public string InvoiceType { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> CheckMeasure { get; set; }
        public string OTRWNotes { get; set; }

        public string OperationNotes { get; set; }
        public string JobNotes { get; set; }

        public Boolean? Photos { get; set; }
        public Boolean? RequiredDocs { get; set; }
        public Boolean? IsPaid { get; set; }
        public Boolean? Stock { get; set; }
        public Boolean? Material { get; set; }

        public Nullable<DateTime> SentDate { get; set; }
        public Nullable<bool>  IsmyobSynced { get; set; }
    }
}
