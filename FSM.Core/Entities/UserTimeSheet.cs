using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class UserTimeSheet
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid JobId { get; set; }
        public DateTime JobDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public string Reason { get; set; }
        public string ReasonToUpdate { get; set; }
        public int IsRunning { get; set; }
        public Nullable<int> JobType { get; set; }
        public int IsFirstTraveling { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
