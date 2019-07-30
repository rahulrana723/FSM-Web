using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class JCLDetailListViewModel
    {
        public Guid JCLId { get; set; }
        public string ItemName { get; set; }
        //public Nullable<decimal> BonusPerItem { get; set; }
        public Nullable<int> DefaultQty { get; set; }
        public Nullable<decimal> Price { get; set; }
        //public Nullable<decimal> BonusMinimum { get; set; }
        public string Description { get; set; }
    }
}