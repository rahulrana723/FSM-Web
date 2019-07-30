﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class ViewEmployeeRates
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Nullable<Guid> SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public decimal ActualRate { get; set; }
        public decimal BaseRate { get; set; }
        [DisplayName("S WC")]
        public decimal S_WC { get; set; }
        [DisplayName("AL PH")]
        public decimal AL_PH { get; set; }
        public decimal TAFE { get; set; }

        public decimal Payroll { get; set; }
        [DisplayName("Payroll Inc Cost")]
        public decimal Payroll_Inc_Cost { get; set; }
        [DisplayName("Cont MV EQ Cost")]
        public decimal Cont_MV_EQ_Cost { get; set; }
        [DisplayName("Emp MV Cost")]
        public decimal Emp_MV_Cost { get; set; }
        [DisplayName("Equip Cost")]
        public decimal Equip_Cost { get; set; }
        [DisplayName("Emp Mob Ph Cost")]
        public decimal Emp_Mob_Ph_Cost { get; set; }
        [DisplayName("Gross Labour Cost")]
        public decimal Gross_Labour_Cost { get; set; }
        [DisplayName("PERF B PAR")]
        public decimal PERF_B_PAR { get; set; }
        [DisplayName("GP Hour PAR")]
        public decimal GP_Hour_PAR { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }

    }
}
