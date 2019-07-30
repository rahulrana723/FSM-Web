using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class EmployeeRatesRepository : GenericRepository<FsmContext, EmployeeRates>, IEmployeeRatesRepository
    {
        public dynamic GetEmployeeIDS()
        {
            string sql = @"
               SELECT EID from EmployeeDetail ";
            return Context.Database.SqlQuery<dynamic>(sql);
        }
        public IQueryable<EmployeeRateCoreViewModel> GetEmployeeRateDetail()
        {
            string sql = @"sELECT Emp.*,asr.Name Role FROM EmployeeRates EMP
                       inner join EmployeeDetail ed on emp.EID=ed.EID
                       inner join AspNetRoles asr on ed.Role=asr.Id";
            var CustomerSiteList = Context.Database.SqlQuery<EmployeeRateCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<EmployeeRateCoreViewModel> GetEmployeeNameWithEID(string Eid)
        {
            string sql = @" SELECT UserName as EmployeeName,EID from EmployeeDetail
                       where Eid not in (select Eid from EmployeeRates where Eid !='"+Eid+"')order by EmployeeName";
            var CustomerSiteList = Context.Database.SqlQuery<EmployeeRateCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

    }
}
