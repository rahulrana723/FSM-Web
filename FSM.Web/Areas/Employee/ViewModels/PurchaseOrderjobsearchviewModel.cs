using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseOrderjobsearchviewModel
    {
        public string SearchKeyword { get; set; }
        public int PageSize { get; set; }
        public Guid JobId { get; set; }
    }
}