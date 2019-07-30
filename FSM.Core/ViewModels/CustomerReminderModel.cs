using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class CustomerReminderViewModel
    {
        [Key]
        public Guid CustomerContactId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public string CustomerId { get; set; }
        public string JobId { get; set; }
        public Nullable<int> ViewJobid { get; set; }
        public Nullable<DateTime> LogDate { get; set; }
        public Nullable<DateTime> ReContactDate { get; set; }
        public string Note { get; set; }
        public string SiteName { get; set; }
        public string EnteredBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}
