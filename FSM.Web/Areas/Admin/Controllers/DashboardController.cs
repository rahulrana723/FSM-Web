using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Admin.ViewModels;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using FSM.Web.FSMConstant;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using FSM.Web.Areas.Customer.ViewModels;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Rotativa;
using TransmitSms;
using System.Text.RegularExpressions;
using log4net;
using System.Text;

namespace FSM.Web.Areas.Admin.Controllers
{

    [Authorize]
    public class DashboardController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [Dependency]
        public IEmployeeJobRepository Employeejob { get; set; }
        [Dependency]
        public IEmployeeDetailRepository EmployeeDetailRepo { get; set; }
        [Dependency]
        public IEmployeeJobRepository JobRepository { get; set; }
        [Dependency]
        public IEmployeeDetailRepository EmployeeRepo { get; set; }

        [Dependency]
        public IWebNotificationRepository WebNotificationRepo { get; set; }
        [Dependency]
        public IUserTimeSheetRepository UserTimeSheetRepo { get; set; }
        [Dependency]
        public IVacationRepository VacationRepo { get; set; }
        [Dependency]
        public IJobAssignToMappingRepository JobAssignToMappingRepo { get; set; }
        [Dependency]
        public IEmployeeWorkTypeRepository EmployeeWorktypeRepo { get; set; }

        [Dependency]
        public ICustomerGeneralInfoRepository CustomerGeneralRepo { get; set; }
        [Dependency]
        public ICustomerSiteDetailRepository CustomerSiteDetailRepo { get; set; }

        [Dependency]
        public IEmployeeJobRepository CustJobsRepo { get; set; }
        [Dependency]
        public ICustomerContactsRepository CustomerContactRepo { get; set; }
        [Dependency]
        public ICustomerReminderRepository CustomerReminderRepo { get; set; }
        [Dependency]
        public ICustomerResidenceDetailRepository CustomerResidenceRepo { get; set; }
        [Dependency]
        public IiNoviceRepository InvoiceRepo { get; set; }

        [Dependency]
        public IAspNetUsersRepository AspNetUsersRepo { get; set; }

        [Dependency]
        public ICustomerJobReminderMapping CustomerJobReminderMapping { get; set; }

        [Dependency]
        public ICustomerContactLogRepository CustomercontactLogRepo { get; set; }

        [Dependency]
        public IContactLogSiteContactsMappingRepository ContactLogSiteContactsMappingRepo { get; set; }

