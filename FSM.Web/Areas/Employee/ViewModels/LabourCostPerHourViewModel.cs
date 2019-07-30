using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class LabourCostViewModel
    {

        public string EID { get; set; }

        public Nullable<Guid> EmployeeId { get; set; }
        public String Name { get; set; }
        public Nullable<Decimal> BaseRate { get; set; }

        public Nullable<Decimal> S_WC { get; set; }

        public Nullable<Decimal> AL_PH { get; set; }
        public Nullable<Decimal> TAFE { get; set; }

        public Nullable<Decimal> Payroll { get; set; }

        public Nullable<Decimal> Cont_MV_EQ_Cost { get; set; }

        public Nullable<Decimal> Emp_MV_Cost { get; set; }

        public Nullable<Decimal> Equip_Cost { get; set; }

        public Nullable<Decimal> Emp_Mob_Ph_Cost { get; set; }

        public Nullable<Decimal> GrossLabourCost { get; set; }
    }
    public class LabourCostperhourReportViewModel
    {
        public string EID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? jobDate { get; set; }
        public string Reason { get; set; }
        public Decimal? TimeSpent { get; set; }
        public TimeSpan? ST { get; set; }
        public string FTOLHour { get; set; }
        public decimal? HoursWorked { get; set; }
        public decimal? HoursTraveled { get; set; }
        public decimal? LunchBreak { get; set; }
        public decimal? PersonalTime { get; set; }
        public string HoursTotal { get; set; }
        public string FirstToLatHour { get; set; }
        public string LucnchTime { get; set; }
        public string CorrectedTime { get; set; }
        public TimeSpan? EST { get; set; }

        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public int? TimeSpentHour { get; set; }
        public int? TimeSpentMinute { get; set; }
        public int? TimeSpentSecond { get; set; }

        public string Lunch_Break { get; set; }
        public string Personal_Time { get; set; }
        public string Hours_Traveled { get; set; }
        public string Hours_Worked { get; set; }
        public string Luchhourcal { get; set; }
        public string lunchminutecal { get; set; }
    }
    public class PerformanceBonusReportViewModel
    {
        public string EmployeeName { get; set; }
        public string HoursWorked { get; set; }
        public Nullable<decimal> LabourProfitPerHour { get; set; }
        public Nullable<decimal> LabourProfit { get; set; }
        public Nullable<decimal> PerformanceBonusRate { get; set; }
        public Nullable<decimal> PerformanceBonus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
    public class PaolhReportViewModel
    {
        public string EID { get; set; }
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public string OTRWName { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> HourAssigned { get; set; }
        public string WorkedHour { get; set; }
        public string TotalHourworked { get; set; }
         public Nullable<Double> TotalPerCen { get; set; }
        public Nullable<decimal> totalHourAssigned { get; set; }

        public Nullable<Double> Percentage { get; set; }

    }
}