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
    public class PurchaseOrderByStockRepository: GenericRepository<FsmContext, PurchaseOrderByStock>, IPurchaseOrderByStock
    {
       public IQueryable<PurchaseOrderByStockCoreViewModel> GetStockPurchaseOrders(string searchkeyword)
        {
            if (!string.IsNullOrEmpty(searchkeyword))
            {
                string purchaseno = "";
                if(searchkeyword.ToLower().Contains("po-"))
                {
                    purchaseno = searchkeyword.ToLower().Replace("po-", "");
                }
                else
                {
                    purchaseno = searchkeyword.ToLower();
                }
                string sql = @"SELECT * FROM(select distinct  p.ID,p.PurchaseOrderNumber   PurchaseOrderNo,'PO-'+convert(nvarchar(50), p.PurchaseOrderNumber )  PurchaseOrderNoformated, p.supplierID,p.Description,p.Cost,s.Name  from  PurchaseOrderByStock p
                            inner join Supplier s on s.Id=p.supplierID)t
                            Where t.PurchaseOrderNo Like '%" + purchaseno + "%'Or t.cost Like '%" + searchkeyword + "%'  Or t.Description Like '%" + searchkeyword + "%' Or t.Name Like '%" + searchkeyword + "%'";
                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaseOrderByStockCoreViewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
            else
            {
                string sql = @"select distinct  p.ID,p.PurchaseOrderNumber   PurchaseOrderNo,'PO-'+convert(nvarchar(50), p.PurchaseOrderNumber )  PurchaseOrderNoformated, p.supplierID,p.Description,p.Cost,s.Name  from  PurchaseOrderByStock p
                            inner join Supplier s on s.Id=p.supplierID ";

                var PurchaseOrdersList = Context.Database.SqlQuery<PurchaseOrderByStockCoreViewModel>(sql).AsQueryable();
                return PurchaseOrdersList;
            }
        }
    }
}
