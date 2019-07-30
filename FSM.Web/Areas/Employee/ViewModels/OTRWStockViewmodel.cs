using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class OTRWStockViewmodel
    {
        [Key]
        public Guid ID { get; set; }
        public string Label { get; set; }
        public string Detail { get; set; }
        public string Type { get; set; }
        public Nullable<int> Typeid { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<Guid> StockID { get; set; }

        [DisplayName("Unit Measure")]
        public string UnitMeasure { get; set; }
        public string OTRWName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<Guid> OTRWID { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public List<StockItem> StockItemList { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public List<OTRWEmployeelist> OTRWEmployee { get; set; }
    }
    public class OTRWEmployeelist
    {
        public String EmployeeName { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }
    }
    public class StockItem
    {
        public Nullable<Guid> StockId { get; set; }
        public string StockName { get; set; }
    }
}