using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class RoleModuleActionMappingRepository : GenericRepository<FsmContext, RoleModuleActionMapping>, IRoleModuleActionMappingRepository
    {
        public IQueryable<RoleModuleActionMapping> GetModuleActionMappingByRole(Guid RoleId)
        {
            string sql = @"Select Id, CONVERT(UNIQUEIDENTIFIER, RoleId) AS RoleId, ActionMasterId,CreatedDate,
                           CreatedBy,ModifiedDate,ModifiedBy from dbo.RoleModuleActionMapping where RoleId='" + RoleId + "'";

            var moduleActionMappingList = Context.Database.SqlQuery<RoleModuleActionMapping>(sql).AsQueryable();
            return moduleActionMappingList;
        }
    }
}
