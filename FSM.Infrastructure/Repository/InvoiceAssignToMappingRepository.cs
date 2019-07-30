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
    public class InvoiceAssignToMappingRepository : GenericRepository<FsmContext, InvoiceAssignToMapping>, IInvoiceAssignToMappingRepository
    {
        public IQueryable<GetSuperVisorCoreViewModel> GetSuperVisorList(Guid JobId)
        {
            string sql = @"select distinct IAP.AssignTo,ed.UserName OTRWUserName from InvoiceAssignToMapping IAP
                         inner Join EmployeeDetail Ed on IAP.AssignTo=Ed.EmployeeId
                         where iap.JobId='" + JobId + "'";

            var OtrwDetail = Context.Database.SqlQuery<GetSuperVisorCoreViewModel>(sql).AsQueryable();
            return OtrwDetail;
        }
    }
}
