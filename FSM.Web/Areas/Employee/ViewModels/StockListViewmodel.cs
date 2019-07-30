using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class StockListViewmodel
    {
        public IEnumerable<StockViewModel> stocklistviewmodel { get; set; }
        public StocksearchViewmodel Stockserarchviewmodel { get; set; }
    }
}