using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class TimeSheetViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid JobId { get; set; }
        public DateTime JobDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public string Reason { get; set; }
        public int IsRunning { get; set; }
        public string Site { get; set; }
        public string UserName { get; set; }
        public string CustomerLastName { get; set; }
        public string pagenum { get; set; }
        public string jobdateSearch { get; set; }
        public string useridSearch { get; set; }
        public int Job { get; set; }
        public Nullable<int> JobNo { get; set; }
        public string TotalTimeSpent { get; set; }
        public string JobTimeSpent { get; set; }
        public string LunchTimeSpent { get; set; }
        public string PersonnalTimeSpent { get; set; }
    }
}
