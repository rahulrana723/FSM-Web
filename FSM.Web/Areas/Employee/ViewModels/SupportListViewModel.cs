using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class SupportListViewModel
    {
        public IEnumerable<SupportJobViewModel> supportJobViewModel { get; set; }
        public SupportJobSearchViewModel supportJobSearchViewModel { get; set; }
    }
}