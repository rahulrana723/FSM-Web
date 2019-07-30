using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class CompulsaryTask
    {
        [Key]
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
    }
}
