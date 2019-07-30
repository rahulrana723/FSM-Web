using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JobsReasonViewModel
    {
        [Key]
        public Guid ReasonId { get; set; }
        public string Reason { get; set; }
        public Guid? JobId { get; set; }
        public DateTime? ReasonTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}