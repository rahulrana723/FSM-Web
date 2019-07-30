﻿using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IRoleModuleActionMappingRepository : IGenericRepository<RoleModuleActionMapping>
    {
        IQueryable<RoleModuleActionMapping> GetModuleActionMappingByRole(Guid RoleId);
    }
}
