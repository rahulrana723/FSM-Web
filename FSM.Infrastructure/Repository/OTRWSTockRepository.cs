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
    public class OTRWSTockRepository : GenericRepository<FsmContext, OTRWStock>, IOTRWStockRepository
    {
        public IQueryable<OTRWSearchViewmodel> GetAssignedStockOTRW(string id, string serachkey)
        {
            Guid stockid = Guid.Parse(id);

            string sql = @"Select * from (SELECT DISTINCT EMP.FirstName +' '+ EMP.LastName As Detail
                            ,'OTRW' Type,2 as Typeid,
                            OTRWS.Quantity FROM OTRwStock OTRWS 
                            INNER JOIN Stock ST ON ST.Id = OTRWS.StockID   
                            INNER JOIN EmployeeDetail EMP  on OTRWS.OTRWID=EMP.EmployeeId            
                            WHERE ST.Id='" + stockid + "' UNION SELECT DISTINCT 'JobId_'+CAST(JB.jobNo as nvarchar(50)) As Detail,'JOB' Type,1 as Typeid,JBS.Quantity FROM JobStock JBS INNER JOIN Stock ST ON ST.Id = JBS.StockID   INNER JOIN JOBS JB  on JB.Id=JBS.JobId WHERE ST.Id='" + stockid + "')t where t.Detail like '%"+ serachkey + "%' or t.Type like '%" + serachkey + "%' or t.Quantity like '%" + serachkey + "%'";
            return Context.Database.SqlQuery<OTRWSearchViewmodel>(sql).AsQueryable();
        }
    }
}


