using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class ModuleCoreViewModel
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; }
        public int IsSelected { get; set; }
    }
}
