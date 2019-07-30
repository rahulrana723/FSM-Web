using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface ICustomerGeneralInfoRepository : IGenericRepository<CustomerGeneralInfo>
    {
       string GetMaxCID();
       int GetMaxCTId();
        IQueryable<CustomerGeneralInfoCoreViewModel> GetCustomerListBySearchKeyword(string keyword);
        IQueryable<CustomerGeneralInfoCoreViewModel> GetCustomerNameWithCID(string Cid);
        IQueryable<WorkingHoursModel> GetAvailableWorkingHours(DateTime date,DateTime Endate,string Type);
    }
}
