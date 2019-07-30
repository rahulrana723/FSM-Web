using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IVacationRepository:IGenericRepository<Vacation>
    {
        IQueryable<Vacation> GetExistVacationDate(DateTime? startDate,DateTime? endDate,Guid Id,Guid? employeeId);
        IQueryable<Guid> CheckEmployeeLeave(Guid AssignTo, DateTime? AssignedDate);
        IQueryable<string> CheckUserLeave(Guid AssignTo, DateTime? AssignedDate);

        IQueryable<string> CheckUserLeave(Guid AssignTo, DateTime? AssignedDate, Guid JobId);
    }
}
