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

    public class StockRepository : GenericRepository<FsmContext, Stock>, IStockRepository
    {
        public IQueryable<StockViewModel> GetCustomerListBySearchkeyword(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                IQueryable<StockViewModel> Query =
             from Stock in Context.Stock
             where (Stock.Label.Contains(keyword) ||
                    Stock.Material.Contains(keyword) ||
                    Stock.Description.Contains(keyword)||
                    Stock.UnitMeasure.Contains(keyword)||
                    Stock.Price.ToString().Contains(keyword)||
                    Stock.Quantity.ToString().Contains(keyword)||
                    Stock.Available.ToString().Contains(keyword)) &&(
                    Stock.IsDelete==false)
             select new StockViewModel
             {
                 ID = Stock.ID,
                 Label = Stock.Label,
                 Description = Stock.Description,
                 UnitMeasure = Stock.UnitMeasure,
                 Price = Stock.Price,
                 Quantity = Stock.Quantity,
                 Date = Stock.Date,
                 Available = Stock.Available,
             };
                return Query;
            }
            else
            {
                string sql = @"select * from stock where IsDelete=0";
                var stocklist = Context.Database.SqlQuery<StockViewModel>(sql).AsQueryable();
                return stocklist;
            }
        }
        public bool UpdateStockFromPurchase(string stockid, List<PurchaseDataCoreviewModel> values)
        {
            try
            {
                Guid stock_id = Guid.Parse(stockid);
                var itemtoupdate = Context.PurchaseorderItemsStock.First(a => a.StockID == stock_id);
                var stock = Context.Stock.First(a => a.ID == stock_id);
                if (itemtoupdate.Quantity > Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity))
                {
                    stock.Available = stock.Available + (itemtoupdate.Quantity - Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity));
                }
                else if (itemtoupdate.Quantity < Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity))
                {
                    stock.Available = stock.Available + (itemtoupdate.Quantity - Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateStockFromjobPurchase(string stockid, List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values)
        {
            try
            {
                Guid stock_id = Guid.Parse(stockid);
                var itemtoupdate = Context.PurchaseorderItemJob.First(a => a.StockID == stock_id);
                var stock = Context.Stock.First(a => a.ID == stock_id);
                if (itemtoupdate.Quantity > Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity))
                {
                    stock.Available = stock.Available + (itemtoupdate.Quantity - Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity));
                }
                else if (itemtoupdate.Quantity < Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity))
                {
                    stock.Available = stock.Available + (itemtoupdate.Quantity - Convert.ToInt32(values.Find(i => i.StockId == stockid).Quantity));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
