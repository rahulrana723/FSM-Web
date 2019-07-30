using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class CustomerSiteDetail
    {
        public CustomerSiteDetail()
        {
            this.CustomerResidenceDetails = new HashSet<CustomerResidenceDetail>();
            this.CustomerConditionReports = new HashSet<CustomerConditionReport>();
        }

        [Key]
        public Guid SiteDetailId { get; set; }
        [ForeignKey("CustomerGeneralInfo")]
        public Guid CustomerGeneralInfoId { get; set; }
        [ForeignKey("CustomerContacts")]
        public Nullable<Guid> ContactId { get; set; }
        public Nullable<Guid> StrataManagerId { get; set; }
        public Nullable<Guid> RealEstateId { get; set; }
        public string SiteFileName { get; set; }
        public string Unit { get; set; }
        public string Street { get; set; }
        public string StreetName { get; set; }
        public Nullable<int> StreetType { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public Nullable<int> PostalCode { get; set; }
        public Nullable<bool> MarkAsPreferred { get; set; }
        public Nullable<bool> IsPrimaryAddress { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string ScheduledPrice { get; set; }
        public Nullable<bool> BlackListed { get; set; }
        public string BlackListReason { get; set; }
        public Nullable<int> Contracted { get; set; }
        public Nullable<int> PrefTimeOfDay { get; set; }
        public string Notes { get; set; }
        public string StrataPlan { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
        public virtual CustomerContacts CustomerContacts { get; set; }
        public virtual ICollection<CustomerResidenceDetail> CustomerResidenceDetails { get; set; }
        public virtual ICollection<CustomerConditionReport> CustomerConditionReports { get; set; }
    }
}
