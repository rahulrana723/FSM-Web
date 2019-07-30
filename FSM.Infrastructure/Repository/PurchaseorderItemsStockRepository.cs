using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
  public  class PurchaseorderItemsStockRepository : GenericRepository<FsmContext, PurchaseorderItemsStock>, IPurchaseorderItemsStock
    {
      public  IQueryable<PurchaseDataCoreviewModel> GetPurchaseitembyPurchaseorder(string PurchaseorderId)
        {
            try
            {
                string sql = @"select convert(nvarchar(50), si.StockId) StockId,st.Label StockLabel ,convert(nvarchar(50), st.Available  )AvailableQuatity,si.UnitOfMeasure UnitMeasure ,  convert(nvarchar(50), si.Quantity) Quantity ,convert(nvarchar(50),si.Price) Price, convert(nvarchar(50),p.Cost) Cost,p.Description ,convert(nvarchar(50), p.supplierID) Supplierid  from  PurchaseOrderByStock p
                            left  join PurchaseorderItemsStock si on si.PurchaseOrderID=p.ID
							left join Stock st on st.ID=si.StockID 
							 where p.ID='" + PurchaseorderId+"' ";

                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaseDataCoreviewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
