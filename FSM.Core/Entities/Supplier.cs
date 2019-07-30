using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
 public   class Supplier
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
    }
}
