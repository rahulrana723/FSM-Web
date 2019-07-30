using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSM.Core.ViewModels;

namespace FSM.Infrastructure.Repository
{
    public class ModuleActionMasterRepository : GenericRepository<FsmContext, Module_ActionDefaultMaster>, IModule_ActionDefaultMaster
    {
        public IQueryable<ModuleActionMasterCore> GetDefaultAction(string ModuleName, string Controller, string Action)
        {
            string sql = @"select MM.ModuleName,MADM.Controller,MADM.ActionResult from dbo.Module_Master MM
                           join dbo.Module_ActionDefaultMaster MADM on MM.Id = MADM.ModuleId
                           where MM.ModuleName='" + ModuleName + "' and MADM.Controller='" + Controller +
                           "' and MADM.ActionResult='" + Action + "'";

            var defaultAction = Context.Database.SqlQuery<ModuleActionMasterCore>(sql).AsQueryable();
            return defaultAction;
        }
    }
}
