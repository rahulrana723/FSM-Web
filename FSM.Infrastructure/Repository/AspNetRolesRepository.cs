using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSM.Core.ViewModels;

namespace FSM.Infrastructure.Repository
{
    public class AspNetRolesRepository : GenericRepository<FsmContext, AspNetRoles>, IAspNetRolesRepository
    {
        public IQueryable<AspNetRolesCore> GetAllRoles(string searchKeyword)
        {
            if (searchKeyword != null)
            {
                searchKeyword = searchKeyword.Replace("'", "''");
            }
            string sql = "SELECT CONVERT(UNIQUEIDENTIFIER, Id) AS Id,Name,CreatedDate FROM [AspNetRoles] Where IsDelete=0 AND Name Like '%" + searchKeyword + "%'";
            var rolesList = Context.Database.SqlQuery<AspNetRolesCore>(sql).AsQueryable();
            return rolesList;
        }
    }
}
