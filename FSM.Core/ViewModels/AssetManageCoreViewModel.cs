using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class AssetManageCoreViewModel
    {
        public Guid AssetManageID { get; set; }
        public Nullable<int> Type { get; set; }
        public string Identifier { get; set; }
        public string Notes { get; set; }
        public Nullable<DateTime> DateAssigned { get; set; }
        public Nullable<Guid> AssignedTo { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string AssignUserName { get; set; }
    }
}
