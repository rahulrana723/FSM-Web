using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IAction_MasterRepository : IGenericRepository<Action_Master>
    {
        IQueryable<ActionCoreViewModel> GetRoleActionByModule(Guid RoleId, string[] ModuleId = null);
    }
}
