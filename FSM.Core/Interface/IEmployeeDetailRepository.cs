using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IEmployeeDetailRepository : IGenericRepository<EmployeeDetail>
    {
        dynamic GetEmployeeDetailById();
        List<string> GetNoRateEmployeesByEID();
        IQueryable<EmployeeDetailViewModelCore> GetEmployeeDetail(string searchKeyword = "");
        string GetMaxEmployeeId();
        IQueryable<EmployeeDetailViewModelDashboard> GetEmployeeDetailDashboard();
        IQueryable<EmployeeDetailViewModelCore> getUserRole(string RoleId);
        IQueryable<EmployeeDetailViewModelCore> GetEmployeeDetailWithUnreadMessage(string LoggedId = "", string searchKeyword = "");
        int GetTotalMessageCount(string LoggedId = "");
        IQueryable<GetLatitudeLongitudeCoreviewModel> GetLatitudeLongitude(string UserIds);
        IQueryable<EmployeeDetailViewModelCore> GetUser();
        string UpdateOtrwOrder(int? otrwOrder);
        string DeleteAllReleatedDataByEmployeeid(string emloyeeId,string userID);
    }
}
