using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class PublicHoliday
    {
        [Key]
        public Guid Id { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string Reason { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
