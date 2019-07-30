using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class OTRWStockListViewmodel
    {
        public IEnumerable<OTRWStockViewmodel> OTRWStockViewListmodel { get; set; }
        public OTRWStocksearchviewmodel OTRWStockSearchmodel { get; set; }

        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<int> Availability { get; set; }

    }
}