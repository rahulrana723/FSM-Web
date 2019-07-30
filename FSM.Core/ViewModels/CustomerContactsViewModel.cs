using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
   public class CustomerContactsCoreViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo1 { get; set; }
        public string EmailId { get; set; }
        public Guid ContactId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public int ContactsType { get; set; }
        public string SiteAddress { get; set; }
        public Nullable<int> Title { get; set; }
    }
}
