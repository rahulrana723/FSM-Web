using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IRoastedOffRepository:IGenericRepository<RoastedOff>
    {
        IQueryable<RoastedOffCoreViewModel> GetRoastedOffList(string keywordSearch);
        string InsertDataIntoVacation(Guid roastedOffId,Guid employeeId,DateTime? startDate,DateTime? endDate,int dayId, int week1,int week2);
    }
}
