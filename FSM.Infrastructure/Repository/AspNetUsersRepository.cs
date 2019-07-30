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
    public class AspNetUsersRepository : GenericRepository<FsmContext, AspNetUsers>, IAspNetUsersRepository
    {
        public IQueryable<OTRWUserViewModel> GetOTRWUser()
        {
            string sql = @"select U.Id,U.UserName from dbo.AspNetUsers U
                           join dbo.AspNetUserRoles UR on U.Id=UR.UserId
                           join dbo.AspNetRoles R on UR.RoleId=R.Id
                           left join dbo.EmployeeDetail ED on U.Id=ED.EmployeeId
                           where R.name='OTRW' and ED.IsDelete=0 And IsActive=1";

            var OTRWUserList = Context.Database.SqlQuery<OTRWUserViewModel>(sql).AsQueryable();
            return OTRWUserList;
        }
    }
}
