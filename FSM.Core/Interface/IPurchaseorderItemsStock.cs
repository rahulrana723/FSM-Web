using FSM.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSM.Core.ViewModels;
namespace FSM.Core.Interface
{
    public interface IPurchaseorderItemsStock : IGenericRepository<PurchaseorderItemsStock>
    {
        IQueryable<PurchaseDataCoreviewModel> GetPurchaseitembyPurchaseorder(string PurchaseorderId);
    }
}
