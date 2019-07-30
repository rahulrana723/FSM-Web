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
    public class RoastedOffRepository : GenericRepository<FsmContext, RoastedOff>, IRoastedOffRepository
    {
        public IQueryable<RoastedOffCoreViewModel> GetRoastedOffList(string keywordSearch)
        {

            string sql = @"SELECT * FROM(select RF.ID, RF.OTRWId,ED.UserName,RF.DayId
                            ,Weeks = 
                            STUFF((SELECT ', Week' + cast(WeekID as nvarchar(50))
                            FROM RoastedOffWeekMapping  
                            WHERE RoastedOffWeekMapping.RoastedOffId = RF.Id
                            FOR XML PATH('')), 1, 2, '')
                            from 
                            dbo.RoastedOff RF
                            join dbo.EmployeeDetail ED on ED.EmployeeId=RF.OTRWId
                            where RF.IsDelete=0)t
                            Where t.UserName Like '%" + keywordSearch + "%' OR t.Weeks Like '%" + keywordSearch + "%'";


            var RoastedList = Context.Database.SqlQuery<RoastedOffCoreViewModel>(sql).AsQueryable();
            return RoastedList;

        }

        public string InsertDataIntoVacation(Guid roastedOffId, Guid employeeId, DateTime? startDate, DateTime? endDate, int dayId, int week1, int week2)
        {
            string startTime="";
            string endTime = "";
            startTime = "'" + Convert.ToDateTime(startDate).ToString("MMM yyyy") + "'";
            endTime = "'" + Convert.ToDateTime(endDate).ToString("MMM yyyy") + "'";
            string sql = @"DECLARE @dow int,
                          @StartDate DATETIME,
                          @EndDate DATETIME

                          SELECT @dow = "+dayId +@", 
                          @StartDate = "+ startTime + @",
                          @EndDate = "+ endTime + @"

                          ;WITH CTE(mth) AS (
                          SELECT @StartDate mth UNION ALL
                          SELECT DATEADD(month,1,mth) FROM CTE
                          WHERE DATEADD(month,1,mth) <= @EndDate
                          )
                             insert into Vacation(Id,EmployeeId,StartDate,EndDate,Hours,Reason,Status,RoastedOffId,IsDelete)
                             SELECT
                             NEWID(),
                             '" + employeeId + @"',
                             DATEADD(DAY, @dow +
                             CASE WHEN DATEPART(dw,mth) > @dow THEN " + week1+@"
                             ELSE "+week2+ @"
                             END
                             - DATEPART(dw, mth), mth) as 'StartDate',
                             DATEADD(DAY, @dow +
                             CASE WHEN DATEPART(dw,mth) > @dow THEN " + week1 + @"
                             ELSE " + week2 + @"
                             END
                             - DATEPART(dw, mth), mth) as 'EndDate'
                             ,'9','Roasted Off','2','" + roastedOffId + @"','0'
                             FROM CTE
                             SELECT '0'";
            return Convert.ToString(Context.Database.SqlQuery<string>(sql).ToList());
        }
    }
}
