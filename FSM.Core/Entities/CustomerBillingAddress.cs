using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class CustomerBillingAddress
    {
        [Key]
        public Guid BillingAddressId { get; set; }
        [ForeignKey("CustomerGeneralInfo")]
        public Guid CustomerGeneralInfoId { get; set; }
        public Nullable<int> CustomerTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneNo3 { get; set; }
        public string Spare1 { get; set; }
        public string Spare2 { get; set; }
        public string ContactPosition { get; set; }
        public bool PO { get; set; }
        public string POAddress { get; set; }
        public string xCO { get; set; }
        public string EmailId { get; set; }
        public string Unit { get; set; }
        public string StreetNo { get; set; }
        public string StreetName { get; set; }
        public Nullable<int> StreetType { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string StrataPlan { get; set; }
        public string RealEstate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
        public Nullable<bool> IsDefault { get; set; }
    }
}
