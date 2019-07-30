using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class JCLProducts_MappingRepository : GenericRepository<FsmContext, JCLProducts_Mapping>, IJCLProducts_MappingRepository
    {
    }
}
