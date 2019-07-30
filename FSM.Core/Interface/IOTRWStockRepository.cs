using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
   public interface IOTRWStockRepository: IGenericRepository<OTRWStock>
    {
         IQueryable<OTRWSearchViewmodel>GetAssignedStockOTRW(string id,string serachkey);
    }
}
