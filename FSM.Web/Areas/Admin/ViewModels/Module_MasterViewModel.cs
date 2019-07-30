using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class Module_MasterViewModel
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; }
        public bool IsSelected { get; set; }
    }
}