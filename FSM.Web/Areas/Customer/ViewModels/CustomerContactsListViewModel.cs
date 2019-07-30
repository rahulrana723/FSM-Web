using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerContactsListViewModel
    {
        public IEnumerable<CustomerContacts> CustomerContactsList { get; set; }
        public IEnumerable<CustomerContactsViewModel> CustomerContactsViewModelList { get; set; }
        public CustomerContacts CustomerContacts { get; set; }
        public ContactsSearchViewModel ContactsDetailInfo { get; set; }
        public IEnumerable<CustomerContactsCoreViewModel> ContactsList { get; set; }
        public CustomerContactsViewModel customerContactsViewModel { get; set; }
        public int ContactCount { get; set; }
    }
}