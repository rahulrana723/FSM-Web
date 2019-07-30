using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class ManageVacactionViewModel
    {
        public IEnumerable<VacationViewModel> listVacations { get; set; }
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> SearchStartDate { get; set; }
        [DisplayName("End Date")]
        public Nullable<DateTime> SearchEndDate { get; set; }
        public int PageSize { get; set; }
    }
}