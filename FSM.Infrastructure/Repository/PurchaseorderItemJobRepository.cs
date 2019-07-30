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
    public class PurchaseorderItemJobRepository : GenericRepository<FsmContext, PurchaseorderItemJob>, IPurchaseorderItemJobRepository
    {
        public IQueryable<PurchaseDatajobCoreviewModel> GetPurchaseorderItemJobs(string PurchaseOrderId)
        {
            string sql = @"select convert(nvarchar(50), pitem.StockId) StockId,st.Label StockLabel ,convert(nvarchar(50), st.Available  )AvailableQuatity,convert(nvarchar(50), pitem.PurchaseOrderID) purchaseId,
                            pitem.ID ItemId,pitem.PurchaseItem,pitem.UnitOfMeasure UnitMeasure ,  convert(nvarchar(50), pitem.Quantity) Quantity ,convert(nvarchar(50),pitem.Price) Price,
                            convert(nvarchar(50),p.Cost) Cost,p.Description ,convert(nvarchar(50), p.supplierID) Supplierid  from  PurchaseOrderByJob p
                            left  join [PurchaseorderItemJob] pitem on pitem.PurchaseOrderID=p.ID
							left join Stock st on st.ID=pitem.StockID 
							 where p.ID='" + PurchaseOrderId + "'  ";
            var PurchaseOrdersList = Context.Database.SqlQuery<PurchaseDatajobCoreviewModel>(sql).AsQueryable();
            return PurchaseOrdersList;
        }
    }
}
