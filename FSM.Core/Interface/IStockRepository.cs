using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IStockRepository: IGenericRepository<Stock>
    {
        IQueryable<StockViewModel> GetCustomerListBySearchkeyword(string keyword);
        bool UpdateStockFromjobPurchase(string stockid, List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> listItem);
        bool UpdateStockFromPurchase(string stockid,List<PurchaseDataCoreviewModel>listItem);
    }
}
