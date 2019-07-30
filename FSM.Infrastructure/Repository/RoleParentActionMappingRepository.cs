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
    public class RoleParentActionMappingRepository : GenericRepository<FsmContext, RoleParentActionMapping>, IRoleParentActionMappingRepository
    {
        public IQueryable<ActionByRoleViewModel> GetActionsByRole(Guid RoleId, string Controller, string ActionResult)
        {
            string sql = @"select RPAM.Id as RoleParentActionMappingId ,PAM.Id as ParentActionMasterId,
                           PAM.ActionResult as ParentActionMasterActionResult,
                           PAM.Action as ParentActionMasterAction,
                           AM.ActionResult as ActionMasterActionResult,
                           AM.Action as ActionMasterAction
                           from dbo.RoleParentActionMapping as RPAM
                           join dbo.ParentAction_Master as PAM on RPAM.ParentActionMasterId = PAM.Id
                           join dbo.Action_Master as AM on AM.ParentActionMasterId = PAM.Id
                           where RPAM.RoleId='" + RoleId + "' and AM.Controller='" + Controller + "' and AM.ActionResult='" 
                           + ActionResult + "'";
            var actionByRole = Context.Database.SqlQuery<ActionByRoleViewModel>(sql).AsQueryable();
            return actionByRole;
        }

        public IQueryable<RoleParentActionMapping> GetParentActionsByRole(Guid RoleId)
        {
            string sql = @"Select CONVERT(UNIQUEIDENTIFIER, Id) AS Id, CONVERT(UNIQUEIDENTIFIER, RoleId) AS RoleId,ParentActionMasterId,
                           CreatedDate,CreatedBy,ModifiedDate,ModifiedBy from dbo.RoleParentActionMapping where RoleId = '" + RoleId + "'";

            var roleParentActionList = Context.Database.SqlQuery<RoleParentActionMapping>(sql).AsQueryable();
            return roleParentActionList;
        }
    }
}
