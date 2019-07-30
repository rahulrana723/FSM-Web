using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class DisplayJCLItemInvoiceViewModel
    {
        public Guid Id { get; set; }
        public Guid JCLId { get; set; }
        public Guid InvoiceId { get; set; }
        public bool HasBonus { get; set; }
        public bool ApplyBonus { get; set; }
        [DisplayName("Item Name")]
        public List<SelectListItem> ItemNameList { get; set; }
        public string ItemName { get; set; }
        [DisplayName("Bonus Per Item")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid BonusPerItem; Max 8 digits")]
        public Nullable<decimal> BonusPerItem { get; set; }
        public Nullable<Constant.JCLCategory> Category { get; set; }
        public string Material { get; set; }

        [DisplayName("Bonus Minimum")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid BonusMinimum; Max 8 digits")]
        public Nullable<decimal> BonusMinimum { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid Price; Max 8 digits")]
        public Nullable<decimal> Price { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid Per; Max 8 digits")]
        public Nullable<decimal> Per { get; set; }
        [DisplayName("Groups Of")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid GroupsOf; Max 8 digits")]
        public Nullable<decimal> GroupsOf { get; set; }
        [Range(0, 99999999.99, ErrorMessage = "Invalid Minimum; Max 8 digits")]
        public Nullable<decimal> Minimum { get; set; }
        [DisplayName("Default Qty")]
        [Range(0, 99999999.99, ErrorMessage = "Invalid DefaultQty; Max 8 digits")]
        public Nullable<int> DefaultQty { get; set; }
        public string Description { get; set; }
        [DisplayName("General Question")]
        public string GeneralQuestion { get; set; }
        [DisplayName("Diagram Question")]
        public string DiagramQuestion { get; set; }
        public bool Diagram { get; set; }
        public int PageSize { get; set; }
        public string DisplayCategoryName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}