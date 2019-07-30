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
    public class Action_MasterRepository : GenericRepository<FsmContext, Action_Master>, IAction_MasterRepository
    {
        public IQueryable<ActionCoreViewModel> GetRoleActionByModule(Guid RoleId, string[] ModuleId = null)
        {
            ModuleId = ModuleId != null ? ModuleId.Select(x => x.Replace(x, "'" + x + "'")).ToArray() : null;

            string sql = @"SELECT Action_Master.Id
                           ,Action
                           ,ActionResult
                           ,CONVERT(UNIQUEIDENTIFIER, RoleModuleActionMapping.RoleId) AS RoleId
                           ,CASE When RoleModuleActionMapping.Id IS NOT NULL
                                 THEN 1
                                 ELSE 0
                           END IsSelected
                           ,Action_Master.ModuleId,
                           Module_Master.ModuleName
                           FROM Action_Master
                           LEFT JOIN RoleModuleActionMapping ON RoleModuleActionMapping.ActionMasterId = Action_Master.ID 
                           AND RoleModuleActionMapping.RoleId = '" + RoleId + "' JOIN Module_Master on Module_Master.Id=Action_Master.ModuleId";

            if (ModuleId!=null)
            {
                sql = sql + " WHERE Action_Master.ModuleId in (" + string.Join(",", ModuleId) + ")";
            }

            sql = sql + " ORDER BY Module_Master.ModuleName,Action";

            var moduleList = Context.Database.SqlQuery<ActionCoreViewModel>(sql).AsQueryable();
            return moduleList;
        }
    }
}
