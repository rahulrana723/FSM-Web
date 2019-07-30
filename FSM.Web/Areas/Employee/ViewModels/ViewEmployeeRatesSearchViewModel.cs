using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class ViewEmployeeRatesSearchViewModel
    {
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public string searchkeyword { get; set; }
    }
}