using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IModule_ActionDefaultMaster : IGenericRepository<Module_ActionDefaultMaster>
    {
        IQueryable<ModuleActionMasterCore> GetDefaultAction(string ModuleName, string Controller, string Action);
    }
}
