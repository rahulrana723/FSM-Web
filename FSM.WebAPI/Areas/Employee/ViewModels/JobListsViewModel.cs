using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JobListsViewModel
    {
        public List<SelectListItem> CustomerLastNames { get; set; }
        public List<SelectListItem> CustomerSites { get; set; }
        public List<SelectListItem> JobTypes { get; set; }
        public List<SelectListItem> Status { get; set; }
        public List<SelectListItem> PreferredTime { get; set; }

    }
}