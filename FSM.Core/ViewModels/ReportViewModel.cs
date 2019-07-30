using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class LaboutCostReportViewModel
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
        public string Name { get; set; }
        public DateTime? jobDate { get; set; }
        public string Reason { get; set; }
        public decimal? TimeSpent { get; set; }
        public TimeSpan? ST { get; set; }
        public TimeSpan? EST { get; set; }
        public Decimal? FTOLHour { get; set; }
        public int? TimeSpentHour { get; set; }
        public int? TimeSpentMinute { get; set; }
        public int? TimeSpentSecond { get; set; }
    }
    public class SalesBonusReportViewModel
    {
        public decimal? SaleIncome { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? LabourIncome { get; set; }
        public decimal? RevHours { get; set; }
        public decimal? JSPOCost { get; set; }
        public decimal? LabourCost { get; set; }
        public decimal? StockItemCost { get; set; }
        public decimal? LabourProfit { get; set; }
        public decimal? LabourHours { get; set; }
        public decimal? SalesBonus { get; set; }

        public decimal? NRLHours { get; set; }

        public Nullable<DateTime> InvoiceDate { get; set; }
        public String CustomerLastName { get; set; }
        public decimal? LobourIncomePerHour { get; set; }
        public decimal? LabourCostPerHour { get; set; }
        public decimal? LabourProfitPerHour { get; set; }

        public int? InvoiceNo { get; set; }

        public string SiteFileName { get; set; }
        public string OTRWName { get; set; }
        

    }

    public class UnpaidInvoiceCoreReportViewModel
    {
        public int? JobNo { get; set; }
        public int? InvoiceNo { get; set; }
        public string JobType { get; set; }
        public string JobStatus { get; set; }
        public string SiteFileName { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DateBooked { get; set; }
        public DateTime? InvoiceDate { get; set; }

    }

    public class OperationalCoreViewModel
    {
        public Decimal? TotalEmployeeTime { get; set; }
        public DateTime? JobStartDate { get; set; }
        public DateTime? JobEndDate { get; set; }
     
        public int?TimeSpentinDays { get; set; }
        public decimal? Price { get; set; }
        public decimal? LabourCost { get; set; }
        public decimal? LabourCostPerHour { get; set; }
        public decimal? LabourIncome { get; set; }
        public decimal? LabourIncomePerHour { get; set; }
        public decimal? LabourProfit{ get; set; }
        public decimal? LabourProfitPerHour { get; set; }
       
    }

    public class PerformanceBonusCoreViewModel
    {
        public string EmployeeName { get; set; }
        public string HoursWorked { get; set; }
        public Nullable<decimal> LabourProfitPerHour { get; set; }
        public Nullable<decimal> LabourProfit { get; set; }
        public Nullable<decimal> PerformanceBonusRate { get; set; }
        public Nullable<decimal> PerformanceBonus { get; set; }
    }
    public class InvoiceSalesReportCoreViewModel
    {
        public int? JobNo { get; set; }
        public int? InvoiceNo { get; set; }
        public string JobType { get; set; }
        public string JobStatus { get; set; }
        public string SiteFileName { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public DateTime? DateBooked { get; set; }
        public DateTime? InvoiceDate { get; set; }

    }

    public class PaolhReportViewModel
    {
        public string EID { get; set; }

        public string OTRWName { get; set; }
        public string Name { get; set; }
        public decimal? HourAssigned { get; set; }
        public string WorkedHour { get; set; }
        public Double? Percentage { get; set; }
        

    }

}
