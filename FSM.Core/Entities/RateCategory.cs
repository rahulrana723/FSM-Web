using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class RateCategory
    {
        [Key]
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
