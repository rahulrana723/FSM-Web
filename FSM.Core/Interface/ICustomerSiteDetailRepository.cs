using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface ICustomerSiteDetailRepository : IGenericRepository<CustomerSiteDetail>
    {
        IQueryable<DisplaySitesViewModel> GetCustomerSiteList(string CustomerGeneralInfoId);
        IQueryable<DisplaySitesViewModel> GetCustomerSiteDetails(string Sid);
        string GetSiteAddress(Guid SiteId);
    }
}
