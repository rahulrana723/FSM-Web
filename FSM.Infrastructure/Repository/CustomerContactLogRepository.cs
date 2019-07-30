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
    public class CustomerContactLogRepository : GenericRepository<FsmContext, CustomerContactLog>, ICustomerContactLogRepository
    {
        public IQueryable<CustomerContactlogcore> GetCustomercontactLogs(string CustomergeneralInfoid,string Keyword)
        {
            try
            {
                Guid Customerid = Guid.Parse(CustomergeneralInfoid);
                if (Keyword != null)
                {
                    Keyword = Keyword.Replace("'", "''");
                }
                string sql = @"select * from(select 
                               Clog.CustomerGeneralInfoId 
                               ,
                               case 
                               when Clog.IsReminder=1 and Clog.IsScheduler=1 
                               then 'Reminder' 
                               when Clog.IsReminder=0 and Clog.IsScheduler=0 
                               then 'ContactLog' 
                               END
                               as Type
                               , Clog.CustomerContactid
                               ,clog.CustomerId
                               ,Clog.JobId 
                               ,Clog.LogDate
                               ,Clog.RecontactDate
                               ,clog.Note
                               ,ANU.UserName as EnteredBy
                               ,Clog.CreatedBy
                               ,Clog.ModifiedBy
                               ,Clog.ModifiedDate
                               ,Clog.CreatedDate
                               ,jobs.JobNo as ViewJobid
                                ,ISNULL(NULLIF(street, ''), '  ') + ISNULL(NULLIF(StreetName, '') + ', ', '')+
                                ISNULL(NULLIF(Suburb, ''), ' ')+ISNULL(NULLIF(state, ''), ' ')+ CONVERT(varchar(10),PostalCode) as SiteName    
                               ,cs.SiteFileName                               
                               from CustomerContactLog Clog
                               left join AspNetUsers ANU on clog.CreatedBy=ANU.Id
                               left join Jobs on Clog.JobId = cast(jobs.Id as nvarchar(50))
                               left join CustomerSiteDetail cs on cs.SiteDetailId = Jobs.SiteId
                               Where Clog.IsDelete=0
                               )t";
                if (!string.IsNullOrEmpty(Keyword))
                {
                    sql = sql + " where t.CustomerGeneralInfoId='" + Customerid + "' and (t.ViewJobId like '%" + Keyword + "%' or t.SiteFileName like '%" + Keyword + "%' or t.Note like '%" + Keyword + "%')";
                }
                else
                {
                    sql = sql + " where t.CustomerGeneralInfoId='" + Customerid + "'";
                }
                var CustomercontractList = Context.Database.SqlQuery<CustomerContactlogcore>(sql).AsQueryable();
                return CustomercontractList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IQueryable<CustomerContactlogcore> GetJobscontactLogs(int? jobNo,string CustomergeneralInfoid, string Keyword)
        {
            try
            {
                Guid Customerid = Guid.Parse(CustomergeneralInfoid);
                if (Keyword != null)
                {
                    Keyword = Keyword.Replace("'", "''");
                }
                string sql = @"select * from(select Clog.CustomerGeneralInfoId 
                                 , case 
                               when Clog.IsReminder=1 and Clog.IsScheduler=1 
                               then 'Reminder' 
                               when Clog.IsReminder=0 and Clog.IsScheduler=0 
                               then 'ContactLog' 
                               END
                               as Type
                                , Clog.CustomerContactid
                                ,clog.CustomerId
                                ,Clog.JobId 
                                ,Clog.LogDate
                                ,Clog.RecontactDate
                                ,clog.Note
                                ,ANU.UserName as EnteredBy
                                ,Clog.CreatedBy
                                ,Clog.ModifiedBy
                                ,Clog.ModifiedDate
                                ,Clog.CreatedDate
                                ,jobs.JobNo as ViewJobid
                                 ,ISNULL(NULLIF(street, ''), ' ') + ISNULL(NULLIF(StreetName, '') + ', ', '')+ISNULL(NULLIF(Suburb, ''), ' ')+ISNULL(NULLIF(state, ''), ' ')+ CONVERT(varchar(10),PostalCode) as SiteName    
                                from CustomerContactLog Clog
                                LEFT join AspNetUsers ANU on clog.CreatedBy=ANU.Id
                                LEFT join Jobs on Clog.JobId = cast(jobs.Id as nvarchar(50))
                                LEFT join CustomerSiteDetail cs on cs.SiteDetailId = Jobs.SiteId
                                Where Clog.IsDelete=0)t";
                if (!string.IsNullOrEmpty(Keyword))
                {
                    sql = sql + " where t.ViewJobId='" + jobNo + "' and (t.ViewJobId like '%" + Keyword + "%' or t.SiteName like '%" + Keyword + "%' or t.Note like '%" + Keyword + "%')";
                }
                else
                {
                    sql = sql + " where t.ViewJobId='" + jobNo + "'";
                }
                var CustomercontractList = Context.Database.SqlQuery<CustomerContactlogcore>(sql).AsQueryable();
                return CustomercontractList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IQueryable<JobDataVM> GetJobByContactLog(string ContactLogId)
        {
            try
            {
                string sql = @"select j.Id, j.JobNo, cs.SiteFileName from CustomerContactLog cl
                               join jobs j on cl.JobId=j.Id
                               join CustomerSiteDetail cs on cs.SiteDetailId=j.SiteId
                               where cl.CustomerContactId='" + ContactLogId + "'";
                var jobData = Context.Database.SqlQuery<JobDataVM>(sql).AsQueryable();
                return jobData;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
