using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class CustomerGeneralInfoCoreViewModel
    {
        public Guid CustomerGeneralInfoId { get; set; }
        public Nullable<int> CID { get; set; }
        public Nullable<int> PrefTimeOfDay { get; set; }
        public string StrataPlan { get; set; }
        public string StrataNumber { get; set; }
        public Nullable<int> Frequency { get; set; }
        public Nullable<DateTime> NextContactDate { get; set; }
        public string TradingName { get; set; }
        public Nullable<int> LeadType { get; set; }
        public string PYV { get; set; }
        public string Explaination { get; set; }

        public string UNCON { get; set; }
        public string CON { get; set; }
        public string Customer { get; set; }
        public Nullable<int> Terms { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> EmailNotification { get; set; }
        public Nullable<bool> External { get; set; }
        public Nullable<bool> Solicited { get; set; }
        public string BlackListReason { get; set; }
        public Nullable<bool> UmbrellaGroup { get; set; }
        public Nullable<bool> Photos { get; set; }
        public string CustomerLastName { get; set; }
        public Nullable<int> CTId { get; set; }
        public string Note { get; set; }
        public string ContactsName { get; set; }
        public Nullable<int> JobNo { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string DisplayCustomerType { get; set; }
        public Nullable<int> CustomerSiteCount { get; set; }
        public Nullable<int> CustomerType { get; set; }
        public Nullable<bool> BlackListed { get; set; }
        public string SiteFileName { get; set; }
        public Guid? SiteDetailId { get; set; }
   //     public Nullable<int> Contracted { get; set; }
    }
}
