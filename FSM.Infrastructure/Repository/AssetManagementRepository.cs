using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class AssetManagementRepository : GenericRepository<FsmContext, AssetManagement>, IAssetManagementRepository
    {
        public IQueryable<AssetManageCoreViewModel> GetAssetManagementListBySearch(string keyword)
        {
            string sql = @"select Asset.Type,Asset.AssetManageID,Asset.Identifier,Asset.Notes,Asset.DateAssigned,Emp.UserName AssignUserName from AssetManagement Asset
                          left Join EmployeeDetail Emp on Emp.EmployeeId=Asset.AssignedTo
                          where Asset.IsDelete=0";
            if (!string.IsNullOrEmpty(keyword))
            {
                sql = sql + " AND (Identifier like '%" + keyword + "%' OR Notes like '%" + keyword + "%'OR Emp.UserName like '%" + keyword + "%')";
            }
            var assetManagelist = Context.Database.SqlQuery<AssetManageCoreViewModel>(sql).AsQueryable();
            return assetManagelist;
        }
    }
}
