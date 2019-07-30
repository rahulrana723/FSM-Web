using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSM.Core.Entities;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerContractListViewModel
    {
        public IEnumerable<CustomerContactLogViewModel> CustomerContactList { get; set; }
        public IEnumerable<CustomerReminderViewModel> CustomerReminderList { get; set; }
        public CustomerContactLog CustomerContactLog { get; set; }
        public CustomerReminder CustomerReminder { get; set; }
        public ContactLogSearchModel ContactLog { get; set; }
        public int PageSize { get; set; }
    }
}