using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class UploadedInvoiceFile
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
    }
}