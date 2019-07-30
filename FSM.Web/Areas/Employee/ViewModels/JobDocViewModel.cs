using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class JobDocViewModel
    {
        public List<JobDocumentList> jobDocumentList { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public List<HttpPostedFileBase> JobDocs { get; set; }
    }
}