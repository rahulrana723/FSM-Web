using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class ContactLogSearchModel
    {
        public Guid CustomerGeneralInfoId { get; set; }
        public string Keyword { get; set; }
    }
}