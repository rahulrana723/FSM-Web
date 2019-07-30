using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class GetHours
    {
        public Nullable<DateTime> DateValue { get; set; }
        public int HoursAvailable { get; set; }
        public int HoursBooked { get; set; }
    }
}
