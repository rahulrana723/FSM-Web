using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class ViewEmployeeRatesCoreViewModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public decimal ActualRate { get; set; }
        public decimal BaseRate { get; set; }
        public decimal S_WC { get; set; }
        public decimal AL_PH { get; set; }
        public decimal TAFE { get; set; }

        public decimal Payroll { get; set; }

        public decimal Payroll_Inc_Cost { get; set; }
        public decimal Cont_MV_EQ_Cost { get; set; }
        public decimal Emp_MV_Cost { get; set; }
        public decimal Equip_Cost { get; set; }
        public decimal Emp_Mob_Ph_Cost { get; set; }

        public decimal Gross_Labour_Cost { get; set; }
        public decimal PERF_B_PAR { get; set; }
        public decimal GP_Hour_PAR { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public int PageSize { get; set; }

    }
}
