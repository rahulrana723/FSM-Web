using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class DisplayJobStocksListViewModel
    {
        public DisplayJobStocksViewModel DisplayJobStocksViewModel { get; set; }
        public IEnumerable<JobStockListCoreViewModel> DisplayJobStocksList { get; set; }
        public JobStock JobStock { get; set; }
    }
}