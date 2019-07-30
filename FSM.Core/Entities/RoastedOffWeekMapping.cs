using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class RoastedOffWeekMapping
    {
        [Key]
        public Guid ID { get; set; }
        [ForeignKey("RoastedOff")]
        public Guid RoastedOffId { get; set; }
        public Nullable<int> WeekID { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModeifiedBy { get; set; }
        public virtual RoastedOff RoastedOff { get; set; }
    }
}
