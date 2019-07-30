using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
   public class CustomerBillingAddressCoreViewModel
    {
        public Guid BillingAddressId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public string BillingAddress { get; set; }
        public string Name { get; set; }
        public string PhoneNo1 { get; set; }
        public string EmailId { get; set; }
    }
}
