using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class SiteNamesViewModel
    {
        public Guid SiteId { get; set; }
        public string SiteName { get; set; }
    }
}