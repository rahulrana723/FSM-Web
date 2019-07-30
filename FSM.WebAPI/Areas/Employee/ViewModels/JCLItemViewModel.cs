using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JCLItemViewModel
    {
        public Guid JCLId { get; set; }
        public string ItemName { get; set; } 
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string description { get; set; }
    }
}