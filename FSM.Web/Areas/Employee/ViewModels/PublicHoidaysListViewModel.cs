using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PublicHoidaysListViewModel
    {
        public IEnumerable<PublicHolidayViewModel> PublicHolidayViewModel { get; set; }
        public HolidaySearchViewModel HolidaySearchViewModel { get; set; }
    }
}