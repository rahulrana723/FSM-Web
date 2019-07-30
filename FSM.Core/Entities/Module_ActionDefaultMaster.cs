using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class Module_ActionDefaultMaster
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }
        public string ActionResult { get; set; }
        public string Controller { get; set; }
    }
}
