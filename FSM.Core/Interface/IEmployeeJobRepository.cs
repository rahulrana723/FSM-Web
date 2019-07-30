using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IEmployeeJobRepository : IGenericRepository<Jobs>
    {
        int GetMaxJobID();
        int GetMaxJobNo();
        IQueryable<EmployeeJobVieweModel> GetEmployeeJobs(); 
        IQueryable<EmployeeJobVieweModel> GetEmployeeJobsWithKeyword(string keyWord,int? jobType,bool? contracted,Guid customerId);
        IQueryable<EmployeeJobVieweModel> GetEmployeeCustomerJobs(Guid custGeneralInfoId);

        IQueryable<EmployeeJobVieweModel> GetJobInfoList();
        IQueryable<Dashboardjobdetailcoreviewmodel> GetEmployeeJobsByJobid(string jobid);
        IQueryable<Dashboardjobdetailcoreviewmodel> GetEmployeeJobsInfo();
        string GetJobTimeSpent(string jobid);
        IQueryable<EmployeeJobVieweModel> GetJobInfoListBySearchkeyword(string keyword, string invoicestatus);
        IQueryable<JobScheduleViewModel> GetJobSchedule(string Jobtype = "null", string Status = "null", string selectdate = "null", string searchkey = "");
        IQueryable<AspNetUsersCore> GetOTRWUser();
        IQueryable<AspNetUsersCore> GetOTRWUserForWorkType(int WorkType);
        IQueryable<Dashboardjobdetailcoreviewmodel> GetEmployeeJobswithJobtype(string Jobtype = "null", string Status = "null", string selectdate = "null", string searchkey = "");
        IQueryable<GetHours> GetAvailableBookedHours();
        IQueryable<Dashboardjobdetailcoreviewmodel> GetJobsOnFullMap(string date, string userId);
        IQueryable<JobScheduleViewModel> SaveEndTime(Guid JobId, string Endtime, string StartTime, Guid ResourceId);
        IQueryable<CheckSiteTwoPeopleJobCoreViewModel> CheckTwoPeopleJob(string siteId);
        IQueryable<int> OTRWHasJob(Guid AssignTo, DateTime AssignedDate, DateTime AssignedEndDate, Guid JobId = default(Guid));
        IQueryable<int> OTRWHasSameJob(Guid AssignTo, DateTime AssignedDate, Guid JobId = default(Guid));
        IQueryable<OtrwJobNoForSheetCoreViewMode> GetOtrwAssignJobNo(Guid UserId);
        bool IsWorkTypeMatch(Guid UserId, Guid JobId);
        IQueryable<GetQuickViewJobDataCoreViewModel> GetQuickViewData(Guid JobId);
        IQueryable<EmployeeJobVieweModel> UpdateRescheduleJobDate(string CurrentDate, string UpdateRescheduleDate);
        IQueryable<CustomerContactsCoreViewModel> GetJobSiteContactsEmail(Guid JobId);
        IQueryable<AspNetUsersCore> OTRWHasJobWithName(Guid AssignTo, DateTime AssignedDate, DateTime AssignedEndDate, Guid JobId = default(Guid));
        IQueryable<JobPerFormanceCoreViewModel> GetJobPerFormanceData(string JobId);
        bool OTRWHasJobBooked(Guid? AssignTo, DateTime? DateBooked);
        bool EmployeeHasJob(string AssignTo, DateTime? StartDate, DateTime? EndDate);
        IQueryable<GetWorkTypeCoreViewModel> GetOTRWUserUsingByWorkType(int? Worktype);
        void SavejobStatus(Jobs empjob);
    }
}
