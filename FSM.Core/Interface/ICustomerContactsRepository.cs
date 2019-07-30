using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
  public interface ICustomerContactsRepository : IGenericRepository<CustomerContacts>
    {
        IQueryable<CustomerContactsCoreViewModel> GetCustomerContactsInfo(Guid CustomerGeneralInfoId,string SiteId,int ContactType, string searchKeywords = "");
        IQueryable<CustomerContactsCountViewModel> GetCustomerContactsList(string CustomerGeneralInfoId); 
        IQueryable<CustomerContactsCoreViewModel> GetContactsInfo(Guid CustomerGeneralInfoId,string Keyword="");
        IQueryable<CustomerContactsCoreViewModel> GetJobContactsInfo(Guid CustomerGeneralInfoId,Guid siteId,int ContactType, string Keyword = "");
        IQueryable<JobContactsVM> GetJobContactList(string jobIds);
    }
}
