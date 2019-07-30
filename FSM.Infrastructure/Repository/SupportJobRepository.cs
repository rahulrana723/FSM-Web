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
    public class SupportJobRepository : GenericRepository<FsmContext, SupportJob>, ISupportJobRepository
    {
        public IQueryable<SupportJobCoreViewModel> GetJobsForSupport()
        {

            string sql = @"select distinct js.jobid,js.JobType,js.DateBooked BookedDate,i.InvoiceNo
							  from 
							  dbo.Jobs js
							  left join dbo.invoice i on i.JobId=js.JobId
							  where js.jobtype!='"+ 3 +"'";


            var SupportJobList = Context.Database.SqlQuery<SupportJobCoreViewModel>(sql).AsQueryable();
            return SupportJobList;
        }

        public IQueryable<SupportJobCoreViewModel> GetSupportJobListBySearchKeyword(string keyword)
        {
            string sql = @"select * from invoice t
                            Where t.InvoiceNo Like '%" + keyword + "%' Or t.ContactName Like '%" + keyword + "%'  Or t.PhoneNo Like '%" + keyword + "%'  Or t.BillingStreetName Like '%" + keyword + "%'";

            var CustomerSiteList = Context.Database.SqlQuery<SupportJobCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }
    }
}
