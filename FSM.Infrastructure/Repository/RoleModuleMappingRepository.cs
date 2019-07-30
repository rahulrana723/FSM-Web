using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class RoleModuleMappingRepository : GenericRepository<FsmContext, RoleModuleMapping>, IRoleModuleMappingRepository
    {
        public IQueryable<RoleModuleMapping> GetModulesMappingByRole(Guid RoleId)
        {
            string sql = @"Select CONVERT(UNIQUEIDENTIFIER, Id) AS Id, CONVERT(UNIQUEIDENTIFIER, RoleId) AS RoleId, ModuleId,CreatedDate,
                           CreatedBy,ModifiedDate,ModifiedBy from dbo.RoleModuleMapping where RoleId = '" + RoleId + "'";

            var modulesMappingList = Context.Database.SqlQuery<RoleModuleMapping>(sql).AsQueryable();
            return modulesMappingList;
        }
    }
}
