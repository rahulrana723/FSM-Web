using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class Module_Master
    {
        public Module_Master()
        {
            this.Action_Masters = new HashSet<Action_Master>();
        }
        [Key]
        public Guid Id { get; set; }
        public string ModuleName { get; set; }
        public string DefaultActionResult { get; set; }
        public virtual ICollection<Action_Master> Action_Masters { get; set; }
    }
}
