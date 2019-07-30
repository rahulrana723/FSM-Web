using FSM.Core.Entities;
using FSM.Infrastructure;
using FSM.WebAPI.App_Start;
using FSM.WebAPI.Areas.Employee.ViewModels;
using FSM.WebAPI.Common;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Web;
using System.Text;
using System.IO;
using System.Web.Hosting;
using System.Text.RegularExpressions;
using System.Dynamic;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.WebAPI.Models;
using log4net;

namespace FSM.WebAPI.Areas.Employee.Controller
{
    //  [System.Web.Http.Authorize]
    public class JobsController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        UnityContainer container = new UnityContainer();
        [Dependency]
        public IEmployeeJobRepository EmployeeJob { get; set; }
        [Dependency]
        public ISupportdojobMapping SupportJoblink { get; set; }
        [Dependency]
        public ICustomerBillingAddressRepository billingAddressRepo { get; set; }
        public JobsController()
        {
            container = (UnityContainer)UnityConfig.GetConfiguredContainer();
            EmployeeJob = container.Resolve<IEmployeeJobRepository>();
            SupportJoblink = container.Resolve<ISupportdojobMapping>();
        }

        public ServiceResponse<JobListsViewModel> GetJobLists(string SearchText)
        {
            ServiceResponse<JobListsViewModel> result = new ServiceResponse<JobListsViewModel>();
            try
            {
                JobListsViewModel JobLists = new JobListsViewModel();
                //JobLists.CustomerLastNames = db.CustomerGeneralInfo.Where(m => m.IsDelete == false).Select(m => new SelectListItem { Text = m.CustomerLastName, Value = m.CustomerGeneralInfoId.ToString() }).ToList();
                //JobLists.CustomerLastNames.OrderBy(m => m.Text);
                string SearchTerm = "";
                if (!string.IsNullOrEmpty(SearchText))
                {
                    SearchTerm = SearchText.Replace("'", "\"");
                    SearchTerm = SearchTerm.Replace('"', ' ').Trim();
                }
                if (string.IsNullOrEmpty(SearchTerm))
                {
                    JobLists.CustomerLastNames = db.CustomerGeneralInfo.Where(m => m.IsDelete == false)
                    .Select(m => new SelectListItem { Text = m.CustomerLastName, Value = m.CustomerGeneralInfoId.ToString() }).ToList();
                }
                else
                {
                    JobLists.CustomerLastNames = db.CustomerGeneralInfo.Where(m => m.IsDelete == false && m.CustomerLastName.Contains(SearchTerm))
                   .Select(m => new SelectListItem { Text = m.CustomerLastName, Value = m.CustomerGeneralInfoId.ToString() }).Take(20).ToList();
                }
                JobLists.CustomerLastNames.OrderBy(m => m.Text);

                JobLists.JobTypes = Enum.GetValues(typeof(Constants.JobType)).Cast<Constants.JobType>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

                JobLists.Status = Enum.GetValues(typeof(Constants.JobStatus)).Cast<Constants.JobStatus>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

                JobLists.PreferredTime = Enum.GetValues(typeof(Constants.PreferredTime)).Cast<Constants.PreferredTime>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();

                result.ResponseList = new List<JobListsViewModel>();
                result.ResponseList.Add(JobLists);
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "null";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;

                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException + "in string " + SearchText;
                return result;
            }
        }

