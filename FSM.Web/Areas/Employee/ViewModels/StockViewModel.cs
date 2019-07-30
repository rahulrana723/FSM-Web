using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class StockViewModel
    {
        [Key]
        public Guid ID { get; set; }
        [Required(ErrorMessage = "*Please provide value for label")]
        [StringLength(50)]
        [RegularExpression(@"^(?!.*<[^>]+>).*", ErrorMessage = "No html tags allowed")]
        public string Label { get; set; }
        [StringLength(50)]
        [RegularExpression(@"^(?!.*<[^>]+>).*", ErrorMessage = "No html tags allowed")]
        public string Material { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public string DisplayDate { get; set; }
        public Nullable<DateTime> Date { get; set; }
        [Required(ErrorMessage = "*Please provide value for unit measure")]
        [StringLength(15)]
        [RegularExpression(@"^(?!.*<[^>]+>).*", ErrorMessage = "No html tags allowed")]
        public string UnitMeasure { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Decimal> Cost { get; set; }
        public Nullable<Guid> EmpId { get; set; }
        [Required(ErrorMessage = "*Please provide Quantity")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value greter than {1}")]
        public Nullable<int> Quantity { get; set; }
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value greter than {1}")]
        public Nullable<int> AddQuantity { get; set; }
        public Nullable<int> Available { get; set; }
        public Nullable<int> OTRW { get; set; }
        public string assignee { get; set; }
        public string UserName { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}