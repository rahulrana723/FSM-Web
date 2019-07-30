using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IParentAction_MasterRepository : IGenericRepository<ParentAction_Master>
    {
        IQueryable<ParentActionCoreViewModel> GetParentActions(Guid RoleId, string[] ModuleId = null);
    }
}
