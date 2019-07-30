using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class SIteContactsListViewModel
    {
        public Guid ContactId { get; set; }
        public Nullable<Guid> SiteId { get; set; }
        public Nullable<int> ContactsType { get; set; }
        public Nullable<int> Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string LandlineNo { get; set; }
        public string AlternateNo { get; set; }
        public string EmailId { get; set; }
        public string ContactsNotes { get; set; }
    }
}