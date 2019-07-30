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
    public class ViewEmployeeRatesRepository: GenericRepository<FsmContext, ViewEmployeeRates>, IViewEmployeeRatesRepository
    {
        public IQueryable<ViewEmployeeRatesCoreViewModel> ViewEmployeeRateDetail()
        {
            string sql = @"select  RC.CategoryId, RC.CategoryName,rsc.SubCategoryId,rsc.SubCategoryName,rsc.ActualRate,rsc.BaseRate,rsc.S_WC,rsc.AL_PH,rsc.TAFE,rsc.Payroll,
                           rsc.Payroll_Inc_Cost,rsc.Cont_MV_EQ_Cost,rsc.Emp_MV_Cost,rsc.Equip_Cost,rsc.Emp_Mob_Ph_Cost,rsc.Gross_Labour_Cost,
                           rsc.PERF_B_PAR,rsc.GP_Hour_PAR
                           from RateCategory RC
                           join RateSubCategory RSC on RSC.CategoryId=rc.CategoryId where RSC.IsDelete=0";
            var RatesList = Context.Database.SqlQuery<ViewEmployeeRatesCoreViewModel>(sql).AsQueryable();
            return RatesList;
        }

    }
}
