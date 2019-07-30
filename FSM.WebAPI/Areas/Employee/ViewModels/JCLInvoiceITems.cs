using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class JCLInvoiceITemsModel
    {
            public List<JCLInvoiceITems> JcLinvoiceitems { get; set; }
        
    }

    public class JCLInvoiceITems
    {
        public Guid ID { get; set; }
        public Nullable<Guid> JCLItemID { get; set; }
        public Nullable<Guid> ColorID { get; set; }
        public Nullable<Guid> SizeID { get; set; }
        public Nullable<Guid> ProductStyleID { get; set; }
        public Nullable<Guid> ExtraID { get; set; }
        public string ItemName { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public string ProductName { get; set; }
        public string ExtraProduct { get; set; }
        public string description { get; set; }
        public Nullable<Guid> JobID { get; set; }
        public int Quantity { get; set; }
        public Decimal Price { get; set; }
        public Decimal TotalPrice { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime>CreateDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
    }
}