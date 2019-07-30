using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class JobPerFormanceCoreViewModel
    {
        public Nullable<decimal> SaleIncome { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public Nullable<decimal> LabourIncome { get; set; }
        public Nullable<decimal> LIPHR { get; set; }
        public Nullable<decimal> RevHours { get; set; }
        public Nullable<decimal> JSPOCost { get; set; }
        public Nullable<decimal> LabourCost { get; set; }
        public Nullable<decimal> LCPHR { get; set; }
        public Nullable<decimal> NRLHours { get; set; }
        public Nullable<decimal> StockItemCost { get; set; }
        public Nullable<decimal> LabourProfit { get; set; }
        public Nullable<decimal> LPPHR { get; set; }
        public Nullable<decimal> Hours { get; set; }
       
        public Nullable<decimal> SalesBonus { get; set; }
    }
}
