using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JCLSizeViewModel
    {
        public Guid Id { get; set; }
        public Guid JCLId { get; set; }
        public string Size { get; set; }
    }
}