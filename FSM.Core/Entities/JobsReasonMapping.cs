using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class JobsReasonMapping
    {
        [Key]
        public Guid ReasonId { get; set; }
        public string Reason { get; set; }
        public Guid? JobId { get; set; }
        [DisplayName("Reason Time")]
        public DateTime? ReasonTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
