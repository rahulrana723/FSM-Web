using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class ImportantDocumentVM
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public HttpPostedFileBase FilePosted { get; set; }
        public string FilePath { get; set; }
        public string DocType { get; set; }
        public bool IsTermAndConditionDoc { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
    }
}