using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class JCLItemInvoiceCoreViewModel
    {
        public Guid Id { get; set; }
        public Guid JCLId { get; set; }
        public Guid InvoiceId { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> BonusPerItem { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<int> DefaultQty { get; set; }
       
    }
}
