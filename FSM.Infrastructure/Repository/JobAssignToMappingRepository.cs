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
    public class JobAssignToMappingRepository: GenericRepository<FsmContext, JobAssignToMapping>, IJobAssignToMappingRepository
    {
        public IQueryable<GetSuperVisorCoreViewModel> GetSuperVisorList(Guid JobId)
        {
            string sql = @"select distinct JAP.AssignTo,ed.UserName OTRWUserName from JobAssignToMapping JAP
                         inner Join EmployeeDetail Ed on JAP.AssignTo=Ed.EmployeeId
                         where jap.JobId='" + JobId+"' and jap.IsDelete=0";

            var OtrwDetail = Context.Database.SqlQuery<GetSuperVisorCoreViewModel>(sql).AsQueryable();
            return OtrwDetail;
        }

        public IQueryable<JobDataVM> GetJobDataByDate(DateTime DateBooked)
        {
            string sql = @"select distinct j.JobNo, j.Id, ltrim(cs.SiteFileName) as SiteFileName from JobAssignToMapping jm
                            join Jobs j on jm.JobId=j.Id
                            join CustomerSiteDetail cs on j.SiteId=cs.SiteDetailId
                            where jm.DateBooked='" + DateBooked.ToString("MM/dd/yyyy") +
                            "' and j.IsDelete=0 and jm.IsDelete=0 order by ltrim(cs.SiteFileName) asc";

            var jobData = Context.Database.SqlQuery<JobDataVM>(sql).AsQueryable();
            return jobData;
        }

        public IQueryable<JobDataViewModel> GetJobByDateBooked(DateTime dateBooked)
        {
            string sql = @"select distinct j.JobNo, j.Id, ltrim(cs.SiteFileName) as SiteFileName from JobAssignToMapping jm
                           join Jobs j on jm.JobId=j.Id
                           join CustomerSiteDetail cs on j.SiteId=cs.SiteDetailId
                           where jm.IsDelete=0 AND jm.DateBooked='" + dateBooked.ToString("MM/dd/yy") +
                           "' and j.IsDelete=0 order by ltrim(cs.SiteFileName) asc ";

            var jobData = Context.Database.SqlQuery<JobDataViewModel>(sql).AsQueryable();
            return jobData;
        }
    }
}
