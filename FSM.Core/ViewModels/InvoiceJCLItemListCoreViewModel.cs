using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class InvoiceJCLItemListCoreViewModel
    {
        public Guid JCLId { get; set; }
        public Guid JobId { get; set; }
        public Nullable<Guid> ColorId { get; set; }
        public Nullable<Guid> SizeId { get; set; }
        public Nullable<Guid> ProductId { get; set; }
        public string ColorName{get;set;}
        public string Size { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
    }
}
