using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class GetJobDocuments
    {
        public Nullable<Guid> Id { get; set; }
        public string ImageName { get; set; }
        public string DocName { get; set; }
        public string DocType { get; set; }
    }
}