using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
   public class CustomerSitesDocumentsViewModelCore
    {
        public Guid SiteDetailId { get; set; }
        public Guid DocumentId { get; set; }
        public string StreetName { get; set; }
        public int filecount { get; set; }
        public string SiteAddress { get; set; }
        public string DocumentName { get; set; }
        public string DocType { get; set; }
    }
}
