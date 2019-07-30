using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Web.Areas.Employee.ViewModels
{
   public class DisplayJobStocksViewModel
    {
        public Guid ID { get; set; }
        public Guid StockID { get; set; }
        public string UnitMeasure { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public List<StockDetail> stockDetail { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Guid JobId { get; set; }
        public int PageSize { get; set; }
        public Nullable<int> AvailableQuantity { get; set; }
        public string InvoiceId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public List<EmployeeJobDetailStock> EmployeeJobDetailStockList { get; set; }
    }
    public class StockDetail
    {
        public Guid StockID { get; set; }
        public string Label { get; set; }
    }
    public class EmployeeJobDetailStock
    {
        public Guid EmployeeJobId { get; set; }

        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobNo { get; set; }
        public string Description { get; set; }

    }
}
