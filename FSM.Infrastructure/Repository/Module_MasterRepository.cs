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
    public class Module_MasterRepository : GenericRepository<FsmContext, Module_Master>, IModule_MasterRepository
    {
        public IQueryable<ModuleCoreViewModel> GetModulesByRole(Guid RoleId)
        {
            string sql = @"select Module_Master.Id
                           ,Module_Master.ModuleName
                           ,CASE When RoleModuleMapping.ModuleId IS NOT NULL
                                 THEN 1
                                 ELSE 0
                           END IsSelected     
                            from Module_Master
                           LEFT JOIN RoleModuleMapping ON RoleModuleMapping.ModuleId = Module_Master.Id 
                           AND RoleModuleMapping.RoleId = '" + RoleId + "' order by Module_Master.ModuleName";

            var moduleList = Context.Database.SqlQuery<ModuleCoreViewModel>(sql).AsQueryable();
            return moduleList;
        }
    }
}
