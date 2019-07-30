using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class JobDataViewModel
    {
        public Guid Id { get; set; }
        public int JobNo { get; set; }
        public string SiteFileName { get; set; }
    }
}
