using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class JobDocumentList
    {
        public Guid Id { get; set; }
        public string DocName { get; set; }
        public string SaveDocName { get; set; }
        public string DocType { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
    }
}