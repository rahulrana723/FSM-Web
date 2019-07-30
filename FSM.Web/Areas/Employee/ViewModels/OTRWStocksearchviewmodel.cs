using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class OTRWStocksearchviewmodel
    {
        public Nullable<Guid> Stockid { get; set; }
        public string searchkeyword { get; set; }
        public int PageSize { get; set; }
        public string StockLabel { get; set; }
        public int Type { get; set; }
    }
}