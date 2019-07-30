using FSM.Core.ViewModels;
using FSM.Web.Areas.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CustomerEmailSendListViewModel
    {
        public CustomerEmailSendViewModel CustomerEmailsendviewModel { get; set; }
        public IEnumerable<EmployeeJobDocumentViewModel> displayEmployeeJobDocumentViewModel { get; set; }
        public IEnumerable<ImportantDocumentViewModel> displayImportantDocumentViewModel { get; set; }
        public IEnumerable<CustomerSitesDocumentsViewModel> displaySiteDocumentViewModel { get; set; }
        public Nullable<Guid> EmployeeJobId { get; set; }
    }
}