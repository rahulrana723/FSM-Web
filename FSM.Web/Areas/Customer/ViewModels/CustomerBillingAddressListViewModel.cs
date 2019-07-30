using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerBillingAddressListViewModel
    {
        public CustomerBillingAddressViewModel customerBillingAddressViewModel { get; set; }
        public IEnumerable<CustomerBillingAddress> CustomerBillingList { get; set; }
        public IEnumerable<CustomerBillingAddressViewModel> CustomerBillingViewModelList { get; set; }
        public CustomerBillingAddress CustomerBillingAddress { get; set; }
        public BillingSearchViewModel BillingSearchInfo { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public Guid BillingAddressId { get; set; }
        public int BillingCount { get; set; }
        public string HideBillingBtn { get; set; }
        public string UserName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}