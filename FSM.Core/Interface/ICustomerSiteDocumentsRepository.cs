using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface ICustomerSiteDocumentsRepository:IGenericRepository<CustomerSitesDocuments>
    {
        IQueryable<CustomerSitesDocumentsViewModelCore> GetSiteDocumentList(string keywordSearch,Guid id);
        IQueryable<DisplaySiteDocumentsCountViewModel> GetCustomerSiteDocumentsList(string CustomerGeneralInfoId); 
        IQueryable<CustomerSitesDocumentsViewModelCore> GetJobSiteDocumentList(string keywordSearch, Guid id);
        IQueryable<CustomerSitesDocumentsViewModelCore> JobsSiteDocumentList(string keywordSearch, Guid id);
        IQueryable<CustomerSitesDocumentsViewModelCore> GetSiteCountForJob(Guid siteid);
        IQueryable<CustomerSitesDocumentsViewModelCore> GetSiteCountForCustomer(Guid CustGeneralInfoId); 
    }
}
