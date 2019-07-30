using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface ICustomerBillingAddressRepository : IGenericRepository<CustomerBillingAddress>
    {
        IQueryable<CustomerBillingAddressCoreViewModel> GetBillingAddressList(Guid CustomerGeneralInfoId, string Keyword = "");
        void UpdateDefaultAddress(Guid customerGeneralInfoId);
    }
}
