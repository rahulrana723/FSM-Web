using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class WorkType
    {
        [Key]
        public int Value { get; set; }
        public string WorkTypeName { get; set; }
    }
}