        [Dependency]
        public IScheduleReminderRepository ScheduleReminderRepo { get; set; }
        [Dependency]
        public IPurchaseOrderByJobRepository JobPurchaseOrder { get; set; }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// Not called anywhere
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetEmployeejobList(string param)
        {
            try
            {
                using (Employeejob)
                {
                    log.Info(base.GetUserName + " viewed list of Employee job");

                    List<EmployeeJobsViewModel> EmployeejobViewModel = new List<EmployeeJobsViewModel>();
                    DashboardViewModels dashboardmodel = new DashboardViewModels();
                    var joblist = Employeejob.GetEmployeeJobs();
                    if (joblist != null)
                    {
                        CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                        EmployeejobViewModel = mapper.MapToList(joblist.ToList());
                        dashboardmodel.EmployeeJoblist = EmployeejobViewModel.ToList();
                        if (!String.IsNullOrEmpty(param))
                        {
                            Constant.JobType jobtypeid;
                            Constant.JobType.TryParse(param, out jobtypeid);
                            dashboardmodel.EmployeeJoblist = dashboardmodel.EmployeeJoblist.Where(i => i.JobType == jobtypeid);
                        }
                        else
                        {
                            dashboardmodel.EmployeeJoblist = dashboardmodel.EmployeeJoblist.OrderByDescending(i => i.JobNo);
                        }
                    }
                    else
                    {
                        dashboardmodel.EmployeeJoblist = EmployeejobViewModel;
                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(dashboardmodel.EmployeeJoblist);
                    return Json(new { list = json, length = dashboardmodel.EmployeeJoblist.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Not called anywhere
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJobListingfordashboard()
        {
            try
            {
                using (Employeejob)
                {

                    log.Info(base.GetUserName + " viewed list of job on dashboard");

                    var joblist = Employeejob.GetEmployeeJobs();
                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(joblist.ToList());
                    DashboardViewModels dashboardmodel = new DashboardViewModels
                    {
                        EmployeeJoblist = EmployeejobViewModel.ToList()
                    };
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(dashboardmodel.EmployeeJoblist);
                    return Json(dashboardmodel.EmployeeJoblist, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }


        public ActionResult GeJobinfoByJobId(string Jobid, string ID)
        {
            try
            {
                int Job_id = Convert.ToInt32(Jobid);
                using (Employeejob)
                {
                    //var debugSqlLogger = LogManager.GetLogger("DebugSqlLogger");
                    //MDC.Set("user", "Nancy");
                    //MDC.Set("application", "ApplicationName");
                    //using (log4net.NDC.Push(Guid.NewGuid().ToString()))
                    //{
                    //    debugSqlLogger.Debug("debug message!");
                    //}

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job descripton by job id");

                    List<DashboardjobInfoViewModel> EmployeejobViewModel = new List<DashboardjobInfoViewModel>();
                    string id = "";
                    if (String.IsNullOrEmpty(ID))
                    {
                        id = Convert.ToString(Employeejob.FindBy(i => i.JobNo == Job_id).FirstOrDefault().Id);
                    }
                    else
                    {
                        id = ID;
                    }
                    var joblist = Employeejob.GetEmployeeJobsByJobid(id);
                    CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel> mapper = new CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel>();
                    EmployeejobViewModel = mapper.MapToList(joblist.ToList());
                    if (EmployeejobViewModel.Count > 0)
                    {
                        EmployeejobViewModel.FirstOrDefault().JobTypeName =
                                                                 (int)EmployeejobViewModel.FirstOrDefault().JobType > 0 ?
                                                                   Convert.ToString((Constant.JobType)EmployeejobViewModel.FirstOrDefault().JobType) : "Not Available";
                        EmployeejobViewModel.FirstOrDefault().WorkTypeName =
                                                               (int)EmployeejobViewModel.FirstOrDefault().WorkType > 0 ?
                                                                 Convert.ToString((Constant.WorkType)EmployeejobViewModel.FirstOrDefault().WorkType) : "Not Available";
                        EmployeejobViewModel.FirstOrDefault().StatusText =
                                                                (int)EmployeejobViewModel.FirstOrDefault().Status > 0 ?
                                                                  Convert.ToString((Constant.JobStatus)EmployeejobViewModel.FirstOrDefault().Status) : "Not Available";
                        EmployeejobViewModel.FirstOrDefault().PreferTimeText =
                                                                (int)EmployeejobViewModel.FirstOrDefault().PreferTime > 0 ?
                                                               Convert.ToString((Constant.PrefTimeOfDay)EmployeejobViewModel.FirstOrDefault().PreferTime) : "Not Available";
                        if (EmployeejobViewModel.FirstOrDefault().DateBooked != null)
                        {
                            EmployeejobViewModel.FirstOrDefault().Datetimetext = EmployeejobViewModel.FirstOrDefault().DateBooked.Value.ToString("dd/MM/yyyy");

                        }
                        else
                        {
                            EmployeejobViewModel.FirstOrDefault().Datetimetext = "Not Available";
                        }
                        EmployeejobViewModel.FirstOrDefault().JobNotes = string.IsNullOrEmpty(EmployeejobViewModel.FirstOrDefault().JobNotes) ? "Not Available" : EmployeejobViewModel.FirstOrDefault().JobNotes;
                        EmployeejobViewModel.FirstOrDefault().OperationNotes = string.IsNullOrEmpty(EmployeejobViewModel.FirstOrDefault().OperationNotes) ? "Not Available" : EmployeejobViewModel.FirstOrDefault().OperationNotes;


                        EmployeejobViewModel.FirstOrDefault().StreetTypeName =
                                                                (int)EmployeejobViewModel.FirstOrDefault().StreetType > 0 ?
                                                               Convert.ToString((Constant.MailingAddressSteetType)EmployeejobViewModel.FirstOrDefault().StreetType) : "";

                        string postaltext = (int)EmployeejobViewModel.FirstOrDefault().PostalCode > 0 ?
                                                             Convert.ToString(EmployeejobViewModel.FirstOrDefault().PostalCode) : "";
                        var jobAddress = "";
                        jobAddress = EmployeejobViewModel.FirstOrDefault().Street + " "
                                        + EmployeejobViewModel.FirstOrDefault().StreetName + " "
                                        + EmployeejobViewModel.FirstOrDefault().StreetTypeName + " "
                                        + EmployeejobViewModel.FirstOrDefault().State + " "
                                        + postaltext;
                        EmployeejobViewModel.FirstOrDefault().jobAddress = jobAddress;
                        EmployeejobViewModel.FirstOrDefault().StreetName = string.IsNullOrEmpty(EmployeejobViewModel.FirstOrDefault().StreetName) ? "Not Available" : EmployeejobViewModel.FirstOrDefault().StreetName;
                        EmployeejobViewModel.FirstOrDefault().SiteNotes = String.IsNullOrEmpty(EmployeejobViewModel.FirstOrDefault().SiteNotes) ? "Not Available" : EmployeejobViewModel.FirstOrDefault().SiteNotes;
                        EmployeejobViewModel.FirstOrDefault().CustomerNotes = String.IsNullOrEmpty(EmployeejobViewModel.FirstOrDefault().CustomerNotes) ? "Not Available" : EmployeejobViewModel.FirstOrDefault().CustomerNotes;
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(EmployeejobViewModel);
                    return Json(new { list = json, length = EmployeejobViewModel.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }

        public ActionResult GetEmployeeJobsInfo()
        {
            try
            {
                using (Employeejob)
                {
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed information of jobs all OTRW.");

                    List<FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> jobScheduleModel;
                    List<DashboardjobInfoViewModel> EmployeejobViewModel = new List<DashboardjobInfoViewModel>();
                    var joblist = Employeejob.GetEmployeeJobsInfo().OrderBy(m => m.JobNo);
                    CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel> mapper = new CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel>();
                    EmployeejobViewModel = mapper.MapToList(joblist.ToList());

                    if (EmployeejobViewModel.Count > 0)
                    {
                        foreach (var emp in EmployeejobViewModel)
                        {
                            emp.JobTypeName = (int)emp.JobType > 0 ? Convert.ToString((Constant.JobType)emp.JobType) : "Not Available";
                            emp.WorkTypeName = (int)emp.WorkType > 0 ? Convert.ToString((Constant.WorkType)emp.WorkType) : "Not Available";
                            emp.StatusText =
                                                                    (int)emp.Status > 0 ?
                                                                      Convert.ToString((Constant.JobStatus)emp.Status) : "Not Available";
                            emp.PreferTimeText =
                                                                    (int)emp.PreferTime > 0 ?
                                                                   Convert.ToString((Constant.PrefTimeOfDay)emp.PreferTime) : "Not Available";
                            if (emp.DateBooked != null)
                            {
                                emp.Datetimetext = emp.DateBooked.Value.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                emp.Datetimetext = "Not Available";
                            }
                            if (emp.WetRequiredType == true)
                            {
                                emp.WetRequiredName = "NW";
                            }
                            else
                            {
                                emp.WetRequiredName = "W";
                            }
                            if (emp.StoreysType != 5)
                            {
                                emp.StoreysName = emp.StoreysType + "ST";
                            }
                            else
                            {
                                emp.StoreysName = "Other";
                            }
                            emp.JobNotes = string.IsNullOrEmpty(emp.JobNotes) ? "Not Available" : emp.JobNotes;
                            emp.OperationNotes = string.IsNullOrEmpty(emp.OperationNotes) ? "Not Available" : emp.OperationNotes;


                            emp.StreetTypeName =
                                                                    (int)emp.StreetType > 0 ?
                                                                   Convert.ToString((Constant.MailingAddressSteetType)emp.StreetType) : "";

                            string postaltext = (int)emp.PostalCode > 0 ?
                                                                 Convert.ToString(emp.PostalCode) : "";

                            var jobAddress = "";
                            jobAddress = emp.Street + " "
                                            + emp.StreetName + " "
                                            + emp.StreetTypeName + " "
                                            + emp.State + " "
                                            + postaltext;
                            emp.jobAddress = jobAddress;
                            emp.StreetName = string.IsNullOrEmpty(emp.StreetName) ? "Not Available" : emp.StreetName;
                            if (emp.OTRWRequired == null)
                            {
                                emp.OTRWRequired = 0;
                            }
                            if (emp.OTRWAssignCount == null)
                            {
                                emp.OTRWAssignCount = 0;
                            }
                            if (emp.OTRWAssignCount > 1)
                            {
                                emp.DisplayOTRWAssignCount = "MP";
                            }
                            else
                            {
                                emp.DisplayOTRWAssignCount = "SP";
                            }
                            if (emp.JobNotes.Length > 50)
                            {
                                emp.JobNotes = emp.JobNotes.Substring(0, 50) + "...";
                            }
                            string startanumber;
                            if (!(String.IsNullOrEmpty(emp.StrataNumber)))
                            {
                                startanumber = System.Environment.NewLine + "SN:" + emp.StrataNumber;
                            }
                            else
                            {
                                startanumber = "";
                            }
                            if (!(String.IsNullOrEmpty(emp.StrataPlan)))
                            {
                                emp.Address = emp.Address + " " + emp.suburb;
                            }
                            //emp.title = !string.IsNullOrEmpty(emp.JobNo.ToString()) ? "Work Type:" + emp.WorkType + System.Environment.NewLine + "Site Address:- Suburb: " + emp.suburb + System.Environment.NewLine + emp.JobNo.ToString() + System.Environment.NewLine + "Strata No: " + emp.StrataNumber + System.Environment.NewLine + "Time: " + emp.PreferTime.ToString() + "Job Notes: " + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            //emp.title = !string.IsNullOrEmpty(emp.JobNo.ToString()) ?  emp.WorkType + System.Environment.NewLine + emp.suburb + System.Environment.NewLine + emp.JobNo.ToString() + System.Environment.NewLine +  emp.StrataNumber + System.Environment.NewLine +  emp.PreferTime.ToString() + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            emp.title = emp.WetRequiredName + ',' + emp.StreetTypeName + ',' + emp.DisplayOTRWAssignCount + System.Environment.NewLine + emp.Address != null && emp.Address != string.Empty && emp.Address != "null" ? Convert.ToString(emp.Address) + startanumber + System.Environment.NewLine + "JID:" + emp.JobId.ToString() + "," + "Type:" + emp.JobTypeName : string.Empty;
                        }
                    }

                    //schedular
                    var jobscheduleviewModel = Employeejob.GetJobSchedule().ToList();
                    CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> mapper1 = new CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel>();
                    jobScheduleModel = mapper1.MapToList(jobscheduleviewModel.OrderBy(m => m.jobNo).ToList());
                    List<Employeedetail> list = new List<Employeedetail>();
                    var employeerList = EmployeeDetailRepo.GetEmployeeDetailDashboard().AsEnumerable();
                    employeerList = employeerList.Where(m => m.IsDelete == false && m.IsActive == true).OrderBy(m => m.OTRWOrder).AsEnumerable();//otrwusers
                    foreach (var emp in employeerList)
                    {
                        list.Add(new Employeedetail
                        {
                            id = emp.EmployeeId.ToString(),
                            title = emp.UserName.ToString(),
                            eventColor = "#FFFFFF"
                        });
                    }
                    List<EventDetail> eventDetail = new List<EventDetail>();
                    foreach (var emp in jobScheduleModel)
                    {
                        Guid EmployeeId = Guid.Parse(emp.EmployeeId.ToString());
                        TimeSpan time = (TimeSpan)emp.StartTime;
                        DateTime startdate = emp.DateBooked.Value.Date + time;
                        String jobstartdate = String.Format("{0:s}", startdate);
                        TimeSpan timeend = (TimeSpan)emp.EndTime;
                        DateTime enddate = emp.DateBooked.Value.Date + timeend;
                        String jobEnddate = String.Format("{0:s}", enddate);
                        double EstimatedHrsPerUser = emp.EstimatedHrsPerUser;

                        string title = string.Empty;

                        if (emp.jobNo == "On Leave" || emp.jobNo == "Not Working")
                        {
                            title = emp.jobNo;
                        }
                        else
                        {
                            emp.JobNotes = string.IsNullOrEmpty(emp.JobNotes) ? "Not Available" : emp.JobNotes;
                            if (emp.JobNotes.Length > 50)
                            {
                                emp.JobNotes = emp.JobNotes.Substring(0, 50) + "...";
                            }
                            if (!string.IsNullOrEmpty(emp.JobNotes))
                            {
                                emp.JobNotes = Regex.Replace(emp.JobNotes, "<.*?>", String.Empty);
                            }
                            string startanumber;
                            if (!(String.IsNullOrEmpty(emp.StrataNumber)))
                            {
                                startanumber = System.Environment.NewLine + "SN:" + emp.StrataNumber;
                            }
                            else
                            {
                                startanumber = "";
                            }
                            if (!(String.IsNullOrEmpty(emp.StrataPlan)))
                            {
                                emp.Address = emp.Address + " " + emp.Suburb;
                            }

                            if (emp.WetRequiredType == true)
                            {
                                emp.WetRequiredName = "NW";
                            }
                            else
                            {
                                emp.WetRequiredName = "W";
                            }

                            if (emp.StoreysType != 5)
                            {
                                emp.StoreysName = emp.StoreysType + "ST";
                            }
                            else
                            {
                                emp.StoreysName = "Other";
                            }

                            if (emp.OTRWAssignCount == null)
                            {
                                emp.OTRWAssignCount = 0;
                            }
                            if (emp.OTRWAssignCount > 1)
                            {
                                emp.DisplayOTRWAssignCount = "MP";
                            }
                            else
                            {
                                emp.DisplayOTRWAssignCount = "SP";
                            }

                            //title = !string.IsNullOrEmpty(emp.jobNo) ? "Work Type:" + emp.WorkType + System.Environment.NewLine + "Site Address:- Suburb: " + emp.Suburb + System.Environment.NewLine + emp.jobNo.ToString() + System.Environment.NewLine + "Strata No: " + emp.StrataNumber + System.Environment.NewLine + "Time: " + emp.PreferTime.ToString() + "Job Notes: " + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            //  title = !string.IsNullOrEmpty(emp.jobNo) ?  emp.WorkType + System.Environment.NewLine +  emp.Suburb + System.Environment.NewLine + emp.jobNo.ToString() + System.Environment.NewLine +  emp.StrataNumber + System.Environment.NewLine +  emp.PreferTime.ToString() + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            //ExpandTime = !string.IsNullOrEmpty(emp.EstimatedHrsPerUser.ToString()) ? emp.EstimatedHrsPerUser.ToString() : string.Empty;
                            title = !string.IsNullOrEmpty(Convert.ToString(emp.Address)) ? "<p style='border:1px solid black;width:48%;padding:3px;color:red;margin-bottom:0'>" + emp.WetRequiredName + ',' + emp.StoreysName + ',' + emp.DisplayOTRWAssignCount + "</p>" + Convert.ToString(emp.Address) + startanumber + System.Environment.NewLine + Convert.ToString(emp.jobNo) + ",Type:" + Convert.ToString(((FSMConstant.Constant.JobType)emp.JobType)) : string.Empty;
                            //title = !string.IsNullOrEmpty(Convert.ToString(emp.Address)) ? "<p style='color:red;margin-bottom:0'>" + emp.WetRequiredName + ',' + emp.StoreysName + ',' + emp.DisplayOTRWAssignCount+ "</p>" + System.Environment.NewLine + Convert.ToString(emp.Address) + startanumber + System.Environment.NewLine + Convert.ToString(emp.jobNo) + ",Type:" + Convert.ToString(((FSMConstant.Constant.JobType)emp.JobType)) : string.Empty;
                        }

                        eventDetail.Add(new EventDetail
                        {
                            JobId = emp.jobNo,
                            id = emp.jobid.ToString() + Guid.NewGuid(),
                            resourceId = emp.EmployeeId.ToString(),
                            start = jobstartdate,
                            end = jobEnddate,
                            title = title,
                            color = EventColor(emp.JobStatus, emp.jobNo),
                            className = "DayEvent",
                            data_Job = "Job # " + emp.JobNoVal.ToString(),
                            data_CustomerName = "<b>Customer Name:&nbsp;&nbsp;</b>" + emp.CustomerLastName,
                            data_JobType = "<br><b>Job Type:&nbsp;&nbsp;</b>" + ((Constant.JobType)emp.JobType).ToString(),
                            data_Status = "<br><b>Status:&nbsp;&nbsp;</b>" + ((Constant.JobStatus)emp.JobStatus).ToString(),
                            data_Date = "<br><b>Date:&nbsp;&nbsp;</b>" + emp.DateBooked.Value.Date.ToString(),
                            data_Suburb = "<br><b>Suburb:&nbsp;&nbsp;</b>" + emp.Suburb,
                            data_JobVal = emp.JobNoVal.ToString(),
                            data_JobId = emp.jobid.ToString(),
                            data_SpanClass = GetJobStatusColor(emp.JobStatus),
                            EstimatedHrsPerUser = EstimatedHrsPerUser
                        }
                        );
                    }

                    //var availablebookedHrs = Employeejob.GetAvailableBookedHours();
                    //foreach (var item in availablebookedHrs)
                    //{
                    //    eventDetail.Add(new EventDetail
                    //    {
                    //        start = item.DateValue.Value.ToString("yyyy-MM-dd"),
                    //        end = item.DateValue.Value.ToString("yyyy-MM-dd"),
                    //        title = "Available hrs : " + item.HoursAvailable + "\n Booked hrs : " + item.HoursBooked,
                    //        className = "MonthEvent"
                    //    });
                    //}

                    if (list.Count > 0 && jobScheduleModel.Count > 0)
                    {
                        jobScheduleModel.FirstOrDefault().EmployeeList = list;
                        jobScheduleModel.FirstOrDefault().EventDetail = eventDetail;

                    }
                    else
                    {
                        jobScheduleModel = new List<ViewModels.JobScheduleViewModel>();
                        ViewModels.JobScheduleViewModel obj = new ViewModels.JobScheduleViewModel();
                        obj.EmployeeList = list;
                        obj.EventDetail = eventDetail;
                        jobScheduleModel.Add(obj);
                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(EmployeejobViewModel);
                    return Json(new { list = json, CalenderList = jobScheduleModel.ToList(), length = EmployeejobViewModel.Count() }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }


        public ActionResult GetEmployeeJobsUsingJobType(string Jobtype = "null", string Status = "null", string selectdate = "null", string searchkey = "")
        {
            try
            {
                using (Employeejob)
                {
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed OTRW job by their job type");
                    List<DashboardjobInfoViewModel> EmployeejobViewModel = new List<DashboardjobInfoViewModel>();
                    if (String.IsNullOrEmpty(Status))
                    {
                        Status = "null";
                    }

                    if (String.IsNullOrEmpty(Jobtype))
                    {
                        Jobtype = "null";
                    }
                    if (String.IsNullOrEmpty(selectdate))
                    {
                        selectdate = "null";
                    }
                    var joblist = Employeejob.GetEmployeeJobswithJobtype(Jobtype, Status, selectdate, searchkey).OrderBy(m => m.JobNo);
                    CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel> mapper = new CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel>();
                    EmployeejobViewModel = mapper.MapToList(joblist.ToList());
                    if (EmployeejobViewModel.Count > 0)
                    {
                        foreach (var emp in EmployeejobViewModel)
                        {
                            emp.JobTypeName = (int)emp.JobType > 0 ? Convert.ToString((Constant.JobType)emp.JobType) : "Not Available";
                            emp.WorkTypeName = (int)emp.WorkType > 0 ? Convert.ToString((Constant.WorkType)emp.WorkType) : "Not Available";
                            emp.StatusText =
                                                                    (int)emp.Status > 0 ?
                                                                      Convert.ToString((Constant.JobStatus)emp.Status) : "Not Available";
                            emp.PreferTimeText =
                                                                    (int)emp.PreferTime > 0 ?
                                                                   Convert.ToString((Constant.PrefTimeOfDay)emp.PreferTime) : "Not Available";
                            if (emp.DateBooked != null)
                            {
                                emp.Datetimetext = emp.DateBooked.Value.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                emp.Datetimetext = "Not Available";
                            }

                            if (emp.WetRequiredType == true)
                            {
                                emp.WetRequiredName = "NW";
                            }
                            else
                            {
                                emp.WetRequiredName = "W";
                            }

                            if (emp.StoreysType != 5)
                            {
                                emp.StoreysName = emp.StoreysType + "ST";
                            }
                            else
                            {
                                emp.StoreysName = "Other";
                            }
                            emp.JobNotes = string.IsNullOrEmpty(emp.JobNotes) ? "Not Available" : emp.JobNotes;
                            emp.OperationNotes = string.IsNullOrEmpty(emp.OperationNotes) ? "Not Available" : emp.OperationNotes;


                            emp.StreetTypeName =
                                                                    (int)emp.StreetType > 0 ?
                                                                   Convert.ToString((Constant.MailingAddressSteetType)emp.StreetType) : "";

                            string postaltext = (int)emp.PostalCode > 0 ?
                                                                 Convert.ToString(emp.PostalCode) : "";
                            var jobAddress = "";
                            jobAddress = emp.Street + " "
                                            + emp.StreetName + " "
                                            + emp.StreetTypeName + " "
                                            + emp.State + " "
                                            + postaltext;
                            emp.jobAddress = jobAddress;
                            emp.StreetName = string.IsNullOrEmpty(emp.StreetName) ? "Not Available" : emp.StreetName;
                            if (emp.OTRWRequired == null)
                            {
                                emp.OTRWRequired = 0;
                            }
                            if (emp.OTRWAssignCount == null)
                            {
                                emp.OTRWAssignCount = 0;
                            }

                            if (emp.OTRWAssignCount > 1)
                            {
                                emp.DisplayOTRWAssignCount = "MP";
                            }
                            else
                            {
                                emp.DisplayOTRWAssignCount = "SP";
                            }

                            if (emp.JobNotes.Length > 50)
                            {
                                emp.JobNotes = emp.JobNotes.Substring(0, 50) + "...";
                            }

                            if (!string.IsNullOrEmpty(emp.JobNotes))
                            {
                                emp.JobNotes = Regex.Replace(emp.JobNotes, "<.*?>", String.Empty);
                            }
                            //emp.title = !string.IsNullOrEmpty(emp.JobNo.ToString()) ? "Work Type:" + emp.WorkType + System.Environment.NewLine + "Site Address:- Suburb: " + emp.suburb + System.Environment.NewLine + "JobId: " + emp.JobNo.ToString() + System.Environment.NewLine + "Strata No: " + emp.StrataNumber + System.Environment.NewLine + "Time: " + emp.PreferTime.ToString() + "Job Notes: " + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            //  emp.title = !string.IsNullOrEmpty(emp.JobNo.ToString()) ? emp.WorkType + System.Environment.NewLine + emp.suburb + System.Environment.NewLine +  emp.JobNo.ToString() + System.Environment.NewLine +  emp.StrataNumber + System.Environment.NewLine + emp.PreferTime.ToString() + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            string startanumber;
                            if (!(String.IsNullOrEmpty(emp.StrataNumber)))
                            {
                                startanumber = System.Environment.NewLine + "SN:" + emp.StrataNumber;
                            }
                            else
                            {
                                startanumber = "";
                            }
                            if (!(String.IsNullOrEmpty(emp.StrataPlan)))
                            {
                                emp.Address = emp.Address + " " + emp.suburb;
                            }
                            emp.title = emp.WetRequiredName + ',' + emp.StreetTypeName + ',' + emp.DisplayOTRWAssignCount + emp.Address != null && emp.Address != string.Empty && emp.Address != "null" ? Convert.ToString(emp.Address) + startanumber + System.Environment.NewLine + "JID:" + emp.JobId.ToString() + "," + "Type:" + emp.JobTypeName : string.Empty;
                        }
                    }

                    var jsonSerialiser = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                    var json = jsonSerialiser.Serialize(EmployeejobViewModel);

                    //schedular
                    var jobscheduleviewModel = Employeejob.GetJobSchedule(Jobtype, Status, selectdate, searchkey).ToList();
                    CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> mapper1 = new CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel>();
                    List<FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> jobScheduleModel = mapper1.MapToList(jobscheduleviewModel.ToList());
                    List<Employeedetail> list = new List<Employeedetail>();

                    var employeerList = EmployeeDetailRepo.GetEmployeeDetailDashboard().AsEnumerable();
                    employeerList = employeerList.Where(m => m.IsDelete == false).OrderBy(m => m.OTRWOrder).AsEnumerable();//otrwusers
                    List<EventDetail> eventDetail = new List<EventDetail>();

                    foreach (var emp in employeerList)
                    {
                        list.Add(new Employeedetail { id = emp.EmployeeId.ToString(), title = emp.UserName.ToString() });
                    }


                    foreach (var emp in jobScheduleModel)
                    {
                        //    DateTime date = emp.DateBooked.Value.Date;
                        //    TimeSpan time = (TimeSpan)emp.StartTime;
                        //    DateTime startdate = date + time;
                        //    String jobstartdate = String.Format("{0:s}", startdate);
                        //    DateTime dateend = emp.DateBooked.Value.Date;
                        //    TimeSpan timeend = (TimeSpan)emp.EndTime;
                        //    DateTime enddate = dateend + timeend;
                        //    String jobEnddate = String.Format("{0:s}", enddate);
                        //    double EstimatedHrsPeruser = emp.EstimatedHrsPerUser;
                        //    eventDetail.Add(new EventDetail { JobId = emp.jobNo, id = emp.EmployeeId.ToString(), resourceId = emp.EmployeeId.ToString(), start = jobstartdate, end = jobEnddate, title = "Job_Id=" + emp.jobNo });
                        //}
                        Guid EmployeeId = Guid.Parse(emp.EmployeeId.ToString());
                        TimeSpan time = (TimeSpan)emp.StartTime;
                        DateTime startdate = emp.DateBooked.Value.Date + time;
                        String jobstartdate = String.Format("{0:s}", startdate);
                        TimeSpan timeend = (TimeSpan)emp.EndTime;
                        DateTime enddate = emp.DateBooked.Value.Date + timeend;
                        String jobEnddate = String.Format("{0:s}", enddate);
                        double EstimatedHrsPerUser = emp.EstimatedHrsPerUser;

                        string title = string.Empty;

                        if (emp.jobNo == "On Leave" || emp.jobNo == "Not Working" || emp.jobNo == "Public Holiday")
                        {
                            title = emp.jobNo;
                        }
                        else
                        {
                            emp.JobNotes = string.IsNullOrEmpty(emp.JobNotes) ? "Not Available" : emp.JobNotes;
                            if (emp.JobNotes.Length > 50)
                            {
                                emp.JobNotes = emp.JobNotes.Substring(0, 50) + "...";
                            }
                            if (!string.IsNullOrEmpty(emp.JobNotes))
                            {
                                emp.JobNotes = Regex.Replace(emp.JobNotes, "<.*?>", String.Empty);
                            }
                            string startanumber;
                            if (!(String.IsNullOrEmpty(emp.StrataNumber)))
                            {
                                startanumber = System.Environment.NewLine + "SN:" + emp.StrataNumber;
                            }
                            else
                            {
                                startanumber = "";
                            }
                            if (!(String.IsNullOrEmpty(emp.StrataPlan)))
                            {
                                emp.Address = emp.Address + " " + emp.Suburb;
                            }
                            if (emp.WetRequiredType == true)
                            {
                                emp.WetRequiredName = "NW";
                            }
                            else
                            {
                                emp.WetRequiredName = "W";
                            }

                            if (emp.StoreysType != 5)
                            {
                                emp.StoreysName = emp.StoreysType + "ST";
                            }
                            else
                            {
                                emp.StoreysName = "Other";
                            }

                            if (emp.OTRWAssignCount == null)
                            {
                                emp.OTRWAssignCount = 0;
                            }
                            if (emp.OTRWAssignCount > 1)
                            {
                                emp.DisplayOTRWAssignCount = "MP";
                            }
                            else
                            {
                                emp.DisplayOTRWAssignCount = "SP";
                            }
                            //title = !string.IsNullOrEmpty(emp.jobNo) ? "Work Type:" + emp.WorkType + System.Environment.NewLine + "Site Address:- Suburb: " + emp.Suburb + System.Environment.NewLine + emp.jobNo.ToString() + System.Environment.NewLine + "Strata No: " + emp.StrataNumber + System.Environment.NewLine + "Time: " + emp.PreferTime.ToString() + "Job Notes: " + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            //  title = !string.IsNullOrEmpty(emp.jobNo) ?  emp.WorkType + System.Environment.NewLine +  emp.Suburb + System.Environment.NewLine + emp.jobNo.ToString() + System.Environment.NewLine +  emp.StrataNumber + System.Environment.NewLine +  emp.PreferTime.ToString() + System.Environment.NewLine + emp.JobNotes : string.Empty;
                            //ExpandTime = !string.IsNullOrEmpty(emp.EstimatedHrsPerUser.ToString()) ? emp.EstimatedHrsPerUser.ToString() : string.Empty;
                            title = !string.IsNullOrEmpty(Convert.ToString(emp.Address)) ? "<p style='border:1px solid black;width:48%;padding:3px;color:red;margin-bottom:0'>" + emp.WetRequiredName + ',' + emp.StoreysName + ',' + emp.DisplayOTRWAssignCount + "</p>" + Convert.ToString(emp.Address) + startanumber + System.Environment.NewLine + Convert.ToString(emp.jobNo) + ",Type:" + Convert.ToString(((FSMConstant.Constant.JobType)emp.JobType)) : string.Empty;
                            //title = !string.IsNullOrEmpty(Convert.ToString(emp.Address)) ? emp.WetRequiredName + ',' + emp.StoreysName + ',' + emp.DisplayOTRWAssignCount + System.Environment.NewLine + Convert.ToString(emp.Address) + startanumber + System.Environment.NewLine + Convert.ToString(emp.jobNo) + ",Type:" + Convert.ToString(((FSMConstant.Constant.JobType)emp.JobType)) : string.Empty;
                        }
                        eventDetail.Add(new EventDetail
                        {
                            JobId = emp.jobNo,
                            id = emp.jobid.ToString() + Guid.NewGuid(),
                            resourceId = emp.EmployeeId.ToString(),
                            start = jobstartdate,
                            end = jobEnddate,
                            title = title,
                            color = EventColor(emp.JobStatus, emp.jobNo),
                            className = "DayEvent",
                            data_Job = "Job # " + emp.JobNoVal.ToString(),
                            data_CustomerName = "<b>Customer Name:&nbsp;&nbsp;</b>" + emp.CustomerLastName,
                            data_JobType = "<br><b>Job Type:&nbsp;&nbsp;</b>" + ((Constant.JobType)emp.JobType).ToString(),
                            data_Status = "<br><b>Status:&nbsp;&nbsp;</b>" + ((Constant.JobStatus)emp.JobStatus).ToString(),
                            data_Date = "<br><b>Date:&nbsp;&nbsp;</b>" + emp.DateBooked.Value.Date.ToString(),
                            data_Suburb = "<br><b>Suburb:&nbsp;&nbsp;</b>" + emp.Suburb,
                            data_JobVal = emp.JobNoVal.ToString(),
                            data_JobId = emp.jobid.ToString(),
                            data_SpanClass = GetJobStatusColor(emp.JobStatus),
                            EstimatedHrsPerUser = EstimatedHrsPerUser
                        }
                      );
                    }

                    if (list.Count > 0 && jobScheduleModel.Count > 0)
                    {
                        jobScheduleModel.FirstOrDefault().EmployeeList = list;
                        jobScheduleModel.FirstOrDefault().EventDetail = eventDetail;

                    }
                    else
                    {
                        jobScheduleModel = new List<ViewModels.JobScheduleViewModel>();
                        ViewModels.JobScheduleViewModel obj = new ViewModels.JobScheduleViewModel();
                        obj.EmployeeList = list;
                        obj.EventDetail = eventDetail;
                        jobScheduleModel.Add(obj);
                    }

                    jobScheduleModel.FirstOrDefault().EmployeeList = list;
                    jobScheduleModel.FirstOrDefault().EventDetail = eventDetail;

                    return Json(new { list = json, CalenderList = jobScheduleModel.ToList(), length = EmployeejobViewModel.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }


        private static string EventColor(int JobStatus, string JobNo)
        {
            var jobColor = "";
            if (JobNo == "On Leave" || JobNo == "Not Working" || JobNo == "Public Holiday")
            {
                return "#D3D3D3";
            }
            switch (JobStatus)
            {
                case 3:
                    jobColor = "#FFFFFF";
                    break;
                case 5:
                    jobColor = "#FFFFFF";
                    break;
                case 15:
                    jobColor = "#00FF00";
                    break;
                case 9:
                    jobColor = "#66CCFF";
                    break;
                case 11:
                    jobColor = "#FF66FF";
                    break;
                case 16:
                    jobColor = "#ff0000";
                    break;
                case 13:
                    jobColor = "#E0E0E0";
                    break;
                case 17:
                    jobColor = "#ee5549";
                    break;
                case 7:
                    jobColor = "#ffff00";
                    break;
            }

            return jobColor;
        }

        private string GetJobStatusColor(int jobstatus)
        {
            string statusColor;
            switch (jobstatus)
            {
                case 15:
                    statusColor = "colored_strip colored_strip2";
                    break;
                case 9:
                    statusColor = "colored_strip colored_strip6";
                    break;
                case 3:
                    statusColor = "colored_strip colored_strip7";
                    break;
                case 11:
                    statusColor = "colored_strip colored_strip8";
                    break;
                case 16:
                    statusColor = "colored_strip colored_strip9";
                    break;
                case 13:
                    statusColor = "colored_strip colored_strip10";
                    break;
                default:
                    statusColor = "";
                    break;
            }
            return statusColor;
        }

        public ActionResult GetUnreadMessage()
        {
            try
            {
                using (EmployeeDetailRepo)
                {

                    //log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    //log.Info(base.GetUserName + " gets number of all unread messages.");

                    Guid LoggedId = Guid.Parse(base.GetUserId);
                    var MessageCount = EmployeeDetailRepo.GetTotalMessageCount(Convert.ToString(LoggedId));
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(MessageCount);
                    return Json(MessageCount, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        [HttpGet]
        public ActionResult JobSchedule()
        {
            try
            {
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " gets list of job schedule.");

                var jobscheduleviewModel = Employeejob.GetJobSchedule().ToList();

                CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> mapper = new CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel>();
                List<FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> jobScheduleModel = mapper.MapToList(jobscheduleviewModel.ToList());
                // jobScheduleModel.ForEach(i => i.EmployeeList = EmployeeDetailRepo.GetAll().Select(m => new SelectListItem { Text = m.FirstName + ' ' + m.LastName, Value = m.EmployeeId.ToString() }).ToList());



                return View();

            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        public ActionResult demo()
        {
            return View();
        }

        /// <summary>
        /// Not called anywhere
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetEmployeesList()
        {
            try
            {
                var jobscheduleviewModel = Employeejob.GetJobSchedule().ToList();
                CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> mapper = new CommonMapper<Core.ViewModels.JobScheduleViewModel, FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel>();
                List<FSM.Web.Areas.Admin.ViewModels.JobScheduleViewModel> jobScheduleModel = mapper.MapToList(jobscheduleviewModel.ToList());
                List<Employeedetail> list = new List<Employeedetail>();
                foreach (var emp in jobScheduleModel)
                {
                    list.Add(new Employeedetail { id = emp.EmployeeId.ToString(), title = emp.Employeename.ToString() });
                }
                List<EventDetail> eventDetail = new List<EventDetail>();
                foreach (var emp in jobScheduleModel)
                {
                    DateTime date = emp.DateBooked.Value.Date;
                    TimeSpan time = (TimeSpan)emp.StartTime;
                    DateTime startdate = date + time;
                    String jobstartdate = String.Format("{0:s}", startdate);
                    DateTime dateend = emp.DateBooked.Value.Date;
                    TimeSpan timeend = (TimeSpan)emp.EndTime;
                    DateTime enddate = dateend + timeend;
                    String jobEnddate = String.Format("{0:s}", enddate);
                    eventDetail.Add(new EventDetail { JobId = emp.jobNo, id = emp.EmployeeId.ToString(), resourceId = emp.EmployeeId.ToString(), start = jobstartdate, end = jobEnddate, title = "Job_Id=" + emp.jobNo.ToString() });
                }
                jobScheduleModel.FirstOrDefault().EmployeeList = list;
                jobScheduleModel.FirstOrDefault().EventDetail = eventDetail;
                return Json(jobScheduleModel.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult GetProfilePics()
        {
            try
            {
                using (EmployeeDetailRepo)
                {
                    Guid LoggedId = Guid.Parse(base.GetUserId);

                    EmployeeDetail employeedetail = EmployeeDetailRepo.FindBy(m => m.EmployeeId == LoggedId).FirstOrDefault();
                    CommonMapper<EmployeeDetail, EmployeeDetailViewModel> maper = new CommonMapper<EmployeeDetail, EmployeeDetailViewModel>();
                    EmployeeDetailViewModel employeedetailViewModel = maper.Mapper(employeedetail);
                    var profilepics = "";
                    if (employeedetailViewModel.ProfilePicture != null)
                    {
                        profilepics = employeedetailViewModel.ProfilePicture;
                    }
                    else
                    {
                        profilepics = "EmptyPics.png";
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(profilepics);
                    return Json(profilepics, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }



        [HttpGet]
        public PartialViewResult AssignJob()
        {
            AssignJobViewModel assignJobViewModel = new AssignJobViewModel();
            try
            {



                Guid JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? Guid.Parse(Request.QueryString["JobId"])
                                : Guid.Empty;
                Nullable<DateTime> AssignedDate = !string.IsNullOrEmpty(Request.QueryString["AssignedDate"]) ? DateTime.Parse(Request.QueryString["AssignedDate"])
                                : (Nullable<DateTime>)null;

                var OTRWList = JobRepository.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).OrderBy(m => m.Text).ToList();

                assignJobViewModel.JobId = JobId;
                assignJobViewModel.AssignedDate = AssignedDate.ToString();
                assignJobViewModel.OTRWList = OTRWList;

                return PartialView("AssignJob", assignJobViewModel);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }

        [HttpPost]
        public ActionResult AssignJob(AssignJobViewModel assignJobViewModel)
        {
            try
            {

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " assigned a job to OTRW");

                assignJobViewModel.AssignedDate = assignJobViewModel.AssignedDate.Replace("‎", "");// Hidden text replaced with empty.
                var Jobinfo = JobRepository.FindBy(m => m.Id == assignJobViewModel.JobId).FirstOrDefault();
                double Estimated_hrsPerUser = Convert.ToDouble(Jobinfo.EstimatedHrsPerUser);

                Guid AssignTo = !string.IsNullOrEmpty(Convert.ToString(assignJobViewModel.Assigned_To)) ? Guid.Parse(Convert.ToString(assignJobViewModel.Assigned_To)) : Guid.Empty;
                DateTime AssignedDate = DateTime.ParseExact(assignJobViewModel.AssignedDate, "M/d/yyyy", CultureInfo.CurrentCulture);
                var job = JobRepository.FindBy(m => m.Id == assignJobViewModel.JobId).FirstOrDefault();
                job.DateBooked = AssignedDate;
                job.ModifiedBy = Guid.Parse(base.GetUserId);
                job.ModifiedDate = DateTime.Now;
                job.StartTime = new DateTime(AssignedDate.Year, AssignedDate.Month, AssignedDate.Day,
                                int.Parse(assignJobViewModel.StartTime), 0, 0);

                DateTime Start_Time = Convert.ToDateTime(job.StartTime);
                job.EndTime = Start_Time.AddHours(Estimated_hrsPerUser);
                int OtrwRequiredCount = Convert.ToInt32(job.OTRWRequired);
                var CheckjobAssignUserCount = JobAssignToMappingRepo.FindBy(m => m.JobId == assignJobViewModel.JobId && m.DateBooked == AssignedDate && m.IsDelete == false).Count();//Check current job already assign
                CheckjobAssignUserCount = CheckjobAssignUserCount + 1;
                if (OtrwRequiredCount == CheckjobAssignUserCount)
                {
                    job.Status = 5;
                }
                JobRepository.Edit(job);
                JobRepository.Save();



                // saving assign to 
                var jobExistInMapping = JobAssignToMappingRepo.FindBy(m => m.JobId == assignJobViewModel.JobId && m.AssignTo == assignJobViewModel.CurrentResourceId && m.IsDelete == false).FirstOrDefault();
                if (jobExistInMapping != null)
                {
                    //JobAssignToMapping jobAssignToMapping = new JobAssignToMapping();
                    jobExistInMapping.AssignTo = AssignTo;
                    jobExistInMapping.IsDelete = false;
                    jobExistInMapping.Status = (int)Constant.JobStatus.Assigned;
                    jobExistInMapping.ModifiedDate = DateTime.Now;
                    jobExistInMapping.ModifiedBy = Guid.Parse(base.GetUserId);
                    jobExistInMapping.StartTime = job.StartTime;
                    double AssignEstimated_hrsPerUser = Convert.ToDouble(jobExistInMapping.EstimatedHrsPerUser);
                    jobExistInMapping.EndTime = Start_Time.AddHours(AssignEstimated_hrsPerUser);
                    //jobExistInMapping.EstimatedHrsPerUser = job.EstimatedHrsPerUser;
                    JobAssignToMappingRepo.Edit(jobExistInMapping);
                    JobAssignToMappingRepo.Save();
                }
                else
                {
                    JobAssignToMapping jobAssignToMapping = new JobAssignToMapping();
                    jobAssignToMapping.Id = Guid.NewGuid();
                    jobAssignToMapping.IsDelete = false;
                    jobAssignToMapping.JobId = assignJobViewModel.JobId;
                    jobAssignToMapping.AssignTo = AssignTo;
                    jobAssignToMapping.Status = (int)Constant.JobStatus.Assigned;
                    jobAssignToMapping.DateBooked = DateTime.ParseExact(assignJobViewModel.AssignedDate, "M/d/yyyy", CultureInfo.CurrentCulture);
                    jobAssignToMapping.CreatedDate = DateTime.Now;
                    jobAssignToMapping.CreatedBy = Guid.Parse(base.GetUserId);
                    jobAssignToMapping.StartTime = job.StartTime;
                    jobAssignToMapping.EndTime = job.EndTime;
                    jobAssignToMapping.EstimatedHrsPerUser = job.EstimatedHrsPerUser;
                    JobAssignToMappingRepo.Add(jobAssignToMapping);
                    JobAssignToMappingRepo.Save();
                }



                return Json(new { msg = "Job assigned successfully !" });

            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }

        public ActionResult OTRWHasJob(Guid AssignTo, string AssignedDate, int StartTime, int EndTime, Nullable<Guid> SourceId, Guid JobId = default(Guid))
        {
            try
            {
                AssignedDate = AssignedDate.Replace("‎", "");// Hidden text replaced with empty.
                DateTime AssignedDateFormatted = DateTime.ParseExact(AssignedDate, "M/d/yyyy", CultureInfo.CurrentCulture);
                DateTime AssignedEndDateFormatted = DateTime.ParseExact(AssignedDate, "M/d/yyyy", CultureInfo.CurrentCulture);
                if (StartTime == 0)
                {
                    return Json(new { JobExist = true, Reason = "Job has not dragged at proper place!" },
                         JsonRequestBehavior.AllowGet);
                }

                var employeeOnLeave = VacationRepo.CheckEmployeeLeave(AssignTo, AssignedDateFormatted);
                if (employeeOnLeave.Any())
                {
                    return Json(new { JobExist = true, Reason = "User on leave !" }, JsonRequestBehavior.AllowGet);
                }

                AssignedDateFormatted = AssignedDateFormatted.AddHours(StartTime);
                AssignedEndDateFormatted = AssignedEndDateFormatted.AddHours(EndTime);// adding time to date
                var jobQuery = JobRepository.OTRWHasJob(AssignTo, AssignedDateFormatted, AssignedEndDateFormatted, JobId);
                if (jobQuery.Any())
                {
                    return Json(new { JobExist = true, Reason = "User already has job assigned on given time!" },
                           JsonRequestBehavior.AllowGet);
                }

                var SamejobQuery = JobRepository.OTRWHasSameJob(AssignTo, AssignedDateFormatted, JobId);
                if (SamejobQuery.Any() && AssignTo != SourceId)
                {
                    return Json(new { JobExist = true, Reason = "Same job cannot be assigned to the same user multiple times!" },
                           JsonRequestBehavior.AllowGet);
                }

                bool IsWorkTypeMatch = JobRepository.IsWorkTypeMatch(AssignTo, JobId);

                if (!IsWorkTypeMatch)
                {
                    return Json(new { JobExist = true, Reason = "Please assign job to a user having same work type!" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { JobExist = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult OTRWHasAssignJob(Guid AssignTo, DateTime AssignedDate, int StartTime, int EndTime, Guid JobId = default(Guid))
        {
            try
            {
                var OtrwRequired = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                if (OtrwRequired.OTRWRequired == null)
                {
                    return Json(new { JobExist = false }, JsonRequestBehavior.AllowGet);
                }
                var CheckjobAssignSameUser = JobAssignToMappingRepo.FindBy(m => m.JobId == JobId && m.AssignTo == AssignTo && m.DateBooked == AssignedDate && m.IsDelete == false);//Check current job already assign
                if (CheckjobAssignSameUser.Any())
                {
                    return Json(new { CheckAssignJobExist = true, Reason = "User already has job assigned!" },
                           JsonRequestBehavior.AllowGet);
                }

                int OtrwRequiredCount = Convert.ToInt32(OtrwRequired.OTRWRequired);
                var CheckjobAssignUserCount = JobAssignToMappingRepo.FindBy(m => m.JobId == JobId && m.DateBooked == AssignedDate && m.IsDelete == false).Count();//Check current job already assign
                CheckjobAssignUserCount = CheckjobAssignUserCount + 1;
                if (OtrwRequiredCount != CheckjobAssignUserCount)
                {
                    return Json(new { CheckAssignJobExist = true, Reason = "Job Assigned" },
                           JsonRequestBehavior.AllowGet);
                }

                return Json(new { JobExist = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult GetNotifications()
        {
            var userid = Guid.Parse(base.GetUserId);
            var userNotifications = WebNotificationRepo.FindBy(m => m.UserId == userid).OrderByDescending(m => m.CreatedDate);
            var notificationList = userNotifications.Where(m => m.IsRead == false).ToList();

            // updating notifications after read
            foreach (var notification in notificationList)
            {
                notification.IsRead = true;
                WebNotificationRepo.Edit(notification);
                WebNotificationRepo.Save();
            }

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " gets list of all notifications. ");


            base.UserNotificationCount = "0";

            return Json(new { notificationList = userNotifications.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BindBackEvent()
        {
            var id = !string.IsNullOrEmpty(Request.QueryString["Id"]) ? Guid.Parse(Request.QueryString["Id"]) : Guid.Empty;
            var job = JobRepository.FindBy(m => m.Id == id).FirstOrDefault();
            job.AssignTo = null;
            job.Status = (int)Constant.JobStatus.NotAssigned;
            JobRepository.Edit(job);
            JobRepository.Save();
            return Json(new { msg = "Job #" + job.JobNo + " status set to NotAssigned successfully !" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeJobResource()
        {
            var jobId = !string.IsNullOrEmpty(Request.QueryString["Id"]) ? Guid.Parse(Request.QueryString["Id"]) : Guid.Empty;
            var resourceId = !string.IsNullOrEmpty(Request.QueryString["ResourceId"]) ?
                              Guid.Parse(Request.QueryString["ResourceId"]) : Guid.Empty;

            var jobAssignToMapping = JobAssignToMappingRepo.FindBy(m => m.JobId == jobId && m.IsDelete == false).FirstOrDefault();
            jobAssignToMapping.AssignTo = resourceId;
            jobAssignToMapping.IsDelete = false;
            jobAssignToMapping.ModifiedDate = DateTime.Now;
            jobAssignToMapping.ModifiedBy = Guid.Parse(base.GetUserId);
            JobAssignToMappingRepo.Edit(jobAssignToMapping);
            JobAssignToMappingRepo.Save();

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " changed job assign to.");

            return Json(new { msg = "Job resource changed successfully !" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetJobsOnFullMap(string Date = "", string userId = "")
        {
            try
            {
                using (Employeejob)
                {
                    List<DashboardjobInfoViewModel> EmployeejobViewModel = new List<DashboardjobInfoViewModel>();

                    var joblist = Employeejob.GetJobsOnFullMap(Date, userId);
                    CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel> mapper = new CommonMapper<Dashboardjobdetailcoreviewmodel, DashboardjobInfoViewModel>();
                    EmployeejobViewModel = mapper.MapToList(joblist.ToList());

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(EmployeejobViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets jobs on map.");

                    return Json(new { list = json, length = EmployeejobViewModel.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }
        // GET: Admin/Dashboard/_ShowReschduleCalender
        public ActionResult _ShowReschduleCalender(string ResourceId)
        {
            try
            {
                RescheduleViewModel model = new RescheduleViewModel();
                model.ResourceId = ResourceId;
                //Guid EmpId = Guid.Parse(ResourceId);
                //var EmployeeWorkType = EmployeeWorktypeRepo.FindBy(m => m.EmployeeId == EmpId).ToList();

                model.OTRWList = JobRepository.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).OrderBy(m => m.Text).ToList();

                return PartialView(model);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }
        // GET: Admin/Dashboard/UpdateRescheduleJob
        [HttpGet]
        public ActionResult UpdateRescheduleJob(string CurrentDate, string UpdateRescheduleDate, Guid ResourceId, string AssignTo)
        {
            try
            {
                int result = 0;
                if (ResourceId == Guid.Empty)
                {
                    DateTime BookedDate = DateTime.ParseExact(CurrentDate, "M/d/yyyy", CultureInfo.CurrentCulture);
                    var Id = JobAssignToMappingRepo.FindBy(m => m.DateBooked == BookedDate && m.IsDelete == false).Select(m => m.Id).ToList();
                    //var updateJobDate = Employeejob.UpdateRescheduleJobDate(CurrentDate, UpdateRescheduleDate).ToList();

                    foreach (var temp in Id)
                    {

                        JobAssignToMapping jobAssign = JobAssignToMappingRepo.FindBy(m => m.Id == temp && m.IsDelete == false).FirstOrDefault();
                        jobAssign.DateBooked = DateTime.ParseExact(UpdateRescheduleDate, "M/d/yyyy", CultureInfo.CurrentCulture);

                        Guid AssignUser = Guid.Empty;
                        if (jobAssign.AssignTo != Guid.Empty)
                        {
                            AssignUser = jobAssign.AssignTo ?? AssignUser;
                        }

                        //Update Startdate And Enddate With updatedDate 
                        string NewStartDate = jobAssign.DateBooked.Value.ToString("d/M/yyyy") + ' ' + jobAssign.StartTime.Value.TimeOfDay;
                        DateTime startdate = DateTime.Parse(NewStartDate);

                        string NewEndDate = jobAssign.DateBooked.Value.ToString("d/M/yyyy") + ' ' + jobAssign.EndTime.Value.TimeOfDay;
                        DateTime enddate = DateTime.Parse(NewEndDate);

                        var hasJob = JobRepository.OTRWHasJobWithName(AssignUser, (DateTime)startdate, (DateTime)enddate);
                        foreach (var user in hasJob)
                        {
                            return Json(new { error = "0", msg = "Job already assign to " + user.UserName + " on given start/end time." }, JsonRequestBehavior.AllowGet);
                        }

                        var employeeOnLeave = VacationRepo.CheckUserLeave(AssignUser, jobAssign.DateBooked);  // Check User On Leave
                        foreach (var user in employeeOnLeave)
                        {
                            return Json(new { error = "0", msg = "" + user + " on leave." }, JsonRequestBehavior.AllowGet);
                        }

                        //check Worktype
                        if (AssignUser != Guid.Empty)
                        {
                            bool IsWorkTypeMatch = JobRepository.IsWorkTypeMatch(AssignUser, jobAssign.JobId);

                            if (!IsWorkTypeMatch)
                            {
                                //result = 2;
                                //return Json(result, JsonRequestBehavior.AllowGet);
                                return Json(new { error = "0", msg = "Please assign job to a user having same work type!" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        jobAssign.IsDelete = false;
                        jobAssign.StartTime = startdate;
                        jobAssign.EndTime = enddate;
                        jobAssign.ModifiedDate = DateTime.Now;
                        jobAssign.ModifiedBy = Guid.Parse(base.GetUserId);
                        JobAssignToMappingRepo.DeAttach(jobAssign);
                        JobAssignToMappingRepo.Edit(jobAssign);
                        JobAssignToMappingRepo.Save();

                        //Send Mail to site contacts
                        var SiteContactsList = Employeejob.GetJobSiteContactsEmail(temp);
                        var Jobno = Employeejob.FindBy(m => m.Id == temp).FirstOrDefault();
                        //foreach (var val in SiteContactsList)
                        //{
                        //    StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/SendJobContactsRescheduleMail.htm"));
                        //    string readFile = reader.ReadToEnd();
                        //    string myString = readFile;
                        //    myString = myString.Replace("JobNo", (Jobno.JobNo).ToString());
                        //    myString = myString.Replace("RescheduleDate", UpdateRescheduleDate);

                        //    string body = Convert.ToString(myString);

                        //    using (MailMessage mm = new MailMessage())
                        //    {
                        //        mm.IsBodyHtml = true;
                        //        mm.Body = body;
                        //        mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                        //        var TOId = val.EmailId;
                        //        mm.To.Add(new MailAddress(TOId)); //Adding To email Id

                        //        using (SmtpClient smtp = new SmtpClient())
                        //        {
                        //            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                        //            smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                        //            smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                        //            smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                        //            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //            smtp.Send(mm);
                        //        }
                        //    }
                        //}
                    }


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated scheduled job.");

                    result = 1;
                    return Json(new { result, msg = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DateTime BookedDate = DateTime.ParseExact(CurrentDate, "M/d/yyyy", CultureInfo.CurrentCulture);
                    var jobAssignMapping = JobAssignToMappingRepo.FindBy(m => m.DateBooked == BookedDate && m.AssignTo == ResourceId && m.IsDelete == false).ToList();
                    foreach (var key in jobAssignMapping)
                    {
                        Guid AssignUser = Guid.Parse(AssignTo);
                        JobAssignToMapping jobAssign = JobAssignToMappingRepo.FindBy(m => m.JobId == key.JobId && m.IsDelete == false).FirstOrDefault();
                        jobAssign.DateBooked = DateTime.ParseExact(UpdateRescheduleDate, "M/d/yyyy", CultureInfo.CurrentCulture);

                        //Update Startdate And Enddate With updatedDate 
                        string NewStartDate = jobAssign.DateBooked.Value.ToString("d/M/yyyy") + ' ' + jobAssign.StartTime.Value.TimeOfDay;
                        DateTime startdate = DateTime.Parse(NewStartDate);

                        string NewEndDate = jobAssign.DateBooked.Value.ToString("d/M/yyyy") + ' ' + jobAssign.EndTime.Value.TimeOfDay;
                        DateTime enddate = DateTime.Parse(NewEndDate);

                        var hasJob = JobRepository.OTRWHasJobWithName(AssignUser, (DateTime)startdate, (DateTime)enddate);
                        foreach (var user in hasJob)
                        {
                            return Json(new { error = "0", msg = "Job already assign to " + user.UserName + " on given start/end time." }, JsonRequestBehavior.AllowGet);
                        }

                        var employeeOnLeave = VacationRepo.CheckUserLeave(AssignUser, jobAssign.DateBooked);  // Check User On Leave
                        foreach (var user in employeeOnLeave)
                        {
                            return Json(new { error = "0", msg = "" + user + " on leave." }, JsonRequestBehavior.AllowGet);
                        }

                        //check Worktype
                        if (AssignUser != Guid.Empty)
                        {
                            bool IsWorkTypeMatch = JobRepository.IsWorkTypeMatch(AssignUser, key.JobId);

                            if (!IsWorkTypeMatch)
                            {
                                return Json(new { error = "0", msg = "Please assign job to a user having same work type!" }, JsonRequestBehavior.AllowGet);
                            }
                        }


                        if (AssignUser != Guid.Empty)
                        {
                            jobAssign.AssignTo = AssignUser;
                        }
                        jobAssign.StartTime = startdate;
                        jobAssign.EndTime = enddate;
                        jobAssign.ModifiedDate = DateTime.Now;
                        jobAssign.ModifiedBy = Guid.Parse(base.GetUserId);
                        jobAssign.IsDelete = false;

                        JobAssignToMappingRepo.DeAttach(jobAssign);
                        JobAssignToMappingRepo.Edit(jobAssign);
                        JobAssignToMappingRepo.Save();

                        //Send Mail to site contacts
                        var SiteContactsList = Employeejob.GetJobSiteContactsEmail(key.JobId);
                        var Jobno = Employeejob.FindBy(m => m.Id == key.JobId).FirstOrDefault();
                        //foreach (var val in SiteContactsList)
                        //{
                        //    StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/SendJobContactsRescheduleMail.htm"));
                        //    string readFile = reader.ReadToEnd();
                        //    string myString = readFile;
                        //    myString = myString.Replace("JobNo", (Jobno.JobNo).ToString());
                        //    myString = myString.Replace("RescheduleDate", UpdateRescheduleDate);

                        //    string body = Convert.ToString(myString);

                        //    using (MailMessage mm = new MailMessage())
                        //    {
                        //        mm.IsBodyHtml = true;
                        //        mm.Body = body;
                        //        mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                        //        var TOId = val.EmailId;
                        //        mm.To.Add(new MailAddress(TOId)); //Adding To email Id

                        //        using (SmtpClient smtp = new SmtpClient())
                        //        {
                        //            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                        //            smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                        //            smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                        //            smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                        //            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //            smtp.Send(mm);
                        //        }
                        //    }
                        //}

                    }
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated scheduled job.");
                    result = 1;
                    return Json(new { result, msg = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }

        // GET: Admin/Dashboard/_ShowSuperVisorPopUp
        public ActionResult _ShowSuperVisorPopUp(Guid JobId)
        {
            try
            {
                //RescheduleViewModel model = new RescheduleViewModel();
                //model.ResourceId = JobId.ToString();

                var Jobs = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                var result = "";
                if (Jobs.Supervisor != null || Jobs.OTRWRequired == 1)
                {
                    result = "1";
                    return Json(new { data = result }, JsonRequestBehavior.AllowGet);
                }
                return PartialView();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Admin/Dashboard/CreateSuperVisor
        /// <summary>
        /// Create Super Visor
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public ActionResult CreateSuperVisor(Guid JobId, Guid UserId)
        {
            try
            {
                using (JobRepository)
                {
                    int result = 0;
                    Jobs job = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                    if (job != null)
                    {
                        job.ModifiedDate = DateTime.Now;
                        job.ModifiedBy = Guid.Parse(base.GetUserId);
                        job.Supervisor = UserId;
                        JobRepository.Edit(job);
                        JobRepository.Save();

                        result = 1;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " created new super visor.");

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = 0;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        [HttpPost]
        public ActionResult SaveEndTime(Guid JobId, string EndTime, string StartTime, Guid ResourceId)
        {
            try
            {
                using (Employeejob)
                {
                    //string End_Time = EndTime.ToString("HH:mm:ss tt");
                    //string Start_Time = StartTime.ToString("HH:mm:ss tt");
                    List<Core.ViewModels.JobScheduleViewModel> JobScheduleViewModel = new List<Core.ViewModels.JobScheduleViewModel>();
                    JobScheduleViewModel = Employeejob.SaveEndTime(JobId, EndTime, StartTime, ResourceId).ToList();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " saved end time of job.");

                    return View(JobScheduleViewModel);

                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }

        // GET: Admin/Dashboard/_ExtendJobPopUp
        public ActionResult _ExtendJobPopUp(Guid JobId)
        {
            try
            {
                JobsViewModel empModel = new JobsViewModel();
                empModel.Id = JobId;
                var job = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                //var workType = JobRepository.FindBy(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();
                var workType = job.WorkType;
                //empModel.EstimatedHours = job.EstimatedHours;

                empModel.DateBooked = DateTime.Now;
                empModel.OTRWList = EmployeeWorktypeRepo.GetOTRWUserUsingByWorkType(workType).Select(m => new SelectListItem()
                {
                    Text = m.OTRWUserName,
                    Value = m.OTRWID.ToString()
                }).OrderBy(m => m.Text).ToList();


                return PartialView(empModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get:Job Extend

        [HttpGet]
        public ActionResult UpdateExtendJob(Guid JobId, DateTime UpdateDateBooked, double EstimatedHours)
        {
            try
            {
                string UserIds = Request.QueryString["AssignTo2"];

                if (UserIds == "")
                {
                    UserIds = "00000000-0000-0000-0000-000000000000";
                }
                string[] Employeeid = UserIds.Split(',');
                int OTRWRequired = Employeeid.Count();

                EmployeeJobsViewModel empModel = new EmployeeJobsViewModel();
                JobAssignToMappingViewModel jobAssignViewModel = new JobAssignToMappingViewModel();
                Jobs jobEntity = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                var jobAssignMappingEntity = JobAssignToMappingRepo.FindBy(m => m.JobId == JobId && m.IsDelete == false).ToList();

                //jobEntity.EstimatedHrsPerUser = jobEntity.EstimatedHours / Convert.ToDouble(OTRWRequired + jobEntity.OTRWRequired);
                jobEntity.EstimatedHrsPerUser = EstimatedHours / Convert.ToDouble(OTRWRequired);

                //if (jobEntity.OTRWRequired + OTRWRequired > 10)
                //{
                //    var MaxSelectOtrwRequired = 10 - jobEntity.OTRWRequired;
                //    return Json(new { Status = "False", Required = "Maximum Select " + MaxSelectOtrwRequired + " OTRW Required !" }, JsonRequestBehavior.AllowGet);
                //}

                //Update Startdate And Enddate With updatedDate 
                string NewStartDate = UpdateDateBooked.ToString("d/M/yyyy") + ' ' + jobEntity.StartTime.Value.TimeOfDay;
                DateTime startdate = DateTime.Parse(NewStartDate);

                DateTime Endtime = startdate.AddHours(Convert.ToDouble(jobEntity.EstimatedHrsPerUser));
                //string NewEndDate = UpdateDateBooked.ToString("d/M/yyyy") + ' ' + Endtime.Value.TimeOfDay;
                DateTime enddate = Endtime;

                //check job alredy assign or user work type
                foreach (var key in Employeeid)
                {
                    Guid AssignUser = Guid.Parse(key);
                    var hasJob = JobRepository.OTRWHasJobWithName(AssignUser, (DateTime)startdate, (DateTime)enddate);
                    foreach (var user in hasJob)
                    {
                        var Error = "Job alread assign " + user.UserName + " on given start/end time.";
                        return Json(new { Status = "False", Required = Error }, JsonRequestBehavior.AllowGet);
                    }

                    if (AssignUser != Guid.Empty)
                    {
                        bool IsWorkTypeMatch = JobRepository.IsWorkTypeMatch(AssignUser, JobId);

                        if (!IsWorkTypeMatch)
                        {
                            var Error = "Please assign job to a user having same work type!";
                            return Json(new { Status = "False", Required = Error }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                foreach (var key in Employeeid)
                {
                    Guid AssignUser = Guid.Parse(key);
                    if (AssignUser != Guid.Empty)
                    {
                        jobAssignViewModel.Id = Guid.NewGuid();
                        jobAssignViewModel.IsDelete = false;
                        jobAssignViewModel.JobId = JobId;
                        jobAssignViewModel.AssignTo = AssignUser;
                        jobAssignViewModel.Status = Constant.JobStatus.Assigned;
                        jobAssignViewModel.DateBooked = UpdateDateBooked;
                        jobAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        jobAssignViewModel.CreatedDate = DateTime.Now;
                        jobAssignViewModel.StartTime = startdate;
                        jobAssignViewModel.EndTime = enddate;
                        jobAssignViewModel.EstimatedHrsPerUser = EstimatedHours / Convert.ToDouble(OTRWRequired);
                        CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper = new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                        JobAssignToMapping jobAssignToMapping = Assignmapper.Mapper(jobAssignViewModel);

                        JobAssignToMappingRepo.Add(jobAssignToMapping);
                        JobAssignToMappingRepo.Save();
                    }
                }

                jobEntity.DateBooked = UpdateDateBooked;

                jobEntity.EstimatedHours = jobEntity.EstimatedHours + EstimatedHours;
                jobEntity.EstimatedHrsPerUser = jobEntity.EstimatedHours / Convert.ToDouble(OTRWRequired + jobEntity.OTRWRequired);
                jobEntity.OTRWRequired = OTRWRequired + jobEntity.OTRWRequired;
                jobEntity.Status = 3;
                jobEntity.Supervisor = null;

                JobRepository.DeAttach(jobEntity);
                JobRepository.Edit(jobEntity);
                JobRepository.Save();

                //Update Site Make Multiple People Need
                CustomerResidenceDetail residenceDetailEntity = CustomerResidenceRepo.FindBy(m => m.SiteDetailId == jobEntity.SiteId).FirstOrDefault();
                if (residenceDetailEntity != null)
                {
                    residenceDetailEntity.NeedTwoPPL = true;
                    CustomerResidenceRepo.DeAttach(residenceDetailEntity);
                    CustomerResidenceRepo.Edit(residenceDetailEntity);
                    CustomerResidenceRepo.Save();
                }
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " changed jod's booked date.");
                return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        // GET: Admin/Dashboard/_SetOrderPopUp
        public ActionResult _SetOrderPopUp()
        {
            try
            {
                Guid Role = Guid.Parse("31cf918d-b8fe-4490-b2d7-27324bfe89b4");
                var employeerList = EmployeeDetailRepo.FindBy(m => m.IsDelete == false && m.IsActive == true && m.Role == Role).OrderBy(m => m.OTRW_Order).AsEnumerable();

                var empDetailListViewModel = employeerList.Select(m => new EmpDetailListViewModel
                {
                    EmployeeId = m.EmployeeId,
                    UserName = m.UserName,
                    OTRWOrder = m.OTRW_Order,
                }).ToList();

                var employeeDetailListViewModel = new EmployeeDetailListViewModel
                {
                    EmployeeDetailList = empDetailListViewModel,
                };
                return PartialView(employeeDetailListViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get:OTRW Order Set
        [HttpGet]
        public ActionResult UpdateSetOrder(Guid OTRWID, string OrderType, int CurrentOrderNo)
        {
            try
            {
                if (OrderType == "Up")
                {
                    //update Current orderNo
                    EmployeeDetail currentEmp = EmployeeDetailRepo.FindBy(m => m.EmployeeId == OTRWID).FirstOrDefault();
                    currentEmp.OTRW_Order = currentEmp.OTRW_Order - 1;
                    currentEmp.ModifiedDate = DateTime.Now;
                    currentEmp.ModifiedBy = Guid.Parse(base.GetUserId);

                    //update Order no Replace With Current Order No
                    int? RepalceOrderNo = currentEmp.OTRW_Order;
                    EmployeeDetail replaceEmp = EmployeeDetailRepo.FindBy(m => m.OTRW_Order == RepalceOrderNo && m.IsDelete == false && m.IsActive == true).FirstOrDefault();
                    replaceEmp.OTRW_Order = currentEmp.OTRW_Order + 1;
                    replaceEmp.ModifiedDate = DateTime.Now;
                    replaceEmp.ModifiedBy = Guid.Parse(base.GetUserId);

                    //update Current No
                    EmployeeDetailRepo.DeAttach(currentEmp);
                    EmployeeDetailRepo.Edit(currentEmp);
                    EmployeeDetailRepo.Save();

                    //update Replace No
                    EmployeeDetailRepo.DeAttach(replaceEmp);
                    EmployeeDetailRepo.Edit(replaceEmp);
                    EmployeeDetailRepo.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " Changes order OTRW.");

                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
                else if (OrderType == "Down")
                {

                    //update Current orderNo
                    EmployeeDetail currentEmp = EmployeeDetailRepo.FindBy(m => m.EmployeeId == OTRWID).FirstOrDefault();
                    currentEmp.OTRW_Order = currentEmp.OTRW_Order + 1;
                    currentEmp.ModifiedDate = DateTime.Now;
                    currentEmp.ModifiedBy = Guid.Parse(base.GetUserId);

                    //update Order no Replace With Current Order No
                    int? RepalceOrderNo = currentEmp.OTRW_Order;
                    EmployeeDetail replaceEmp = EmployeeDetailRepo.FindBy(m => m.OTRW_Order == RepalceOrderNo && m.IsDelete == false && m.IsActive == true).FirstOrDefault();
                    replaceEmp.OTRW_Order = currentEmp.OTRW_Order - 1;
                    replaceEmp.ModifiedDate = DateTime.Now;
                    replaceEmp.ModifiedBy = Guid.Parse(base.GetUserId);

                    //update Current No
                    EmployeeDetailRepo.DeAttach(currentEmp);
                    EmployeeDetailRepo.Edit(currentEmp);
                    EmployeeDetailRepo.Save();

                    //update Replace No
                    EmployeeDetailRepo.DeAttach(replaceEmp);
                    EmployeeDetailRepo.Edit(replaceEmp);
                    EmployeeDetailRepo.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " Changes order OTRW.");

                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }

        // GET: Admin/Dashboard/_ReAssignedJobPopUp
        public ActionResult _ReAssignedJobPopUp(Guid JobId, Guid OTRWID)
        {
            try
            {
                JobsViewModel empModel = new JobsViewModel();
                empModel.Id = JobId;
                empModel.AssignTo = OTRWID;
                var Worktype = JobRepository.FindBy(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();

                empModel.OTRWList = EmployeeWorktypeRepo.GetOTRWUserUsingByWorkType(Worktype).Select(m => new SelectListItem()
                {
                    Text = m.OTRWUserName,
                    Value = m.OTRWID.ToString()
                }).OrderBy(m => m.Text).ToList();

                return PartialView(empModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get:Job Re-Assigned
        [HttpGet]
        public ActionResult UpdateAssignJob(Guid JobId, DateTime UpdateDateBooked, DateTime CurrentDate, Guid NewAssignTo, Guid OldAssignTo)
        {
            try
            {
                Jobs jobEntity = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                JobAssignToMapping jobAssignMappingEntity = JobAssignToMappingRepo.FindBy(m => m.JobId == JobId && m.DateBooked == CurrentDate && m.AssignTo == OldAssignTo && m.IsDelete == false).FirstOrDefault();

                if (jobAssignMappingEntity != null)
                {
                    //Update In JobAssignTomapping

                    jobAssignMappingEntity.DateBooked = UpdateDateBooked;

                    string NewStartDate = jobAssignMappingEntity.DateBooked.Value.ToString("d/M/yyyy") + ' ' + jobAssignMappingEntity.StartTime.Value.TimeOfDay;
                    DateTime startdate = DateTime.Parse(NewStartDate);

                    string NewEndDate = jobAssignMappingEntity.DateBooked.Value.ToString("d/M/yyyy") + ' ' + jobAssignMappingEntity.EndTime.Value.TimeOfDay;
                    DateTime enddate = DateTime.Parse(NewEndDate);

                    jobAssignMappingEntity.StartTime = startdate;
                    jobAssignMappingEntity.EndTime = enddate;
                    jobAssignMappingEntity.AssignTo = NewAssignTo;
                    jobAssignMappingEntity.IsDelete = false;
                    jobAssignMappingEntity.ModifiedDate = DateTime.Now;
                    jobAssignMappingEntity.ModifiedBy = Guid.Parse(base.GetUserId);

                    JobAssignToMappingRepo.DeAttach(jobAssignMappingEntity);
                    JobAssignToMappingRepo.Edit(jobAssignMappingEntity);
                    JobAssignToMappingRepo.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " made changes in assigned job.");

                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }

        public ActionResult UnAssignedJob(Guid JobId, Guid OTRWID, DateTime UnAssignedDate)
        {
            try
            {
                var jobUnassign = JobAssignToMappingRepo.FindBy(m => m.JobId == JobId && m.AssignTo == OTRWID && m.IsDelete == false && m.DateBooked == UnAssignedDate).FirstOrDefault();
                var Job = Employeejob.FindBy(m => m.Id == JobId).FirstOrDefault();
                Job.Status = 3;
                Job.DateBooked = UnAssignedDate;
                Employeejob.Edit(Job);
                Employeejob.Save();

                jobUnassign.IsDelete = true;
                jobUnassign.ModifiedDate = DateTime.Now;
                jobUnassign.ModifiedBy = Guid.Parse(base.GetUserId);

                JobAssignToMappingRepo.DeAttach(jobUnassign);
                JobAssignToMappingRepo.Edit(jobUnassign);
                JobAssignToMappingRepo.Save();

                // var UserList = AspNetUsers.
                string OTRWId = Convert.ToString(OTRWID);
                var UserName = AspNetUsersRepo.FindBy(m => m.Id == OTRWId).Select(m => m.UserName).FirstOrDefault();
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " unassigned job from " + UserName);

                return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }


        // GET: Admin/Dashboard/_ExtendJobPopUp
        public ActionResult _GetTrackUserPopup()
        {
            try
            {
                JobsViewModel empModel = new JobsViewModel();
                empModel.OTRWList = JobRepository.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();
                return PartialView(empModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult trackMapLatitudeLongitude()
        {
            string UserIds = Request.QueryString["UserId"];

            var result = EmployeeDetailRepo.GetLatitudeLongitude(UserIds).ToList();
            string ids = "E11B6DB3-D67D-4121-BBC1-6A23A56BC271";
            Guid id = Guid.Parse(ids);
            //var result = EmployeeDetailRepo.FindBy(x => x.EmployeeId == id).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public PartialViewResult _DashboardReminderCreate(DateTime ReminderDate)
        {
            try
            {

                Customer.ViewModels.CustomerReminderViewModel model = new Customer.ViewModels.CustomerReminderViewModel();
                List<SelectListItem> JobList = new List<SelectListItem>();
                List<SelectListItem> JobListAll = new List<SelectListItem>();

                JobList = JobAssignToMappingRepo.GetJobDataByDate(ReminderDate).Select(m =>
                  new SelectListItem
                  {
                      Text = m.SiteFileName + "(JobNo :" + m.JobNo.ToString() + ")",
                      Value = m.Id.ToString()

                  }).ToList();
                IEnumerable<SelectListItem> JobList1 = JobListAll.Union(JobList);
                model.FinalJobList = JobList1;
                model.ReminderDate = ReminderDate;

                model.ContactList = new List<SelectListItem>();
                model.ContactListIds = new List<SelectListItem>();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " created reminder on dashboard.");

                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetMessageTemplate(string templateName)
        {
            if (!string.IsNullOrEmpty(templateName))
            {
                string filePath = "~/EmailTemplate/" + templateName + ".htm";
                StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(filePath));
                string readFile = reader.ReadToEnd();
                return Json(new { msgTemplate = readFile }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { msgTemplate = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult _GetJobContacts(string[] jobIds)
        {
            FSM.Web.Areas.Customer.ViewModels.CustomerReminderViewModel customerReminderViewModel =
                new FSM.Web.Areas.Customer.ViewModels.CustomerReminderViewModel();
            if (jobIds != null)
            {
                string strJobIds = string.Join("','", jobIds); // separating each element by "','"
                strJobIds = strJobIds.Insert(0, "'"); // putting "'" at 0 index
                strJobIds = strJobIds.Insert(strJobIds.Count(), "'"); // putting "'" at last index

                var jobContacts = CustomerContactRepo.GetJobContactList(strJobIds).Select(m => new
                {
                    m.ContactId,
                    m.EmailId,
                    m.FirstName,
                    m.LastName,
                    m.Phone,
                    m.SiteFileName
                }).Distinct();
                customerReminderViewModel.ContactList = jobContacts.Select(m => new SelectListItem
                {
                    Text = m.FirstName + " " + m.LastName + " (" + m.SiteFileName + ")",
                    Value = m.ContactId.ToString()
                });
            }
            else
            {
                customerReminderViewModel.ContactList = new List<SelectListItem>();
            }

            return PartialView(customerReminderViewModel);
        }

        [HttpPost]
        public ActionResult GetJobByCustomerId(string CustomerGeneralInfoId)
        {
            try
            {
                Guid Custid = Guid.Parse(CustomerGeneralInfoId);
                Guid JobID = Guid.Empty;
                var jobList = CustJobsRepo.FindBy(m => m.CustomerGeneralInfoId == Custid).Select(m =>
                       new SelectListItem { Text = m.JobNo.ToString(), Value = m.Id.ToString() }).ToList();
                var sitedetail = CustomerSiteDetailRepo.FindBy(m => m.CustomerGeneralInfoId == Custid).Select(m =>
                      new SelectListItem
                      {
                          Text = m.StreetName.ToString(),
                          Value = m.SiteDetailId.ToString()
                      }).ToList();

                var UserName = AspNetUsersRepo.FindBy(m => Guid.Parse(m.Id) == Custid).Select(m => m.UserName).FirstOrDefault();
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " gets jobs of " + UserName);

                return Json(new { JobList = jobList, SiteNameList = sitedetail, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GetSiteByJobId(string JobId)
        {
            try
            {
                Guid JobID = Guid.Parse(JobId);
                var siteDetailId = JobRepository.FindBy(m => m.Id == JobID).Select(m => m.SiteId).FirstOrDefault();

                var sitedetaillist = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteDetailId).Select(m =>
                new SelectListItem
                {
                    Text = m.StreetName.ToString(),
                    Value = m.SiteDetailId.ToString()
                }).ToList();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " gets site by job");

                return Json(new { SiteNameList = sitedetaillist, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveSendReminder(DateTime ReminderDate, string[] JobId, string[] ContactListIds, Nullable<FSMConstant.Constant.DashboardTemplateMessage> TemplateMessageId, string Note, bool HasSMS, bool HasEmail, string fromEmail, string fromEmailVal)
        {
            {
                Customer.ViewModels.CustomerReminderViewModel model = new Customer.ViewModels.CustomerReminderViewModel();
                CommonMapper<Customer.ViewModels.CustomerReminderViewModel, CustomerReminder> mapper = new CommonMapper<Customer.ViewModels.CustomerReminderViewModel, CustomerReminder>();
                model.ReminderId = Guid.NewGuid();
                model.ReminderDate = DateTime.Now;
                model.ReContactDate = DateTime.Now;
                //model.MessageTypeId = MessageTypeId;
                model.TemplateMessageId = TemplateMessageId;
                model.Note = Note;
                model.HasSMS = HasSMS;
                model.HasEmail = HasEmail;
                int fromemailValue = Convert.ToInt32(fromEmailVal);
                model.FromEmail = (Constant.FromEmail)fromemailValue;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = Guid.Parse(base.GetUserId);
                model.IsDelete = false;
                CustomerReminder dashreminder = mapper.Mapper(model);
                CustomerReminderRepo.Add(dashreminder);
                CustomerReminderRepo.Save();
                foreach (var job in JobId)
                {
                    Guid Job_id = Guid.Parse(job);
                    var CustomerID = Employeejob.FindBy(m => m.Id == Job_id).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                    var Customer = CustomerGeneralRepo.FindBy(m => m.CustomerGeneralInfoId == CustomerID).FirstOrDefault();
                    var InvoiceNo = InvoiceRepo.FindBy(m => m.EmployeeJobId == Job_id).Select(m => m.InvoiceNo).FirstOrDefault();
                    var customerInfo = JobRepository.FindBy(m => m.Id == Job_id).Select(m => new
                    {
                        m.CustomerGeneralInfo.CustomerGeneralInfoId,
                        m.CustomerGeneralInfo.CTId
                    }).FirstOrDefault();
                    var JobDetail = CustJobsRepo.FindBy(m => m.Id == Job_id).FirstOrDefault();
                    string SiteAddress = CustomerSiteDetailRepo.GetSiteAddress(JobDetail.SiteId);
                    #region save contactlog for each job
                    // save contactlog for each job
                    CustomerContactLog customerContactLog = new CustomerContactLog();
                    customerContactLog.CustomerContactId = Guid.NewGuid();
                    customerContactLog.CustomerGeneralInfoId = customerInfo.CustomerGeneralInfoId;
                    customerContactLog.CustomerId = customerInfo.CTId.ToString();
                    customerContactLog.JobId = job;

                    customerContactLog.LogDate = DateTime.Now;
                    customerContactLog.Note = TemplateMessageId.GetAttribute<DisplayAttribute>().Name;
                    customerContactLog.IsDelete = false;
                    customerContactLog.IsReminder = true;
                    customerContactLog.IsScheduler = true;
                    customerContactLog.CreatedDate = DateTime.Now;
                    customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);
                    CustomercontactLogRepo.Add(customerContactLog);
                    CustomercontactLogRepo.Save();
                    #endregion
                    Customer.ViewModels.CustomerReminderJobMapping mapingmodel = new Customer.ViewModels.CustomerReminderJobMapping();
                    CommonMapper<Customer.ViewModels.CustomerReminderJobMapping, FSM.Core.Entities.CustomerJobReminderMapping> mappermapping = new CommonMapper<Customer.ViewModels.CustomerReminderJobMapping, FSM.Core.Entities.CustomerJobReminderMapping>();
                    mapingmodel.ID = Guid.NewGuid();
                    mapingmodel.Jobid = Job_id;
                    mapingmodel.ReminderId = model.ReminderId;
                    mapingmodel.CustomerGeneralInfoId = JobDetail.CustomerGeneralInfoId;
                    mapingmodel.CreatedBy = Guid.Parse(base.GetUserId);
                    mapingmodel.CreatedDate = DateTime.Now;
                    FSM.Core.Entities.CustomerJobReminderMapping remindermapping = mappermapping.Mapper(mapingmodel);
                    CustomerJobReminderMapping.DeAttach(remindermapping);
                    CustomerJobReminderMapping.Add(remindermapping);
                    CustomerJobReminderMapping.Save();

                    // find job contacts
                    var jobContacts = CustomerContactRepo.GetJobContactList("'" + JobDetail.Id + "'");
                    foreach (var contact in jobContacts)
                    {
                        if (Array.IndexOf(ContactListIds, contact.ContactId.ToString()) >= 0)
                        {
                            // saving in ContactLogSiteContactsMapping for each contact
                            ContactLogSiteContactsMapping contactLogSiteContactsMapping = new ContactLogSiteContactsMapping();
                            contactLogSiteContactsMapping.Id = Guid.NewGuid();
                            contactLogSiteContactsMapping.ContactLogId = customerContactLog.CustomerContactId;
                            contactLogSiteContactsMapping.JobId = Job_id;
                            contactLogSiteContactsMapping.ContactId = contact.ContactId;
                            contactLogSiteContactsMapping.FirstName = contact.FirstName;
                            contactLogSiteContactsMapping.LastName = contact.LastName;
                            contactLogSiteContactsMapping.CreatedDate = DateTime.Now;
                            contactLogSiteContactsMapping.CreatedBy = Guid.Parse(base.GetUserId);
                            ContactLogSiteContactsMappingRepo.Add(contactLogSiteContactsMapping);
                            ContactLogSiteContactsMappingRepo.Save();

                            // saving in ScheduleReminder for each contact
                            ScheduleReminder scheduleReminder = new ScheduleReminder();
                            scheduleReminder.Id = Guid.NewGuid();
                            scheduleReminder.CustomerContactLogId = customerContactLog.CustomerContactId;
                            scheduleReminder.Schedule = false;
                            scheduleReminder.FromEmail = Convert.ToInt32(fromEmailVal);
                            scheduleReminder.ScheduleDate = ReminderDate;
                            scheduleReminder.CreatedDate = DateTime.Now;
                            scheduleReminder.CreatedBy = Guid.Parse(base.GetUserId);

                            string msgBody = model.Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);
                            if (contact.DateBooked.HasValue)
                            {
                                if (ReminderDate >= contact.DateBooked && ReminderDate >= DateTime.Now.Date)
                                {
                                    // msgBody = msgBody.Replace("{{DateBooked}}", contact.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                    msgBody = msgBody.Replace("{{DateBooked}}", ReminderDate.ToString("dddd, dd MMMM yyyy"));

                                    int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                    string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= ReminderDate).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                    if (!string.IsNullOrEmpty(startDate))
                                    {
                                        DateTime getDateTime = Convert.ToDateTime(startDate);
                                        string getDate = getDateTime.ToString("dd/MM/yyyy");
                                        string day = ReminderDate.ToString("dddd");
                                        DateTime endDate = DateTime.Now;
                                        if (getDateTime != null)
                                        {
                                            endDate = getDateTime.AddMinutes(+30);
                                        }
                                        TimeSpan startTime = new TimeSpan(getDateTime.Hour, getDateTime.Minute, getDateTime.Second);
                                        TimeSpan endTime = new TimeSpan(endDate.Hour, endDate.Minute, endDate.Second);
                                        msgBody = msgBody.Replace("{{DateSend}}", day + " " + getDate);
                                        msgBody = msgBody.Replace("{{StartEndTime}}", startTime + " - " + endTime);
                                    }
                                    //else
                                    //{
                                    //    msgBody = msgBody.Replace("{{DateSend}}", "today");
                                    //    msgBody = msgBody.Replace("{{StartEndTime}}", "");
                                    //}

                                    if (HasSMS == true || (HasSMS == false && HasEmail == false))
                                    {
                                        if (Convert.ToString(TemplateMessageId) == "ReminderForRain")
                                        {
                                            var JobRepoDetail = JobRepository.FindBy(m => m.Id == Job_id).FirstOrDefault();
                                            JobRepoDetail.Status = Convert.ToInt16(Constant.JobStatus.Rain);
                                            JobRepoDetail.ModifiedDate = DateTime.Now;
                                            JobRepoDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                                            JobRepository.Edit(JobRepoDetail);
                                            JobRepository.Save();

                                            var JobAssigntoMappingDetail = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false).ToList());
                                            foreach (JobAssignToMapping JSM in JobAssigntoMappingDetail)
                                            {
                                                JSM.Status = Convert.ToInt16(Constant.JobStatus.Rain);
                                                JSM.ModifiedDate = DateTime.Now;
                                                JSM.ModifiedBy = Guid.Parse(base.GetUserId);
                                                JobAssignToMappingRepo.Edit(JSM);
                                                JobAssignToMappingRepo.Save();

                                            }
                                        }
                                        else if (Convert.ToString(TemplateMessageId) == "ConfirmationAppointmentStrataRealestate" || Convert.ToString(TemplateMessageId) == "ConfirmationAppointmentDomesticCustomer")
                                        {
                                            var JobRepoDetail = JobRepository.FindBy(m => m.Id == Job_id).FirstOrDefault();
                                            JobRepoDetail.Status = Convert.ToInt16(Constant.JobStatus.Confirmed);
                                            JobRepoDetail.ModifiedDate = DateTime.Now;
                                            JobRepoDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                                            JobRepository.Edit(JobRepoDetail);
                                            JobRepository.Save();

                                            var JobAssigntoMappingDetail = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false).ToList());
                                            foreach (JobAssignToMapping JSM in JobAssigntoMappingDetail)
                                            {
                                                JSM.Status = Convert.ToInt16(Constant.JobStatus.Confirmed);
                                                JSM.ModifiedDate = DateTime.Now;
                                                JSM.ModifiedBy = Guid.Parse(base.GetUserId);
                                                JobAssignToMappingRepo.Edit(JSM);
                                                JobAssignToMappingRepo.Save();

                                            }
                                        }

                                        // If Job assign to Luiz app no need to send msg as per tejpal sir (23-02-2018)  fixed by kamal, verified by  tsingh
                                        var jobAssignId = JobRepository.FindBy(m => m.Id == Job_id).First();
                                        string UserName = "";
                                        if (jobAssignId.AssignTo != null) {
                                            UserName = EmployeeRepo.FindBy(m => m.EmployeeId == jobAssignId.AssignTo).Select(m => m.UserName).FirstOrDefault();
                                        }
                                         
                                        // replacing img tag with alt text
                                        var pattern = @"<img.*?alt='(.*?)'[^\>]*>";
                                        var replacePattern = @"$1";
                                        var phoneTemplate = Regex.Replace(msgBody, pattern, replacePattern, RegexOptions.IgnoreCase);
                                        phoneTemplate = Regex.Replace(phoneTemplate, "[\\r\\n]+", "\n", System.Text.RegularExpressions.RegexOptions.Multiline);
                                        phoneTemplate = phoneTemplate.Trim();
                                        if (!string.IsNullOrEmpty(contact.Phone))
                                        {
                                            scheduleReminder.Phone = contact.Phone;
                                            scheduleReminder.PhoneTemplate = phoneTemplate;
                                        }
                                        if (UserName != "LuizApp")
                                        {
                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                int errorCounter = Regex.Matches(contact.Phone, @"[a-zA-Z]").Count;
                                                if (errorCounter == 0)
                                                {
                                                    contact.Phone = "+61" + contact.Phone;
                                                    string[] to = { contact.Phone };
                                                    string sendFrom = "+61414363865";
                                                      TransmitSmsWrapper manager = new TransmitSmsWrapper("23dac442668a809eeaa7d9aaad5f91c7", "clientapisecret", "https://api.transmitsms.com");
                                                      var response = manager.SendSms("" + msgBody + "", to, sendFrom, null, null, "", "", 0);
                                                }
                                            }
                                        }
                                    }
                                    if (HasEmail == true)
                                    {

                                      


                                        if (Convert.ToString(TemplateMessageId) == "ReminderForRain")
                                        {
                                            var JobRepoDetail = JobRepository.FindBy(m => m.Id == Job_id).FirstOrDefault();
                                            JobRepoDetail.Status = Convert.ToInt16(Constant.JobStatus.Rain);
                                            JobRepoDetail.ModifiedDate = DateTime.Now;
                                            JobRepoDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                                            JobRepository.Edit(JobRepoDetail);
                                            JobRepository.Save();

                                            var JobAssigntoMappingDetail = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false).ToList());
                                            foreach (JobAssignToMapping JSM in JobAssigntoMappingDetail)
                                            {
                                                JSM.Status = Convert.ToInt16(Constant.JobStatus.Rain);
                                                JSM.ModifiedDate = DateTime.Now;
                                                JSM.ModifiedBy = Guid.Parse(base.GetUserId);
                                                JobAssignToMappingRepo.Edit(JSM);
                                                JobAssignToMappingRepo.Save();

                                            }
                                        }
                                        else if (Convert.ToString(TemplateMessageId) == "ConfirmationAppointmentStrataRealestate" || Convert.ToString(TemplateMessageId) == "ConfirmationAppointmentDomesticCustomer" || Convert.ToString(TemplateMessageId) == "CustomerReminder")
                                        {
                                            var JobRepoDetail = JobRepository.FindBy(m => m.Id == Job_id).FirstOrDefault();
                                            JobRepoDetail.Status = Convert.ToInt16(Constant.JobStatus.Confirmed);
                                            JobRepoDetail.ModifiedDate = DateTime.Now;
                                            JobRepoDetail.ModifiedBy = Guid.Parse(base.GetUserId);
                                            JobRepository.Edit(JobRepoDetail);
                                            JobRepository.Save();

                                            var JobAssigntoMappingDetail = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false).ToList());
                                            foreach (JobAssignToMapping JSM in JobAssigntoMappingDetail)
                                            {
                                                JSM.Status = Convert.ToInt16(Constant.JobStatus.Confirmed);
                                                JSM.ModifiedDate = DateTime.Now;
                                                JSM.ModifiedBy = Guid.Parse(base.GetUserId);
                                                JobAssignToMappingRepo.Edit(JSM);
                                                JobAssignToMappingRepo.Save();

                                            }
                                        }
                                        msgBody = msgBody.Replace("\n", "<p>");

                                        if (!string.IsNullOrEmpty(contact.EmailId))
                                        {
                                            scheduleReminder.Email = contact.EmailId;
                                            scheduleReminder.EmailTemplate = msgBody;
                                        }

                                        if (!string.IsNullOrEmpty(contact.EmailId))
                                        {
                                            // sending mail
                                            MailMessage mmm = new MailMessage();
                                            mmm.Subject = "Reminding You";
                                            mmm.IsBodyHtml = true;
                                            mmm.To.Add(contact.EmailId);
                                            mmm.From = new MailAddress(fromEmail, "Sydney Roof and Gutter");
                                            //mmm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                                            string Body = Convert.ToString(msgBody);
                                            mmm.Body = Body;
                                            SmtpClient Smtp = new SmtpClient();
                                            Smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                            Smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                            Smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                            Smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                            await Smtp.SendMailAsync(mmm);
                                        }
                                    }
                                }
                            }
                            ScheduleReminderRepo.Add(scheduleReminder);
                            ScheduleReminderRepo.Save();
                        }
                    }
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }
        //Get:Get Current Job Status Right Click Of Menu
        //public ActionResult GetCurrentJobStatus(Guid JobId)
        //{
        //    try
        //    {
        //        using (JobRepository)
        //        {
        //            var Status = JobRepository.FindBy(m => m.Id == JobId).Select(m => m.Status).FirstOrDefault();
        //            var jsonSerialiser = new JavaScriptSerializer();
        //            var json = jsonSerialiser.Serialize(Status);
        //            return Json(Status, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //Get:Get Current Job Status Right Click Of Menu
        public ActionResult GetCurrentJobStatus(Guid JobId)
        {
            try
            {
                using (JobRepository)
                {
                    var Status = JobRepository.FindBy(m => m.Id == JobId).Select(m => m.Status).FirstOrDefault();
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(Status);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets current job status.");

                    return Json(Status, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        [HttpGet]
        //   public ActionResult GetAvailableHourMonths(string monthdate)
        public ActionResult GetAvailableHourMonths(string Year, string Month)
        {
            try
            {
                DateTime date;
                int currentmonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;
                if (Convert.ToInt32(Year) == currentYear && Convert.ToInt32(Month) == currentmonth)
                {
                    date = DateTime.Now.Date;
                }

                else
                {
                    date = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), 1);
                }

                var value = CustomerGeneralRepo.GetAvailableWorkingHours(date, date, "Month");

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(value);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed schedule according to month.");

                return Json(new { list = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult GetAvailableHoursWeek(string firstdate, string seconddate, string firstmonth, string secondmonth, string Year, string Formatteddata)
        {
            try
            {
                DateTime startdate, Enddate;
                //startdate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(firstmonth), Convert.ToInt32(firstdate));
                string startdating = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(firstmonth), Convert.ToInt32(firstdate)).ToString("yyyy-MM-dd h:mm tt");
                //  Enddate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(secondmonth), Convert.ToInt32(seconddate));
                string enddating = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(secondmonth), Convert.ToInt32(seconddate)).ToString("yyyy-MM-dd h:mm tt");
                var value = CustomerGeneralRepo.GetAvailableWorkingHours(Convert.ToDateTime(startdating), Convert.ToDateTime(enddating), "Week");
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(value);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed schedule according to week.");

                return Json(new { list = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetAvailableHoursDays(string Year, string Month, string Day)
        {
            try
            {
                DateTime startdate;
                string startingdate = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day)).ToString("yyyy-MM-dd h:mm tt"); ;
                var value = CustomerGeneralRepo.GetAvailableWorkingHours(Convert.ToDateTime(startingdate), Convert.ToDateTime(startingdate), "Day");
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(value);
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed schedule according to day.");

                return Json(new { list = json }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }
        [HttpGet]
        public ActionResult UnassignOtrwJob(string OTRWID, DateTime Date)
        {
            try
            {
                if (!String.IsNullOrEmpty(OTRWID))
                {
                    Guid OTRW = Guid.Parse(OTRWID);
                    var jobUnassign = JobAssignToMappingRepo.FindBy(m => m.AssignTo == OTRW && m.DateBooked == Date && m.IsDelete == false).ToList();
                    foreach (var job in jobUnassign)
                    {
                        job.IsDelete = true;
                        job.ModifiedDate = DateTime.Now;
                        job.ModifiedBy = Guid.Parse(base.GetUserId);

                        JobAssignToMappingRepo.DeAttach(job);
                        JobAssignToMappingRepo.Edit(job);
                        JobAssignToMappingRepo.Save();
                    }
                    // var UserList = AspNetUsers.
                    string OTRWId = Convert.ToString(OTRWID);
                    var UserName = AspNetUsersRepo.FindBy(m => m.Id == OTRWId).Select(m => m.UserName).FirstOrDefault();
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " unassigned job from " + UserName);
                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "2" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Delete Employee Job By jobid
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Redirects dashboard</returns>
        [Authorize(Roles = "Admin,OPERATIONS")]
        [HttpGet]
        public ActionResult DeleteEmployeeJob(string id)
        {
            Guid jobid = Guid.Parse(id);
            try
            {
                using (Employeejob)
                {
                    int result = 0;
                    Jobs empjob = Employeejob.FindBy(i => i.Id == jobid).FirstOrDefault();
                    string DisplayStatus = ((Constant.JobStatus)empjob.Status).ToString();
                    if (empjob.Status != 11 || empjob.Status != 15)
                    {
                        //Delete data in job Assign to Maaping
                        var jobAssignEntity = JobAssignToMappingRepo.FindBy(m => m.JobId == jobid && m.IsDelete == false).ToList();
                        if (jobAssignEntity != null)
                        {
                            foreach (var assign in jobAssignEntity)
                            {
                                assign.IsDelete = true;
                                assign.ModifiedDate = DateTime.Now;
                                assign.ModifiedBy = Guid.Parse(base.GetUserId);

                                JobAssignToMappingRepo.Edit(assign);
                                JobAssignToMappingRepo.Save();
                            }
                        }

                        //Delete Data in Invoice

                        var invoiceDelete = InvoiceRepo.FindBy(m => m.EmployeeJobId == jobid && m.IsDelete == false).FirstOrDefault();
                        if (invoiceDelete != null)
                        {
                            invoiceDelete.IsDelete = true;
                            invoiceDelete.ModifiedDate = DateTime.Now;
                            invoiceDelete.ModifiedBy = Guid.Parse(base.GetUserId);
                            InvoiceRepo.Edit(invoiceDelete);
                            InvoiceRepo.Save();
                        }

                        //Delete Data in PurchaseOrderByJob

                        var purchaseDelete = JobPurchaseOrder.FindBy(m => m.JobID == jobid && m.IsDelete==false).FirstOrDefault();
                        if(purchaseDelete != null)
                        {
                            purchaseDelete.IsDelete = true;
                            purchaseDelete.ModifiedDate = DateTime.Now;
                            purchaseDelete.ModifiedBy = Guid.Parse(base.GetUserId);
                            JobPurchaseOrder.Edit(purchaseDelete);
                            InvoiceRepo.Save();
                        }
                        
                        // Delete Data in UserTimeSheet

                        var userTimeSheet = UserTimeSheetRepo.FindBy(m => m.JobId == jobid && m.IsDelete == false).FirstOrDefault();
                         if(userTimeSheet != null)
                        {
                            userTimeSheet.IsDelete = true;
                            userTimeSheet.ModifiedDate = DateTime.Now;
                            userTimeSheet.ModifiedBy = Guid.Parse(base.GetUserId);
                            UserTimeSheetRepo.Edit(userTimeSheet);
                            UserTimeSheetRepo.Save();
                        }
                        
                        
                        //Delete data in jobs

                        result = 0;
                        JobRepository.SavejobStatus(empjob);
                        //empjob.IsDelete = true;
                        //Employeejob.Edit(empjob);
                        //Employeejob.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " deleted employee job.");
                        return Json(new { result, status = DisplayStatus }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = 1;
                        return Json(new { result, status = DisplayStatus }, JsonRequestBehavior.AllowGet);
                    }
                }


                //return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
