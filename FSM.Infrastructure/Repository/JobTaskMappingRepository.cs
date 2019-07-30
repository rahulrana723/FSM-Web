using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSM.Core.Interface;
using FSM.Core.Entities;

namespace FSM.Infrastructure.Repository
{
   public class JobTaskMappingRepository : GenericRepository<FsmContext,JobTaskMapping>,IJobTaskMappingRepository
    {

    }
}
