using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class SupplierListViewModel
    {
        public List<SelectListItem> SupplierName { get; set; }
    }
}