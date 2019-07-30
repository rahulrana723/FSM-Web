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
    public class EmployeeWorkTypeRepository : GenericRepository<FsmContext, EmployeeWorkType>, IEmployeeWorkTypeRepository
    {
        public IQueryable<GetWorkTypeCoreViewModel> GetOTRWUserUsingByWorkType(int? Worktype)
        {
            string sql = @"select EWT.EmployeeId OTRWID,Ed.UserName OTRWUserName from EmployeeWorkType EWT
                         inner join EmployeeDetail Ed on EWT.EmployeeId=Ed.EmployeeId
                         where Isactive=1 and Ed.isdelete = 0 and role = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' and  EWT.WorkType='" + Worktype + "' ";

            var OtrwDetail = Context.Database.SqlQuery<GetWorkTypeCoreViewModel>(sql).AsQueryable();
            return OtrwDetail;
        }
    }
}
