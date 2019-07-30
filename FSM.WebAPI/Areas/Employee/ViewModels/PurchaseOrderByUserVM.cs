using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class PurchaseOrderByUserVM
    {
        public string Id { get; set; }
        public string JobId { get; set; }
        public string JobKey { get; set; }
        public string PO { get; set; }
        public string SupplierName { get; set; }
    }
}