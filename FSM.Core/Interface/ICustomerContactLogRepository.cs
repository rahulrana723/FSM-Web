using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface ICustomerContactLogRepository : IGenericRepository<CustomerContactLog>
    {
        IQueryable<CustomerContactlogcore> GetCustomercontactLogs(string CustomergeneralInfoid, string Keyword = "");
        IQueryable<CustomerContactlogcore> GetJobscontactLogs(int? jobNo,string CustomergeneralInfoid, string Keyword = "");
        IQueryable<JobDataVM> GetJobByContactLog(string ContactLogId);
    }
}
