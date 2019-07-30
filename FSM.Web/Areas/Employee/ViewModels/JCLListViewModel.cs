using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class JCLListViewModel
    {
        public IEnumerable<JCLDetailListViewModel> jCLDetailListViewModel { get; set; }
        public JCLSearchViewModel jCLSearchViewModel { get; set; }
    }
}