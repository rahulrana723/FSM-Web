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
    public class ParentAction_MasterRepository : GenericRepository<FsmContext, ParentAction_Master>, IParentAction_MasterRepository
    {
        public IQueryable<ParentActionCoreViewModel> GetParentActions(Guid RoleId, string[] ModuleId = null)
        {
            ModuleId = ModuleId != null ? ModuleId.Select(x => x.Replace(x, "'" + x + "'")).ToArray() : null;

            string sql = @"SELECT  ParentAction_Master.Id ,ParentAction_Master.Action ,ParentAction_Master.ActionResult
                           ,CONVERT(UNIQUEIDENTIFIER, RoleParentActionMapping.RoleId) AS RoleId
                           ,CASE When RoleParentActionMapping.Id IS NOT NULL
                                 THEN 1
                                 ELSE 0
                           END IsSelected
                           ,ParentAction_Master.ModuleId,Module_Master.ModuleName FROM ParentAction_Master LEFT JOIN 
                            RoleParentActionMapping ON RoleParentActionMapping.ParentActionMasterId = ParentAction_Master.ID 
                            AND RoleParentActionMapping.RoleId = '" + RoleId + "' LEFT JOIN Module_Master " +
                           "on Module_Master.Id=ParentAction_Master.ModuleId";

            if (ModuleId != null)
            {
                sql = sql + " WHERE ParentAction_Master.ModuleId in (" + string.Join(",", ModuleId) + ")";
            }

            sql = sql + " ORDER BY Module_Master.ModuleName,ParentAction_Master.Action";

            var parentActionsList = Context.Database.SqlQuery<ParentActionCoreViewModel>(sql).AsQueryable();
            return parentActionsList;
        }
    }
}
