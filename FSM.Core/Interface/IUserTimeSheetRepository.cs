using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IUserTimeSheetRepository : IGenericRepository<UserTimeSheet>
    {
        IQueryable<TimeSheetTotal> GetSheetTotalHrs(DateTime? JobStartDate, DateTime? JobEndDate, string UserIds, string Keyword);
        IQueryable<string> GetSheetHrs(DateTime? JobStartDate, DateTime? JobEndDate, string UserIds, string Keyword);
        IQueryable<TimeSheetViewModel> GetSheetAll(string UserIds);
        IQueryable<TimeSheetViewModel> GetAggregateSheetAll(string UserIds);
        IQueryable<LaboutCostReportViewModel> GetLabourCostPerHour(List<Nullable<Guid>> EmployeeIds, DateTime? StartDate, DateTime? EndDate);
        IQueryable<LabourCostperhourReportViewModel> GetLabourCost(DateTime? StartDate, DateTime? EndDate, List<Nullable<Guid>> EmployeeIds);
        IQueryable<SalesBonusReportViewModel> GetSalesBonusReport(List<Guid?> employeeIds, DateTime? startDate, DateTime? endDate);
        IQueryable<UnpaidInvoiceCoreReportViewModel> GetUnpaidReportList(DateTime? startDate, DateTime? endDate);
        IQueryable<PerformanceBonusCoreViewModel> GetPerformanceBonusReportList(DateTime? StartDate, DateTime? EndDate, List<Nullable<Guid>> EmployeeIds);
        IQueryable<OperationalCoreViewModel> GetoperationalReport(DateTime? startDate, DateTime? endDate, int duration);
        IQueryable<InvoiceSalesReportCoreViewModel> InvoiceSalesReportDailyOrByRange(DateTime? startDate, DateTime? endDate);
        IQueryable<PaolhReportViewModel> GetPaolhReport(DateTime? StartDate, DateTime? EndDate, List<Nullable<Guid>> EmployeeIds);



    }
}
