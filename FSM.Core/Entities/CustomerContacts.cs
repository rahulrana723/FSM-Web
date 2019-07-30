using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class CustomerContacts
    {
        [Key]
        public Guid ContactId { get; set; }
        [ForeignKey("CustomerGeneralInfo")]
        public Guid CustomerGeneralInfoId { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public Nullable<int> Title { get; set; }
        public Nullable<int> ContactsType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneNo3 { get; set; }
        public string Spare1 { get; set; }
        public string Spare2 { get; set; }
        public string Spare3 { get; set; }
        public string EmailId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsBillingContact { get; set; }
        public Nullable<bool> IsContact { get; set; }
        public Nullable<bool> ContactConfirmation { get; set; }
        public Nullable<bool> IsStrataManager { get; set; }
        public Nullable<bool> IsRealEstate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
    }
}
