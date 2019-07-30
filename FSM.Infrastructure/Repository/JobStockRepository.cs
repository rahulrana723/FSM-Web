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
    public class JobStockRepository:GenericRepository<FsmContext,JobStock>,IJobStockRepository
    {
        public IQueryable<JobStockListCoreViewModel> GetJobStockList()
        {
            string sql = @"select s.ID as StockId, s.Label,js.UnitMeasure,js.Price,js.Quantity,js.ID,js.jobid as Jobid
							from
							dbo.JobStock js
							join dbo.Stock s on s.ID=js.StockID where js.IsDelete=0";

            var CustomerjobStockList = Context.Database.SqlQuery<JobStockListCoreViewModel>(sql).AsQueryable();
            return CustomerjobStockList;
        }
    }
}
