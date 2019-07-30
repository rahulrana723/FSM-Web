using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class JobContactsVM
    {
        public Guid ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SiteFileName { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public Nullable<DateTime> DateBooked { get; set; }
    }
}
