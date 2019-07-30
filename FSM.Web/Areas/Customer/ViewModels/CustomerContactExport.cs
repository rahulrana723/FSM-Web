using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerContactExport
    {
        public Guid CustomerContactId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }

        [DisplayName("Customer Id")]
        public string CustomerId { get; set; }
        [DisplayName("Job Id")]
        public string JobId { get; set; }
        [DisplayName("Log Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string LogDate { get; set; }
        [DisplayName("ReContact Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string ReContactDate { get; set; }
        public string Note { get; set; }
        public string PageNum { get; set; }
    }
}