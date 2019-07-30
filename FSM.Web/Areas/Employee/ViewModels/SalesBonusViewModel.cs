using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class SalesBonusViewModel
    {
        public decimal? SaleIncome { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? LabourIncome { get; set; }
        public decimal? RevHours { get; set; }
        public string Rev_Hours { get; set; }
        public decimal? JSPOCost { get; set; }
        public decimal? LabourCost { get; set; }
        public decimal? StockItemCost { get; set; }
        public decimal? LabourProfit { get; set; }
        public decimal? LabourHours { get; set; }
        public string Labour_Hours { get; set; }
        public decimal? SalesBonus { get; set; }
        public decimal? NRLHours { get; set; }
        public string NRL_Hours { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public String CustomerLastName { get; set; }
        public decimal? LobourIncomePerHour { get; set; }
        public decimal? LabourCostPerHour { get; set; }
        public decimal? LabourProfitPerHour { get; set; }
        public int? InvoiceNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SiteFileName { get; set; }

        public string OTRWName { get; set; }
        
    }

    public class UnpaidInvoiceReportViewModel
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
        public string DateBooked { get; set; }
        public string InvoiceDate { get; set; }
    }

    public class OperationalViewModel
    {
        public Decimal? TotalEmployeeTime { get; set; }
        public string Total_EmployeeTime { get; set; }
        public string JobStartDate { get; set; }
        public string JobEndDate { get; set; }
        public string OperationReportType { get; set; }
        public int? TimeSpentinDays { get; set; }
        public decimal? Price { get; set; }
        public decimal? LabourCost { get; set; }
        public decimal? LabourCostPerHour { get; set; }
        public decimal? LabourIncome { get; set; }
        public decimal? LabourIncomePerHour { get; set; }
        public decimal? LabourProfit { get; set; }
        public decimal? LabourProfitPerHour { get; set; }

    }

    public class InvoiceSalesReportViewModel
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
        public string DateBooked { get; set; }
        public string InvoiceDate { get; set; }
    }

}