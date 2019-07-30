using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
   public class WorkTypeRepository : GenericRepository<FsmContext, WorkType>, IWorkTypeRepository
    {
    }
}
