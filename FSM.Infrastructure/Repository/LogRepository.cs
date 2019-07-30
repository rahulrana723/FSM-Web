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
    public class LogRepository : GenericRepository<FsmContext, Log>, ILogRepository
    {
        public IQueryable<LogDetailsCoreViewModel> GetLogDetail()
        {
            IQueryable<LogDetailsCoreViewModel> logList = from log in Context.Log
                                                          join user in Context.AspNetUsers on log.UserId equals user.Id
                                                          join employee in Context.EmployeeDetail on user.Id equals (employee.EmployeeId).ToString()
                                                          where log.Level == "INFO"
                                                          select new LogDetailsCoreViewModel()
                                                          {
                                                              Date = log.Date,
                                                              Message = log.Message,
                                                              UserName = user.UserName,
                                                              FullName = employee.FirstName + " " + employee.LastName
                                                          };

            //string sql = @"select Message,cast(Date as date) Date from log ";

            //var logList = Context.Database.SqlQuery<LogDetailsCoreViewModel>(sql).AsQueryable();
            return logList;
        }
    }
}
