using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class StockListViewModel
    {
        public List<SelectListItem> Stock { get; set; }
    }
}