using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class AssetManagementListViewModel
    {
        public IEnumerable<AssetManageViewModel> assetMangeListModel { get; set; }
        public AssetManagementSearchViewModel AssetManageSerarchViewmodel { get; set; }
    }
}