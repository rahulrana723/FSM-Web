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
    public class AssetManageViewModel
    {
        [Key]
        public Guid AssetManageID { get; set; }
        public Nullable<Constant.AssetType> Type { get; set; }
        public string Identifier { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public List<SelectListItem> OTRWList { get; set; }
        public Nullable<DateTime> DateAssigned { get; set; }
        [DisplayName("Assigned To")]
        public Nullable<Guid> AssignedTo { get; set; }
        public string AssignUserName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}