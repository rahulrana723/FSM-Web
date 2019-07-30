using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class VacationRepository : GenericRepository<FsmContext, Vacation>, IVacationRepository
    {
        public IQueryable<Vacation> GetExistVacationDate(DateTime? startDate, DateTime? endDate, Guid Id, Guid? employeeId)
        {
            var StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            var EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : string.Empty;

            string sql = @"SELECT * FROM Vacation
                WHERE ((StartDate <= '"+ StartDate + "' AND EndDate >= '"+ StartDate + "')or (StartDate<= '"+ EndDate + "' AND EndDate >= '"+ EndDate + "'))AND Id !='"+Id+ "'AND EmployeeId ='"+employeeId+"'";
            return Context.Database.SqlQuery<Vacation>(sql).AsQueryable();
        }
        public IQueryable<Guid>CheckEmployeeLeave(Guid AssignTo,DateTime? DateFormated)
        {
            var Date = DateFormated.HasValue ? DateFormated.Value.ToString("yyyy-MM-dd") : string.Empty;

            string sql = @"SELECT Id FROM Vacation
                WHERE ((StartDate <= '" + Date + "' AND EndDate >= '" + Date + "')or (StartDate<= '" + Date + "' AND EndDate >= '" + Date + "'))AND EmployeeId ='" + AssignTo + "'AND Status=2 AND RoastedOffId Is Null";
            var hasJob = Context.Database.SqlQuery<Guid>(sql).AsQueryable();
            return hasJob;
        }
        public IQueryable<string> CheckUserLeave(Guid AssignTo, DateTime? DateFormated)
        {
            var Date = DateFormated.HasValue ? DateFormated.Value.ToString("yyyy-MM-dd") : string.Empty;
            DayOfWeek DayOfWeek;
            string sqlForDayHrs = "";

            DayOfWeek = Convert.ToDateTime(Date).DayOfWeek;    //Get Day 

            switch (Convert.ToString(DayOfWeek))
            {
                case "Monday":
                    sqlForDayHrs = " (MondayHrs = 0 OR MondayHrs IS NULL)";
                    break;
                case "Tuesday":
                    sqlForDayHrs = " (TuesdayHrs = 0 OR TuesdayHrs IS NULL)";
                    break;
                case "Wednesday":
                    sqlForDayHrs = " (WednesdayHrs = 0 OR WednesdayHrs IS NULL)";
                    break;
                case "Thursday":
                    sqlForDayHrs = " (ThursdayHrs = 0 OR ThursdayHrs IS NULL)";
                    break;
                case "Friday":
                    sqlForDayHrs = " (FridayHrs = 0 OR FridayHrs IS NULL)";
                    break;
                case "Saturday":
                    sqlForDayHrs = " (Saturdayhrs = 0 OR Saturdayhrs IS NULL)";
                    break;
                case "Sunday":
                    sqlForDayHrs = " (SundayHrs = 0 OR SundayHrs IS NULL)";
                    break;
                default:
                    sqlForDayHrs = "";
                    break;
            }


            string sql = @"SELECT Emp.UserName FROM Vacation
                inner Join EmployeeDetail Emp on Vacation.EmployeeId=Emp.EmployeeId
                WHERE ((StartDate <= '" + Date + "' AND EndDate >= '" + Date + "')or (StartDate<= '" + Date + "' AND EndDate >= '" + Date + "'))AND Vacation.EmployeeId ='" + AssignTo + "'AND Status=2 AND RoastedOffId Is Null Union select Emp.UserName from EmployeeDetail Emp where(" + sqlForDayHrs+ ")and IsDelete = 0 and IsActive = 1 and ROLE = '31cf918d-b8fe-4490-b2d7-27324bfe89b4' AND EmployeeId='"+AssignTo+"'";
            var hasJob = Context.Database.SqlQuery<string>(sql).AsQueryable();
            return hasJob;
        }

        public IQueryable<string> CheckUserLeave(Guid AssignTo, DateTime? DateFormated, Guid JobId)
        {
            var Date = DateFormated.HasValue ? DateFormated.Value.ToString("yyyy-MM-dd") : string.Empty;
            DayOfWeek DayOfWeek;
            string sqlForDayHrs = "";

            DayOfWeek = Convert.ToDateTime(Date).DayOfWeek;    //Get Day 

            switch (Convert.ToString(DayOfWeek))
            {
                case "Monday":
                    sqlForDayHrs = " (MondayHrs = 0 OR MondayHrs IS NULL)";
                    break;
                case "Tuesday":
                    sqlForDayHrs = " (TuesdayHrs = 0 OR TuesdayHrs IS NULL)";
                    break;
                case "Wednesday":
                    sqlForDayHrs = " (WednesdayHrs = 0 OR WednesdayHrs IS NULL)";
                    break;
                case "Thursday":
                    sqlForDayHrs = " (ThursdayHrs = 0 OR ThursdayHrs IS NULL)";
                    break;
                case "Friday":
                    sqlForDayHrs = " (FridayHrs = 0 OR FridayHrs IS NULL)";
                    break;
                case "Saturday":
                    sqlForDayHrs = " (Saturdayhrs = 0 OR Saturdayhrs IS NULL)";
                    break;
                case "Sunday":
                    sqlForDayHrs = " (SundayHrs = 0 OR SundayHrs IS NULL)";
                    break;
                default:
                    sqlForDayHrs = "";
                    break;
            }


            string sql = @"SELECT Emp.UserName FROM Vacation
                inner Join EmployeeDetail Emp on Vacation.EmployeeId=Emp.EmployeeId
                WHERE ((StartDate <= '" + Date + "' AND EndDate >= '" + Date + "')or (StartDate<= '" + Date + "' AND EndDate >= '" + Date + "'))AND Vacation.EmployeeId ='" + 
                AssignTo + "'AND Status=2 AND RoastedOffId Is Null Union select Emp.UserName from EmployeeDetail Emp where(" + sqlForDayHrs 
                + ")and IsDelete = 0 and IsActive = 1 and ROLE = '31cf918d-b8fe-4490-b2d7-27324bfe89b4' AND EmployeeId='" +
                AssignTo + "'  and EmployeeId not in (Select distinct userid from Usertimesheet where jobid ='" +
                JobId + "') ";
            var hasJob = Context.Database.SqlQuery<string>(sql).AsQueryable();
            return hasJob;
        }
    }
}
