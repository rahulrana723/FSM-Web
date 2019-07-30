using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class SupportDoJobMappingViewModel
    {
        [Key]
        public Guid ID { get; set; }
        public Guid SupportJobId { get; set; }
        public Nullable<Guid> LinkedJobId { get; set; }
    }
}