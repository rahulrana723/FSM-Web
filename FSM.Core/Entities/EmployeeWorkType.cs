using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class EmployeeWorkType
    {
        [Key]
        public Guid Id { get; set; }
        //[ForeignKey("EmployeeDetail")]
        public Guid EmployeeId { get; set; }

        public Nullable<int> WorkType { get; set; }
        //public virtual EmployeeDetail EmployeeDetail { get; set; }

    }
}
