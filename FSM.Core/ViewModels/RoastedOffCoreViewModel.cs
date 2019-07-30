using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class RoastedOffCoreViewModel
    {
        public Guid ID { get; set; }
       
        public Guid OTRWId { get; set; }
        public string UserName { get; set; }
        public Nullable<int> DayId { get; set; }
        public string Days { get; set; }

        public string Weeks { get; set; }
    }
}
