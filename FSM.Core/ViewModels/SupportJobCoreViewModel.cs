using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
   public class SupportJobCoreViewModel
    {
        public Guid Id { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> InvoiceNo { get; set; }
        public Nullable<int> JobType { get; set; }
        public Nullable<DateTime> BookedDate { get; set; }
        public Guid SupportJobId { get; set; }
    }
}