        // POST: api/AddEmployeeJob
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/AddEmployeeJob")]
        public ServiceResponse<string> AddEmployeeJob(EmployeejobsViewModel employeeJobViewModel)
        {
            var s = DateTime.Now;
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                //Check Job Assign On Given time

                if (employeeJobViewModel.StartTime == null && employeeJobViewModel.JobCategory != "2")
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = ("Start time cannot be null!");
                    return result;
                }
                if (employeeJobViewModel.EndTime == null && employeeJobViewModel.JobCategory != "2")
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = ("End time cannot be null!");
                    return result;
                }
                //if (employeeJobViewModel.AssignTo != null && employeeJobViewModel.JobCategory != "2")
                //{
                //    var assignTo = Guid.Parse(employeeJobViewModel.AssignTo.ToString());

                //    if (employeeJobViewModel.JobCategory != "2" && employeeJobViewModel.JobType != 4)
                //    {
                //        var hasJob = EmployeeJob.OTRWHasJob(assignTo, (DateTime)employeeJobViewModel.StartTime, (DateTime)employeeJobViewModel.EndTime);
                //        if (hasJob.Any())
                //        {
                //            result.ResponseCode = 0;
                //            result.ResponseErrorMessage = ("Job already exist on given start/end time.");
                //            return result;
                //        }
                //    }
                //}

                employeeJobViewModel.Category = Convert.ToInt16(employeeJobViewModel.JobCategory);
                employeeJobViewModel.Id = Guid.NewGuid();
                CommonMapper<EmployeejobsViewModel, Jobs> mapper = new CommonMapper<EmployeejobsViewModel, Jobs>();
                Jobs employeeJobEnity = mapper.Mapper(employeeJobViewModel);
                if (employeeJobViewModel.JobCategory == "1")
                {
                    employeeJobEnity.JobCategory = "Booked";
                }
                else
                {
                    employeeJobEnity.JobCategory = "Stand By";
                }

                employeeJobEnity.JobNo = db.Jobs.Max(m => m.JobNo) + 1;
                employeeJobEnity.CreatedDate = DateTime.Now;
                employeeJobEnity.CreatedBy = employeeJobViewModel.BookedBy;
                employeeJobEnity.IsDelete = false;
                db.Jobs.Add(employeeJobEnity);
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " add employee jobs.");


                if (employeeJobViewModel.Category == 1)
                {
                    // adding job assign mapping
                    JobAssignToMapping jobAssignToMapping = new JobAssignToMapping();
                    jobAssignToMapping.Id = Guid.NewGuid();
                    jobAssignToMapping.JobId = employeeJobViewModel.Id;
                    jobAssignToMapping.AssignTo = employeeJobViewModel.AssignTo;
                    jobAssignToMapping.Status = 5; // represents job status as assigned
                    jobAssignToMapping.CreatedDate = DateTime.Now;
                    jobAssignToMapping.CreatedBy = employeeJobViewModel.BookedBy;
                    jobAssignToMapping.StartTime = employeeJobViewModel.StartTime;
                    jobAssignToMapping.EndTime = employeeJobViewModel.EndTime;
                    jobAssignToMapping.DateBooked = employeeJobViewModel.DateBooked;
                    db.JobAssignToMapping.Add(jobAssignToMapping);
                    db.SaveChanges();
                }

                //check if support job linking 
                if (employeeJobViewModel.LinkedJobId != null && employeeJobViewModel.LinkedJobId != Guid.Empty)
                {
                    SupportDoJobMappingViewModel supportDoJobMappingViewModel = new SupportDoJobMappingViewModel();
                    supportDoJobMappingViewModel.ID = Guid.NewGuid();
                    supportDoJobMappingViewModel.SupportJobId = employeeJobViewModel.Id;
                    supportDoJobMappingViewModel.LinkedJobId = employeeJobViewModel.LinkedJobId;
                    CommonMapper<SupportDoJobMappingViewModel, SupportdojobMapping> maper = new CommonMapper<SupportDoJobMappingViewModel, SupportdojobMapping>();
                    SupportdojobMapping supportdojobMappingEntity = maper.Mapper(supportDoJobMappingViewModel);

                    db.SupportdojobMapping.Add(supportdojobMappingEntity);
                    db.SaveChanges();
                }
                result.ResponseList = new List<string> { Convert.ToString(employeeJobViewModel.Id) };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateEmployeeJob")]
        public ServiceResponse<string> UpdateEmployeeJob(EmployeejobsViewModel employeeJobViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<EmployeejobsViewModel, Jobs> mapper = new CommonMapper<EmployeejobsViewModel, Jobs>();
                Jobs employeeJobEnity = mapper.Mapper(employeeJobViewModel);
                employeeJobEnity.IsDelete = false;
                db.Entry(employeeJobEnity).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update employee jobs.");


                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateOTRWNotes")]
        public ServiceResponse<string> UpdateOTRWNotes(UpdateOTRWNotesViewModel model)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;
                DateTime? datebooked;

                Guid JobId = new Guid(model.Id);
                Guid NewuserId = new Guid(userId);
                string estimateHours = model.estimated_hour;
                Jobs jobs = db.Jobs.FirstOrDefault(m => m.Id == JobId);
                //if (estimateHours != "" && !string.IsNullOrEmpty(estimateHours))
                //{
                //    jobs.EstimatedHours = Convert.ToDouble(estimateHours);
                //}
                //db.Entry(jobs).State = EntityState.Modified;
                //db.SaveChanges();

                if (model.DateBooked.HasValue)
                {
                    datebooked = model.DateBooked.Value;
                    JobAssignToMapping jobAssign = db.JobAssignToMapping.FirstOrDefault(m => m.AssignTo == NewuserId && m.JobId == JobId && m.DateBooked == model.DateBooked.Value && m.IsDelete == false);
                    if (jobAssign != null)
                    {
                        jobAssign.OTRWNotes = model.OTRWjobNotes;
                        jobAssign.CreatedDate = DateTime.Now;
                        jobAssign.ModifiedBy = new Guid(userId);
                        db.Entry(jobAssign).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                // adding OTRWNotes in Contact Log
                string newNotesAdded = "<b>OTRW Notes Added by:</b> " + userName + " On " + DateTime.Now + "<br/>" + model.OTRWjobNotes;
                CustomerContactLog customerContactLog = new CustomerContactLog();
                customerContactLog.CustomerContactId = Guid.NewGuid();
                customerContactLog.CustomerGeneralInfoId = jobs.CustomerGeneralInfoId;
                customerContactLog.CustomerId = Convert.ToString(db.CustomerGeneralInfo.Where(m => m.CustomerGeneralInfoId == jobs.CustomerGeneralInfoId).Select(m => m.CTId).FirstOrDefault());
                customerContactLog.JobId = Convert.ToString(jobs.Id);
                customerContactLog.LogDate = DateTime.Now;
                customerContactLog.Note = newNotesAdded;
                customerContactLog.IsDelete = false;
                customerContactLog.CreatedBy = new Guid(userId);
                customerContactLog.CreatedDate = DateTime.Now;
                db.CustomerContactLog.Add(customerContactLog);
                db.SaveChanges();

                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update OTRW notes.");

                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                return result;

            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetSitesByCustomerId")]
        public ServiceResponse<SiteNamesViewModel> GetSitesByCustomerId(dynamic json)
        {
            ServiceResponse<SiteNamesViewModel> result = new ServiceResponse<SiteNamesViewModel>();
            try
            {
                Guid CustomerId = new Guid(json.CustomerId.Value);
                IQueryable<SiteNamesViewModel> lstCustomerSites = from customerSiteDetial in db.CustomerSiteDetail
                                                                  where customerSiteDetial.CustomerGeneralInfoId == CustomerId && customerSiteDetial.IsDelete == false
                                                                  select new SiteNamesViewModel { SiteId = customerSiteDetial.SiteDetailId, SiteName = customerSiteDetial.StreetName };

                result.ResponseList = lstCustomerSites.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get site by customer id.");

                return result;

            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetEmployeeJobs")]
        public ServiceResponsePaging<GetEmployeeJobsViewModel> GetEmployeeJobs(dynamic json)
        {
            ServiceResponsePaging<GetEmployeeJobsViewModel> result = new ServiceResponsePaging<GetEmployeeJobsViewModel>();
            try
            {


                Guid EmployeeId = new Guid(json.EmployeeId.Value);
                DateTime? DateBooked = json.DateBooked == null ? null : json.DateBooked.Value;
                DateTime? StartDate = json.StartDate == null ? null : json.StartDate.Value;
                DateTime? EndDate = json.EndDate == null ? null : json.EndDate.Value;

                string Keyword = json.Keyword == null ? null : json.Keyword.Value;
                string OrderBy = json.OrderBy == null ? "JobId" : string.IsNullOrEmpty(json.OrderBy.Value) ? "JobId" : json.OrderBy.Value;
                int PageSize = json.PageSize == null ? 10 : Convert.ToInt16(json.PageSize.Value);
                int PageNumber = json.PageNumber == null ? 1 : Convert.ToInt16(json.PageNumber.Value);
                string Filter = json.Filter == null ? "" : json.Filter.Value;

                IQueryable<GetEmployeeJobsViewModel> lstGetEmployeeJobsViewModel = from JB in db.Jobs
                                                                                   join CSD in db.CustomerSiteDetail on JB.SiteId equals CSD.SiteDetailId
                                                                                   join CGI in db.CustomerGeneralInfo on CSD.CustomerGeneralInfoId equals CGI.CustomerGeneralInfoId
                                                                                   join JATM in db.JobAssignToMapping on JB.Id equals JATM.JobId
                                                                                   where JATM.AssignTo == EmployeeId && JB.IsDelete == false && JATM.IsDelete == false /*&& JATM.DateBooked != null*/
                                                                                   select new GetEmployeeJobsViewModel
                                                                                   {
                                                                                       Id = JB.Id,
                                                                                       JobId = JB.JobId,
                                                                                       JobType = JB.JobType,
                                                                                       status = JATM.Status,
                                                                                       JobNo = JB.JobNo,
                                                                                       DateBooked = JATM.DateBooked,
                                                                                       CustomerLastName = CGI.CustomerLastName,
                                                                                       JobAddress = CSD.Street
                                                                                      + " " + CSD.StreetName + " " + CSD.Suburb + " " + CSD.State,
                                                                                       JobNotes = JB.JobNotes != null ? JB.JobNotes : "",
                                                                                       Latitude = CSD.Latitude,
                                                                                       Longitude = CSD.Longitude,
                                                                                       StartTime = JATM.StartTime,
                                                                                       OTRWjobNotes = JATM.OTRWNotes
                                                                                   };


                //if (DateBooked.HasValue)
                //{
                //    lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => m.DateBooked == DateBooked && m.status != 15);
                //}
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => m.DateBooked >= StartDate && m.DateBooked <= EndDate);

                }


                if (!string.IsNullOrEmpty(Keyword))
                {
                    lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m =>
                                                ((!string.IsNullOrEmpty(m.CustomerLastName)) && m.CustomerLastName.Contains(Keyword)) ||
                                                ((!string.IsNullOrEmpty(m.JobAddress)) && m.JobAddress.Contains(Keyword))
                                                || ((!string.IsNullOrEmpty(m.JobNotes)) && m.JobNotes.Contains(Keyword))
                                                || ((!string.IsNullOrEmpty(m.JobNo.ToString())) && m.JobNo.ToString().Contains(Keyword)));
                }

                if (!string.IsNullOrEmpty(Filter))
                {
                    if (Filter.ToLower() == "completed")
                    {
                        lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => m.status == 15 && m.DateBooked != null);
                    }
                    else if (Filter.ToLower() == "upcoming")
                    {
                        lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => m.DateBooked > DateTime.Now && m.status == 5);
                    }
                    else if (Filter.ToLower() == "pendingquotes")
                    {
                        lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => (m.JobType == 1) && (m.status == 5 || m.status == 11 || m.status == 3 || m.status == 9));
                    }
                    else if (Filter.ToLower() == "assignedjobs")
                    {
                        lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => (m.status == 5 || m.status == 15 || m.status == 11 || m.status == 3 || m.status == 9 || m.status == 16) && m.DateBooked != null);
                    }
                    if (Filter.ToLower() == "alljobs")
                    {
                        lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => m.DateBooked != null);
                    }
                }

                //if (DateBooked.HasValue)
                //{
                //    lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.Where(m => m.DateBooked == DateBooked);
                //}


                int skipRecords = (PageNumber - 1) * PageSize;
                int totalcount = lstGetEmployeeJobsViewModel.ToList().Count;
                lstGetEmployeeJobsViewModel = lstGetEmployeeJobsViewModel.OrderBy(OrderBy).Skip(skipRecords).Take(PageSize);
                foreach (var otr in lstGetEmployeeJobsViewModel)
                {
                    if (!string.IsNullOrEmpty(otr.OTRWjobNotes))
                    {
                        otr.OTRWjobNotes = Regex.Replace(otr.OTRWjobNotes, "<.*?>", String.Empty);
                    }
                    else
                    {
                        otr.OTRWjobNotes = string.Empty;
                    }
                }
                foreach (var jobnote in lstGetEmployeeJobsViewModel)
                {
                    if (!string.IsNullOrEmpty(jobnote.JobNotes))
                    {
                        jobnote.JobNotes = Regex.Replace(jobnote.JobNotes, "<.*?>", String.Empty);
                    }
                    else
                    {
                        jobnote.JobNotes = string.Empty;
                    }
                }
                result.ResponseList = lstGetEmployeeJobsViewModel.ToList().Select(m => new GetEmployeeJobsViewModel
                {
                    Id = m.Id,
                    JobId = m.JobId,
                    JobType = m.JobType,
                    status = m.status,
                    JobNo = m.JobNo,
                    DateBooked = m.DateBooked,
                    CustomerLastName = m.CustomerLastName,
                    JobAddress = m.JobAddress,
                    JobNotes = m.JobNotes,
                    OTRWjobNotes = m.OTRWjobNotes,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude
                }).ToList();

                result.TotalPage = (totalcount % PageSize == 0) ? (totalcount / PageSize).ToString() : ((totalcount / PageSize) + 1).ToString();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get employee jobs.");

                return result;

            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;

            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/StartEndTime")]
        public ServiceResponse<string> StartEndTime(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                DateTime? starttime = json.StartTime == null ? null : Convert.ToDateTime(json.StartTime.Value);
                DateTime? endtime = json.EndTime == null ? null : Convert.ToDateTime(json.EndTime.Value);


                Guid JobId = new Guid(json.Id.Value);
                Jobs jobs = db.Jobs.FirstOrDefault(m => m.Id == JobId);
                string ActualTime = "Start Time Saved successfully.";

                if (starttime.HasValue)
                {
                    jobs.StartTime = starttime;
                    jobs.Status = 11;
                }
                if (endtime.HasValue)
                {
                    jobs.EndTime = endtime;
                    jobs.Status = 15;
                    ActualTime = new Common.CommonFunctions().GetCalculatedTime(jobs.StartTime, endtime);
                    jobs.TimeSpent = ActualTime;
                }

                db.Entry(jobs).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { ActualTime };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobDetail")]
        public ServiceResponse<GetJobDetailViewModel> GetJobDetail(dynamic json)
        {
            ServiceResponse<GetJobDetailViewModel> result = new ServiceResponse<GetJobDetailViewModel>();
            try
            {
                Guid Id = new Guid(json.Id.Value);
                Guid UserId = new Guid(json.UserId.Value);

                var TimeSpent = from UTS in db.UserTimeSheet
                                join JATM in db.JobAssignToMapping on UTS.JobId equals JATM.JobId
                                where UTS.JobId == Id && UTS.Reason == "job" && UTS.UserId == UserId && JATM.AssignTo == UserId && JATM.Status == 15
                                select new { UTS.TimeSpent };

                string UserTimeSpent = TimeSpent.ToList().Select(m => m.TimeSpent).Aggregate(new TimeSpan(0), (p, v) => p.Add(v)).ToString();

                var otrwnotes = (from jb in db.Jobs join JATM in db.JobAssignToMapping on jb.Id equals JATM.JobId where JATM.JobId == Id && JATM.AssignTo == UserId && JATM.IsDelete == false select JATM).ToList();
                string otrnotes = "";
                if (otrwnotes.Count > 0)
                {
                    foreach (var notes in otrwnotes)
                    {
                        otrnotes = otrnotes + notes.OTRWNotes;
                    }
                }
                var lstGetJobDetailViewModel = (from JB in db.Jobs
                                                join JATM in db.JobAssignToMapping on JB.Id equals JATM.JobId
                                                join CSD in db.CustomerSiteDetail on JB.SiteId equals CSD.SiteDetailId
                                                join CCR in db.CustomerConditionReport on JB.SiteId equals CCR.SiteDetailId
                                                  into CR
                                                from CCR in CR.DefaultIfEmpty()
                                                join CRD in db.CustomerResidenceDetail on JB.SiteId equals CRD.SiteDetailId
                                                join CGI in db.CustomerGeneralInfo on CSD.CustomerGeneralInfoId equals CGI.CustomerGeneralInfoId
                                                join USRT in db.UserTimeSheet.Where(uts => uts.IsRunning == 1 && uts.UserId == UserId) on JB.Id equals USRT.JobId
                                                  into g
                                                from USRT in g.DefaultIfEmpty()

                                                join INV in db.Invoice on JB.JobNo equals INV.JobId
                                                 into h
                                                from INV in h.DefaultIfEmpty()

                                                where JB.Id == Id && JATM.AssignTo == UserId && JATM.IsDelete == false && JB.IsDelete == false

                                                select new GetJobDetailViewModel
                                                {
                                                    Id = JB.Id,
                                                    JobId = JB.JobId,
                                                    JobNo = JB.JobNo,
                                                    JobType = JB.JobType,
                                                    JobStatus = JATM.Status,
                                                    CustomerLastName = CGI.CustomerLastName,
                                                    TradingName = CGI.TradingName,
                                                    CustomerType = CGI.CustomerType,
                                                    LeadType = CGI.LeadType,
                                                    Terms = CGI.Terms,
                                                    CustomerNotes = CGI.CustomerNotes,
                                                    SiteDetailId = CSD.SiteDetailId,
                                                    JobAddress = CSD.Street
                                                                                    + " " + CSD.StreetName + " " + CSD.Suburb + " " + CSD.State,
                                                    JobNotes = JB.JobNotes != null ? JB.JobNotes : "",
                                                    //OTRWjobNotes = JB.OTRWjobNotes,
                                                    OperationNotes = JB.OperationNotes,
                                                    Latitude = CSD.Latitude,
                                                    Longitude = CSD.Longitude,
                                                    SiteFileName = CSD.SiteFileName,
                                                    PrefTimeOfDay = CSD.PrefTimeOfDay,
                                                    StrataPlan = CSD.StrataPlan,
                                                    Contracted = CSD.Contracted,
                                                    ScheduledPrice = CSD.ScheduledPrice,
                                                    Notes = CSD.Notes,
                                                    TimeSpent = UserTimeSpent != null && UserTimeSpent != "00:00:00" ? UserTimeSpent : "",
                                                    ConditionDetailId = CCR.ConditionReportId,
                                                    RoofTilesSheets = CCR.RoofTilesSheets,
                                                    BargeCappings = CCR.BargeCappings,
                                                    RidgeCappings = CCR.RidgeCappings,
                                                    Valleys = CCR.Valleys,
                                                    Flashings = CCR.Flashings,
                                                    Gutters = CCR.Gutters,
                                                    DownPipes = CCR.DownPipes,
                                                    ConditionNote = CCR.ConditionNote,
                                                    IsRunning = USRT.IsRunning,
                                                    ResidenceDetailId = CRD.ResidenceDetailId,
                                                    TypeOfResidence = CRD.TypeOfResidence,
                                                    ResidenceUnit = CRD.Unit,
                                                    NoBldgs = CRD.NoBldgs,
                                                    Height = CRD.Height,
                                                    Pitch = CRD.Pitch,
                                                    RoofType = CRD.RoofType,
                                                    GutterGaurd = CRD.GutterGaurd,
                                                    SRASinstalled = CRD.SRASinstalled,
                                                    NotWet = CRD.NotWet,
                                                    NeedTwoPPL = CRD.NeedTwoPPL,
                                                    Reason = USRT.Reason,
                                                    StartTime = USRT.StartTime,
                                                    JobDate = USRT.JobDate != null ? USRT.JobDate.ToString() : "",
                                                    HasInvoice = INV.Id != null ? "1" : "0",
                                                    ConditionOfRoof = CCR.ConditionOfRoof,
                                                }).ToList();

                //int jobInt = (int)lstGetJobDetailViewModel[0].JobId;
                //var jobInvoice = db.Invoice.Where(m => m.JobId == jobInt).FirstOrDefault();

                result.ResponseList = lstGetJobDetailViewModel.Select(m => new GetJobDetailViewModel
                {
                    Id = m.Id,
                    JobId = m.JobId,
                    JobNo = m.JobNo,
                    JobType = m.JobType,
                    JobStatus = m.JobStatus,
                    CustomerLastName = m.CustomerLastName,
                    TradingName = m.TradingName,
                    CustomerType = m.CustomerType,
                    LeadType = m.LeadType,
                    Terms = m.Terms,
                    CustomerNotes = m.CustomerNotes,
                    JobAddress = m.JobAddress,
                    JobNotes = Regex.Replace(m.JobNotes, "<.*?>", String.Empty),
                    OTRWjobNotes = Regex.Replace(otrnotes, "<.*?>", String.Empty),
                    OperationNotes = m.OperationNotes,
                    SiteDetailId = m.SiteDetailId,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    TimeSpent = m.TimeSpent,
                    SiteFileName = m.SiteFileName,
                    PrefTimeOfDay = m.PrefTimeOfDay,
                    StrataPlan = m.StrataPlan,
                    Contracted = m.Contracted,
                    ScheduledPrice = m.ScheduledPrice,
                    Notes = m.Notes,
                    IsRunning = m.IsRunning,
                    ResidenceDetailId = m.ResidenceDetailId,
                    TypeOfResidence = m.TypeOfResidence,
                    ResidenceUnit = m.ResidenceUnit,
                    NoBldgs = m.NoBldgs,
                    Height = m.Height,
                    Pitch = m.Pitch,
                    ConditionDetailId = m.ConditionDetailId,
                    RoofType = m.RoofType,
                    GutterGaurd = m.GutterGaurd,
                    SRASinstalled = m.SRASinstalled,
                    NotWet = m.NotWet,
                    NeedTwoPPL = m.NeedTwoPPL,
                    Reason = m.Reason,
                    RoofTilesSheets = m.RoofTilesSheets,
                    BargeCappings = m.BargeCappings,
                    RidgeCappings = m.RidgeCappings,
                    Valleys = m.Valleys,
                    Flashings = m.Flashings,
                    Gutters = m.Gutters,
                    DownPipes = m.DownPipes,
                    ConditionNote = m.ConditionNote,
                    StartTime = m.StartTime,
                    JobDate = m.JobDate,
                    HasInvoice = m.HasInvoice,
                    ConditionOfRoof = m.ConditionOfRoof
                }).ToList();

                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job details.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetTotalCostOfQuote")]
        public ServiceResponse<string> GetTotalCostOfQuote(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid employeeId = new Guid(json.EmployeeId.Value);
                double? EstimatedTime = json.EstimatedTime == null ? null : Convert.ToDouble(json.EstimatedTime.Value);
                EmployeeDetail employeeDetial = db.EmployeeDetail.FirstOrDefault(m => m.EmployeeId == employeeId);
                double? totalCostOfQuote = employeeDetial == null ? null : employeeDetial.HourlyRate * EstimatedTime;
                result.ResponseList = new List<string> { Convert.ToString(totalCostOfQuote) };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get total cost of quote.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.Message;
                return result;
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/SendQuotes")]
        public ServiceResponse<string> SendQuotes(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid JobId = new Guid(json.Id.Value);
                double? EstimatedTime = json.EstimatedTime == null ? null : Convert.ToDouble(json.EstimatedTime.Value);
                double? TotalCost = json.TotalCost == null ? null : Convert.ToDouble(json.TotalCost.Value);

                Jobs jobs = db.Jobs.FirstOrDefault(m => m.Id == JobId);
                jobs.JobNotes = json.JobNotes.Value;
                jobs.StockRequired = json.StockRequired.Value;
                jobs.EstimatedTime = json.EstimatedTime.Value;
                jobs.TotalCost = json.TotalCost.Value;
                jobs.Status = 9;
                db.Entry(jobs).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record saved successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " send quotes.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/SaveJobDocs")]
        public ServiceResponse<string> SaveJobDocs()
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            string JobId = "";
            JobId = HttpContext.Current.Request.Form["JobId"] ?? ""; //adding empty string incase no content was 
            string DocType = HttpContext.Current.Request.Form["DocType"] ?? "";

            Guid jobGuid;
            Guid.TryParse(JobId, out jobGuid);

            var miliSeconds = DateTime.Now.Millisecond;

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body

                // Read the form data and return an async task.
                //   await Request.Content.ReadAsMultipartAsync(provider);

                HttpPostedFile uploadedFile = HttpContext.Current.Request.Files["Jobdocs"];
                if (uploadedFile == null)
                {
                    // throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                else
                {
                    //retrieve the string with name value...
                    var httpRequest = HttpContext.Current.Request;

                    if (httpRequest.Files.Count > 0)
                    {
                        int i = 0;
                        foreach (string file in httpRequest.Files)
                        {

                            var postedFile = httpRequest.Files[i];
                            var filePath = postedFile.FileName;
                            string firstName = filePath.Substring(0, filePath.LastIndexOf('.')) + "_" + Guid.NewGuid();
                            string lastName = filePath.Substring(filePath.LastIndexOf('.')).ToLower();
                            filePath = firstName + lastName;

                            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "JobDocImagesPath/" + JobId))
                            {
                                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "JobDocImagesPath/" + JobId);
                            }
                            postedFile.SaveAs(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "JobDocImagesPath/" + JobId, filePath));

                            // save in database
                            JobDocuments jobDocuments = new JobDocuments();
                            jobDocuments.Id = Guid.NewGuid();
                            jobDocuments.JobId = jobGuid;
                            jobDocuments.DocName = postedFile.FileName;
                            jobDocuments.SaveDocName = filePath;
                            jobDocuments.DocType = GetDocumentType(lastName);
                            jobDocuments.IsDelete = false;
                            db.JobDocuments.Add(jobDocuments);
                            db.SaveChanges();

                            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                            response = CommonFunctions.GetUserInfoByToken();
                            string userId = response.ResponseList[0].UserId;
                            string userName = response.ResponseList[0].UserName;


                            log4net.ThreadContext.Properties["UserId"] = userId;
                            log.Info(userName + " save job document.");


                            i = i + 1;
                        }
                    }
                }

                result.ResponseList = new List<string> { "Job doc uploaded succesfully" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (System.Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        private static string GetDocumentType(string extension)
        {
            string documentType;
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".png":
                    documentType = "Image";
                    break;
                case ".doc":
                case ".docx":
                    documentType = "Word Document";
                    break;
                case ".xls":
                case ".xlsx":
                    documentType = "Word Excel";
                    break;
                case ".pdf":
                    documentType = "Pdf";
                    break;
                case ".text":
                case ".txt":
                    documentType = "Text File";
                    break;
                default:
                    documentType = "Unknown";
                    break;
            }

            return documentType;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobDocs")]
        public ServiceResponse<GetJobDocuments> GetJobDocs(dynamic json)
        {
            ServiceResponse<GetJobDocuments> result = new ServiceResponse<GetJobDocuments>();
            try
            {
                string jobId = json.JobId.Value;
                Guid jobGuid;
                Guid.TryParse(jobId, out jobGuid);
                var siteId = db.Jobs.Where(m => m.Id == jobGuid).Select(m => m.SiteId).FirstOrDefault();
                string jobDocsPath = AppDomain.CurrentDomain.BaseDirectory + "/GetJobDocImagesPath." + jobId + "/";
                var DocName = from jobDocuments in db.JobDocuments
                              join job in db.Jobs on jobDocuments.JobId equals job.Id
                              where job.SiteId == siteId && jobDocuments.IsDelete == false
                              select new GetJobDocuments
                              {
                                  Id = jobDocuments.Id,
                                  ImageName = jobDocuments.DocName,
                                  DocName = jobDocsPath + jobDocuments.SaveDocName,
                                  DocType = jobDocuments.DocType,
                              };

                result.ResponseList = DocName.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job document.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.Message;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateJobDocsName")]
        public ServiceResponse<string> UpdateJobDocsName(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid PhotoId = new Guid(json.PhotoId.Value);
                JobDocuments jobsDoc = db.JobDocuments.FirstOrDefault(m => m.Id == PhotoId);
                jobsDoc.DocName = json.ImageName.Value;
                db.Entry(jobsDoc).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update OTRW notes.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/SaveUserTimeSheet")]
        public ServiceResponse<string> SaveUserTimeSheet(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid JobId = new Guid(json.JobId.Value);
                Guid UserId = new Guid(json.UserId.Value);
                string jobPerform = json.JobPerform;
                string reason = json.Reason;
                Guid loginUser = new Guid(json.CreatedBy.Value);

                if (JobId != null && UserId != null && !string.IsNullOrEmpty(jobPerform))
                {
                    var jobAssignToMapping = db.JobAssignToMapping.Where(m => m.JobId == JobId && m.AssignTo == UserId && m.IsDelete == false).FirstOrDefault();

                    var completedJobCount = db.JobAssignToMapping.Where(m => m.JobId == JobId && m.Status == 15 && m.IsDelete == false).Count();

                    if (Constants.JobPerform.IndexOf(jobPerform) > -1 || jobPerform == "JobLeft")
                    {
                        var currDate = DateTime.Now.Date;

                        //getting jobs data according to jobid
                        Jobs jobs = db.Jobs.FirstOrDefault(m => m.Id == JobId);

                        // updating timespent for the job till now
                        UserTimeSheet userTimeSheet = db.UserTimeSheet.Where(m => m.UserId == UserId && m.JobId == JobId && m.IsRunning == 1).FirstOrDefault();
                        if (userTimeSheet != null)
                        {
                            userTimeSheet.IsFirstTraveling = 0;
                            userTimeSheet.JobType = jobs.JobType;
                            userTimeSheet.EndTime = DateTime.Now.TimeOfDay;
                            userTimeSheet.TimeSpent = DateTime.Now.Date.Add(userTimeSheet.EndTime) - userTimeSheet.JobDate.Add(userTimeSheet.StartTime);
                            userTimeSheet.IsRunning = 0;
                            db.Entry(userTimeSheet).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        if (jobPerform != "JobLeft")
                        {
                            var Usertravling = db.UserTimeSheet.Where(m => m.UserId == UserId && m.IsFirstTraveling == 0).FirstOrDefault();
                            if (jobPerform != "End" && jobPerform != "JobEnd" && jobPerform != "JobPause")
                            {
                                // adding starttime for the job 
                                UserTimeSheet userTimeSheetNew = new UserTimeSheet();
                                userTimeSheetNew.Id = Guid.NewGuid();
                                if (Usertravling == null)
                                {
                                    userTimeSheetNew.IsFirstTraveling = 1;
                                }
                                else
                                {
                                    userTimeSheetNew.IsFirstTraveling = 0;
                                }
                                userTimeSheetNew.JobType = jobs.JobType;
                                userTimeSheetNew.UserId = UserId;
                                userTimeSheetNew.JobId = JobId;
                                userTimeSheetNew.JobDate = DateTime.Now.Date;
                                userTimeSheetNew.StartTime = DateTime.Now.TimeOfDay;
                                if (jobPerform == "JobResume" || jobPerform == "JobStart")
                                {
                                    jobs.Status = 11;
                                    jobAssignToMapping.Status = 11;
                                    userTimeSheetNew.Reason = "Job";
                                }
                                else if (jobPerform == "Travelling")
                                {
                                    jobAssignToMapping.Status = 9;
                                    userTimeSheetNew.Reason = "Travelling";
                                }
                                else
                                {
                                    userTimeSheetNew.Reason = jobPerform;
                                }
                                userTimeSheetNew.IsRunning = 1;
                                db.UserTimeSheet.Add(userTimeSheetNew);
                                db.SaveChanges();
                            }

                            if (jobPerform == "JobEnd")
                            {
                                if (jobs.OTRWRequired == completedJobCount + 1)
                                {
                                    jobs.Status = 15;
                                    //Code on 14-Aug-2017
                                    #region Timesheet is running to 0
                                    var timesheet = db.UserTimeSheet.Where(i => i.JobId == JobId);
                                    if (timesheet != null)
                                    {
                                        foreach (UserTimeSheet record in timesheet.ToList())
                                        {
                                            record.IsRunning = 0;
                                            db.Entry(record).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                    #endregion
                                    //
                                    var TimeSpent = from UTS in db.UserTimeSheet
                                                    where UTS.JobId == JobId && UTS.Reason == "job"
                                                    select new { UTS.TimeSpent };

                                    jobs.TimeSpent = TimeSpent.ToList().Select(m => m.TimeSpent).Aggregate(new TimeSpan(0), (p, v) => p.Add(v)).ToString();
                                }
                                else
                                {
                                    jobs.Status = 11;
                                }
                                jobAssignToMapping.Status = 15;
                            }
                            if (jobPerform == "JobEnd" || jobPerform == "JobStart")
                            {
                                db.Entry(jobs).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        if (jobPerform == "JobLeft")
                        {
                            jobAssignToMapping.Status = 16;
                            UserMessageSend(UserId, reason);
                        }

                        db.Entry(jobAssignToMapping).State = EntityState.Modified;
                        db.SaveChanges();

                        //Otrw action perform then save in contactlog
                        var employeejob = db.Jobs.FirstOrDefault(m => m.Id == JobId);   //get job data
                        var customerInfo = db.CustomerGeneralInfo.FirstOrDefault(m => m.CustomerGeneralInfoId == employeejob.CustomerGeneralInfoId);  //get customer data

                        CustomerContactLog customerContactLog = new CustomerContactLog();
                        customerContactLog.CustomerContactId = Guid.NewGuid();
                        customerContactLog.CustomerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                        customerContactLog.CustomerId = (customerInfo.CTId).ToString();
                        customerContactLog.JobId = employeejob.Id.ToString();
                        customerContactLog.Note = jobPerform + " successfully !";
                        customerContactLog.LogDate = DateTime.Now;
                        customerContactLog.IsDelete = false;
                        customerContactLog.CreatedDate = DateTime.Now;
                        customerContactLog.CreatedBy = loginUser;

                        db.CustomerContactLog.Add(customerContactLog);
                        db.SaveChanges();

                        ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                        response = CommonFunctions.GetUserInfoByToken();
                        string userId = response.ResponseList[0].UserId;
                        string userName = response.ResponseList[0].UserName;


                        log4net.ThreadContext.Properties["UserId"] = userId;
                        log.Info(userName + " save user time sheet.");

                        result.ResponseList = new List<string> { "Record saved successfully." };
                        result.ResponseCode = 1;
                        result.ResponseErrorMessage = null;
                        return result;
                    }
                    else
                    {
                        result.ResponseList = new List<string>();
                        result.ResponseCode = 0;
                        result.ResponseErrorMessage = "JobPerform is not valid !";
                        return result;
                    }
                }
                else
                {
                    result.ResponseList = new List<string>();
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "JobId, UserId, & JobPerform are mandatory !";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }


        private void UserMessageSend(Guid userId, string reason)
        {
            Guid allOperationUserId = Guid.Parse("3360DC19-7B64-4D53-854C-BD387BDB72F0");
            Guid operationRole = Guid.Parse("cde8f045-239f-4531-aa17-d8aecb0fa732");
            var employeerList = db.EmployeeDetail.Where(m => m.Role == operationRole && m.IsDelete == false && m.IsActive == true).AsEnumerable().OrderBy(m => m.UserName);
            var empUserName = db.EmployeeDetail.Where(m => m.EmployeeId == userId).Select(m => m.UserName).FirstOrDefault();

            foreach (var Emp in employeerList)
            {
                Guid toId = Emp.EmployeeId;
                var ThreadList = db.UserMessageThread.Where(i => i.LoggedInUser == userId && i.ToMessageUser == allOperationUserId).FirstOrDefault();
                //var ThreadList = UserMessageThreadRepo.FindBy(i => i.LoggedInUser == fromId && i.ToMessageUser == toId).FirstOrDefault();
                var ThreadReverseList = db.UserMessageThread.Where(i => i.LoggedInUser == toId && i.ToMessageUser == userId).FirstOrDefault();
                if (ThreadList == null)
                {
                    UserMessageThreadviewModel userMessageThreadviewModel = new UserMessageThreadviewModel();
                    userMessageThreadviewModel.ID = Guid.NewGuid();
                    userMessageThreadviewModel.LoggedInUser = userId;
                    userMessageThreadviewModel.ToMessageUser = allOperationUserId;
                    userMessageThreadviewModel.CreatedDate = DateTime.Now;
                    userMessageThreadviewModel.CreatedBy = userId;
                    userMessageThreadviewModel.ModifiedBy = userId;
                    userMessageThreadviewModel.ModifiedDate = DateTime.Now;
                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageThreadviewModel, UserMessageThread> mapper = new CommonMapper<UserMessageThreadviewModel, UserMessageThread>();
                    UserMessageThread userMessageThread = mapper.Mapper(userMessageThreadviewModel);
                    db.UserMessageThread.Add(userMessageThread);
                    db.SaveChanges();

                }
                else
                {
                    Guid threadId = ThreadList.ID;
                    UserMessageThread ThreadDetail = db.UserMessageThread.Where(i => i.ID == threadId).FirstOrDefault();

                    ThreadDetail.ModifiedBy = userId;
                    ThreadDetail.ModifiedDate = DateTime.Now;
                    db.Entry(ThreadDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (ThreadReverseList == null)
                {
                    UserMessageThreadviewModel userMessageThreadReverseviewModel = new UserMessageThreadviewModel();
                    userMessageThreadReverseviewModel.ID = Guid.NewGuid();
                    userMessageThreadReverseviewModel.LoggedInUser = toId;
                    userMessageThreadReverseviewModel.ToMessageUser = userId;
                    userMessageThreadReverseviewModel.CreatedDate = DateTime.Now;
                    userMessageThreadReverseviewModel.CreatedBy = userId;
                    userMessageThreadReverseviewModel.ModifiedBy = userId;
                    userMessageThreadReverseviewModel.ModifiedDate = DateTime.Now;

                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserMessageThreadviewModel, UserMessageThread> mapper = new CommonMapper<UserMessageThreadviewModel, UserMessageThread>();
                    UserMessageThread userMessageThread = mapper.Mapper(userMessageThreadReverseviewModel);
                    db.UserMessageThread.Add(userMessageThread);
                    db.SaveChanges();
                }
                else
                {
                    Guid threadReverseId = ThreadReverseList.ID;
                    UserMessageThread ThreadReverseDetail = db.UserMessageThread.Where(i => i.ID == threadReverseId).FirstOrDefault();

                    ThreadReverseDetail.ModifiedBy = userId;
                    ThreadReverseDetail.ModifiedDate = DateTime.Now;

                    db.Entry(ThreadReverseDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var ThreadUserReverseMessage = db.UserMessageThread.Where(i => i.LoggedInUser == toId && i.ToMessageUser == userId).FirstOrDefault();

                if (ThreadUserReverseMessage != null)
                {
                    UserSendMessageViewModel userMessageReverseViewModel = new UserSendMessageViewModel();
                    userMessageReverseViewModel.ID = Guid.NewGuid();
                    userMessageReverseViewModel.MessageThreadID = ThreadUserReverseMessage.ID;
                    userMessageReverseViewModel.From_Id = userId;
                    userMessageReverseViewModel.To_Id = toId;
                    userMessageReverseViewModel.Message = "" + empUserName + " left the job due to this reason " + reason + ".";
                    userMessageReverseViewModel.IsMessageRead = false;
                    userMessageReverseViewModel.CreatedDate = DateTime.Now;
                    userMessageReverseViewModel.CreatedBy = userId;

                    // mapping <viewmodel> to <entity>
                    CommonMapper<UserSendMessageViewModel, UserMessage> mappers = new CommonMapper<UserSendMessageViewModel, UserMessage>();
                    UserMessage userReverseMessage = mappers.Mapper(userMessageReverseViewModel);
                    db.UserMessage.Add(userReverseMessage);
                    db.SaveChanges();
                }
            }
            var ThreadUserMessage = db.UserMessageThread.Where(i => i.LoggedInUser == userId && i.ToMessageUser == allOperationUserId).FirstOrDefault();
            if (ThreadUserMessage != null)
            {
                UserSendMessageViewModel userMessageViewModel = new UserSendMessageViewModel();
                userMessageViewModel.ID = Guid.NewGuid();
                userMessageViewModel.MessageThreadID = ThreadUserMessage.ID;
                userMessageViewModel.From_Id = userId;
                userMessageViewModel.To_Id = allOperationUserId;
                userMessageViewModel.Message = "" + empUserName + " left the job due to this reason " + reason + ".";
                userMessageViewModel.IsMessageRead = true;
                userMessageViewModel.CreatedDate = DateTime.Now;
                userMessageViewModel.CreatedBy = userId;

                // mapping <viewmodel> to <entity>
                CommonMapper<UserSendMessageViewModel, UserMessage> mapper = new CommonMapper<UserSendMessageViewModel, UserMessage>();
                UserMessage userMessage = mapper.Mapper(userMessageViewModel);
                db.UserMessage.Add(userMessage);
                db.SaveChanges();
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/IsTaskRunning")]
        public ServiceResponse<dynamic> IsTaskRunning(dynamic json)
        {
            ServiceResponse<dynamic> result = new ServiceResponse<dynamic>();
            try
            {
                Guid UserId = new Guid(json.UserId.Value);

                IQueryable<dynamic> resultObject = from UTS in db.UserTimeSheet
                                                   join JB in db.Jobs on UTS.JobId equals JB.Id
                                                   where UTS.UserId == UserId && UTS.IsRunning == 1 && JB.IsDelete == false
                                                   select new
                                                   {
                                                       Id = JB.Id,
                                                       JobId = JB.JobId,
                                                       Reason = UTS.Reason,
                                                       JobNo = JB.JobNo,
                                                       DateBooked = JB.DateBooked,
                                                       JobType = JB.JobType,
                                                       Status = JB.Status
                                                   };

                result.ResponseList = resultObject.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetUserHoursSpent")]
        public ServiceResponse<TimeSheetTotalViewModel> GetUserHoursSpent(dynamic json)
        {
            ServiceResponse<TimeSheetTotalViewModel> result = new ServiceResponse<TimeSheetTotalViewModel>();
            try
            {
                string JobStartDate = json.JobStartDate != null ? json.JobStartDate.Value : string.Empty;
                string JobEndDate = json.JobEndDate != null ? json.JobEndDate.Value : string.Empty;
                string UserId = json.UserId != null ? json.UserId.Value : string.Empty;

                string sql = @"DECLARE @JobStartDate nvarchar(50)= '" + JobStartDate + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + JobEndDate + @"'
                           DECLARE @UserId nvarchar(50)= '" + UserId + @"'
                           DECLARE @Sql nvarchar(max)

                           SET @Sql = '
                           SELECT * INTO #tmp FROM(SELECT
                           Reason
                          ,JobDate
                          ,cast(sum(datediff(second,0,TimeSpent))/3600 as varchar(12)) + '':'' + 
                            right(''0'' + cast(sum(datediff(second,0,TimeSpent))/60%60 as varchar(2)),2) +
                            '':'' + right(''0'' + cast(sum(datediff(second,0,TimeSpent))%60 as varchar(2)),2) as CalculatedHour 
                           FROM
                           (
                           SELECT Reason
                           ,TimeSpent
                           ,CAST(JobDate as nvarchar(50)) JobDate
                           FROM UserTimeSheet
                           
                           WHERE 1=1'

                           IF(ISNULL(Cast(@UserId as nvarchar(50)),'') <> '' )
                              BEGIN
                                    SET @Sql +=' AND UserId = '''+Cast(@UserId as nvarchar(50)) +''''
                              END

                           IF(ISNULL(cast(@JobStartDate as nvarchar(50)),'') <> '' and ISNULL(cast(@JobEndDate as nvarchar(50)),'') <> '')
                                 BEGIN
                                       SET @Sql +=' AND JobDate between '''+ cast(@JobStartDate as nvarchar(50))+''' 
						               AND '''+ cast(@JobEndDate as nvarchar(50))+''''
                                 END

                           ELSE IF(ISNULL(cast(@JobStartDate as nvarchar(50)),'') <> '' )
                                 BEGIN
                                       SET @Sql +=' AND JobDate >= '''+ cast(@JobStartDate as nvarchar(50))+''''
                                 END

                           ELSE IF(ISNULL(cast(@JobEndDate as nvarchar(50)),'') <> '' )
                                 BEGIN
                                       SET @Sql +=' AND JobDate <= '''+ cast(@JobEndDate as nvarchar(50))+''''
                                 END

                           SET @Sql +=')t
                           Group by t.Reason,t.JobDate)tt'
                          
                            
                        SET @Sql +=' DECLARE @cols AS NVARCHAR(MAX),
                            @query  AS NVARCHAR(MAX)

                        select @cols = STUFF((SELECT '','' + QUOTENAME(Reason) 
                                            from #tmp
                                            group by Reason
                                            order by Reason
                                    FOR XML PATH(''''), TYPE
                                    ).value(''.'', ''NVARCHAR(MAX)'') 
                                ,1,1,'''')

                        set @query = ''SELECT JobDate,'' + @cols + '' from 
                                     (
                                        select Reason, JobDate, CalculatedHour
                                        from #tmp
                                    ) x
                                    pivot 
                                    (
                                        MAX(CalculatedHour)  
                                        for Reason in ('' + @cols + '')
                                    ) p ''

                      
                         EXEC(@query)
                           

'
                           EXEC(@Sql)";

                var userHoursSpent = db.Database.SqlQuery<TimeSheetTotalViewModel>(sql).AsQueryable();
                result.ResponseList = userHoursSpent.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get hours spent by user.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.Message;
                return result;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/jobs/GetLinkJobList")]
        public ServiceResponse<Joblist> GetLinkJobList(string CustomerGeneralinfoId)
        {

            Guid Customerid = Guid.Parse(CustomerGeneralinfoId);
            ServiceResponse<Joblist> result = new ServiceResponse<Joblist>();
            try
            {

                List<Joblist> listjob = new List<Joblist>();

                //var jobs = EmployeeJob.FindBy(i => i.JobType == (int)Constants.JobType.Do).OrderBy(i => i.JobId).Skip(pageNumber*30).Take(30);
                //foreach (var job in jobs)
                //{
                //    var Linkedid = SupportJoblink.FindBy(i => i.LinkedJobId == job.Id).FirstOrDefault();
                //    if (Linkedid == null)
                //    {
                //        Joblist obj = new Joblist();
                //        obj.Id = job.Id;
                //        obj.JobId = Convert.ToInt32(job.JobId);
                //        listjob.Add(obj);
                //    }
                //}

                var jobs = EmployeeJob.FindBy(m => m.CustomerGeneralInfoId == Customerid).OrderBy(i => i.JobId);
                foreach (var job in jobs)
                {
                    var Linkedid = SupportJoblink.FindBy(i => i.LinkedJobId == job.Id).FirstOrDefault();
                    if (Linkedid == null)
                    {
                        Joblist obj = new Joblist();
                        obj.Id = job.Id;
                        obj.JobId = Convert.ToInt32(job.JobId);
                        listjob.Add(obj);
                    }
                }


                result.ResponseList = listjob.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get support job link.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobBillingAddressList")]
        public ServiceResponse<GetBillingListViewModel> GetJobBillingAddressList(dynamic json)
        {
            Guid JobId = new Guid(json.Id.Value);
            var CustomerGeneralInfoId = EmployeeJob.FindBy(i => i.Id == JobId && i.IsDelete == false).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
            ServiceResponse<GetBillingListViewModel> result = new ServiceResponse<GetBillingListViewModel>();
            try
            {
                IQueryable<GetBillingListViewModel> lstCustomerSites = from customerBillingDetail in db.CustomerBillingAddress
                                                                       where customerBillingDetail.CustomerGeneralInfoId == CustomerGeneralInfoId && customerBillingDetail.IsDelete == false
                                                                       select new GetBillingListViewModel
                                                                       {
                                                                           BillingAddressId = customerBillingDetail.BillingAddressId,
                                                                           FirstName = customerBillingDetail.FirstName,
                                                                           LastName = customerBillingDetail.LastName,
                                                                           MobileNo = customerBillingDetail.PhoneNo1,
                                                                           LandlineNo = customerBillingDetail.PhoneNo2,
                                                                           AlternateNo = customerBillingDetail.PhoneNo3,
                                                                           EmailId = customerBillingDetail.EmailId,
                                                                           ContactPosition = customerBillingDetail.ContactPosition,
                                                                           StrataPlan = customerBillingDetail.StrataPlan,
                                                                           RealEstate = customerBillingDetail.RealEstate,
                                                                           BillingNotes = customerBillingDetail.Spare1,
                                                                           BillingAddress = customerBillingDetail.Unit + " " + customerBillingDetail.StreetNo + " " + customerBillingDetail.StreetName + " " +
                                                                                        customerBillingDetail.Suburb + " " + customerBillingDetail.State
                                                                       };

                result.ResponseList = lstCustomerSites.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job biling address list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobContactLogList")]
        public ServiceResponse<GetContactLogListViewModel> GetJobContactLogList(dynamic json)
        {
            Guid JobId = Guid.Parse(json.Id.Value);
            var siteId = db.Jobs.Where(m => m.Id == JobId).Select(m => m.SiteId).FirstOrDefault();
            ServiceResponse<GetContactLogListViewModel> result = new ServiceResponse<GetContactLogListViewModel>();
            try
            {
                IQueryable<GetContactLogListViewModel> lstCustomerSites = from contactLog in db.CustomerContactLog
                                                                          join job in db.Jobs on contactLog.JobId equals job.Id.ToString()
                                                                          where job.SiteId == siteId && contactLog.IsDelete == false
                                                                          select new GetContactLogListViewModel
                                                                          {
                                                                              ContactLogId = contactLog.CustomerContactId,
                                                                              CustomerGeneralInfoId = contactLog.CustomerGeneralInfoId,
                                                                              JobId = contactLog.JobId,
                                                                              LogDate = contactLog.LogDate,
                                                                              RecontactDate = contactLog.ReContactDate,
                                                                              ContactLogNote = contactLog.Note,
                                                                              JobNo = job.JobNo
                                                                          };

                result.ResponseList = lstCustomerSites.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job contact log list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobStockList")]
        public ServiceResponse<JobStockListViewmodel> GetJobStockList(dynamic json)
        {
            Guid JobId = Guid.Parse(json.Id.Value);
            ServiceResponse<JobStockListViewmodel> result = new ServiceResponse<JobStockListViewmodel>();
            try
            {
                IQueryable<JobStockListViewmodel> lstCustomerSites = from jobStock in db.JobStock
                                                                     join stock in db.Stock on jobStock.StockID equals stock.ID
                                                                     where jobStock.JobId == JobId && jobStock.IsDelete == false
                                                                     select new JobStockListViewmodel
                                                                     {
                                                                         Id = jobStock.ID,
                                                                         StockId = jobStock.StockID,
                                                                         StockName = stock.Label,
                                                                         JobId = jobStock.JobId,
                                                                         UnitMeasure = jobStock.UnitMeasure,
                                                                         Price = jobStock.Price,
                                                                         Quantity = jobStock.Quantity
                                                                     };

                result.ResponseList = lstCustomerSites.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job stock list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobSiteContactsList")]
        public ServiceResponse<SIteContactsListViewModel> GetJobSiteContactsList(dynamic json)
        {
            Guid JobId = Guid.Parse(json.Id.Value);
            var SiteId = EmployeeJob.FindBy(i => i.Id == JobId && i.IsDelete == false).Select(m => m.SiteId).FirstOrDefault();
            ServiceResponse<SIteContactsListViewModel> result = new ServiceResponse<SIteContactsListViewModel>();
            try
            {
                IQueryable<SIteContactsListViewModel> lstCustomerSites = from contacts in db.CustomerContacts
                                                                         where contacts.SiteId == SiteId && contacts.IsDelete == false
                                                                         select new SIteContactsListViewModel
                                                                         {
                                                                             ContactId = contacts.ContactId,
                                                                             SiteId = contacts.SiteId,
                                                                             ContactsType = contacts.ContactsType,
                                                                             Title = contacts.Title,
                                                                             FirstName = contacts.FirstName,
                                                                             LastName = contacts.LastName,
                                                                             MobileNo = contacts.PhoneNo1,
                                                                             LandlineNo = contacts.PhoneNo2,
                                                                             AlternateNo = contacts.PhoneNo3,
                                                                             EmailId = contacts.EmailId,
                                                                             ContactsNotes = contacts.Spare1
                                                                         };

                result.ResponseList = lstCustomerSites.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job site contact list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateSiteDetail")]
        public ServiceResponse<string> UpdateSiteDetail(SiteDetailViewModel siteDetailViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<SiteDetailViewModel, CustomerSiteDetail> mapper = new CommonMapper<SiteDetailViewModel, CustomerSiteDetail>();
                CustomerSiteDetail siteDetailEnity = mapper.Mapper(siteDetailViewModel);
                siteDetailEnity.IsDelete = false;
                db.Entry(siteDetailEnity).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update site details.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateConditionDetail")]
        public ServiceResponse<string> UpdateConditionDetail(ConditionDetailViewModel conditionDetailViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<ConditionDetailViewModel, CustomerConditionReport> mapper = new CommonMapper<ConditionDetailViewModel, CustomerConditionReport>();
                CustomerConditionReport conditionDetailEnity = mapper.Mapper(conditionDetailViewModel);
                conditionDetailEnity.IsDelete = false;

                if (conditionDetailViewModel.ConditionReportId != null && conditionDetailViewModel.ConditionReportId != Guid.Empty)
                {
                    db.Entry(conditionDetailEnity).State = EntityState.Modified;
                    db.SaveChanges();
                    result.ResponseList = new List<string> { "Record updated successfully." };
                }
                else
                {
                    conditionDetailEnity.ConditionReportId = Guid.NewGuid();
                    db.CustomerConditionReport.Add(conditionDetailEnity);
                    db.SaveChanges();
                    result.ResponseList = new List<string> { "Record saved successfully." };
                }
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update condition details.");

                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateStockDetail")]
        public ServiceResponse<string> UpdateStockDetail(StockDetailViewModel stockDetailViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<StockDetailViewModel, Stock> mapper = new CommonMapper<StockDetailViewModel, Stock>();
                Stock stockDetailEnity = mapper.Mapper(stockDetailViewModel);
                stockDetailEnity.IsDelete = false;
                db.Entry(stockDetailEnity).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update stock details.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateResidenceDetail")]
        public ServiceResponse<string> UpdateResidenceDetail(ResidenceDetailViewModel residenceDetailViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<ResidenceDetailViewModel, CustomerResidenceDetail> mapper = new CommonMapper<ResidenceDetailViewModel, CustomerResidenceDetail>();
                CustomerResidenceDetail residenceDetailEnity = mapper.Mapper(residenceDetailViewModel);
                residenceDetailEnity.IsDelete = false;
                db.Entry(residenceDetailEnity).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update residence list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        // POST: api/AddJobRiskAssessment
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/AddJobRiskAssessment")]
        public ServiceResponse<string> AddJobRiskAssessment(JobRiskAssessMentViewModel Model)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Model.Id = Guid.NewGuid();
                CommonMapper<JobRiskAssessMentViewModel, JobRiskAssessment> mapper = new CommonMapper<JobRiskAssessMentViewModel, JobRiskAssessment>();
                JobRiskAssessment JobRiskEnity = mapper.Mapper(Model);
                JobRiskEnity.CreatedDate = DateTime.Now;
                db.JobRiskAssessment.Add(JobRiskEnity);
                db.SaveChanges();

                result.ResponseList = new List<string> { "Record Inserted successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " add job risk assessment.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/UpdateJobRiskAssessment")]
        public ServiceResponse<string> UpdateJobRiskAssessment(JobRiskAssessMentViewModel Model)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<JobRiskAssessMentViewModel, JobRiskAssessment> mapper = new CommonMapper<JobRiskAssessMentViewModel, JobRiskAssessment>();
                JobRiskAssessment JobRiskEnity = mapper.Mapper(Model);
                JobRiskEnity.ModifiedDate = DateTime.Now;
                db.Entry(JobRiskEnity).State = EntityState.Modified;
                db.SaveChanges();
                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update job risk assessment.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobRiskAssementByJobId")]
        public ServiceResponse<JobRiskAssessMentViewModel> GetJobRiskAssementByJobId(dynamic json)
        {
            ServiceResponse<JobRiskAssessMentViewModel> result = new ServiceResponse<JobRiskAssessMentViewModel>();
            try
            {
                Guid JobId = new Guid(json.JobId.Value);
                IQueryable<JobRiskAssessMentViewModel> lstJobRiskAssessMent = from jobRiskAssessment in db.JobRiskAssessment
                                                                              where jobRiskAssessment.JobId == JobId
                                                                              select new JobRiskAssessMentViewModel
                                                                              {
                                                                                  Id = jobRiskAssessment.Id,
                                                                                  JobId = jobRiskAssessment.JobId,
                                                                                  SiteInspected = jobRiskAssessment.SiteInspected,
                                                                                  IssuedPPE = jobRiskAssessment.IssuedPPE,
                                                                                  ReviewedPublicSafety = jobRiskAssessment.ReviewedPublicSafety,
                                                                                  ElectricalRisk = jobRiskAssessment.ElectricalRisk,
                                                                                  SafetySystemAvailable = jobRiskAssessment.SafetySystemAvailable,
                                                                                  WeatherConsidered = jobRiskAssessment.WeatherConsidered
                                                                              };

                result.ResponseList = lstJobRiskAssessMent.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job risk assessment by job id.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/jobs/GetEmployeeJobs")]
        public ServiceResponsePaging<JobsViewModel> GetEmployeeJobs(string id)
        {
            ServiceResponsePaging<JobsViewModel> result = new ServiceResponsePaging<JobsViewModel>();
            try
            {
                Guid jobId = Guid.Parse(id);
                var jobsViewModel = from j in db.Jobs
                                    join m in db.JobAssignToMapping on j.Id equals m.JobId
                                    where m.AssignTo == jobId && j.IsDelete == false
                                    select new JobsViewModel
                                    {
                                        jobId = j.Id.ToString(),
                                        jobNo = j.JobNo.ToString()
                                    };
                result.ResponseList = jobsViewModel.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get employee job details.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;

            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/AddContactLog")]
        public ServiceResponse<string> AddContactLog(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid JobId = new Guid(json.EmployeeJobId.Value);
                Guid LoggedInUser = new Guid(json.LogInUser.Value);
                if (JobId != Guid.Empty)
                {
                    CustomerContactLogViewModel model = new CustomerContactLogViewModel();
                    var JobsInfo = db.Jobs.Where(m => m.Id == JobId).FirstOrDefault();
                    var customerInfo = db.CustomerGeneralInfo.Where(m => m.CustomerGeneralInfoId == JobsInfo.CustomerGeneralInfoId).FirstOrDefault();
                    model.CustomerContactId = Guid.NewGuid();
                    model.CustomerGeneralInfoId = JobsInfo.CustomerGeneralInfoId;
                    model.CustomerId = customerInfo.CTId.ToString();
                    model.JobId = JobId.ToString();
                    model.LogDate = DateTime.Now;
                    model.Note = "Invoice Sent Successfully!";
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = LoggedInUser;
                    model.IsDelete = false;

                    CommonMapper<CustomerContactLogViewModel, CustomerContactLog> mapper = new CommonMapper<CustomerContactLogViewModel, CustomerContactLog>();
                    CustomerContactLog ContactLogEnity = mapper.Mapper(model);
                    db.CustomerContactLog.Add(ContactLogEnity);
                    db.SaveChanges();

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " add contact log.");

                }
                result.ResponseList = new List<string> { "Record Inserted successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        // POST: api/SaveJobsReason
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/SaveJobsReason")]
        public ServiceResponse<string> SaveJobsReason(JobsReasonViewModel reasonViewmodel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                // Job Reason Save
                reasonViewmodel.ReasonId = Guid.NewGuid();
                CommonMapper<JobsReasonViewModel, JobsReasonMapping> mapper = new CommonMapper<JobsReasonViewModel, JobsReasonMapping>();
                JobsReasonMapping reasonEnity = mapper.Mapper(reasonViewmodel);
                reasonEnity.ReasonTime = reasonViewmodel.ReasonTime;
                reasonEnity.CreatedDate = DateTime.Now;
                reasonEnity.CreatedBy = reasonViewmodel.CreatedBy;
                db.JobsReasonMapping.Add(reasonEnity);
                db.SaveChanges();

                //Save Contact log
                CustomerContactLogViewModel model = new CustomerContactLogViewModel();
                var JobsInfo = db.Jobs.Where(m => m.Id == reasonViewmodel.JobId).FirstOrDefault();
                var customerInfo = db.CustomerGeneralInfo.Where(m => m.CustomerGeneralInfoId == JobsInfo.CustomerGeneralInfoId).FirstOrDefault();
                model.CustomerContactId = Guid.NewGuid();
                model.CustomerGeneralInfoId = JobsInfo.CustomerGeneralInfoId;
                model.CustomerId = customerInfo.CTId.ToString();
                model.JobId = reasonViewmodel.JobId.ToString();
                model.LogDate = DateTime.Now;
                model.Note = reasonViewmodel.Reason;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = reasonViewmodel.CreatedBy;
                model.IsDelete = false;

                CommonMapper<CustomerContactLogViewModel, CustomerContactLog> mappers = new CommonMapper<CustomerContactLogViewModel, CustomerContactLog>();
                CustomerContactLog ContactLogEnity = mappers.Mapper(model);
                db.CustomerContactLog.Add(ContactLogEnity);
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " Save job reason.");

                result.ResponseList = new List<string> { Convert.ToString(reasonViewmodel.ReasonId) };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/jobs/GetJobTask")]
        public ServiceResponsePaging<GetJobTaskViewModel> GetJobTask(dynamic json)
        {
            ServiceResponsePaging<GetJobTaskViewModel> result = new ServiceResponsePaging<GetJobTaskViewModel>();
            try
            {
                Guid jobId = new Guid(json.JobId.Value);

                IQueryable<GetJobTaskViewModel> lstGetJobTaskViewModel = from cT in db.CompulsaryTask
                                                                         join jT in db.JobTaskMapping on cT.TaskId equals jT.CompulsaryId
                                                                         where jT.JobId == jobId
                                                                         select new GetJobTaskViewModel
                                                                         {
                                                                             Task = cT.TaskName
                                                                         };

                result.ResponseList = lstGetJobTaskViewModel.ToList().Select(m => new GetJobTaskViewModel
                {
                    Task = m.Task
                }).ToList();

                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get compulsary task.");

                return result;

            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;

            }
        }
    }
}
