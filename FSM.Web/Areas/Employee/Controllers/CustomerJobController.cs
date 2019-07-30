using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Areas.Customer.ViewModels;
using FSM.Web.Common;
using FSM.Core.Entities;
using Microsoft.Practices.Unity;
using FSM.Core.Interface;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using FSM.Web.FSMConstant;
using FSM.Core.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Rotativa;
using TransmitSms;
using log4net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace FSM.Web.Areas.Employee.Controllers
{
    [Authorize]
    public class CustomerJobController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                          ().DeclaringType);

        [Dependency]
        public IEmployeeJobRepository JobRepository { get; set; }
        public IEmployeeWorkTypeRepository EmployeeWorktypeRepo { get; set; }

        [Dependency]
        public ICustomerGeneralInfoRepository CustomerGeneralInfoRepo { get; set; }
        [Dependency]
        public ICustomerSiteDetailRepository CustomerSiteDetailRepo { get; set; }
        [Dependency]
        public IEmployeeJobDocumentRepository EmployeeJobDoc { get; set; }
        [Dependency]
        public ICustomerResidenceDetailRepository CustomerResidence { get; set; }
        [Dependency]
        public ICustomerConditionReportRepository ConditionReport { get; set; }
        [Dependency]
        public ICustomerContactsRepository Customercontacts { get; set; }
        [Dependency]
        public ICustomerBillingAddressRepository CustomerBilling { get; set; }
        [Dependency]
        public ICustomerSiteDocumentsRepository CustomerSitesDocumentsRepo { get; set; }
        [Dependency]
        public ICustomerContactLogRepository CustomercontactLogRepo { get; set; }

        [Dependency]
        public ISupportdojobMapping SupportdojobRepo { get; set; }

        [Dependency]
        public IUserTimeSheetRepository UserTimeSheetRepo { get; set; }

        [Dependency]
        public IJobAssignToMappingRepository JobAssignMapping { get; set; }
        [Dependency]
        public IEmployeeJobDocumentRepository JobDocumentRepo { get; set; }
        [Dependency]
        public ICustomerReminderRepository CustomerReminderRepo { get; set; }
        [Dependency]
        public IAspNetUsersRepository AspNetUsersRepo { get; set; }
        [Dependency]
        public IEmployeeDetailRepository EmployeeRepo { get; set; }
        [Dependency]
        public IiNoviceRepository InvoiceRepo { get; set; }
        [Dependency]
        public IScheduleReminderRepository ScheduleReminderRepo { get; set; }
        [Dependency]
        public IContactLogSiteContactsMappingRepository ContactLogSiteContactsMappingRepo { get; set; }
        [Dependency]
        public IVacationRepository VacationRepo { get; set; }
        [Dependency]
        public ICompulsaryTaskRepository CompulsaryTaskRepo { get; set; }
        [Dependency]
        public IJobTaskMappingRepository JobTaskMappingRepo { get; set; }
        [Dependency]
        public IinvoicedJCLItemMappingRepository invoiceJCLItemRepo { get; set; }
        [Dependency]
        public IPurchaseOrderByJobRepository JobPurchaseOrder { get; set; }
        [Dependency]
        public IJCLRepository JCLRepo { get; set; }
        [Dependency]
        public IiNvoicePaymentRepository InvoicePaymentRepo { get; set; }
        public ActionResult SaveJobInfo(string id, string activetab, string success, string pagenum)
        {
            try
            {
                JobTabPanelViewModel objJobTabPanelViewModel = new JobTabPanelViewModel();
                objJobTabPanelViewModel.JobId = id;
                objJobTabPanelViewModel.ActiveTab = activetab;
                objJobTabPanelViewModel.Success = success;
                objJobTabPanelViewModel.PageNum = pagenum;
                if (!string.IsNullOrEmpty(id))
                {
                    Jobs Jobs = JobRepository.FindBy(m => m.Id == new Guid(id)).FirstOrDefault();

                    CustomerSiteDetail Site = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == Jobs.SiteId).FirstOrDefault();
                    objJobTabPanelViewModel.SiteDocumentsCount = CustomerSitesDocumentsRepo.FindBy(m => m.SiteId == Jobs.SiteId).Count().ToString();

                    var billingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == Site.CustomerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                    CustomerBillingAddress BillingAddress;
                    if (billingaddress != null)
                        BillingAddress = billingaddress;
                    else
                    {
                        BillingAddress = CustomerBilling.GetAll().Where(i => i.CustomerGeneralInfoId == Site.CustomerGeneralInfoId).OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                    }


                    if (Site != null)
                    {
                        objJobTabPanelViewModel.SiteContactId = Site.ContactId.ToString();
                        objJobTabPanelViewModel.CustomerGeneralInfoId = Site.CustomerGeneralInfoId.ToString();
                        objJobTabPanelViewModel.CustomerSiteDetailId = Site.SiteDetailId.ToString();
                    }

                    if (BillingAddress != null)
                    {
                        objJobTabPanelViewModel.BillingAddressId = BillingAddress.BillingAddressId.ToString();
                    }
                    if (Jobs != null)
                    {
                        objJobTabPanelViewModel.JobNo = Jobs.JobNo;
                        objJobTabPanelViewModel.JobType = Jobs.JobType;
                    }
                }
                TempData["ShowMsg"] = !string.IsNullOrEmpty(Request.QueryString["showmsg"]) ? "Yes" : string.Empty;

                ViewBag.CustomerId = Request.QueryString["CustomerId"];
                ViewBag.CustomerName = Request.QueryString["CustomerName"];

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added new job details.");

                return View(objJobTabPanelViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }

        [HttpGet]
        public ActionResult AddJob(string id)
        {
            try
            {
                 JobsViewModel jobsViewModel = new JobsViewModel();
                List<SelectListItem> siteList = new List<SelectListItem>();
                List<SelectListItem> customerList = new List<SelectListItem>();
                List<SelectListItem> linkJobList = new List<SelectListItem>();

                jobsViewModel.CustomerInfoId = Request.QueryString["CustomerId"];

                //jobsViewModel.CustomerId = Request.QueryString["CustomerId"];
                jobsViewModel.CustomerName = Request.QueryString["CustomerName"];
                jobsViewModel.PreferTime = FSMConstant.Constant.PrefTimeOfDay.Anytime;

                Guid CustomerId = !string.IsNullOrEmpty(jobsViewModel.CustomerInfoId) ? Guid.Parse(jobsViewModel.CustomerInfoId) : Guid.Empty;
                if (CustomerId != Guid.Empty)
                {
                    var SiteDetailId = CustomerSiteDetailRepo.FindBy(m => m.CustomerGeneralInfoId == CustomerId).Select(m => m.SiteDetailId).FirstOrDefault();
                    jobsViewModel.tempSiteId = SiteDetailId.ToString();

                    siteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == CustomerId && m.IsDelete == false).
                             Select(m => new SelectListItem()
                             {
                                 Text = m.SiteFileName,
                                 Value = m.SiteDetailId.ToString()
                             }).ToList();
                }

                var OTRWList = JobRepository.GetOTRWUser().Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();

                var supervisorList = JobAssignMapping.GetSuperVisorList(Guid.Empty).Select(m => new SelectListItem()
                {
                    Text = m.OTRWUserName,
                    Value = m.AssignTo.ToString()
                }).OrderBy(m => m.Text).ToList();

                //var linkJobList = JobRepository.FindBy(m => m.JobType != 3).Select(m => new SelectListItem()
                //{
                //    Text = "Job_" + m.JobId.ToString(),
                //    Value = m.Id.ToString()
                //}).ToList();
                jobsViewModel.JobCategory = "Stand By";
                //jobsViewModel.JobId = JobRepository.GetMaxJobID();
                jobsViewModel.JobId = JobRepository.GetMaxJobNo() != 0 ? JobRepository.GetMaxJobNo() : 0;
                jobsViewModel.JobNo = JobRepository.GetMaxJobNo() != 0 ? JobRepository.GetMaxJobNo() : 0;

                if (jobsViewModel.CustomerInfoId != "00000000-0000-0000-0000-000000000000")
                {
                    jobsViewModel.CustomerList = CustomerGeneralInfoRepo.GetAll().Where(m => m.CustomerGeneralInfoId == CustomerId && m.IsDelete == false).
                          Select(m => new SelectListItem()
                          {
                              Text = m.CustomerLastName,
                              Value = m.CustomerGeneralInfoId.ToString()
                          }).ToList();
                }
                else
                {
                    jobsViewModel.CustomerList = customerList;
                }
                jobsViewModel.SiteList = siteList;
                jobsViewModel.OTRWList = OTRWList;
                jobsViewModel.SuperVisorList = supervisorList;
                jobsViewModel.LinkJobList = linkJobList;
                List<AssignViewModel> assignViewModel = new List<AssignViewModel>();
                jobsViewModel.AssignInfo = assignViewModel;

                jobsViewModel.CompulsaryList = CompulsaryTaskRepo.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.TaskName,
                    Value = x.TaskId.ToString(),
                    Selected = true

                }).ToList();

                return View(jobsViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CustomerGeneralInfoRepo.Dispose();
                JobRepository.Dispose();
            }
        }

        [HttpPost]
        public ActionResult AddJob(JobsViewModel jobsViewModel)
        {
            Helper helper = new Helper();
            try
            {
                double hoursLeft = 0;
                if (ModelState.IsValid)
                {
                    var SuccessMsg = "";
                    var savedId = "";

                    var SiteId = Guid.Parse(jobsViewModel.tempSiteId);
                    var customerSite = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == SiteId).Select(m =>
                        new
                        {
                            m.SiteDetailId,
                            m.Contracted
                        }
                    ).FirstOrDefault();

                    var ObjCustomerResidence = CustomerResidence
                        .FindBy(m => m.SiteDetailId.ToString() == jobsViewModel.tempSiteId).FirstOrDefault();
                    if (ObjCustomerResidence == null)
                    {
                        ModelState.AddModelError("ResidenceRequired",
                            "Please add resident detail for the selected site.");
                        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        return Json(new { status = "failure", errors = errCollection });
                    }
                    //if (ObjCustomerResidence.NeedTwoPPL == true)
                    //{
                    if (jobsViewModel.OTRWRequired == null)
                    {
                        jobsViewModel.OTRWRequired = 1;
                    }
                    //}

                    //Get Assign job data in temp table

                    //if (TempData["TempAssignJobData"] != null)
                    //{
                    List<JobAssignToMapping> assignMappingList =
                        (List<JobAssignToMapping>)TempData["TempAssignJobData"];

                    if (assignMappingList != null)
                    {
                        if (jobsViewModel.DateBooked == null)
                        {
                            ModelState.AddModelError("DateBooked", "Date booked is required!");
                            var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                            return Json(new { status = "failure", errors = errCollection });
                        }
                        foreach (var assign in assignMappingList)
                        {
                            DateTime dt = assign.StartTime.Value;
                            TimeSpan startTime = new TimeSpan(dt.Hour, dt.Minute, dt.Second);
                            DateTime Start_Time = Convert.ToDateTime(jobsViewModel.DateBooked + startTime);
                            var dtEndTime = DateTime.ParseExact("08:00:00 PM",
                                "h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                            TimeSpan endTime = new TimeSpan(dtEndTime.Hour, dtEndTime.Minute, dtEndTime.Second);
                            var Cal_EndTime = Convert.ToDateTime(jobsViewModel.DateBooked + endTime);
                            TimeSpan DiffTime = Cal_EndTime.Subtract(Start_Time);
                            double Hours = DiffTime.Hours;
                            jobsViewModel.StartTime = Start_Time;
                            jobsViewModel.EndTime = Cal_EndTime;
                            var PerPersonAssignHours =
                                jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                            if (PerPersonAssignHours > Hours)
                            {
                                jobsViewModel.EndTime = Cal_EndTime;
                            }
                            else
                            {
                                jobsViewModel.EndTime = Start_Time.AddHours(Convert.ToDouble(PerPersonAssignHours));
                            }

                            ModelState.Clear();

                            var assignTo = Guid.Parse(assign.AssignTo.ToString());
                            //var hasJob = JobRepository.OTRWHasJobWithName(assignTo, Start_Time,
                            //    (DateTime)jobsViewModel.DateBooked);

                            //foreach (var user in hasJob)
                            //{
                            //    ModelState.AddModelError("JobExist",
                            //        "Job already assign to " + user.UserName + " on given start/end time.");

                            //    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                            //    return Json(new { status = "failure", errors = errCollection });
                            //}
                            var employeeOnLeave =
                                VacationRepo.CheckUserLeave(assignTo, jobsViewModel.DateBooked); // Check User On Leave
                            foreach (var user in employeeOnLeave)
                            {
                                ModelState.AddModelError("UserLeave", "" + user + " on leave.");

                                var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                                return Json(new { status = "failure", errors = errCollection });
                            }
                        }
                    }


                    var existjob = JobRepository.FindBy(m => m.JobNo == jobsViewModel.JobNo).Select(m =>
                        new
                        {
                            m.Id,
                            m.CreatedDate,
                            m.CreatedBy
                        }).FirstOrDefault();



                    jobsViewModel.Id = existjob == null ? Guid.NewGuid() : existjob.Id;
                    jobsViewModel.CustomerGeneralInfoId = !string.IsNullOrEmpty(jobsViewModel.CustomerInfoId)
                        ? Guid.Parse(jobsViewModel.CustomerInfoId)
                        : Guid.Empty;
                    jobsViewModel.SiteId = !string.IsNullOrEmpty(jobsViewModel.tempSiteId)
                        ? Guid.Parse(jobsViewModel.tempSiteId)
                        : Guid.Empty;

                    jobsViewModel.BookedBy = Guid.Parse(base.GetUserId);
                    jobsViewModel.JobNotes = jobsViewModel.Job_Notes;
                    jobsViewModel.OperationNotes = jobsViewModel.Operation_Notes;
                    //if ((jobsViewModel.DateBooked == null) && (jobsViewModel.AssignTo == (Nullable<Guid>)null && jobsViewModel.AssignTo2 == null))
                    if ((jobsViewModel.DateBooked == null) && (assignMappingList == null))
                    {
                        jobsViewModel.Status = Constant.JobStatus.OnHold;
                    }
                    else if (assignMappingList != null)
                    {
                        //if (jobsViewModel.DateBooked == null)
                        //{
                        //    ModelState.AddModelError("DateBooked", "Date booked is required!");
                        //    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        //    return Json(new { status = "failure", errors = errCollection });
                        //}
                        if (jobsViewModel.OTRWRequired == null)
                        {
                            jobsViewModel.OTRWRequired = 1;
                        }
                        if (assignMappingList.Count == (int)jobsViewModel.OTRWRequired)
                        {
                            jobsViewModel.Status = Constant.JobStatus.Assigned;
                        }
                        else
                        {
                            jobsViewModel.Status = Constant.JobStatus.Booked;
                        }

                    }
                    else if (jobsViewModel.DateBooked != null)
                    {
                        jobsViewModel.Status = Constant.JobStatus.Booked;
                    }
                    if (jobsViewModel.OTRWRequired == 0)
                    {

                        jobsViewModel.EstimatedHrsPerUser = jobsViewModel.EstimatedHours;
                    }
                    else
                    {
                        jobsViewModel.EstimatedHrsPerUser =
                            jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                    }
                    if (jobsViewModel.EstimatedHrsPerUser < 1)
                    {
                        ModelState.AddModelError("EstimatedHrsPerUser",
                            "Minimum estimated hour per person must be 1 hour!");
                        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        return Json(new { status = "failure", errors = errCollection });
                    }

                    if (jobsViewModel.OTRWRequired == 0)
                    {
                        jobsViewModel.OTRWRequired = 1;
                    }

                    CommonMapper<JobsViewModel, Jobs> mapper = new CommonMapper<JobsViewModel, Jobs>();
                    Jobs jobs = mapper.Mapper(jobsViewModel);
                    jobsViewModel.StartTime = jobsViewModel.StartTime;

                    if (existjob == null)
                    {
                        if (customerSite.Contracted.HasValue)
                        {
                            jobs.ContractDueDate = getContractDueDate(customerSite.Contracted, jobsViewModel);
                        }
                        jobs.CreatedBy = Guid.Parse(base.GetUserId);
                        jobs.CreatedDate = DateTime.Now;
                        jobs.IsDelete = false;
                        JobRepository.Add(jobs);
                        JobRepository.Save();

                        //operation notes save in contact log
                        if (jobs.OperationNotes != null && jobs.OperationNotes != "")
                        {
                            var getUserName = EmployeeRepo.FindBy(m => m.EmployeeId == jobs.CreatedBy).Select(m => m.UserName).FirstOrDefault();
                            string operationNotes = "<b>Operation Notes Added by:</b> " + getUserName + " On " + DateTime.Now + "<br/>" + jobs.OperationNotes;

                            SaveDataContactLog(jobs.Id, operationNotes);
                        }

                        // saving supoort job
                        SaveSupportJob(jobsViewModel);

                        SuccessMsg = "Record saved successfully !";
                        savedId = jobsViewModel.Id.ToString();
                    }
                    else
                    {
                        jobs.CreatedDate = existjob.CreatedDate;
                        jobs.CreatedBy = existjob.CreatedBy;
                        jobs.ModifiedDate = DateTime.Now;
                        jobs.ModifiedBy = Guid.Parse(base.GetUserId);


                        JobRepository.DeAttach(jobs);
                        JobRepository.Edit(jobs);
                        JobRepository.Save();



                        // saving supoort job
                        SaveSupportJob(jobsViewModel);

                        SuccessMsg = "Record updated successfully !";
                        savedId = string.Empty;
                    }

                    //data insert into jobTaskMapping
                    if (jobsViewModel.TaskId != null)
                    {
                        foreach (var compulsary in jobsViewModel.TaskId)
                        {
                            JobTaskMapping jobtaskmapping = new JobTaskMapping();
                            jobtaskmapping.Id = Guid.NewGuid();
                            jobtaskmapping.JobId = jobsViewModel.Id;
                            jobtaskmapping.CompulsaryId = compulsary.Value;
                            jobtaskmapping.CreatedDate = DateTime.Now;
                            jobtaskmapping.CreatedBy = Guid.Parse(base.GetUserId);

                            JobTaskMappingRepo.Add(jobtaskmapping);
                            JobTaskMappingRepo.Save();
                        }
                    }


                    if (assignMappingList != null)
                    {
                        foreach (var assign in assignMappingList)
                        {
                            //saving assignto into jobassigntomapping
                            if (jobsViewModel.Status != Constant.JobStatus.Created ||
                                jobsViewModel.Status != Constant.JobStatus.NotCreated
                                || jobsViewModel.Status != Constant.JobStatus.Booked ||
                                jobsViewModel.Status != Constant.JobStatus.NotBooked
                                || jobsViewModel.Status != Constant.JobStatus.NotAssigned)
                            {
                                JobAssignToMappingViewModel jobAssignViewModel = new JobAssignToMappingViewModel();

                                double jobTimeDiff = 0;

                                DateTime start_Time = Convert.ToDateTime(assign.StartTime);
                                TimeSpan startTime = new TimeSpan(start_Time.Hour, start_Time.Minute, start_Time.Second);
                                DateTime Start_Time = Convert.ToDateTime(jobsViewModel.DateBooked + startTime);
                                var Cal_EndTime = DateTime.ParseExact("08:00:00 PM", "h:mm:ss tt",
                                    System.Globalization.CultureInfo.InvariantCulture);
                                TimeSpan endTime = new TimeSpan(Cal_EndTime.Hour, Cal_EndTime.Minute, Cal_EndTime.Second);
                                Cal_EndTime = Convert.ToDateTime(jobsViewModel.DateBooked + endTime);
                                TimeSpan DiffTime = Cal_EndTime.Subtract(start_Time);
                                double Hours = DiffTime.Hours;
                                if (jobsViewModel.EstimatedHours > Hours)
                                {
                                    jobAssignViewModel.EndTime = Cal_EndTime;
                                }
                                else
                                {
                                    jobAssignViewModel.EndTime =
                                        Start_Time.AddHours(Convert.ToDouble(jobsViewModel.EstimatedHrsPerUser));
                                }


                                //hoursLeft = ((double)(jobsViewModel.EstimatedHrsPerUser) -
                                //             (jobsViewModel.EndTime.Value - jobsViewModel.StartTime.Value).TotalHours);

                                TimeSpan span = jobsViewModel.EndTime.Value.Subtract(jobsViewModel.StartTime.Value);
                                hoursLeft = ((span.Hours + span.Minutes / 60.0) - (double)(jobsViewModel.EstimatedHrsPerUser));

                                jobAssignViewModel.Id = Guid.NewGuid();
                                jobAssignViewModel.IsDelete = false;
                                jobAssignViewModel.JobId = jobsViewModel.Id;
                                jobAssignViewModel.AssignTo = assign.AssignTo;
                                jobAssignViewModel.Status = Constant.JobStatus.Assigned;
                                jobAssignViewModel.DateBooked = jobsViewModel.DateBooked;
                                jobAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                                jobAssignViewModel.CreatedDate = DateTime.Now;
                                jobAssignViewModel.StartTime = Start_Time;
                                jobAssignViewModel.EstimatedHrsPerUser =
                                    jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);

                                var jobAssignDateBooked = jobsViewModel.DateBooked;
                                var jobAssignedStartDate = assign.StartTime;
                                var jobAssignedEndDate = jobAssignViewModel.EndTime;

                                CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper1 =
                                    new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                                JobAssignToMapping jobAssignToMapping1 = Assignmapper1.Mapper(jobAssignViewModel);

                                JobAssignMapping.Add(jobAssignToMapping1);
                                JobAssignMapping.Save();

                                while (hoursLeft > 0)
                                {
                                    TimeSpan jobTime = new TimeSpan(0, 0, 0);
                                    TimeSpan jobStartTime = new TimeSpan(9, 0, 0);
                                    TimeSpan jobEndTime = new TimeSpan(19, 0, 0);
                                    jobAssignDateBooked = jobsViewModel.DateBooked.Value.AddDays(1);

                                    // check for saturday & sunday
                                    var jobbookDay = jobsViewModel.DateBooked.Value.ToString("dddd");
                                    if (jobbookDay == "Friday")
                                    {
                                        jobAssignDateBooked = jobsViewModel.DateBooked.Value.AddDays(3);
                                    }
                                    else if (jobbookDay == "Saturday")
                                    {
                                        jobAssignDateBooked = jobsViewModel.DateBooked.Value.AddDays(2);
                                    }

                                    // if otrw has job already booked
                                    var otrwHasJob =
                                        JobRepository.OTRWHasJobBooked(assign.AssignTo, jobsViewModel.DateBooked);
                                    while (otrwHasJob)
                                    {
                                        jobAssignDateBooked = jobsViewModel.DateBooked.Value.AddDays(1);

                                        // check for saturday & sunday
                                        jobbookDay = jobsViewModel.DateBooked.Value.ToString("dddd");
                                        if (jobbookDay == "Friday")
                                        {
                                            jobAssignDateBooked = jobsViewModel.DateBooked.Value.AddDays(3);
                                        }
                                        else if (jobbookDay == "Saturday")
                                        {
                                            jobAssignDateBooked = jobsViewModel.DateBooked.Value.AddDays(2);
                                        }

                                        otrwHasJob = JobRepository.OTRWHasJobBooked(assign.AssignTo,
                                            jobsViewModel.DateBooked.Value);
                                    }



                                    jobAssignDateBooked = jobsViewModel.DateBooked.Value + jobTime;
                                    jobAssignedStartDate = jobsViewModel.DateBooked.Value + jobStartTime;
                                    if (hoursLeft < 10)
                                    {
                                        jobAssignedEndDate = jobAssignedStartDate.Value.AddHours(hoursLeft);
                                    }
                                    else
                                    {
                                        jobAssignedEndDate = jobsViewModel.DateBooked.Value + jobEndTime;
                                    }
                                    jobTimeDiff = (jobEndTime - jobStartTime).Hours;
                                    hoursLeft = hoursLeft - jobTimeDiff;

                                    // saving jobassign mapping
                                    jobAssignViewModel.Id = Guid.NewGuid();
                                    jobAssignViewModel.IsDelete = false;
                                    jobAssignViewModel.JobId = jobsViewModel.Id;
                                    jobAssignViewModel.AssignTo = assign.AssignTo;
                                    jobAssignViewModel.Status = Constant.JobStatus.Assigned;
                                    jobAssignViewModel.DateBooked = jobsViewModel.DateBooked;
                                    jobAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                                    jobAssignViewModel.CreatedDate = DateTime.Now;
                                    jobAssignViewModel.StartTime = jobAssignedStartDate;
                                    jobAssignViewModel.EndTime = jobAssignedEndDate;
                                    jobAssignViewModel.EstimatedHrsPerUser =
                                        jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                                    CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper =
                                        new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                                    JobAssignToMapping jobAssignToMapping = Assignmapper.Mapper(jobAssignViewModel);
                                    JobAssignMapping.Add(jobAssignToMapping);
                                    JobAssignMapping.Save();


                                }
                                //helper.SaveAndSendNotification(Guid.Parse(base.GetUserId), (Guid)jobsViewModel.AssignTo2, jobsViewModel.Id, "Job");
                            }
                        }
                    }
                    //}
                    //Send Confirmation Mail
                    if (jobsViewModel.SendJobEmail == Constant.SendJobEmail.SendConfirmationEmail)
                    {
                        Guid customerId = Guid.Parse(jobsViewModel.CustomerInfoId);
                        Guid siteDetailId = Guid.Parse(jobsViewModel.tempSiteId);

                        var customerDetail = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == customerId).FirstOrDefault();
                        var siteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                        StreamReader reader;
                        if (siteDetail.StrataPlan == null)
                        {
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ConfirmationAppointmentDomesticCustomer.htm"));
                        }
                        else
                        {
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ConfirmationAppointmentStrataRealestate.htm"));
                        }

                        string readFile = reader.ReadToEnd();
                        string myString = readFile;
                        myString = myString.Replace("{{ClientName}}", customerDetail.CustomerLastName);
                        myString = myString.Replace("{{SiteAdress}}", siteDetail.SiteFileName);
                        myString = myString.Replace("{{DateBooked}}", jobsViewModel.DateBooked.ToString());

                        string body = Convert.ToString(myString);

                        using (MailMessage mm = new MailMessage())
                        {
                            mm.IsBodyHtml = true;
                            mm.Body = body;
                            mm.Subject = "Job Confirmation " + jobsViewModel.JobNo;
                            mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                            var TOId = Customercontacts.FindBy(m => m.CustomerGeneralInfoId == customerId).Select(m => m.EmailId).ToList();
                            if (TOId.Count > 0)
                            {
                                foreach (string ToEmail in TOId)
                                {
                                    mm.To.Add(new MailAddress(ToEmail)); //Adding Multiple To email Id
                                    using (SmtpClient smtp = new SmtpClient())
                                    {
                                        smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                        smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                        smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                        smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                        smtp.Send(mm);


                                        var employeejob = JobRepository.FindBy(m => m.JobNo == jobsViewModel.JobNo).FirstOrDefault();   //get job data
                                        var customerInfo = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == employeejob.CustomerGeneralInfoId).FirstOrDefault();  //get customer data

                                        CustomerContactLog customerContactLog = new CustomerContactLog();
                                        customerContactLog.CustomerContactId = Guid.NewGuid();
                                        customerContactLog.CustomerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                                        customerContactLog.CustomerId = (customerInfo.CTId).ToString();
                                        customerContactLog.JobId =jobsViewModel.Id.ToString();
                                        customerContactLog.LogDate = DateTime.Now;
                                        customerContactLog.IsDelete = false;
                                        customerContactLog.CreatedDate = DateTime.Now;
                                        customerContactLog.Note = " <b> Sent email to a cutomer </b> " + customerInfo.CustomerLastName + " <b> regarding job confirmation for jobNo </b> " + jobsViewModel.JobNo;
                                        customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);
                                        CustomercontactLogRepo.Add(customerContactLog);
                                        CustomercontactLogRepo.Save();
                                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                        log.Info(base.GetUserName + " sent email to a cutomer regarding job confirmation ");
                                       
                                    }
                                }
                            }
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added new job.");


                    return Json(new { status = "saved", msg = SuccessMsg, SavedId = savedId });
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }

        private Nullable<DateTime> getContractDueDate(Nullable<int> contractDuration, JobsViewModel jobsViewModel)
        {
            int dueMonths = 0;
            switch (contractDuration)
            {
                case 1:
                    dueMonths = 1;
                    break;
                case 2:
                    dueMonths = 2;
                    break;
                case 3:
                    dueMonths = 3;
                    break;
                case 4:
                    dueMonths = 4;
                    break;
                case 5:
                    dueMonths = 6; // represent 6 months
                    break;
                case 6:
                    dueMonths = 12; // represent 12 months
                    break;
                default:
                    break;
            }
            if (jobsViewModel.DateBooked.HasValue)
            {
                var dueDate = jobsViewModel.DateBooked.Value.AddMonths(dueMonths);
                var currDueDate = dueDate.AddDays(1);
                var contractDay = currDueDate.ToString("dddd"); // getting day
                if (contractDay == "Saturday")
                {
                    jobsViewModel.ContractDueDate = currDueDate.AddDays(2);
                }
                else if (contractDay == "Sunday")
                {
                    jobsViewModel.ContractDueDate = currDueDate.AddDays(1);
                }
                else
                {
                    jobsViewModel.ContractDueDate = currDueDate;
                }
            }

            return jobsViewModel.ContractDueDate;
        }
        public ActionResult GetCustomerList(string SearchTerm)
        {
            var customerList = CustomerGeneralInfoRepo.GetAll().Where(m => m.IsDelete == false && m.CustomerLastName.Contains(SearchTerm))
            .Select(m => new SelectListItem()
            {
                Text = m.CustomerLastName,
                Value = m.CustomerGeneralInfoId.ToString()
            }).Take(20).ToList();

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed all customer list.");

            return Json(new { customerData = customerList }, JsonRequestBehavior.AllowGet);
        }

        //[NoCache]
        [HttpGet]
        public ActionResult EditJob(string id)
        {
            try
            {
                Guid Id = !string.IsNullOrEmpty(id) ? Guid.Parse(id) : Guid.Empty;
                var job = JobRepository.FindBy(m => m.Id == Id).FirstOrDefault();
                var invoiceQuoteList = InvoiceRepo.FindBy(m => m.JobId == job.JobNo && m.IsDelete == false).ToList();
                int invoiceTypeCount = InvoiceRepo.FindBy(m => m.JobId == job.JobNo && m.IsDelete == false && m.InvoiceType == "Invoice").Count();
                invoiceQuoteList = invoiceQuoteList.DistinctBy(m => m.InvoiceType).OrderByDescending(m => m.CreatedDate).ToList();

                CommonMapper<Invoice, InvoiceData> mapperColor = new CommonMapper<Invoice, InvoiceData>();
                List<InvoiceData> invoiceList = mapperColor.MapToList(invoiceQuoteList).ToList();

                var userName = "";
                if (job.ModifiedBy == null)
                {
                    userName = EmployeeRepo.FindBy(m => m.EmployeeId == job.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                }
                else
                {
                    userName = EmployeeRepo.FindBy(m => m.EmployeeId == job.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                }
                if (job.ModifiedDate == null)
                {
                    job.CreatedDate = job.CreatedDate;
                }
                else
                {
                    job.ModifiedDate = job.ModifiedDate;
                }

                CommonMapper<Jobs, JobsViewModel> mapper = new CommonMapper<Jobs, JobsViewModel>();
                var jobsViewModel = mapper.Mapper(job);
                jobsViewModel.invoiceTypeCount = invoiceTypeCount;
                var JobAssignPeople = CustomerResidence.FindBy(m => m.SiteDetailId == job.SiteId && m.NeedTwoPPL == true).FirstOrDefault();
                if (JobAssignPeople != null)
                {
                    ViewBag.MultiplePeopleAssign = 1;
                }

                Guid CustomerId = job.CustomerGeneralInfoId;
                int? CurrentJobNo = job.JobNo;
                var customerList = CustomerGeneralInfoRepo.FindBy(m => m.IsDelete == false && m.CustomerGeneralInfoId == CustomerId).Select(m => new SelectListItem()
                {
                    Text = m.CustomerLastName,
                    Value = m.CustomerGeneralInfoId.ToString()
                }).ToList();

                var OTRWList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(job.WorkType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();

                var linkJobList = JobRepository.FindBy(m => m.CustomerGeneralInfoId == CustomerId && m.JobNo != CurrentJobNo).Select(m => new SelectListItem()
                {
                    Text = "Job_" + m.JobNo.ToString(),
                    Value = m.Id.ToString()
                }).OrderBy(m => m.Text).ToList();

                var SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == jobsViewModel.CustomerGeneralInfoId && m.IsDelete == false).
                               Select(m => new SelectListItem()
                               {
                                   Text = m.SiteFileName,
                                   Value = m.SiteDetailId.ToString()
                               }).ToList();

                var supervisorList = JobAssignMapping.GetSuperVisorList(Id).Select(m => new SelectListItem()
                {
                    Text = m.OTRWUserName,
                    Value = m.AssignTo.ToString()
                }).OrderBy(m => m.Text).ToList();

                if (jobsViewModel.SendJobEmail == Constant.SendJobEmail.SendConfirmationEmail)
                {
                    jobsViewModel.ReSendJobEmail = Constant.ReSendJobEmail.ConfirmationSent;
                }
                var JoBAssign = JobAssignMapping.FindBy(m => m.JobId == Id && m.IsDelete == false).Select(m => m.AssignTo).FirstOrDefault();
                var JoBAssignList = JobAssignMapping.FindBy(m => m.JobId == Id && m.IsDelete == false).ToList();
                string otrnotes = "";
                if (JoBAssignList.Count > 0)
                {
                    foreach (var notes in JoBAssignList)
                    {
                        otrnotes = otrnotes + notes.OTRWNotes;
                    }
                    jobsViewModel.OTRWjobNotes = otrnotes;
                }
                else
                {
                    jobsViewModel.OTRWjobNotes = otrnotes;
                }

                var compulsaryTaskId = JobTaskMappingRepo.FindBy(m => m.JobId == Id).Select(m => m.CompulsaryId).ToList();

                jobsViewModel.CompulsaryList = CompulsaryTaskRepo.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.TaskName,
                    Value = x.TaskId.ToString()
                }).ToList();
                jobsViewModel.TaskId = compulsaryTaskId;
                string Notes = "";
                List<AssignViewModel> items = new List<AssignViewModel>();
                foreach (var assign in JoBAssignList)
                {
                    AssignViewModel item = new AssignViewModel();
                    item.AssignId = assign.Id;
                    item.AssignStartTime = assign.StartTime;
                    item.AssignTo = assign.AssignTo;
                    item.AssignDateBooked = assign.DateBooked;
                    item.AssignOTRWList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(job.WorkType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id,
                        Selected = m.Id.ToLower() == item.AssignTo.ToString().ToLower()
                    }).ToList();
                    //item.AssignTo = JobAssignMapping.GetAll().Select(m => new SelectListItem()
                    //{
                    //    Text = m,
                    //    Value = m.JCLId.ToString(),
                    //    Selected = m.JCLId == item.GCLID
                    //}).OrderBy(k => k.Text).ToList();

                    var CreatedBy = assign.AssignTo;
                    string CreatedByName = "";
                    if (CreatedBy != null)
                    {
                        var emp = EmployeeRepo.FindBy(i => i.EmployeeId == CreatedBy).FirstOrDefault();
                        if (emp != null)
                        {
                            CreatedByName = emp.UserName;
                        }
                    }
                    if (!string.IsNullOrEmpty(assign.OTRWNotes))
                    {
                        Notes = Notes + "<b> Added By: </b> " + CreatedByName + "</br>" + assign.OTRWNotes + "</br>";
                    }
                    item.OTRWNotes = assign.OTRWNotes;
                    items.Add(item);
                }
                jobsViewModel.OTRWjobNotes = Notes;
                jobsViewModel.AssignInfo = items.OrderBy(m => m.AssignDateBooked).ToList();
                //jobsViewModel.AssignTo2 = JoBAssign2;
                jobsViewModel.AssignTo = JoBAssign;

                jobsViewModel.CurrentJobtype = Convert.ToInt32(jobsViewModel.JobType).ToString();
                jobsViewModel.CurrentJobStatus = Convert.ToInt32(jobsViewModel.Status);
                jobsViewModel.ChangeJobType = false;
                jobsViewModel.InvoiceQuoteList = invoiceList;
                jobsViewModel.CustomerList = customerList;
                jobsViewModel.SiteList = SiteList;
                jobsViewModel.OTRWList = OTRWList;
                jobsViewModel.SuperVisorList = supervisorList;
                jobsViewModel.LinkJobList = linkJobList;
                jobsViewModel.CustomerInfoId = jobsViewModel.CustomerGeneralInfoId.ToString();
                jobsViewModel.tempSiteId = jobsViewModel.SiteId.ToString();
                jobsViewModel.ModifiedDate = jobsViewModel.ModifiedDate;
                jobsViewModel.CreatedDate = jobsViewModel.CreatedDate;
                jobsViewModel.ModifiedBy = jobsViewModel.ModifiedBy;
                jobsViewModel.UserName = userName;
                jobsViewModel.GetUserRoles = this.GetUserRoles;
                jobsViewModel.ApproveStatus = jobsViewModel.IsApproved != null ?
                               jobsViewModel.IsApproved == true ? "[ Approved ]" : "[ Not Approved ]" : "[ Not Approved ]";

                jobsViewModel.OldDateBooked = jobsViewModel.DateBooked;
                //List<AssignViewModel> assignViewModel = new List<AssignViewModel>();
                //jobsViewModel.AssignInfo = assignViewModel;

                //var UserName = AspNetUsersRepo.FindBy(m => Guid.Parse(m.Id) == (jobsViewModel.ModifiedBy)).Select(m=>m.UserName).FirstOrDefault();

                var linkJob = SupportdojobRepo.FindBy(m => m.SupportJobId == Id).FirstOrDefault();
                jobsViewModel.LinkJobId = linkJob != null ? linkJob.LinkedJobId.ToString() : string.Empty;

                var userTimeReport = UserTimeSheetRepo.FindBy(m => m.JobId == Id && m.IsRunning == 1).FirstOrDefault();
                var jobassign = JobAssignMapping.FindBy(m => m.JobId == Id && m.IsDelete == false).FirstOrDefault();
                if (userTimeReport != null && jobassign != null)
                {
                    jobsViewModel.IsJobStart = true;
                }
                else
                {
                    jobsViewModel.IsJobStart = false;
                }
                ViewBag.ShowMsg = TempData["ShowMsg"] != null ? TempData["ShowMsg"].ToString() : string.Empty;
                return View(jobsViewModel);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                CustomerGeneralInfoRepo.Dispose();
                JobRepository.Dispose();
                CustomerSiteDetailRepo.Dispose();
                SupportdojobRepo.Dispose();
            }
        }

        [HttpPost]
        public ActionResult EditJob(JobsViewModel jobsViewModel)
        {
            try
            {
                if (ModelState.IsValid)

                {
                    var SiteId = Guid.Parse(jobsViewModel.tempSiteId);
                    var customerSite = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == SiteId).Select(m =>
                        new
                        {
                            m.SiteDetailId,
                            m.Contracted
                        }
                    ).FirstOrDefault();

                    var ObjCustomerResidence = CustomerResidence
                        .FindBy(m => m.SiteDetailId.ToString() == jobsViewModel.tempSiteId).FirstOrDefault();
                    //if (ObjCustomerResidence.NeedTwoPPL == true)
                    //{
                    if (jobsViewModel.OTRWRequired == null)
                    {
                        jobsViewModel.OTRWRequired = 1;
                    }

                    //}




                    //if (TempData["TempAssignJobData"] != null)
                    //{
                    List<JobAssignToMapping> assignMappingList =
                        (List<JobAssignToMapping>)TempData["TempAssignJobData"];

                    if (assignMappingList != null)
                    {
                        if (jobsViewModel.DateBooked == null)
                        {
                            ModelState.AddModelError("DateBooked", "Date booked is required!");
                            var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                            return Json(new { status = "failure", errors = errCollection });
                        }
                        foreach (var assign in assignMappingList)
                        {
                            DateTime Start_Time = Convert.ToDateTime(assign.StartTime);
                            //change starttime according datebooked

                            DateTime dt = DateTime.Parse(assign.StartTime.ToString());
                            TimeSpan time = dt.TimeOfDay;
                            DateTime dateonly;
                            if (assign.DateBooked == null)
                            {
                                dateonly = Convert.ToDateTime(jobsViewModel.DateBooked).Date;
                            }
                            else
                            {
                                dateonly = Convert.ToDateTime(assign.DateBooked).Date;
                            }
                            DateTime result = dateonly + time;
                            Start_Time = result;
                            jobsViewModel.StartTime = Start_Time;

                            var dtEndTime = DateTime.ParseExact("08:00:00 PM",
                                "h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                            TimeSpan endTime = new TimeSpan(dtEndTime.Hour, dtEndTime.Minute, dtEndTime.Second);
                            DateTime Cal_EndTime;
                            if (assign.DateBooked == null)
                            {
                                Cal_EndTime = Convert.ToDateTime(jobsViewModel.DateBooked + endTime);
                            }
                            else
                            {
                                Cal_EndTime = Convert.ToDateTime(assign.DateBooked + endTime);
                            }

                            TimeSpan DiffTime = Cal_EndTime.Subtract(Start_Time);
                            double Hours = DiffTime.Hours;
                            var PerPersonAssignHours =
                                jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                            if (PerPersonAssignHours > Hours)
                            {
                                jobsViewModel.EndTime = Cal_EndTime;
                            }
                            else
                            {
                                jobsViewModel.EndTime = Start_Time.AddHours(Convert.ToDouble(PerPersonAssignHours));
                            }

                            ModelState.Clear();

                            var assignTo = Guid.Parse(assign.AssignTo.ToString());
                            if (jobsViewModel.JobType == Constant.JobType.CheckMeasure ||
                                jobsViewModel.JobType == Constant.JobType.CallBack)
                            {
                                if (jobsViewModel.CurrentJobtype == "5" || jobsViewModel.CurrentJobtype == "7" &&
                                    jobsViewModel.DateBooked == jobsViewModel.OldDateBooked)
                                {
                                    var jobAssignmapper = JobAssignMapping.GetAll()
                                        .Where(m => m.JobId == jobsViewModel.Id && m.AssignTo == assign.AssignTo &&
                                                    m.IsDelete == false).OrderByDescending(i => i.CreatedDate)
                                        .FirstOrDefault();
                                    DateTime startdate =
                                        DateTime.Parse(assign.StartTime?.ToString("d/M/yyyy HH:mm:ss"));
                                    jobsViewModel.StartTime = startdate;
                                    DateTime enddate = startdate.AddHours(Convert.ToDouble(
                                        jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired)));
                                    jobsViewModel.EndTime = enddate;
                                }
                                else
                                {
                                    DateTime? MaxEndTime = JobAssignMapping.FindBy(m =>
                                        m.DateBooked == jobsViewModel.DateBooked && m.AssignTo == assign.AssignTo &&
                                        m.IsDelete == false).Max(m => m.EndTime); //get maximum End time With Datebooked

                                    string NewStartDate; //Update Startdate And Enddate With updatedDate 
                                    if (MaxEndTime != null)
                                    {
                                        NewStartDate = jobsViewModel.DateBooked?.ToString("d/M/yyyy") + ' ' +
                                                       MaxEndTime.Value.TimeOfDay;
                                    }
                                    else
                                    {
                                        NewStartDate =
                                            jobsViewModel.DateBooked?.ToString("d/M/yyyy") + ' ' + "06:00:00";
                                    }
                                    DateTime startdate = DateTime.Parse(NewStartDate);
                                    jobsViewModel.StartTime = startdate;

                                    DateTime Endtime = startdate.AddHours(Convert.ToDouble(
                                        jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired)));

                                    DateTime enddate = Endtime;
                                    jobsViewModel.EndTime = enddate;
                                }
                            }
                            if (jobsViewModel.CurrentJobtype == "1" && jobsViewModel.JobType != Constant.JobType.Quote)
                            {
                                //var hasJob = JobRepository.OTRWHasJobWithName(assignTo, Start_Time,
                                //(DateTime)jobsViewModel.EndTime, Guid.Empty);

                                //foreach (var user in hasJob)
                                //{
                                //    ModelState.AddModelError("JobExist",
                                //        "Job already assign to " + user.UserName + " on given start/end time.");

                                //    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                                //    return Json(new { status = "failure", errors = errCollection });
                                //}
                            }
                            else
                            {
                                //var hasJobWithJobId = JobRepository.OTRWHasJobWithName(assignTo, Start_Time,
                                // (DateTime)jobsViewModel.EndTime, jobsViewModel.Id);

                                //foreach (var user in hasJobWithJobId)
                                //{
                                //    ModelState.AddModelError("JobExist",
                                //        "Job already assign to " + user.UserName + " on given start/end time.");

                                //    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                                //    return Json(new { status = "failure", errors = errCollection });
                                //}
                            }
                            //if (hasJob.Count>0)
                            //{
                            dynamic employeeOnLeave = null;
                            if (assign.DateBooked != null)
                            {
                                employeeOnLeave =
                                   VacationRepo.CheckUserLeave(assignTo, assign.DateBooked, jobsViewModel.Id); // Check User On Leave
                            }
                            else
                            {
                                employeeOnLeave =
                                   VacationRepo.CheckUserLeave(assignTo, jobsViewModel.DateBooked, jobsViewModel.Id); // Check User On Leave
                            }
                            foreach (var user in employeeOnLeave)
                            {
                                ModelState.AddModelError("UserLeave", "" + user + " on leave.");

                                var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                                return Json(new { status = "failure", errors = errCollection });
                            }
                        }
                    }

                    jobsViewModel.CustomerGeneralInfoId = !string.IsNullOrEmpty(jobsViewModel.CustomerInfoId)
                        ? Guid.Parse(jobsViewModel.CustomerInfoId)
                        : Guid.Empty;
                    jobsViewModel.SiteId = !string.IsNullOrEmpty(jobsViewModel.tempSiteId)
                        ? Guid.Parse(jobsViewModel.tempSiteId)
                        : Guid.Empty;

                    jobsViewModel.BookedBy = Guid.Parse(base.GetUserId);
                    jobsViewModel.JobNotes = jobsViewModel.Job_Notes;
                    jobsViewModel.OperationNotes = jobsViewModel.Operation_Notes;
                    if ((jobsViewModel.DateBooked == null))
                    {
                        jobsViewModel.Status = Constant.JobStatus.OnHold;
                    }
                    else if (assignMappingList != null)
                    {
                        if (jobsViewModel.OTRWRequired == null)
                        {
                            jobsViewModel.OTRWRequired = 1;
                        }
                        if (assignMappingList.Count == (int)jobsViewModel.OTRWRequired)
                        {
                            if (Convert.ToString(jobsViewModel.Status) != "Completed")
                            {
                                jobsViewModel.Status = Constant.JobStatus.Assigned;
                            }
                        }
                        else
                        {
                            if (Convert.ToString(jobsViewModel.Status) != "Completed")
                            {
                                jobsViewModel.Status = Constant.JobStatus.Booked;
                            }
                        }
                    }
                    else if (jobsViewModel.DateBooked != null)
                    {
                        if (Convert.ToString(jobsViewModel.Status) != "Completed")
                        {
                            jobsViewModel.Status = Constant.JobStatus.Booked;
                        }
                    }
                    if (jobsViewModel.OTRWRequired == null)
                    {

                        jobsViewModel.EstimatedHrsPerUser = jobsViewModel.EstimatedHours;
                    }
                    else
                    {
                        jobsViewModel.EstimatedHrsPerUser =
                            jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                    }
                    if (jobsViewModel.EstimatedHrsPerUser < 1)
                    {
                        ModelState.AddModelError("EstimatedHrsPerUser",
                            "Minimum estimated hour per person must be 1 hour!");
                        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        return Json(new { status = "failure", errors = errCollection });
                    }
                    if (jobsViewModel.ActionStatus > 0)
                    {
                        if (jobsViewModel.ActionStatus == Constant.ActionStatus.Completed)
                        {
                            jobsViewModel.Status = Constant.JobStatus.Completed;
                        }
                        if (jobsViewModel.ActionStatus == Constant.ActionStatus.Cancel)
                        {
                            jobsViewModel.Status = Constant.JobStatus.Booked;
                        }
                        if (jobsViewModel.ActionStatus == Constant.ActionStatus.InComplete)
                        {
                            jobsViewModel.Status = Constant.JobStatus.Assigned;
                        }
                    }

                    CommonMapper<JobsViewModel, Jobs> mapper = new CommonMapper<JobsViewModel, Jobs>();
                    Jobs jobs = mapper.Mapper(jobsViewModel);
                    if (customerSite.Contracted.HasValue)
                    {
                        jobs.ContractDueDate = getContractDueDate(customerSite.Contracted, jobsViewModel);
                    }
                    int prvJobType = Convert.ToInt32(jobsViewModel.CurrentJobtype);
                    int jobTypeChange = Convert.ToInt32(jobsViewModel.JobType);
                    // if (prvJobType != jobTypeChange && jobsViewModel.JobType != Constant.JobType.Quote && jobsViewModel.ChangeJobType == true)
                    if (prvJobType != jobTypeChange && jobsViewModel.ChangeJobType == true)
                    {
                        jobs.Id = Guid.NewGuid();
                        Guid existJobId = jobsViewModel.Id;

                        jobsViewModel.Id = jobs.Id;
                        jobs.CreatedDate = DateTime.Now;
                        jobs.CreatedBy = Guid.Parse(base.GetUserId);

                        JobRepository.Add(jobs);
                        JobRepository.Save();

                        AddNewJobDocuments(existJobId, jobs.Id);   //save job documents 

                        if (jobsViewModel.JobType == Constant.JobType.Do && Convert.ToInt32(jobsViewModel.CurrentJobtype) == Convert.ToInt32(Constant.JobType.Quote))
                        {
                            var isInvoiceExist = InvoiceRepo.FindBy(i => i.EmployeeJobId == existJobId && i.JobId == jobsViewModel.JobNo).ToList();
                            if (isInvoiceExist.Count > 0)
                            {
                                var invoice = isInvoiceExist.FirstOrDefault();
                                var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == invoice.Id).ToList();
                                var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == existJobId && i.InvoiceId == invoice.Id).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == existJobId).ToList().OrderBy(i => i.OrderNo);
                                // var invoicejclitemmapping = invoiceJCLItemRepo.FindBy(i => i.JobID == existJobId && i.InvoiceId == invoice.Id).ToList();
                                invoice.EmployeeJobId = jobs.Id;
                                invoice.InvoiceType = "Invoice";
                                invoice.InvoiceDate = jobs.DateBooked;
                                InvoiceRepo.Save();

                                foreach (var item in InvoiceJCLItemlist)
                                {
                                    item.JobID = jobs.Id;
                                    invoiceJCLItemRepo.Save();
                                }
                            }

                            // update job id in purchase order issue pointed out by badoni on 05 march 2018
                            PurchaseOrderByJobviewmodel purchaseOrderByJobviewmodel = new PurchaseOrderByJobviewmodel();
                            var purchaseorder = JobPurchaseOrder.FindBy(m => m.JobID == existJobId && m.IsDelete == false).ToList();
                            if (purchaseorder.Count() > 0)
                            {
                                foreach (var item in purchaseorder)
                                {
                                    item.JobID = jobs.Id;
                                    JobPurchaseOrder.Edit(item);
                                    JobPurchaseOrder.Save();
                                }
                                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                log.Info(base.GetUserName + "job id updated in purchaseorder");
                            }
                        }

                        //operation notes save in contact log
                        if (jobs.OperationNotes != null && jobs.OperationNotes != "")
                        {
                            var getUserName = EmployeeRepo.FindBy(m => m.EmployeeId == jobs.CreatedBy).Select(m => m.UserName).FirstOrDefault();
                            string operationNotes = "<b>Operation Notes Added by:</b>" + getUserName + " On " + DateTime.Now + "<br/>" + jobs.OperationNotes;

                            SaveDataContactLog(jobs.Id, operationNotes);
                        }
                    }
                    else
                    {
                        //get previous operation notes
                        string previousOperationNotes = JobRepository.FindBy(m => m.Id == jobs.Id).Select(m => m.OperationNotes).FirstOrDefault();

                        jobs.ModifiedDate = DateTime.Now;
                        jobs.ModifiedBy = Guid.Parse(base.GetUserId);

                        JobRepository.DeAttach(jobs);
                        JobRepository.Edit(jobs);
                        JobRepository.Save();

                        //operation notes save in contact log
                        if (jobs.OperationNotes != "" && previousOperationNotes != jobs.OperationNotes)
                        {
                            var getUserName = EmployeeRepo.FindBy(m => m.EmployeeId == jobs.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                            string operationNotes = "<b>Operation Notes Added by:</b>" + getUserName + " On " + DateTime.Now + "<br/>" + jobs.OperationNotes;

                            SaveDataContactLog(jobs.Id, operationNotes);
                        }


                        //convert Quote to invoice
                        var invoiceData = InvoiceRepo.FindBy(m => m.JobId == jobs.JobNo && (m.IsDelete == false || m.IsDelete == null)).FirstOrDefault();
                        if (invoiceData != null)
                        {
                            if (invoiceData.InvoiceType == "Quote" && jobs.JobType != 1)
                            {
                                invoiceData.InvoiceType = "Invoice";
                                invoiceData.InvoiceDate = jobs.DateBooked;
                                invoiceData.ModifiedDate = DateTime.Now;
                                invoiceData.ModifiedBy = Guid.Parse(base.GetUserId);

                                InvoiceRepo.Edit(invoiceData);
                                InvoiceRepo.Save();

                            }
                            else if (invoiceData.InvoiceType == "Invoice" && jobs.JobType == 1)
                            {
                                invoiceData.InvoiceType = "Quote";
                                invoiceData.InvoiceDate = jobs.DateBooked;
                                invoiceData.ModifiedDate = DateTime.Now;
                                invoiceData.ModifiedBy = Guid.Parse(base.GetUserId);

                                InvoiceRepo.Edit(invoiceData);
                                InvoiceRepo.Save();
                            }
                        }
                    }

                    // Delete task from JobTaskMapping
                    var compulsaryTask = JobTaskMappingRepo.FindBy(m => m.JobId == jobsViewModel.Id).ToList();
                    if (compulsaryTask != null)
                    {
                        foreach (var task in compulsaryTask)
                        {
                            JobTaskMappingRepo.Delete(task);
                            JobTaskMappingRepo.Save();
                        }
                    }

                    // Save Task In JobTaskMapping
                    if (jobsViewModel.TaskId != null)
                    {
                        foreach (var compulsary in jobsViewModel.TaskId)
                        {
                            JobTaskMapping jobtaskmapping = new JobTaskMapping();
                            jobtaskmapping.Id = Guid.NewGuid();
                            jobtaskmapping.JobId = jobsViewModel.Id;
                            jobtaskmapping.CompulsaryId = compulsary.Value;
                            jobtaskmapping.CreatedDate = DateTime.Now;
                            jobtaskmapping.CreatedBy = Guid.Parse(base.GetUserId);
                            jobtaskmapping.ModifiedBy = Guid.Parse(base.GetUserId);
                            jobtaskmapping.ModifiedDate = DateTime.Now;

                            JobTaskMappingRepo.Add(jobtaskmapping);
                            JobTaskMappingRepo.Save();
                        }
                    }
                    // saving supoort job
                    SaveSupportJob(jobsViewModel);

                    if ((jobsViewModel.JobType == Constant.JobType.CheckMeasure ||
                         jobsViewModel.JobType == Constant.JobType.CallBack)
                        && (jobsViewModel.CurrentJobtype == "6" || jobsViewModel.CurrentJobtype == "1" ||
                            jobsViewModel.CurrentJobtype == "2"))
                    {
                        var jobType = jobsViewModel.JobType; //No need delete
                    }
                    else if ((jobsViewModel.JobType == Constant.JobType.Do) && (jobsViewModel.CurrentJobtype == "7"))
                    {
                        var jobType = jobsViewModel.JobType; //No need delete
                    }
                    //else if (jobsViewModel.JobType == Constant.JobType.CheckMeasure ||
                    //         jobsViewModel.JobType == Constant.JobType.CallBack)
                    //{
                    //    var jobAssignmapper = JobAssignMapping.GetAll()
                    //        .Where(m => m.JobId == jobsViewModel.Id && m.IsDelete == false)
                    //        .OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                    //    if (jobAssignmapper != null)
                    //    {
                    //        jobAssignmapper.IsDelete = true;
                    //        jobAssignmapper.ModifiedDate = DateTime.Now;
                    //        jobAssignmapper.ModifiedBy = Guid.Parse(base.GetUserId);

                    //        JobAssignMapping.Edit(jobAssignmapper);
                    //        JobAssignMapping.Save();
                    //    }
                    //}
                    else
                    {
                        //Delete Assign job
                        var jobAssignEntity = JobAssignMapping
                            .FindBy(m => m.JobId == jobsViewModel.Id && m.IsDelete == false).ToList();
                        //If job id exist then record delete
                        if (jobAssignEntity != null)
                        {
                            foreach (var assign in jobAssignEntity)
                            {
                                assign.IsDelete = true;
                                assign.ModifiedDate = DateTime.Now;
                                assign.ModifiedBy = Guid.Parse(base.GetUserId);

                                JobAssignMapping.Edit(assign);
                                JobAssignMapping.Save();
                            }
                        }
                    }

                    if (assignMappingList != null)
                    {
                        foreach (var assign in assignMappingList)
                        {
                            //saving assignto into jobassigntomapping
                            if (jobsViewModel.Status != Constant.JobStatus.Created &&
                                jobsViewModel.Status != Constant.JobStatus.NotCreated &&
                                jobsViewModel.Status != Constant.JobStatus.NotBooked &&
                                jobsViewModel.Status != Constant.JobStatus.NotAssigned)
                            {
                                JobAssignToMappingViewModel jobAssignViewModel = new JobAssignToMappingViewModel();

                                if (jobsViewModel.ActionStatus != Constant.ActionStatus.Cancel)
                                {
                                    double jobTimeDiff = 0;

                                    DateTime dt = assign.StartTime.Value;
                                    TimeSpan startTime = new TimeSpan(dt.Hour, dt.Minute, dt.Second);
                                    DateTime Start_Time;
                                    if (assign.DateBooked == null)
                                    {
                                        Start_Time = Convert.ToDateTime(jobsViewModel.DateBooked + startTime);
                                    }
                                    else
                                    {
                                        Start_Time = Convert.ToDateTime(assign.DateBooked + startTime);
                                    }
                                    var dtEndTime = DateTime.ParseExact("08:00:00 PM",
                                        "h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                                    TimeSpan endTime = new TimeSpan(dtEndTime.Hour, dtEndTime.Minute, dtEndTime.Second);
                                    DateTime Cal_EndTime;
                                    if (assign.DateBooked == null)
                                    {
                                        Cal_EndTime = Convert.ToDateTime(jobsViewModel.DateBooked + endTime);
                                    }
                                    else
                                    {
                                        Cal_EndTime = Convert.ToDateTime(assign.DateBooked + endTime);
                                    }
                                    TimeSpan DiffTime = Cal_EndTime.Subtract(Start_Time);
                                    double Hours = DiffTime.Hours;
                                    jobsViewModel.StartTime = Start_Time;
                                    var PerPersonAssignHours =
                                        jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                                    if (PerPersonAssignHours > Hours)
                                    {
                                        jobsViewModel.EndTime = Cal_EndTime;
                                    }
                                    else
                                    {
                                        jobsViewModel.EndTime = Start_Time.AddHours(Convert.ToDouble(PerPersonAssignHours));
                                    }

                                    //var hoursLeft = ((double)(jobsViewModel.EstimatedHrsPerUser) -
                                    //                 jobsViewModel.EndTime.Value.Subtract(jobsViewModel.StartTime.Value)
                                    //                     .Hours);
                                    TimeSpan span = jobsViewModel.EndTime.Value.Subtract(jobsViewModel.StartTime.Value);
                                    double hoursLeft = ((span.Hours + span.Minutes / 60.0) - (double)(jobsViewModel.EstimatedHrsPerUser));
                                    jobAssignViewModel.Id = Guid.NewGuid();
                                    jobAssignViewModel.IsDelete = false;

                                    // Set previous status for job assign  

                                    var assignMapping = JobAssignMapping.FindBy(m => m.AssignTo == assign.AssignTo && m.JobId == jobsViewModel.Id && m.IsDelete == true).OrderByDescending(m => m.CreatedDate).FirstOrDefault();
                                    if (assignMapping != null)
                                    {
                                        if (assignMapping.Status != null)
                                        {
                                            Constant.JobStatus jobstatus = (Constant.JobStatus)assignMapping.Status;
                                            jobAssignViewModel.Status = jobstatus;
                                        }
                                    }
                                    jobAssignViewModel.OTRWNotes = assign.OTRWNotes;
                                    jobAssignViewModel.JobId = jobsViewModel.Id;
                                    jobAssignViewModel.AssignTo = assign.AssignTo;
                                    if (assign.DateBooked != null)
                                    {
                                        jobAssignViewModel.DateBooked = assign.DateBooked;
                                    }
                                    else
                                    {
                                        jobAssignViewModel.DateBooked = jobsViewModel.DateBooked;
                                    }
                                    jobAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                                    jobAssignViewModel.CreatedDate = DateTime.Now;

                                    jobAssignViewModel.StartTime = jobsViewModel.StartTime;
                                    jobAssignViewModel.EndTime = jobsViewModel.EndTime;

                                    jobAssignViewModel.Status = jobsViewModel.Status;


                                    jobAssignViewModel.EstimatedHrsPerUser =
                                        jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);

                                    var jobAssignDateBooked = jobsViewModel.DateBooked;
                                    if (assign.DateBooked != null)
                                    {
                                        jobAssignDateBooked = assign.DateBooked;
                                    }
                                    var jobAssignedStartDate = jobsViewModel.StartTime;
                                    var jobAssignedEndDate = jobsViewModel.EndTime;

                                    CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper1 =
                                        new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                                    JobAssignToMapping jobAssignToMapping1 = Assignmapper1.Mapper(jobAssignViewModel);

                                    JobAssignMapping.Add(jobAssignToMapping1);
                                    JobAssignMapping.Save();

                                    while (hoursLeft > 0)
                                    {
                                        TimeSpan jobTime = new TimeSpan(0, 0, 0);
                                        TimeSpan jobStartTime = new TimeSpan(9, 0, 0);
                                        TimeSpan jobEndTime = new TimeSpan(19, 0, 0);
                                        jobAssignDateBooked = jobAssignDateBooked.Value.AddDays(1);

                                        // check for saturday & sunday
                                        var jobbookDay = jobAssignDateBooked.Value.ToString("dddd");
                                        if (jobbookDay == "Friday")
                                        {
                                            jobAssignDateBooked = jobAssignDateBooked.Value.AddDays(3);
                                        }
                                        else if (jobbookDay == "Saturday")
                                        {
                                            jobAssignDateBooked = jobAssignDateBooked.Value.AddDays(2);
                                        }

                                        // if otrw has job already booked
                                        var otrwHasJob =
                                            JobRepository.OTRWHasJobBooked(assign.AssignTo, jobAssignDateBooked);
                                        while (otrwHasJob)
                                        {
                                            jobAssignDateBooked = jobAssignDateBooked.Value.AddDays(1);

                                            // check for saturday & sunday
                                            jobbookDay = jobAssignDateBooked.Value.ToString("dddd");
                                            if (jobbookDay == "Friday")
                                            {
                                                jobAssignDateBooked = jobAssignDateBooked.Value.AddDays(3);
                                            }
                                            else if (jobbookDay == "Saturday")
                                            {
                                                jobAssignDateBooked = jobAssignDateBooked.Value.AddDays(2);
                                            }

                                            otrwHasJob =
                                                JobRepository.OTRWHasJobBooked(assign.AssignTo, jobAssignDateBooked);
                                        }



                                        jobAssignDateBooked = jobAssignDateBooked + jobTime;
                                        jobAssignedStartDate = jobAssignDateBooked + jobStartTime;
                                        if (hoursLeft < 10)
                                        {
                                            jobAssignedEndDate = jobAssignedStartDate.Value.AddHours(hoursLeft);
                                        }
                                        else
                                        {
                                            jobAssignedEndDate = jobAssignDateBooked + jobEndTime;
                                        }
                                        jobTimeDiff = (jobEndTime - jobStartTime).Hours;
                                        hoursLeft = hoursLeft - jobTimeDiff;

                                        // saving jobassign mapping
                                        jobAssignViewModel.Id = Guid.NewGuid();
                                        jobAssignViewModel.IsDelete = false;
                                        jobAssignViewModel.JobId = jobsViewModel.Id;
                                        jobAssignViewModel.AssignTo = assign.AssignTo;
                                        jobAssignViewModel.Status = Constant.JobStatus.Assigned;
                                        jobAssignViewModel.DateBooked = jobAssignDateBooked;
                                        jobAssignViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                                        jobAssignViewModel.CreatedDate = DateTime.Now;
                                        jobAssignViewModel.StartTime = jobAssignedStartDate;
                                        jobAssignViewModel.EndTime = jobAssignedEndDate;
                                        jobAssignViewModel.EstimatedHrsPerUser =
                                            jobsViewModel.EstimatedHours / Convert.ToDouble(jobsViewModel.OTRWRequired);
                                        CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper =
                                            new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                                        JobAssignToMapping jobAssignToMapping = Assignmapper.Mapper(jobAssignViewModel);
                                        JobAssignMapping.Add(jobAssignToMapping);
                                        JobAssignMapping.Save();
                                    }

                                    //helper.SaveAndSendNotification(Guid.Parse(base.GetUserId), (Guid)jobsViewModel.AssignTo2, jobsViewModel.Id, "Job");
                                }
                            }
                        }
                    }
                    //}
                    //Send Confirmation Mail
                    if (jobsViewModel.SendJobEmail == Constant.SendJobEmail.SendConfirmationEmail || jobsViewModel.ReSendJobEmail == Constant.ReSendJobEmail.ResendConfirmationEmail)
                    {
                        Guid customerId = Guid.Parse(jobsViewModel.CustomerInfoId);
                        Guid siteDetailId = Guid.Parse(jobsViewModel.tempSiteId);

                        var customerDetail = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == customerId).FirstOrDefault();
                        var siteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                        StreamReader reader;
                        if (siteDetail.StrataPlan == null)
                        {
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ConfirmationAppointmentDomesticCustomer.htm"));
                        }
                        else
                        {
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/ConfirmationAppointmentStrataRealestate.htm"));
                        }

                        string readFile = reader.ReadToEnd();
                        string myString = readFile;
                        myString = myString.Replace("{{ClientName}}", customerDetail.CustomerLastName);
                        myString = myString.Replace("{{SiteAdress}}", siteDetail.SiteFileName);
                        myString = myString.Replace("{{DateBooked}}", jobsViewModel.DateBooked.ToString());

                        string body = Convert.ToString(myString);

                        using (MailMessage mm = new MailMessage())
                        {
                            mm.IsBodyHtml = true;
                            mm.Body = body;
                            mm.Subject = "Job Confirmation " + jobsViewModel.JobNo;
                            mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                            var TOId = Customercontacts.FindBy(m => m.CustomerGeneralInfoId == customerId).Select(m => m.EmailId).ToList();
                            if (TOId.Count > 0)
                            {
                                foreach (string ToEmail in TOId)
                                {
                                    if (ToEmail != null)
                                    {
                                        mm.To.Add(new MailAddress(ToEmail)); //Adding Multiple To email Id
                                        using (SmtpClient smtp = new SmtpClient())
                                        {
                                            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                            smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                            smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                            smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                            smtp.Send(mm);
                                        }
                                    }
                                }

                            }
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated a job.");

                    return Json(new { status = "saved", SavedId = jobsViewModel.Id.ToString(), msg = "<strong>Record updated successfully !</strong>" });
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }

        private void SaveSupportJob(JobsViewModel jobsViewModel)
        {
            try
            {
                Guid linkJobId = !string.IsNullOrEmpty(jobsViewModel.LinkJobId) ? Guid.Parse(jobsViewModel.LinkJobId) : Guid.Empty;
                var supportdojobMapping = SupportdojobRepo.FindBy(m => m.SupportJobId == jobsViewModel.Id).FirstOrDefault();

                if (linkJobId != Guid.Empty)
                {
                    if (supportdojobMapping != null)
                    {
                        supportdojobMapping.LinkedJobId = Guid.Parse(jobsViewModel.LinkJobId);
                        supportdojobMapping.ModifiedBy = Guid.Parse(base.GetUserId);
                        supportdojobMapping.ModifiedDate = DateTime.Now;

                        SupportdojobRepo.Edit(supportdojobMapping);
                        SupportdojobRepo.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " added support job.");
                    }
                    else
                    {
                        SupportdojobMapping supportdojobEntity = new SupportdojobMapping();
                        supportdojobEntity.ID = Guid.NewGuid();
                        supportdojobEntity.SupportJobId = jobsViewModel.Id;

                        supportdojobEntity.LinkedJobId = linkJobId;
                        supportdojobEntity.CreatedBy = Guid.Parse(base.GetUserId);
                        supportdojobEntity.CreatedDate = DateTime.Now;

                        SupportdojobRepo.Add(supportdojobEntity);
                        SupportdojobRepo.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated support job.");
                    }
                }
                else
                {
                    if (supportdojobMapping != null)
                    {
                        SupportdojobRepo.Delete(supportdojobMapping);
                        SupportdojobRepo.Save();
                    }
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added support job.");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SupportdojobRepo.Dispose();
            }
        }

        public void SaveJobDocs(List<HttpPostedFileBase> JobDocs, Guid Id)
        {
            try
            {
                for (int i = 0; i < JobDocs.Count(); i++)
                {
                    var File = JobDocs[i];
                    if (File != null && File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(File.FileName);
                        string Jobid = Id.ToString();
                        string DateTimeForDoc = DateTime.Now.ToString("MM-dd-yyyy-hh.mm.ss.ffffff");
                        Directory.CreateDirectory(Server.MapPath("~/Images/EmployeeJobs/" + Jobid));
                        File.SaveAs(Path.Combine(Server.MapPath("~/Images/EmployeeJobs/" + Jobid), DateTimeForDoc + "_" + fileName));

                        EmployeeJobDocumentViewModel employeeJobDocumentViewModel = new EmployeeJobDocumentViewModel();
                        employeeJobDocumentViewModel.Id = Guid.NewGuid();
                        employeeJobDocumentViewModel.JobId = Id;
                        employeeJobDocumentViewModel.DocName = fileName.ToString();
                        employeeJobDocumentViewModel.SaveDocName = DateTimeForDoc + "_" + fileName.ToString();
                        employeeJobDocumentViewModel.CreatedDate = DateTime.Now;
                        employeeJobDocumentViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                        CommonMapper<EmployeeJobDocumentViewModel, JobDocuments> mapperdoc = new CommonMapper<EmployeeJobDocumentViewModel, JobDocuments>();
                        JobDocuments employeeJobDocuments = mapperdoc.Mapper(employeeJobDocumentViewModel);
                        EmployeeJobDoc.Add(employeeJobDocuments);
                        EmployeeJobDoc.Save();
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved job document.");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EmployeeJobDoc.Dispose();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult SiteListByCustomer()
        {
            try
            {
                ModelState.Clear();

                JobsViewModel jobsViewModel = new JobsViewModel();
                Guid CustomerGeneralInfoId = !string.IsNullOrEmpty(Request.QueryString["CustomerInfoId"]) ?
                                              Guid.Parse(Request.QueryString["CustomerInfoId"]) : Guid.Empty;

                var SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId && (m.IsDelete == false)).
                         Select(m => new SelectListItem()
                         {
                             Text = m.SiteFileName,
                             Value = m.SiteDetailId.ToString()
                         }).ToList();

                jobsViewModel.SiteList = SiteList;

                return PartialView("_SiteList", jobsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerSiteDetailRepo.Dispose();
            }
        }

        [HttpGet]
        public ActionResult UpdateCustomerDetails(string id, Guid JobId)
        {
            try
            {
                using (CustomerGeneralInfoRepo)
                {
                    Guid CustomerGeneralInfoId;
                    if (!string.IsNullOrEmpty(id))
                    {
                        Guid.TryParse(id, out CustomerGeneralInfoId);
                    }
                    else
                    {
                        int jobId = (JobRepository.GetMaxJobID() - 1);
                        var jobs = JobRepository.FindBy(m => m.JobId == jobId).FirstOrDefault();
                        CustomerGeneralInfoId = jobs.CustomerGeneralInfoId;
                    }
                    CustomerGeneralInfo customerGeneralInfo = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId)
                                                              .FirstOrDefault();
                    var userName = "";
                    if (customerGeneralInfo.ModifiedBy == null)
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerGeneralInfo.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerGeneralInfo.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (customerGeneralInfo.ModifiedDate == null)
                    {
                        customerGeneralInfo.CreatedDate = customerGeneralInfo.CreatedDate;
                    }
                    else
                    {
                        customerGeneralInfo.ModifiedDate = customerGeneralInfo.ModifiedDate;
                    }
                    // mapping entity to viewmodel
                    CommonMapper<CustomerGeneralInfo, CustomerGeneralInfoViewModel> mapper = new CommonMapper<CustomerGeneralInfo, CustomerGeneralInfoViewModel>();
                    CustomerGeneralInfoViewModel customerGeneralInfoViewModel = mapper.Mapper(customerGeneralInfo);
                    customerGeneralInfoViewModel.UserName = userName;
                    ViewBag.JobId = JobId;

                    return View(customerGeneralInfoViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult UpdateCustomerDetails(CustomerGeneralInfoViewModel customerGeneralInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (CustomerGeneralInfoRepo)
                    {
                        customerGeneralInfoViewModel.ModifiedDate = DateTime.Now;
                        customerGeneralInfoViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                        // mapping viewmodel to entity
                        CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo>();
                        CustomerGeneralInfo customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);

                        CustomerGeneralInfoRepo.Edit(customerGeneralInfo);
                        CustomerGeneralInfoRepo.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated customer details.");

                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                    }
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult EditCustomerSiteDetail(string id)
        {
            id = "6A3F9EA3-A65C-4D67-A85E-11E01583BC15";
            Guid SiteDetailId;
            Guid.TryParse(id, out SiteDetailId);
            var userName = "";
            try
            {
                using (CustomerSiteDetailRepo)
                {
                    CustomerSiteDetail customerSiteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == SiteDetailId).FirstOrDefault();
                    if (customerSiteDetail.ModifiedBy == null)
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerSiteDetail.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerSiteDetail.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (customerSiteDetail.ModifiedDate == null)
                    {
                        customerSiteDetail.CreatedDate = customerSiteDetail.CreatedDate;
                    }
                    else
                    {
                        customerSiteDetail.ModifiedDate = customerSiteDetail.ModifiedDate;
                    }
                    // mapping entity to viewmodel
                    CommonMapper<CustomerSiteDetail, Customer.ViewModels.CustomerSiteDetailViewModel> mapper = new CommonMapper<CustomerSiteDetail, Customer.ViewModels.CustomerSiteDetailViewModel>();
                    Customer.ViewModels.CustomerSiteDetailViewModel customerSiteDetailViewModel = mapper.Mapper(customerSiteDetail);

                    return View(customerSiteDetailViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EditCustomerSiteDetail(Customer.ViewModels.CustomerSiteDetailViewModel customerSiteDetailViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (CustomerSiteDetailRepo)
                    {
                        // mapping viewmodel to entity
                        CommonMapper<Customer.ViewModels.CustomerSiteDetailViewModel, CustomerSiteDetail> mapper = new CommonMapper<Customer.ViewModels.CustomerSiteDetailViewModel, CustomerSiteDetail>();
                        CustomerSiteDetail customerSiteDetail = mapper.Mapper(customerSiteDetailViewModel);

                        CustomerSiteDetailRepo.Edit(customerSiteDetail);
                        CustomerSiteDetailRepo.Save();
                        return Json(new { id = customerSiteDetailViewModel.CustomerGeneralInfoId.ToString(), activetab = "Site Details" });
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " updated customer site details.");

                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception)
            {
                throw;
            }
        }


        //GET:Manage Customer Sites
        /// <summary>
        /// Manage Customer Sites Using SiteDetailId
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult UpdateCustomerSiteDetail(string id)
        {
            try
            {
                CustomerSitesViewModel customerSitesViewModel = new CustomerSitesViewModel();
                Nullable<Guid> siteDetailId;
                if (!string.IsNullOrEmpty(id))
                {
                    siteDetailId = Guid.Parse(id);
                }
                else
                {
                    int jobId = (JobRepository.GetMaxJobID() - 1);
                    var jobs = JobRepository.FindBy(m => m.JobId == jobId).FirstOrDefault();
                    siteDetailId = jobs.SiteId;
                }

                CustomerSiteDetail customerSiteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel> mapsitedetail = new CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel>();
                CustomerSiteDetailViewModel customerSiteDetailViewModel = mapsitedetail.Mapper(customerSiteDetail);
                var userName = "";
                if (customerSiteDetailViewModel.ModifiedBy == null)
                {
                    userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerSiteDetailViewModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                }
                else
                {
                    userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerSiteDetailViewModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                }
                if (customerSiteDetailViewModel.ModifiedDate == null)
                {
                    customerSiteDetailViewModel.CreatedDate = customerSiteDetailViewModel.CreatedDate;
                }
                else
                {
                    customerSiteDetailViewModel.ModifiedDate = customerSiteDetailViewModel.ModifiedDate;
                }
                CustomerResidenceDetail customerResidenceDetail = CustomerResidence.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                if (customerResidenceDetail != null)
                {
                    CommonMapper<CustomerResidenceDetail, CustomerResidenceDetailViewModel> mapresidencedetail = new CommonMapper<CustomerResidenceDetail, CustomerResidenceDetailViewModel>();
                    CustomerResidenceDetailViewModel customerResidenceDetailViewModel = mapresidencedetail.Mapper(customerResidenceDetail);
                    customerSitesViewModel.CustomerResidenceDetailViewModel = customerResidenceDetailViewModel;
                    customerSitesViewModel.ResidenceDetailId = customerResidenceDetail.ResidenceDetailId;
                }

                CustomerConditionReport customerConditionReport = ConditionReport.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                if (customerConditionReport != null)
                {
                    if (customerConditionReport.ConditionRoof == null)
                    {
                        customerConditionReport.ConditionRoof = 0;
                    }
                    CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel> mapconditiondetail = new CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel>();
                    CustomerConditionReportViewModel customerConditionReportViewModel = mapconditiondetail.Mapper(customerConditionReport);
                    customerSitesViewModel.CustomerConditionReportViewModel = customerConditionReportViewModel;
                    customerSitesViewModel.ConditionReportId = customerConditionReport.ConditionReportId;
                }
                customerSitesViewModel.CustomerSiteDetailViewModel = customerSiteDetailViewModel;
                customerSitesViewModel.UserName = userName;
                customerSitesViewModel.ModifiedDate = customerSiteDetailViewModel.ModifiedDate;
                customerSitesViewModel.CustomerGeneralInfoId = customerSiteDetailViewModel.CustomerGeneralInfoId;
                customerSitesViewModel.SiteDetailId = (Guid)siteDetailId;


                // binding drodownlist
                customerSitesViewModel.ContactList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId).Select(m =>
                    new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.ContactList.OrderBy(m => m.Text);

                //binding Strata manager drodownlist
                customerSitesViewModel.StrataManagerList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId && m.IsStrataManager == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.StrataManagerList.OrderBy(m => m.Text);

                //binding Real Estate drodownlist
                customerSitesViewModel.RealEstateList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId && m.IsRealEstate == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.RealEstateList.OrderBy(m => m.Text);

                ViewBag.JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? Guid.Parse(Request.QueryString["JobId"]) : Guid.Empty;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed customer sites using Site Detail Id.");

                return View("_CustomerSiteForm", customerSitesViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //GET:Save Customer 
        /// <summary>
        /// Save List Of Customer
        /// </summary>
        /// <param name="customerSitesViewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveCustomerList(CustomerSitesViewModel customerSitesViewModel)
        {
            try
            {
                //Guid ContactId;
                //Guid.TryParse(customerSitesViewModel.CustomerSiteDetailViewModel.ContactId, out ContactId);

                if (ModelState.IsValid)
                {
                    //if ((int)customerSitesViewModel.CustomerResidenceDetailViewModel.Height >= 4)
                    //{
                    //    if ((int)customerSitesViewModel.CustomerResidenceDetailViewModel.SRASinstalled == 0)
                    //    {
                    //        ModelState.AddModelError("SRASError", @"For residential height 4 or more SRAS installed
                    //                                  shouldn't be none !");
                    //        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    //        return Json(new { status = "failure", errors = errCollection });
                    //    }
                    //}
                    //if (customerSitesViewModel.CustomerResidenceDetailViewModel.TypeOfResidence == null)
                    //{
                    //    ModelState.AddModelError("TypeOfResidence", @"Residence type is required  !");
                    //    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    //    return Json(new { status = "failure", errors = errCollection });
                    //}

                    //if (customerSitesViewModel.CustomerSiteDetailViewModel.Contracted != 0 && customerSitesViewModel.CustomerSiteDetailViewModel.Contracted != null)
                    //{
                    //    if (customerSitesViewModel.CustomerSiteDetailViewModel.ScheduledPrice == null)
                    //    {
                    //        ModelState.AddModelError("ScheduledPrice", "Scheduled Price is required!");
                    //        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    //        return Json(new { status = "failure", errors = errCollection });
                    //    }
                    //}
                    Customer.ViewModels.CustomerSiteDetailViewModel customerSiteDetailViewModel = customerSitesViewModel.CustomerSiteDetailViewModel;
                    customerSiteDetailViewModel.SiteDetailId = customerSitesViewModel.SiteDetailId;
                    customerSiteDetailViewModel.ModifiedDate = DateTime.Now;
                    customerSiteDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    CustomerResidenceDetailViewModel customerResidenceDetailViewModel = customerSitesViewModel.CustomerResidenceDetailViewModel;
                    customerResidenceDetailViewModel.ResidenceDetailId = customerSitesViewModel.ResidenceDetailId;
                    customerResidenceDetailViewModel.ModifiedDate = DateTime.Now;
                    customerResidenceDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    CustomerConditionReportViewModel customerConditionReportViewModel = customerSitesViewModel.CustomerConditionReportViewModel;
                    customerConditionReportViewModel.ConditionReportId = customerSitesViewModel.ConditionReportId;
                    customerConditionReportViewModel.ModifiedDate = DateTime.Now;
                    customerConditionReportViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    // saving sitedetail info
                    using (CustomerSiteDetailRepo)
                    {
                        CommonMapper<Customer.ViewModels.CustomerSiteDetailViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<Customer.ViewModels.CustomerSiteDetailViewModel, CustomerSiteDetail>();
                        CustomerSiteDetail customerSiteDetail = mapsitedetail.Mapper(customerSiteDetailViewModel);
                        customerSiteDetail.CustomerGeneralInfoId = customerSitesViewModel.CustomerGeneralInfoId;
                        //customerSiteDetail.ContactId = ContactId;

                        // getting lattitude and longitude
                        string address = customerSiteDetailViewModel.Street + " " + customerSiteDetailViewModel.StreetName + " " +
                                        customerSiteDetailViewModel.Suburb + " " + customerSiteDetailViewModel.State + " " + customerSiteDetailViewModel.PostalCode;
                        string[] arrLatLong = { };

                        for (int Index = 0; Index < 4; Index++)
                        {
                            arrLatLong = GetLatitudeLongitude(address);
                            if (arrLatLong[2] == "OVER_QUERY_LIMIT")
                            {
                                System.Threading.Thread.Sleep(2000);
                                if (Index < 3)
                                {
                                    continue;
                                }
                                else
                                {
                                    ModelState.AddModelError("SRASError", @"Query limit exceeded for google map. Please try again on tommorow.");
                                    return Json(ModelState.Values.SelectMany(m => m.Errors));
                                }
                            }
                            break;
                        }

                        if (arrLatLong[0] == "" || arrLatLong[1] == "" || arrLatLong[0] == "0" || arrLatLong[0] == "0")
                        {
                            ModelState.AddModelError("SRASError", @"Site address is not correct!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }

                        customerSiteDetail.Latitude = !string.IsNullOrEmpty(arrLatLong[0]) ? double.Parse(arrLatLong[0]) : (double?)null;
                        customerSiteDetail.Longitude = !string.IsNullOrEmpty(arrLatLong[1]) ? double.Parse(arrLatLong[1]) : (double?)null;

                        CustomerSiteDetailRepo.Edit(customerSiteDetail);
                        CustomerSiteDetailRepo.Save();
                    }

                    // saving residencedetail info
                    using (CustomerResidence)
                    {
                        CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                        CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(customerResidenceDetailViewModel);
                        customerResidenceDetail.SiteDetailId = customerSiteDetailViewModel.SiteDetailId;
                        CustomerResidence.Edit(customerResidenceDetail);
                        CustomerResidence.Save();
                    }

                    // saving conditionreport info
                    using (ConditionReport)
                    {
                        CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport> mapper = new CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport>();
                        CustomerConditionReport customerConditionReport = mapper.Mapper(customerConditionReportViewModel);
                        customerConditionReport.SiteDetailId = customerSiteDetailViewModel.SiteDetailId;
                        ConditionReport.Edit(customerConditionReport);
                        ConditionReport.Save();
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " saved customer list.");

                    return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                }
                else
                {
                    var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                    return Json(new { status = "failure", errors = errCollection });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string[] GetLatitudeLongitude(string address)
        {
            string url = "http://maps.googleapis.com/maps/api/geocode/json?address=" + address + "&sensor=false";
            string[] arrLatLong = new string[3];
            string result = string.Empty; //Get geocode response 
            WebClient Client = new WebClient();
            using (Stream strm = Client.OpenRead(url))
            {
                StreamReader sr = new StreamReader(strm);
                result = sr.ReadToEnd();
            }
            //Deserialize into .Net object 
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MyGeoCodeResponse _MyGeoCodeResponse = ser.Deserialize<MyGeoCodeResponse>(result);
            string _latitude = _MyGeoCodeResponse.results.Length > 0 ? _MyGeoCodeResponse.results[0].geometry.location.lat : string.Empty;
            string _longitude = _MyGeoCodeResponse.results.Length > 0 ? _MyGeoCodeResponse.results[0].geometry.location.lng : string.Empty;
            arrLatLong[0] = _latitude;
            arrLatLong[1] = _longitude;
            arrLatLong[2] = _MyGeoCodeResponse.status;
            return arrLatLong;
        }

        //GET: Customer/Customer/EditBillingAddress
        /// <summary>
        /// Edit BillingAddress of Customer
        /// </summary>
        /// <param name="BillingAddressId"></param>
        /// <returns></returns>
        public ActionResult UpdateBillingAddress(string id, string customerid)
        {
            try
            {
                CustomerBillingAddressViewModel model = new CustomerBillingAddressViewModel();

                using (CustomerBilling)
                {
                    Guid custbilId = Guid.Empty;
                    CustomerBillingAddress CustomerBillingAdderss = null;

                    ViewBag.JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? Guid.Parse(Request.QueryString["JobId"]) : Guid.Empty;

                    Guid customerGeneralInfoId = Guid.Parse(customerid);
                    model.CustomerGeneralInfoId = customerGeneralInfoId;
                    var contacts = Customercontacts.FindBy(m => m.IsBillingContact == true && m.CustomerGeneralInfoId ==
                                   customerGeneralInfoId).FirstOrDefault();

                    if (!string.IsNullOrEmpty(id))
                    {
                        custbilId = Guid.Parse(id);


                        CustomerBillingAdderss = CustomerBilling.FindBy(i => i.BillingAddressId == custbilId).FirstOrDefault();
                    }
                    else
                    {
                        var DefaultBillingaddress = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                        if (DefaultBillingaddress == null)
                        {

                            CustomerBillingAdderss = CustomerBilling.GetAll().Where(i => i.CustomerGeneralInfoId == customerGeneralInfoId).OrderByDescending(i => i.CreatedDate).FirstOrDefault();
                        }
                        else
                        {
                            CustomerBillingAdderss = DefaultBillingaddress;
                        }
                    }

                    if (CustomerBillingAdderss != null)
                    {
                        //mapping entity to viewmodel
                        CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel> mapper = new CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel>();
                        model = mapper.Mapper(CustomerBillingAdderss);
                        var userName = "";
                        if (CustomerBillingAdderss.ModifiedBy == null)
                        {
                            userName = EmployeeRepo.FindBy(m => m.EmployeeId == CustomerBillingAdderss.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                        }
                        else
                        {
                            userName = EmployeeRepo.FindBy(m => m.EmployeeId == CustomerBillingAdderss.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                        }
                        if (CustomerBillingAdderss.ModifiedDate == null)
                        {
                            CustomerBillingAdderss.CreatedDate = CustomerBillingAdderss.CreatedDate;
                        }
                        else
                        {
                            CustomerBillingAdderss.ModifiedDate = CustomerBillingAdderss.ModifiedDate;
                        }
                        model.UserName = userName;
                        model.ModifiedDate = CustomerBillingAdderss.ModifiedDate;
                        model.CreatedDate = CustomerBillingAdderss.CreatedDate;
                        return View(model);
                    }

                    if (contacts != null)
                    {
                        model.CustomerTitle = contacts.Title != null ? (Constant.Title)contacts.Title : 0;
                        model.FirstName = contacts.FirstName;
                        model.LastName = contacts.LastName;
                        model.PhoneNo1 = contacts.PhoneNo1;
                        model.PhoneNo2 = contacts.PhoneNo2;
                        model.PhoneNo3 = contacts.PhoneNo3;
                        model.Spare1 = contacts.Spare1;
                        model.Spare2 = contacts.Spare2;
                        model.EmailId = contacts.EmailId;
                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Customer/Customer/EditBillingAddress
        /// <summary>
        /// Edit Billing Address of customer
        /// </summary>
        /// <param name="CustomerBillingAddressModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateBillingAddress(CustomerBillingAddressViewModel customerBillingAddressModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerBillingAddressModel.PO == false)
                    {
                        //if (customerBillingAddressModel.Unit == null)
                        //{
                        //    ModelState.AddModelError("Unit", "Unit is required!");
                        //}
                        //if (customerBillingAddressModel.StreetNo == null)
                        //{
                        //    ModelState.AddModelError("StreetNo", "Street Number is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.StreetName == null)
                        //{
                        //    ModelState.AddModelError("StreetName", "Street Name is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.StreetType == null)
                        //{
                        //    ModelState.AddModelError("StreetType", "Street Type is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.Suburb == null)
                        //{
                        //    ModelState.AddModelError("Suburb", "Suburb is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.State == null)
                        //{
                        //    ModelState.AddModelError("State", "State is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.PostalCode == null)
                        //{
                        //    ModelState.AddModelError("PostalCode", "Postal Code is required!");
                        //   return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}

                    }
                    else
                    {
                        //if (customerBillingAddressModel.POAddress == null)
                        //{
                        //    ModelState.AddModelError("PoAddress", "PO Address is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}

                        //if (customerBillingAddressModel.Suburb == null)
                        //{
                        //    ModelState.AddModelError("Suburb", "Suburb is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.State == null)
                        //{
                        //    ModelState.AddModelError("State", "State is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingAddressModel.PostalCode == null)
                        //{
                        //    ModelState.AddModelError("PostalCode", "Postal Code is required!");
                        //   return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //return Json(ModelState.Values.SelectMany(m => m.Errors));
                    }
                    using (CustomerBilling)
                    {
                        if (customerBillingAddressModel.IsDefault)
                        {
                            CustomerBilling.UpdateDefaultAddress(customerBillingAddressModel.CustomerGeneralInfoId);
                        }
                        var CustomerBillingAdderss = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId ==
                                                     customerBillingAddressModel.CustomerGeneralInfoId).Select(m => m.BillingAddressId)
                                                     .FirstOrDefault();

                        //mapping viewmodel to entity
                        CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress> mapper = new CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress>();
                        CustomerBillingAddress customerBillingAdderss = mapper.Mapper(customerBillingAddressModel);


                        //CustomerBilling.DeleteState(customerBillingAdderss);

                        if (CustomerBillingAdderss == null || CustomerBillingAdderss == Guid.Empty)
                        {
                            customerBillingAdderss.BillingAddressId = Guid.NewGuid();
                            customerBillingAdderss.CreatedDate = DateTime.Now;
                            customerBillingAdderss.CreatedBy = Guid.Parse(base.GetUserId);
                            CustomerBilling.Add(customerBillingAdderss);
                        }
                        else
                        {
                            customerBillingAdderss.ModifiedDate = DateTime.Now;
                            customerBillingAdderss.ModifiedBy = Guid.Parse(base.GetUserId);
                            CustomerBilling.DeAttach(customerBillingAdderss);
                            customerBillingAdderss.BillingAddressId = CustomerBillingAdderss;
                            CustomerBilling.DeleteState(customerBillingAdderss);
                            CustomerBilling.DeAttach(customerBillingAdderss);
                            CustomerBilling.Edit(customerBillingAdderss);
                        }
                        CustomerBilling.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated billing address.");

                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                    }
                }
                var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                return Json(new { status = "failure", errors = errCollection });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Add New Customer Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="customercontactid"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public ActionResult UpdateSiteContact(string id)
        {
            try
            {
                CustomerContactsViewModel model = new CustomerContactsViewModel();
                using (Customercontacts)
                {
                    Nullable<Guid> custContactid;
                    if (!string.IsNullOrEmpty(id))
                    {
                        custContactid = Guid.Parse(id);
                    }
                    else
                    {
                        int jobId = (JobRepository.GetMaxJobID() - 1);
                        var jobs = JobRepository.FindBy(m => m.JobId == jobId).FirstOrDefault();
                        var siteId = jobs != null ? jobs.SiteId : Guid.Empty;

                        var custometSiteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == siteId).FirstOrDefault();
                        custContactid = custometSiteDetail != null ? custometSiteDetail.ContactId : Guid.Empty;

                        model.SiteId = siteId.ToString();
                        model.CustomerGeneralInfoId = jobs.CustomerGeneralInfoId;

                    }

                    CustomerContacts cotactlog = Customercontacts.FindBy(i => i.ContactId == custContactid).FirstOrDefault();

                    if (cotactlog != null)
                    {
                        CommonMapper<CustomerContacts, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContacts, CustomerContactsViewModel>();
                        model = mapper.Mapper(cotactlog);

                        model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.SiteDetailId == cotactlog.SiteId).Select(m =>
                        new SelectListItem { Text = m.StreetName, Value = m.SiteDetailId.ToString() }).ToList();
                        model.SiteList.OrderBy(m => m.Text);
                    }


                }

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Post: Add New Customer Contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects AddCustomerInfo</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateSiteContact(CustomerContactsViewModel model)
        {
            try
            {
                using (Customercontacts)
                {
                    if (model.ContactId == Guid.Empty)
                    {
                        Guid SiteID = Guid.Parse(model.SiteId);
                        model.ContactId = Guid.NewGuid();
                        var custometSiteDetail = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == SiteID).FirstOrDefault();

                        if (custometSiteDetail.ContactId == null)    //Customer site detail add contact id if null
                        {
                            custometSiteDetail.ContactId = model.ContactId;
                        }

                        //mapping site detail entity to model
                        CommonMapper<CustomerSiteDetail, Customer.ViewModels.CustomerSiteDetailViewModel> mapp = new CommonMapper<CustomerSiteDetail, Customer.ViewModels.CustomerSiteDetailViewModel>();
                        Customer.ViewModels.CustomerSiteDetailViewModel customerSiteDetailModel = mapp.Mapper(custometSiteDetail);

                        //mapping model to site detail entity
                        CommonMapper<Customer.ViewModels.CustomerSiteDetailViewModel, CustomerSiteDetail> mappers = new CommonMapper<Customer.ViewModels.CustomerSiteDetailViewModel, CustomerSiteDetail>();
                        CustomerSiteDetail customerSiteDetailEntity = mappers.Mapper(customerSiteDetailModel);

                        //Update site Detail
                        CustomerSiteDetailRepo.DeAttach(customerSiteDetailEntity);
                        CustomerSiteDetailRepo.Save();

                        //mapping contact view model to entity
                        model.CreatedBy = Guid.Parse(base.GetUserId);
                        model.CreatedDate = DateTime.Now;
                        CommonMapper<CustomerContactsViewModel, CustomerContacts> mapper1 = new CommonMapper<CustomerContactsViewModel, CustomerContacts>();
                        CustomerContacts customercontactEntity = mapper1.Mapper(model);

                        Customercontacts.Add(customercontactEntity);
                        Customercontacts.Save();

                        return Json(new { status = "saved", msg = "<strong>Record saved successfully !</strong>" });
                    }
                    Guid custGeneralinfoid = model.CustomerGeneralInfoId;
                    var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();

                    CommonMapper<CustomerContactsViewModel, CustomerContacts> mapper = new CommonMapper<CustomerContactsViewModel, CustomerContacts>();
                    if (ModelState.IsValid)
                    {
                        if (model.IsBillingContact == true)
                        {
                            foreach (var i in CustomerContactsList)
                            {
                                CustomerContacts contact = Customercontacts.FindBy(m => m.ContactId == i.ContactId).FirstOrDefault();
                                contact.IsBillingContact = false;
                                Customercontacts.Edit(contact);
                                Customercontacts.Save();
                            }
                        }
                        if (model.ContactId != Guid.Empty)
                        {
                            model.ModifiedDate = DateTime.Now;
                            model.ModifiedBy = Guid.Parse(base.GetUserId);
                            CustomerContacts customerContact = Customercontacts.FindBy(m => m.ContactId == model.ContactId).FirstOrDefault();
                            Customercontacts.DeAttach(customerContact);
                            customerContact = mapper.Mapper(model);
                            Customercontacts.Edit(customerContact);
                            Customercontacts.Save();
                        }

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated site contact.");

                        return Json(new { status = "saved", msg = "<strong>Record updated successfully !</strong>" });
                    }
                    else
                    {
                        var errCollection = ModelState.Values.SelectMany(m => m.Errors);
                        return Json(new { status = "failure", errors = errCollection });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult JobDocuments()
        {
            try
            {
                var jobId = Request.RequestContext.RouteData.Values["id"] != null ?
                        Guid.Parse(Request.RequestContext.RouteData.Values["id"].ToString()) : Guid.Empty;
                var jobDocList = JobDocumentRepo.FindBy(m => m.JobId == jobId && m.IsDelete == false).ToList();

                CommonMapper<JobDocuments, JobDocumentList> mapper = new CommonMapper<JobDocuments, JobDocumentList>();
                var jobdocs = mapper.MapToList(jobDocList); // job docs list

                JobDocViewModel jobDocViewModel = new JobDocViewModel();
                jobDocViewModel.JobId = jobId;
                jobDocViewModel.jobDocumentList = jobdocs;

                return View(jobDocViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SaveJobDocument(JobDocViewModel jobDocViewModel)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Images/JobDocuments"));
                if (!di.Exists)
                {
                    // create directory
                    Directory.CreateDirectory(Server.MapPath("~/Images/JobDocuments"));

                    // create sub directory
                    di.CreateSubdirectory(jobDocViewModel.JobId.ToString());
                }
                else
                {
                    // create sub directory
                    di.CreateSubdirectory(jobDocViewModel.JobId.ToString());
                }

                // saving doc to server
                Guid attchGuid = Guid.NewGuid();
                string jobDoc = Path.GetFileNameWithoutExtension(jobDocViewModel.JobDocs[0].FileName);
                string extension = Path.GetExtension(jobDocViewModel.JobDocs[0].FileName).ToLower();
                var filepath = Path.Combine(Server.MapPath("~/Images/JobDocuments/" + jobDocViewModel.JobId),
                                  jobDoc + "_" + attchGuid + extension);
                jobDocViewModel.JobDocs[0].SaveAs(filepath);

                // saving jobdocument entity
                JobDocuments jobDocuments = new JobDocuments();
                jobDocuments.Id = Guid.NewGuid();
                jobDocuments.JobId = jobDocViewModel.JobId;
                jobDocuments.DocName = System.IO.Path.GetFileName(jobDocViewModel.JobDocs[0].FileName);
                jobDocuments.DocType = GetDocumentType(extension);
                jobDocuments.SaveDocName = jobDoc + "_" + attchGuid + extension;
                jobDocuments.IsDelete = false;
                jobDocuments.CreatedDate = DateTime.Now;
                jobDocuments.CreatedBy = Guid.Parse(base.GetUserId);

                JobDocumentRepo.Add(jobDocuments);
                JobDocumentRepo.Save();

                // adding job docs list
                var jobDocList = JobDocumentRepo.FindBy(m => m.JobId == jobDocViewModel.JobId && m.IsDelete == false).ToList();
                CommonMapper<JobDocuments, JobDocumentList> mapper = new CommonMapper<JobDocuments, JobDocumentList>();
                var jobdocs = mapper.MapToList(jobDocList);
                jobDocViewModel.jobDocumentList = jobdocs;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved job document.");

                return PartialView("_JobDocumentList", jobDocViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Job Convert then exist job document saving with new job id
        private void AddNewJobDocuments(Guid existJobId, Guid newJobId)
        {
            try
            {
                var jobDocList = JobDocumentRepo.FindBy(m => m.JobId == existJobId && m.IsDelete == false).ToList();



                foreach (var docs in jobDocList)
                {
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Images/JobDocuments"));
                    if (!di.Exists)
                    {
                        // create directory
                        Directory.CreateDirectory(Server.MapPath("~/Images/JobDocuments"));

                        // create sub directory
                        di.CreateSubdirectory(newJobId.ToString());
                    }
                    else
                    {
                        // create sub directory
                        di.CreateSubdirectory(newJobId.ToString());
                    }

                    // saving doc to server
                    Guid attchGuid = Guid.NewGuid();

                    HttpPostedFileBase fileBase = Request.Files[docs.DocName];

                    string jobDoc = Path.GetFileNameWithoutExtension(docs.DocName);
                    string extension = Path.GetExtension(docs.DocName).ToLower();
                    var filepath = Path.Combine(Server.MapPath("~/Images/JobDocuments/" + newJobId),
                                      jobDoc + "_" + attchGuid + extension);
                    string oldpath = Path.Combine(Server.MapPath("~/Images/JobDocuments/" + existJobId), docs.SaveDocName);

                    if (System.IO.File.Exists(oldpath))
                    {
                        System.IO.File.Copy(oldpath, filepath);

                        // saving jobdocument entity
                        JobDocuments jobDocuments = new JobDocuments();
                        jobDocuments.Id = Guid.NewGuid();
                        jobDocuments.JobId = newJobId;
                        jobDocuments.DocName = System.IO.Path.GetFileName(docs.DocName);
                        jobDocuments.DocType = GetDocumentType(extension);
                        jobDocuments.SaveDocName = jobDoc + "_" + attchGuid + extension;
                        jobDocuments.IsDelete = false;
                        jobDocuments.CreatedDate = DateTime.Now;
                        jobDocuments.CreatedBy = Guid.Parse(base.GetUserId);

                        JobDocumentRepo.Add(jobDocuments);
                        JobDocumentRepo.Save();
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved job document.");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public ActionResult DeleteJobDocument()
        {
            JobDocViewModel jobDocViewModel = new JobDocViewModel();

            var jobdocId = Request.QueryString["id"] != null ? Guid.Parse(Request.QueryString["id"]) : Guid.Empty;
            var jobId = Request.QueryString["jobid"] != null ? Guid.Parse(Request.QueryString["jobid"]) : Guid.Empty;

            var jobDoc = JobDocumentRepo.FindBy(m => m.Id == jobdocId).FirstOrDefault();

            // deleting from folder
            var hasFile = System.IO.File.Exists(Server.MapPath("~/Images/JobDocuments/" + jobId + "/" + jobDoc.SaveDocName));
            if (hasFile)
            {
                System.IO.File.Delete(Server.MapPath("~/Images/JobDocuments/" + jobId + "/" + jobDoc.SaveDocName));
            }

            // deleting from database
            jobDoc.IsDelete = true;
            JobDocumentRepo.Edit(jobDoc);
            JobDocumentRepo.Save();

            // adding job docs list
            var jobDocList = JobDocumentRepo.FindBy(m => m.JobId == jobId && m.IsDelete == false).ToList();
            CommonMapper<JobDocuments, JobDocumentList> mapper = new CommonMapper<JobDocuments, JobDocumentList>();
            var jobdocs = mapper.MapToList(jobDocList);
            jobDocViewModel.JobId = jobId;
            jobDocViewModel.jobDocumentList = jobdocs;

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " deleted job document.");

            return PartialView("_JobDocumentList", jobDocViewModel);
        }
        public ActionResult QuickSiteView()
        {
            var siteDetailId = Request.QueryString["id"] != null ? Guid.Parse(Request.QueryString["id"]) : Guid.Empty;
            var siteDocList = CustomerSitesDocumentsRepo.FindBy(m => m.SiteId == siteDetailId && m.IsDelete == false).ToList();

            CommonMapper<CustomerSitesDocuments, CustomerSitesDocuments> mapper = new CommonMapper<CustomerSitesDocuments, CustomerSitesDocuments>();
            var sitedocs = mapper.MapToList(siteDocList); // job docs list

            CustomerSiteDocumentsListViewModel siteDocViewModel = new CustomerSiteDocumentsListViewModel();
            //siteDocViewModel.SiteId = siteDetailId;
            siteDocViewModel.CustomerSiteDocumentsViewModelList = sitedocs;

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " quick site view.");

            return PartialView("_QuickSiteView", siteDocViewModel);

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

        public ActionResult QuickView()
        {
            var jobId = Request.QueryString["id"] != null ? Guid.Parse(Request.QueryString["id"]) : Guid.Empty;
            var jobDocList = JobDocumentRepo.FindBy(m => m.JobId == jobId && m.IsDelete == false).ToList();

            CommonMapper<JobDocuments, JobDocumentList> mapper = new CommonMapper<JobDocuments, JobDocumentList>();
            var jobdocs = mapper.MapToList(jobDocList); // job docs list

            JobDocViewModel jobDocViewModel = new JobDocViewModel();
            jobDocViewModel.JobId = jobId;
            jobDocViewModel.jobDocumentList = jobdocs;

            return PartialView("_QuickView", jobDocViewModel);
        }

        [HttpGet]
        public ActionResult AddJobDocuments(string id)
        {
            try
            {
                var keyWordSearch = "";
                Guid Jobid = Guid.Parse(id);
                var jobSiteDetaillist = JobRepository.FindBy(m => m.Id == Jobid).FirstOrDefault();
                string siteId = (jobSiteDetaillist.SiteId).ToString();
                var customerSiteDetaillist = CustomerSitesDocumentsRepo.GetSiteCountForJob(Guid.Parse(siteId)).ToList();
                var customerSiteDocumentsist = CustomerSitesDocumentsRepo.JobsSiteDocumentList(keyWordSearch, Guid.Parse(siteId)).ToList();

                CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                List<Customer.ViewModels.SiteDetail> li = new List<Customer.ViewModels.SiteDetail>();
                foreach (var i in customerSiteDetaillist)
                {
                    Customer.ViewModels.SiteDetail obj = new Customer.ViewModels.SiteDetail();
                    obj.SiteDetailId = i.SiteDetailId;
                    obj.SiteAddress = i.SiteAddress;
                    li.Add(obj);
                }
                customersitemodel.siteDetail = li;

                var customerSiteDocumentsListViewModel = new CustomerSiteDocumentsListViewModel
                {
                    CustomerSiteDocumentsCoreViewModelList = customerSiteDocumentsist,
                    SiteCountviewModel = customersitemodel,
                    CustomerSiteDocuments = new CustomerSitesDocuments()
                };
                ViewBag.JobId = Jobid;
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.SiteId = Guid.Parse(siteId);
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.CustomerGeneralInfoId = jobSiteDetaillist.CustomerGeneralInfoId;
                return View(customerSiteDocumentsListViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult AddJobDocuments(CustomerSiteDocumentsListViewModel model, IEnumerable<HttpPostedFileBase> filename)
        {
            if (model != null)
            {
                try
                {
                    CustomerSitesDocumentsViewModel customerSiteDocumentsViewModel = new CustomerSitesDocumentsViewModel();

                    if (ModelState.IsValid)
                    {
                        using (CustomerSitesDocumentsRepo)
                        {
                            customerSiteDocumentsViewModel.IsDelete = false;
                            customerSiteDocumentsViewModel.CreatedDate = DateTime.Now;
                            customerSiteDocumentsViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                            for (int i = 0; i < filename.Count(); i++)
                            {
                                var File = Request.Files[i];
                                if (File != null && File.ContentLength > 0)
                                {
                                    customerSiteDocumentsViewModel.DocumentId = Guid.NewGuid();
                                    var fileName = Path.GetFileName(File.FileName);
                                    string extension = Path.GetExtension(fileName).ToLower();
                                    string docId = customerSiteDocumentsViewModel.DocumentId.ToString();
                                    Directory.CreateDirectory(Server.MapPath("~/Images/CustomerDocs/" + docId));
                                    File.SaveAs(Path.Combine(Server.MapPath("~/Images/CustomerDocs/" + docId), fileName));

                                    customerSiteDocumentsViewModel.SiteId = model.SiteCountviewModel.SiteDetailId;
                                    customerSiteDocumentsViewModel.CustomerGeneralInfoId = model.CustomerSiteDocuments.CustomerGeneralInfoId;
                                    customerSiteDocumentsViewModel.DocumentName = fileName.ToString();
                                    customerSiteDocumentsViewModel.DocType = GetDocumentType(extension);
                                    CommonMapper<CustomerSitesDocumentsViewModel, CustomerSitesDocuments> mapperdoc = new CommonMapper<CustomerSitesDocumentsViewModel, CustomerSitesDocuments>();
                                    CustomerSitesDocuments customerSiteDocuments = mapperdoc.Mapper(customerSiteDocumentsViewModel);
                                    CustomerSitesDocumentsRepo.Add(customerSiteDocuments);
                                    CustomerSitesDocumentsRepo.Save();
                                }
                            }

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added new job document.");

                            return RedirectToAction("AddDocumentsPartial", "CustomerJob", new { siteID = model.CustomerSiteDocuments.SiteId });
                            //return RedirectToAction("SaveJobInfo", new { id = model.CustomerSiteDocuments.CustomerGeneralInfoId.ToString(), activetab = "Documents", success = "ok" });
                        }
                    }
                    else
                    {
                        return View(model);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View(model);
        }

        public ActionResult AddDocumentsPartial(string siteID)
        {
            try
            {
                var keyWordSearch = "";
                Guid siteId = Guid.Parse(siteID);
                var customerSiteDetaillist = CustomerSitesDocumentsRepo.GetSiteCountForJob(siteId).ToList();
                var customerSiteDocumentsist = CustomerSitesDocumentsRepo.JobsSiteDocumentList(keyWordSearch, siteId).ToList();

                CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                List<Customer.ViewModels.SiteDetail> li = new List<Customer.ViewModels.SiteDetail>();
                foreach (var i in customerSiteDetaillist)
                {
                    Customer.ViewModels.SiteDetail obj = new Customer.ViewModels.SiteDetail();
                    obj.SiteDetailId = i.SiteDetailId;
                    obj.SiteAddress = i.SiteAddress;
                    li.Add(obj);
                }
                customersitemodel.siteDetail = li;

                var customerSiteDocumentsListViewModel = new CustomerSiteDocumentsListViewModel
                {
                    CustomerSiteDocumentsCoreViewModelList = customerSiteDocumentsist,
                    SiteCountviewModel = customersitemodel,
                    CustomerSiteDocuments = new CustomerSitesDocuments()
                };
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.SiteId = siteId;
                return PartialView("_JobSiteDocumentList", customerSiteDocumentsListViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CustomerSiteDocumentsList()
        {
            CustomerSiteDocumentsListViewModel customerSitesViewModel = new CustomerSiteDocumentsListViewModel();
            string Name = Request.QueryString["Name"];
            string custgeninfoid = Request.QueryString["siteInfoId"];
            Guid Id = Guid.Parse(custgeninfoid);
            var CustomerSiteList = CustomerSitesDocumentsRepo.JobsSiteDocumentList(Name, Id).ToList();


            // mapping list<entity> to list<viewmodel>
            CommonMapper<FSM.Core.ViewModels.CustomerSitesDocumentsViewModelCore, CustomerSitesDocumentsViewModelCore> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerSitesDocumentsViewModelCore, CustomerSitesDocumentsViewModelCore>();
            List<CustomerSitesDocumentsViewModelCore> customerSiteList = mapper.MapToList(CustomerSiteList.ToList());

            customerSitesViewModel.CustomerSiteDocumentsCoreViewModelList = customerSiteList;
            //customerSitesViewModel.PageSize = Request.QueryString["page_size"] == null ? 2 : Convert.ToInt32(Request.QueryString["page_size"]);

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed customer site document list.");

            return PartialView("_JobSiteDocumentList", customerSitesViewModel);
        }
        public ActionResult DeleteJobSiteDocuments(string SiteDetailId, string DocumentId, string PageNum)
        {
            try
            {
                using (CustomerSitesDocumentsRepo)
                {
                    Guid sitedetailid = Guid.Parse(SiteDetailId);
                    Guid docId = Guid.Parse(DocumentId);
                    List<CustomerSitesDocuments> docdelete = CustomerSitesDocumentsRepo.FindBy(i => i.DocumentId == docId).ToList();
                    foreach (CustomerSitesDocuments Doc in docdelete)
                    {
                        Doc.IsDelete = true;
                        CustomerSitesDocumentsRepo.Edit(Doc);
                        CustomerSitesDocumentsRepo.Save();
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted job site document.");

                    return RedirectToAction("AddDocumentsPartial", "CustomerJob", new { siteID = sitedetailid });
                    // return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult DownloadFile(string DocumentId)
        {
            try
            {
                Guid docId = Guid.Parse(DocumentId);
                var ImageName = CustomerSitesDocumentsRepo.FindBy(i => i.DocumentId == docId).FirstOrDefault();
                var FileVirtualPath = "/Images/CustomerDocs/" + docId + '/' + ImageName.DocumentName;
                //byte[] fileBytes = System.IO.File.ReadAllBytes(FileVirtualPath);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download file.");

                return Json(FileVirtualPath, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult _ViewSitesDocuments(string Sitedetailid, string PageNum)
        {
            try
            {
                Guid siteDetailId = Guid.Parse(Sitedetailid);
                using (CustomerSitesDocumentsRepo)
                {
                    var docs = CustomerSitesDocumentsRepo.FindBy(i => i.SiteId == siteDetailId).ToList();
                    CommonMapper<CustomerSitesDocuments, CustomerSitesDocumentsViewModel> mapper = new CommonMapper<CustomerSitesDocuments, CustomerSitesDocumentsViewModel>();
                    List<CustomerSitesDocumentsViewModel> customerGeneralInfoViewModel = mapper.MapToList(docs.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(customerGeneralInfoViewModel);
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed site document.");

                    return Json(new { list = json, length = customerGeneralInfoViewModel.Count() });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult ViewCustomerContactLog(string id)
        {
            try
            {
                using (CustomercontactLogRepo)
                {
                    Guid jobGeneralinfoid = Guid.Parse(id);
                    Guid Customercontactid = Guid.Empty;
                    int? jobNo = JobRepository.FindBy(m => m.Id == jobGeneralinfoid).Select(m => m.JobNo).FirstOrDefault();
                    //var CustomerContactList = CustomercontactLog.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid);
                    var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, jobGeneralinfoid.ToString()).ToList();
                    CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
                    List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.OrderByDescending(m => m.LogDate).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]); ; // default page size;
                    var customerContractListViewModel = new CustomerContractListViewModel
                    {
                        CustomerContactList = CustomerContactListing.ToList(),
                        PageSize = PageSize,
                        CustomerContactLog = new CustomerContactLog()
                    };
                    customerContractListViewModel.CustomerContactLog.JobId = jobGeneralinfoid.ToString();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed customer contact log.");

                    return View(customerContractListViewModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult ViewCustomerContactLogPartial(string JobId, string Keyword)
        {
            try
            {
                using (CustomercontactLogRepo)
                {
                    Guid jobGeneralinfoid = Guid.Parse(JobId);
                    Guid Customercontactid = Guid.Empty;
                    int? jobNo = JobRepository.FindBy(m => m.Id == jobGeneralinfoid).Select(m => m.JobNo).FirstOrDefault();
                    var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, jobGeneralinfoid.ToString(), Keyword).ToList();
                    CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
                    List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.OrderByDescending(m => m.LogDate).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                              Convert.ToInt32(Request.QueryString["page_size"]);

                    var customerContractListViewModel = new CustomerContractListViewModel
                    {
                        CustomerContactList = CustomerContactListing.ToList(),
                        PageSize = PageSize,
                        CustomerContactLog = new CustomerContactLog()
                    };
                    customerContractListViewModel.CustomerContactLog.JobId = jobGeneralinfoid.ToString();

                    return PartialView("_JobContactLogList", customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomercontactLogRepo.Dispose();
            }
        }

        //POST:  /Customer/Customer/CustomerContactLogSearch
        /// <summary>
        /// Search Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="Keyword"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContactLogSearch(string JobGeneralinfoid, string Keyword, string PageNum)
        {
            try
            {
                using ((Customercontacts))
                {
                    if (Keyword == null)
                        Keyword = "";

                    Guid Customerid = Guid.Parse(JobGeneralinfoid);
                    int? jobNo = JobRepository.FindBy(m => m.Id == Customerid).Select(m => m.JobNo).FirstOrDefault();
                    var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, JobGeneralinfoid, Keyword).ToList();
                    CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
                    List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.OrderByDescending(m => m.LogDate).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);
                    var customerContractListViewModel = new CustomerContractListViewModel
                    {
                        CustomerContactList = CustomerContactListing.ToList(),
                        PageSize = PageSize,
                        CustomerContactLog = new CustomerContactLog()
                    };
                    customerContractListViewModel.CustomerContactLog.JobId = JobGeneralinfoid;

                    return PartialView("_JobContactLogList", customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult JobContactLogList()
        {
            string jobGeneralinfoid = Request.QueryString["JobId"];
            Guid jobGeneralId = Guid.Parse(jobGeneralinfoid);
            int? jobNo = JobRepository.FindBy(m => m.Id == jobGeneralId).Select(m => m.JobNo).FirstOrDefault();
            var CustomerContactList = CustomercontactLogRepo.GetJobscontactLogs(jobNo, jobGeneralinfoid.ToString()).ToList();

            // mapping list<entity> to list<viewmodel>
            CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerContactlogcore, CustomerContactLogViewModel>();
            List<CustomerContactLogViewModel> CustomerContactListing = mapper.MapToList(CustomerContactList.OrderByDescending(m => m.LogDate).ToList());

            var customerContractListViewModel = new CustomerContractListViewModel
            {
                CustomerContactList = CustomerContactListing.ToList(),
                CustomerContactLog = new CustomerContactLog()
            };

            customerContractListViewModel.CustomerContactLog.JobId = jobGeneralinfoid.ToString();
            customerContractListViewModel.PageSize = Request.QueryString["page_size"] == null ? 2 : Convert.ToInt32(Request.QueryString["page_size"]);

            return PartialView("_JobContactLogList", customerContractListViewModel);
        }
        public PartialViewResult _CustomercontactAddEdit(string JobId, string Customercontactid, string PageNum, string SiteName)
        {
            try
            {
                CustomerContactLogViewModel model = new CustomerContactLogViewModel();

                Guid jobGeneralInfoId = Guid.Parse(JobId);
                var employeejob = JobRepository.FindBy(i => i.Id == jobGeneralInfoId).FirstOrDefault();
                var Customerjobs = GetCustomerjobsByCustomerid(JobId);

                model.PageNum = PageNum;
                if (!string.IsNullOrEmpty(Customercontactid))
                {
                    using (CustomercontactLogRepo)
                    {
                        Guid custContactid = Guid.Parse(Customercontactid);
                        CustomerContactLog cotactlog = CustomercontactLogRepo.FindBy(i => i.CustomerContactId == custContactid).FirstOrDefault();

                        if (cotactlog != null)
                        {
                            //mapping of entity to viewmodel 
                            CommonMapper<CustomerContactLog, CustomerContactLogViewModel> mapper = new CommonMapper<CustomerContactLog, CustomerContactLogViewModel>();
                            model = mapper.Mapper(cotactlog);
                            model.Customerjobs = Customerjobs;
                            model.SiteName = SiteName;
                            if (cotactlog.LogDate == null)
                            {
                                model.LogDate = model.LogDate = DateTime.Now.Date;
                            }
                            return PartialView(model);
                        }
                        else
                        {
                            return PartialView(model);
                        }
                    }
                }
                else
                {
                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (CustomerGeneralInfoRepo)
                    {
                        Guid customerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                        CustomerGeneralinfo = CustomerGeneralInfoRepo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
                    }
                    Guid custContactid = Guid.Parse(JobId);
                    model = new CustomerContactLogViewModel();
                    model.Customerjobs = Customerjobs;
                    model.CustomerGeneralInfoId = employeejob.CustomerGeneralInfoId;
                    model.CustomerId = CustomerGeneralinfo.CTId.ToString();
                    model.SiteName = SiteName;
                    model.LogDate = model.LogDate = DateTime.Now.Date;
                    return PartialView(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<CustomerJobs> GetCustomerjobsByCustomerid(string Customerinfoid)
        {
            try
            {
                using (JobRepository)
                {
                    Guid Customerid = Guid.Parse(Customerinfoid);
                    List<CustomerJobs> joblist = new List<CustomerJobs>();
                    var jobs = JobRepository.FindBy(i => i.Id == Customerid).ToList();
                    foreach (var job in jobs)
                    {
                        CustomerJobs obj = new CustomerJobs();
                        obj.CustJobId = job.Id;
                        obj.Jobtext = job.JobNo.ToString();
                        joblist.Add(obj);
                    }
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed customer job customer id.");

                    return joblist;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult _CustomercontactAddEdit(CustomerContactLogViewModel model)
        {
            if (model != null)
            {
                try
                {
                    //mapping of viewmodel to entity
                    CommonMapper<CustomerContactLogViewModel, CustomerContactLog> mapper = new CommonMapper<CustomerContactLogViewModel, CustomerContactLog>();

                    if (ModelState.IsValid)
                    {
                        if (model.CustomerContactId != Guid.Empty)
                        {
                            model.ModifiedDate = DateTime.Now;
                            model.ModifiedBy = Guid.Parse(base.GetUserId);
                            CustomerContactLog customerContactLog = mapper.Mapper(model);
                            CustomercontactLogRepo.Edit(customerContactLog);
                            CustomercontactLogRepo.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " updated customer contact log.");

                            return RedirectToAction("ViewCustomerContactLogPartial", "CustomerJob", new { JobId = model.JobId });
                        }
                        else
                        {
                            model.IsDelete = false;
                            model.CreatedDate = DateTime.Now;
                            model.CreatedBy = Guid.Parse(base.GetUserId);
                            model.CustomerContactId = Guid.NewGuid();
                            CustomerContactLog customerContactLog = mapper.Mapper(model);
                            CustomercontactLogRepo.Add(customerContactLog);
                            CustomercontactLogRepo.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added customer contact log.");

                            return RedirectToAction("ViewCustomerContactLogPartial", "CustomerJob", new { JobId = model.JobId });
                        }
                    }
                    else
                    {
                        return PartialView(model);
                    }

                }
                catch (Exception)
                {
                    throw;
                }

            }
            return PartialView(model);
        }


        public ActionResult DeleteCustomerContactLog(string Customercontactid, string PageNum)
        {
            try
            {
                using (CustomercontactLogRepo)
                {
                    Guid contactid = Guid.Parse(Customercontactid);
                    CustomerContactLog logtodelete = CustomercontactLogRepo.FindBy(i => i.CustomerContactId == contactid).FirstOrDefault();
                    CustomercontactLogRepo.Delete(logtodelete);
                    CustomercontactLogRepo.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted customer contact log.");

                    return RedirectToAction("ViewCustomerContactLogPartial", "CustomerJob", new { JobId = logtodelete.JobId });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CustomerByJobId()
        {
            try
            {
                ModelState.Clear();

                Guid JobId = !string.IsNullOrEmpty(Request.QueryString["LinkJobId"]) ? Guid.Parse(Request.QueryString["LinkJobId"]) : Guid.Empty;

                var customer = JobRepository.FindBy(m => m.Id == JobId).Select(m => new
                {
                    m.CustomerGeneralInfoId,
                    m.CustomerGeneralInfo.CustomerLastName,
                    m.SiteId,
                    m.CustomerSiteDetail.StreetName
                }).FirstOrDefault();


                return Json(new { Customer = customer }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }

        public ActionResult GetJobDocuments()
        {
            try
            {
                int JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? int.Parse(Request.QueryString["JobId"]) : 0;
                var employeeJobDocs = EmployeeJobDoc.FindBy(m => m.Jobs.JobId == JobId).Select(m => new
                {
                    m.DocName,
                    m.SaveDocName,
                    m.Id
                }).ToList();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " gets job document.");

                return Json(new { JobDocs = employeeJobDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EmployeeJobDoc.Dispose();
            }
        }

        [HttpGet]
        public ActionResult DeletejobDocumentByDocId()
        {
            try
            {
                Guid DocId = !string.IsNullOrEmpty(Request.QueryString["DocId"]) ? Guid.Parse(Request.QueryString["DocId"]) : Guid.Empty;
                int JobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ? int.Parse(Request.QueryString["JobId"]) : 0;

                var docs = EmployeeJobDoc.FindBy(i => i.Id == DocId).FirstOrDefault();
                EmployeeJobDoc.Delete(docs);
                EmployeeJobDoc.Save();
                if ((System.IO.File.Exists(Server.MapPath("~/Images/EmployeeJobs/" + docs.JobId + '/' + docs.DocName))))
                {
                    System.IO.File.Delete(Server.MapPath("~/Images/EmployeeJobs/" + docs.JobId + '/' + docs.DocName));
                }

                var employeeJobDocs = EmployeeJobDoc.FindBy(m => m.Jobs.JobId == JobId).Select(m => new
                {
                    m.DocName,
                    m.SaveDocName,
                    m.Id
                }).ToList();
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted job document.");

                return Json(new { JobDocs = employeeJobDocs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EmployeeJobDoc.Dispose();
            }
        }

        public ActionResult DownloadDocuments(string Id)
        {
            try
            {
                Guid DocId = Guid.Parse(Id);

                var file = EmployeeJobDoc.FindBy(i => i.Id == DocId).FirstOrDefault();
                var FileVirtualPath = Server.MapPath("~/Images/EmployeeJobs/" + file.JobId + '/' + file.SaveDocName);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download document.");


                return File(FileVirtualPath, MimeMapping.GetMimeMapping(FileVirtualPath), file.DocName);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                EmployeeJobDoc.Dispose();
            }
        }

        [HttpGet]
        public ActionResult _AddNewCustomer()
        {
            CustomerGeneralInfoViewModel customerinfoviewmodel = new CustomerGeneralInfoViewModel();
            using (CustomerGeneralInfoRepo)
            {
                var CID = CustomerGeneralInfoRepo.GetMaxCID();
                if (CID != null)
                {
                    customerinfoviewmodel.CID = Convert.ToInt32(CID);
                }
                customerinfoviewmodel.CTId = CustomerGeneralInfoRepo.GetMaxCTId();
            }
            return PartialView(customerinfoviewmodel);
        }

        [HttpPost]
        public ActionResult _AddNewCustomer(string Cid, string CustLName, FSMConstant.Constant.CustomerType CustType)
        {
            try
            {
                using (CustomerGeneralInfoRepo)
                {
                    CustomerGeneralInfoViewModel customerGeneralInfoViewModel = new CustomerGeneralInfoViewModel();
                    customerGeneralInfoViewModel.CustomerGeneralInfoId = Guid.NewGuid();
                    customerGeneralInfoViewModel.CTId = CustomerGeneralInfoRepo.GetMaxCTId();
                    customerGeneralInfoViewModel.CustomerLastName = CustLName;
                    customerGeneralInfoViewModel.CustomerType = CustType;

                    customerGeneralInfoViewModel.IsDelete = false;
                    customerGeneralInfoViewModel.CreatedDate = DateTime.Now;
                    customerGeneralInfoViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    // mapping viewmodel to entity
                    CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo>();
                    CustomerGeneralInfo customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);

                    CustomerGeneralInfoRepo.Add(customerGeneralInfo);
                    CustomerGeneralInfoRepo.Save();

                    // get all customers
                    var customers = CustomerGeneralInfoRepo.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.CustomerLastName,
                        Value = m.CustomerGeneralInfoId.ToString()
                    }).ToList();

                    // serializing customer list
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var customerList = serializer.Serialize(customers);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added new customer.");

                    return new JsonResult()
                    {
                        Data = customerList,
                        MaxJsonLength = Int32.MaxValue
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet]
        public ActionResult _AddNewCustomerSite(Guid GeneralInfoId)
        {
            CustomerSiteJobViewModel customerSiteJobViewModel = new CustomerSiteJobViewModel();
            customerSiteJobViewModel.CustomerGeneralInfoId = GeneralInfoId;
            return PartialView(customerSiteJobViewModel);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult _AddNewCustomerSite(CustomerSiteJobViewModel model)
        {
            if (ModelState.IsValid)
            {

                model.SiteDetailId = Guid.NewGuid();
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = Guid.Parse(base.GetUserId);
                CommonMapper<CustomerSiteJobViewModel, CustomerSiteDetail> mapper = new CommonMapper<CustomerSiteJobViewModel, CustomerSiteDetail>();
                CustomerSiteDetail customerSiteInfo = mapper.Mapper(model);

                customerSiteInfo.SiteFileName = model.Unit + " " + model.Street + " " + model.StreetName + " " + model.Suburb + "," + model.State + " " + model.PostalCode;
                customerSiteInfo.Contracted = 0;
                customerSiteInfo.IsDelete = false;
                CustomerSiteDetailRepo.Add(customerSiteInfo);
                CustomerSiteDetailRepo.Save();

                CustomerResidenceDetailViewModel customerResidenceDetailViewModel = new CustomerResidenceDetailViewModel();
                customerResidenceDetailViewModel.ResidenceDetailId = Guid.NewGuid();
                customerResidenceDetailViewModel.SiteDetailId = model.SiteDetailId;
                customerResidenceDetailViewModel.SRASinstalled = 0;
                customerResidenceDetailViewModel.NeedTwoPPL = true;
                customerResidenceDetailViewModel.CreatedDate = DateTime.Now;
                customerResidenceDetailViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                customerResidenceDetailViewModel.IsDelete = false;
                customerResidenceDetailViewModel.CreatedDate = DateTime.Now;
                customerResidenceDetailViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                customerResidenceDetailViewModel.TypeOfResidence = 0;
                customerResidenceDetailViewModel.SRASinstalled = 0;

                CustomerConditionReportViewModel customerConditionReportViewModel = new CustomerConditionReportViewModel();
                customerConditionReportViewModel.ConditionReportId = Guid.NewGuid();
                customerConditionReportViewModel.SiteDetailId = model.SiteDetailId;

                customerConditionReportViewModel.CreatedDate = DateTime.Now;
                customerConditionReportViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                customerConditionReportViewModel.IsDelete = false;
                customerConditionReportViewModel.CreatedDate = DateTime.Now;
                customerConditionReportViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                using (CustomerResidence)
                {
                    CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                    CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(customerResidenceDetailViewModel);
                    CustomerResidence.Add(customerResidenceDetail);
                    CustomerResidence.Save();
                }
                // saving conditionreport info
                using (ConditionReport)
                {
                    CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport> mappercondition = new CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport>();
                    CustomerConditionReport customerConditionReport = mappercondition.Mapper(customerConditionReportViewModel);
                    customerConditionReport.ConditionOfRoof = 0;
                    ConditionReport.Add(customerConditionReport);
                    ConditionReport.Save();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added new customer site.");

                return PartialView(model);
            }
            //return Json(ModelState.Values.SelectMany(m => m.Errors));
            return Json(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(k => k.Errors)
                             .Select(m => m.ErrorMessage).ToArray()
            });
        }

        //GET: checking Site Two People Job
        /// <summary>
        /// Check Two people job
        /// </summary>
        /// <param name="SiteId"></param>
        /// <returns></returns>
        public ActionResult CheckSiteNeedTwoPeople(string SiteId)
        {
            try
            {
                using (JobRepository)
                {
                    int result = 0;
                    var SiteDetail = JobRepository.CheckTwoPeopleJob(SiteId).FirstOrDefault();
                    if (SiteDetail != null)
                    {
                        result = 1;
                        return Json(result = 1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = 0;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult BindOTRWWithWorkType()
        {
            try
            {
                ModelState.Clear();

                JobsViewModel jobsViewModel = new JobsViewModel();
                int WorkType = Convert.ToInt32(Request.QueryString["WorkType"]);

                var OTRWList = JobRepository.GetOTRWUserForWorkType(WorkType).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                {
                    Text = m.UserName,
                    Value = m.Id
                }).ToList();

                jobsViewModel.OTRWList = OTRWList;

                return Json(jobsViewModel.OTRWList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerSiteDetailRepo.Dispose();
            }
        }

        public ActionResult LinkJobIdByCustomer()
        {
            try
            {
                ModelState.Clear();

                JobsViewModel jobsViewModel = new JobsViewModel();
                Guid CustomerGeneralInfoId = !string.IsNullOrEmpty(Request.QueryString["CustomerInfoId"]) ?
                                              Guid.Parse(Request.QueryString["CustomerInfoId"]) : Guid.Empty;

                var linkJobList = JobRepository.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId && m.IsDelete == false).Select(m => new SelectListItem()
                {
                    Text = "Job_" + m.JobNo.ToString(),
                    Value = m.Id.ToString()
                }).OrderBy(m => m.Text).ToList();

                jobsViewModel.LinkJobList = linkJobList;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " linked job id by customer.");

                return Json(jobsViewModel.LinkJobList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CustomerSiteDetailRepo.Dispose();
            }
        }
        public ActionResult ViewJobContacts(string siteId, string custGeneralInfoId, string JobId)
        {
            try
            {
                using (Customercontacts)
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    string Searchstring = Request.QueryString["searchkeyword"];
                    int ContactType = 0;
                    Guid sitedetailId = Guid.Parse(siteId);
                    Guid custGeneralinfoid = Guid.Parse(custGeneralInfoId);
                    Guid Customercontactid = Guid.Empty;
                    //var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();
                    var CustomerContactsList = Customercontacts.GetJobContactsInfo(custGeneralinfoid, sitedetailId, ContactType, Searchstring).ToList();
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactsList.ToList());

                    List<CustomerContactsViewModel> customerSiteCollection =
                            customerContactsCoreViewModel.Select(m => new CustomerContactsViewModel
                            {

                                FirstName = m.FirstName,
                                LastName = m.LastName,
                                PhoneNo1 = m.PhoneNo1,
                                EmailId = m.EmailId,
                                SiteAddress = m.SiteAddress,
                                ContactId = m.ContactId,
                                CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                                DisplayContactsType = (int)m.ContactsType != 0 ? m.ContactsType.GetAttribute<DisplayAttribute>() != null ? m.ContactsType.GetAttribute<DisplayAttribute>().Name : m.ContactsType.ToString() : string.Empty
                            }).ToList();

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);  // default page size;
                    contactsSearchViewModel.PageSize = PageSize;

                    CustomerContactsViewModel model = new CustomerContactsViewModel();


                    if (CustomerContactsList.Count == 1)
                    {
                        var ContactId = CustomerContactsList[0].ContactId;
                        CustomerContacts customerContacts = Customercontacts.FindBy(i => i.ContactId == ContactId).FirstOrDefault();
                        CommonMapper<CustomerContacts, CustomerContactsViewModel> mapper1 = new CommonMapper<CustomerContacts, CustomerContactsViewModel>();
                        model = mapper1.Mapper(customerContacts);
                    }


                    model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.CustomerGeneralInfoId == custGeneralinfoid).Select(m =>
                    new SelectListItem { Text = m.StreetName, Value = m.SiteDetailId.ToString() }).ToList();
                    model.SiteList.OrderBy(m => m.Text);

                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        //ContactsList = customerContactsCoreViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = model,
                        ContactCount = CustomerContactsList.Count
                    };
                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = custGeneralinfoid;
                    customerContactsListViewModel.CustomerContacts.SiteId = sitedetailId;
                    customerContactsListViewModel.customerContactsViewModel.JobId = JobId;
                    customerContactsListViewModel.customerContactsViewModel.UserName = base.GetUserName;
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job contact.");

                    return View(customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:  /Employee/CustomerJob/ViewJobContacts
        /// <summary>
        /// Search Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="name"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewJobContacts(string CustomerGeneralinfoid, string SiteId, int ContactType, string name, string PageNum)
        {
            try
            {
                using ((Customercontacts))
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    if (name == null)
                        name = "";

                    Guid Customerid = Guid.Parse(CustomerGeneralinfoid);
                    Guid siteDetailId = Guid.Parse(SiteId);
                    var CustomerContactsList = Customercontacts.GetJobContactsInfo(Customerid, siteDetailId, ContactType, name).ToList();

                    //Guid custGeneralinfoid = Guid.Parse(CustomerGeneralInfoId);
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactsList.ToList());

                    List<CustomerContactsViewModel> customerSiteCollection =
                            customerContactsCoreViewModel.Select(m => new CustomerContactsViewModel
                            {

                                FirstName = m.FirstName,
                                LastName = m.LastName,
                                PhoneNo1 = m.PhoneNo1,
                                EmailId = m.EmailId,
                                SiteAddress = m.SiteAddress,
                                ContactId = m.ContactId,
                                CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                                DisplayContactsType = (int)m.ContactsType != 0 ? m.ContactsType.GetAttribute<DisplayAttribute>() != null ? m.ContactsType.GetAttribute<DisplayAttribute>().Name : m.ContactsType.ToString() : string.Empty
                            }).ToList();



                    contactsSearchViewModel.PageSize = Request.QueryString["page_size"] == null ? 10 : Convert.ToInt32(Request.QueryString["page_size"]); ;
                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = new CustomerContactsViewModel()
                    };

                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = Customerid;
                    customerContactsListViewModel.CustomerContacts.SiteId = siteDetailId;
                    return PartialView("_JobContactsList", customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        ///Post: View All Customer Contacts
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        ///  <param name="SiteId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewJobContactsPartial(string SiteId, string customerGeneralinfoid, int ContactType, string Keyword)
        {
            try
            {
                using (Customercontacts)
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    Guid siteDetailId = Guid.Parse(SiteId);
                    Guid Customercontactid = Guid.Empty;
                    //var CustomerContactList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();
                    var CustomerContactsList = Customercontacts.GetJobContactsInfo(custGeneralinfoid, siteDetailId, ContactType, Keyword).ToList();
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactsList.ToList());

                    List<CustomerContactsViewModel> customerSiteCollection =
                            customerContactsCoreViewModel.Select(m => new CustomerContactsViewModel
                            {

                                FirstName = m.FirstName,
                                LastName = m.LastName,
                                PhoneNo1 = m.PhoneNo1,
                                EmailId = m.EmailId,
                                SiteAddress = m.SiteAddress,
                                ContactId = m.ContactId,
                                CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                                DisplayContactsType = (int)m.ContactsType != 0 ? m.ContactsType.GetAttribute<DisplayAttribute>() != null ? m.ContactsType.GetAttribute<DisplayAttribute>().Name : m.ContactsType.ToString() : string.Empty
                            }).ToList();


                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);  // default page size;
                    contactsSearchViewModel.PageSize = PageSize;
                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = new CustomerContactsViewModel()
                    };
                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = custGeneralinfoid;
                    customerContactsListViewModel.CustomerContacts.SiteId = siteDetailId;
                    return PartialView("_JobContactsList", customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Add New Customer Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="customercontactid"></param>
        /// <param name="SiteId"></param>
        ///  /// <param name="JobId"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public PartialViewResult _UpdateJobContactsAddEdit(string CustomerGeneralinfoid, string SiteId, string JobId, string customercontactid, string PageNum)
        {
            try
            {
                CustomerContactsViewModel model = new CustomerContactsViewModel();
                Guid customerGeneralinfoid = Guid.Parse(CustomerGeneralinfoid);
                Guid siteDetailId = Guid.Parse(SiteId);
                model.PageNum = PageNum;
                if (!string.IsNullOrEmpty(customercontactid))
                {
                    using (Customercontacts)
                    {
                        Guid custContactid = Guid.Parse(customercontactid);
                        CustomerContacts cotactlog = Customercontacts.FindBy(i => i.ContactId == custContactid).FirstOrDefault();

                        if (cotactlog != null)
                        {
                            CommonMapper<CustomerContacts, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContacts, CustomerContactsViewModel>();
                            model = mapper.Mapper(cotactlog);
                            model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.SiteDetailId == siteDetailId).Select(m =>
                            new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).ToList();
                            model.SiteList.OrderBy(m => m.Text);
                            model.JobId = JobId;




                            return PartialView(model);
                        }
                        else
                        {

                            return PartialView(model);
                        }
                    }
                }
                else
                {
                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (CustomerGeneralInfoRepo)
                    {
                        CustomerGeneralinfo = CustomerGeneralInfoRepo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralinfoid).FirstOrDefault();
                    }
                    Guid custContactid = Guid.Parse(CustomerGeneralinfoid);
                    model = new CustomerContactsViewModel();
                    model.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);

                    model.SiteList = CustomerSiteDetailRepo.GetAll().Where(m => m.SiteDetailId == siteDetailId).Select(m =>
                       new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).ToList();
                    model.SiteList.OrderBy(m => m.Text);
                    model.HideAddContacts = Request.QueryString["HideAddContacts"];
                    model.JobId = JobId;



                    return PartialView(model);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Post: Add New Customer Contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects AddCustomerInfo</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult _UpdateJobContactsAddEdit(CustomerContactsViewModel model)
        {
            if (model != null)
            {
                try
                {
                    using (Customercontacts)
                    {
                        Guid custGeneralinfoid = model.CustomerGeneralInfoId;
                        var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();

                        CommonMapper<CustomerContactsViewModel, CustomerContacts> mapper = new CommonMapper<CustomerContactsViewModel, CustomerContacts>();
                        if (ModelState.IsValid)
                        {
                            if (model.ContactId != Guid.Empty)
                            {
                                model.ModifiedDate = DateTime.Now;
                                model.ModifiedBy = Guid.Parse(base.GetUserId);
                                CustomerContacts customerContact = Customercontacts.FindBy(m => m.ContactId == model.ContactId).FirstOrDefault();
                                Customercontacts.DeAttach(customerContact);
                                customerContact = mapper.Mapper(model);
                                Customercontacts.Edit(customerContact);
                                Customercontacts.Save();
                                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                log.Info(base.GetUserName + " updated job contact.");

                                return RedirectToAction("SaveJobInfo", new { id = model.JobId, activetab = "Contact", success = "ok", pagenum = model.PageNum });
                            }
                            else
                            {
                                model.IsDelete = false;
                                model.CreatedDate = DateTime.Now;
                                model.CreatedBy = Guid.Parse(base.GetUserId);
                                model.ContactId = Guid.NewGuid();
                                CustomerContacts customerContacts = mapper.Mapper(model);
                                Customercontacts.Add(customerContacts);
                                Customercontacts.Save();

                                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                log.Info(base.GetUserName + " updated job contact.");


                                return RedirectToAction("SaveJobInfo", new { id = model.JobId, activetab = "Contact", success = "ok", pagenum = model.PageNum });
                            }
                        }
                        else
                        {
                            return PartialView(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return PartialView(model);
        }
        //POST:  /Employee/CustomerJob/DeleteJobContacts
        /// <summary>
        /// Delete customer Contact 
        /// </summary>
        /// <param name="Customercontactid"></param>
        /// <returns></returns>
        public ActionResult DeleteJobContacts(string Customercontactid, string JobId)
        {
            try
            {
                using (Customercontacts)
                {
                    Guid contactid = Guid.Parse(Customercontactid);
                    CustomerContacts contactdelete = Customercontacts.FindBy(i => i.ContactId == contactid).FirstOrDefault();
                    Customercontacts.Delete(contactdelete);
                    Customercontacts.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted job contact.");

                    return Json(JobId, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction("ViewJobContactsPartial", "CustomerJob", new { SiteId = contactdelete.SiteId, customerGeneralinfoid=contactdelete.CustomerGeneralInfoId });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult GetQuickViewJobData(Guid JobId)
        {
            try
            {
                var QuickData = JobRepository.GetQuickViewData(JobId).FirstOrDefault();
                Constant.JobType enumDisplaytype = (Constant.JobType)QuickData.JobType;
                QuickData.DisplayType = enumDisplaytype.ToString();
                if (QuickData.CustomerNotes == null)
                {
                    QuickData.CustomerNotes = "Not Available";
                }
                if (QuickData.OperationNotes == null)
                {
                    QuickData.OperationNotes = "Not Available";
                }
                if (QuickData.JobNotes == null)
                {
                    QuickData.JobNotes = "Not Available";
                }
                if (QuickData.InvoicePrice == null)
                {
                    QuickData.InvoicePrice = 0;
                }
                if (QuickData.Paid == null)
                {
                    QuickData.Paid = 0;
                }
                if (QuickData.ApprovedByName == null)
                {
                    QuickData.ApprovedByName = "Not Available";
                }
                if (QuickData.TimeTaken == null)
                {
                    QuickData.TimeTaken = "Not Available";
                }
                return Json(QuickData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PartialViewResult _JobReminderCreate(Guid JobId, string CustomerReminderId, string PageNum, string SiteName)
        {
            try
            {
                CustomerReminderVM model = new CustomerReminderVM();
                //List<SelectListItem> JobList = null;

                string strJobId = Convert.ToString(JobId);
                string strJobIds = string.Join("','", strJobId); // separating each element by "','"
                strJobIds = strJobIds.Insert(0, "'"); // putting "'" at 0 index
                strJobIds = strJobIds.Insert(strJobIds.Count(), "'"); // putting "'" at last index

                var Jobs = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                model.DisplayJobNo = Jobs.JobNo.ToString();
                model.JobId = JobId;
                model.SiteName = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == Jobs.SiteId).Select(m => m.SiteFileName).FirstOrDefault();

                if (!string.IsNullOrEmpty(CustomerReminderId))
                {
                    Guid contactLogId = Guid.Parse(CustomerReminderId);
                    var customercontactLogInfo = CustomercontactLogRepo.FindBy(m => m.CustomerContactId == contactLogId).
                                                Select(m => new { m.Note, m.CustomerId }).FirstOrDefault();



                    model.TemplateMessageId = GetCustomerJobTemplate(customercontactLogInfo.Note);
                    model.CustomerId = customercontactLogInfo.CustomerId;

                    var scheduleReminder = ScheduleReminderRepo.FindBy(m => m.CustomerContactLogId == contactLogId).Select(m => new
                    {
                        m.ScheduleDate,
                        m.EmailTemplate,
                        m.PhoneTemplate,
                        m.Schedule,
                        m.FromEmail
                    }).FirstOrDefault();

                    if (scheduleReminder != null)
                    {
                        model.FromEmail = (Constant.FromEmail)scheduleReminder.FromEmail;
                        model.Schedule = scheduleReminder.Schedule;
                        model.ReminderDate = scheduleReminder.ScheduleDate;
                        if (string.IsNullOrEmpty(scheduleReminder.EmailTemplate) && string.IsNullOrEmpty(scheduleReminder.PhoneTemplate))
                        {
                            model.Note = string.Empty;
                        }
                        else if (string.IsNullOrEmpty(scheduleReminder.EmailTemplate))
                        {
                            model.Note = scheduleReminder.PhoneTemplate;
                            model.Note = model.Note.Replace("<p>", System.Environment.NewLine).
                                     Replace("</p>", System.Environment.NewLine);
                        }
                        else if (string.IsNullOrEmpty(scheduleReminder.PhoneTemplate))
                        {
                            model.Note = scheduleReminder.EmailTemplate;
                            model.Note = model.Note.Replace("<p>", System.Environment.NewLine).
                                     Replace("</p>", System.Environment.NewLine);
                        }
                    }

                    model.JobId2 = CustomercontactLogRepo.GetJobByContactLog(CustomerReminderId).Select(m =>
                                   new JobDataVM
                                   {
                                       Id = m.Id
                                   }).Select(m => m.Id).ToList();

                    model.ContactList = Customercontacts.GetJobContactList(strJobIds).Select(m => new
                    {
                        m.ContactId,
                        m.EmailId,
                        m.FirstName,
                        m.LastName,
                        m.Phone,
                        m.SiteFileName
                    }).Distinct().Select(m => new SelectListItem
                    {
                        Text = m.FirstName + " " + m.LastName + " (" + m.SiteFileName + ")",
                        Value = m.ContactId.ToString()
                    }).ToList();
                    model.ContactListIds = ContactLogSiteContactsMappingRepo.FindBy(m => m.ContactLogId == contactLogId)
                                           .Select(m => m.ContactId).ToList();
                    model.ReminderId = Guid.Parse(CustomerReminderId);
                    return PartialView(model);
                }
                else
                {
                    Guid CustomerGeneralinfoid = Jobs.CustomerGeneralInfoId;

                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (CustomerGeneralInfoRepo)
                    {
                        CustomerGeneralinfo = CustomerGeneralInfoRepo.FindBy(i => i.CustomerGeneralInfoId == CustomerGeneralinfoid).FirstOrDefault();
                    }
                    //model = new CustomerReminderVM();
                    model.CustomerGeneralInfoId = CustomerGeneralinfoid;


                    model.ContactList = new List<SelectListItem>();


                    var jobContacts = Customercontacts.GetJobContactList(strJobIds).Select(m => new
                    {
                        m.ContactId,
                        m.EmailId,
                        m.FirstName,
                        m.LastName,
                        m.Phone,
                        m.SiteFileName
                    }).Distinct();
                    model.ContactList = jobContacts.Select(m => new SelectListItem
                    {
                        Text = m.FirstName + " " + m.LastName + " (" + m.SiteFileName + ")",
                        Value = m.ContactId.ToString()
                    }).ToList();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + "  created customer Reminder.");

                    return PartialView(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Constant.CustomerJobTemplateMessage GetCustomerJobTemplate(string customercontactLogInfo)
        {
            Constant.CustomerJobTemplateMessage TemplateMessageId = 0;
            switch (customercontactLogInfo)
            {
                case "Confirmation Appointment Strata Realestate":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ConfirmationAppointmentStrataRealestate;
                    break;
                case "Confirmation Appointment Domestic Customer":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ConfirmationAppointmentDomesticCustomer;
                    break;
                case "Due to Rain Job is Postponed to tomorrow":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.DueToRainJobPostponed;
                    break;
                case "Unavailable OTRW Due To BadHealth":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.UnavailableOTRWDueToBadHealth;
                    break;
                case "Price Increase for Gutter Clean Contract":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ContractedGutterCleanPriceIncrease;
                    break;
                case "SRAS Needed for Gutter Clean Contract":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ContractedGutterCleanSRASNeeded;
                    break;
                case "Reminder":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.CustomerReminder;
                    break;
                case "Rain":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ReminderForRain;
                    break;
                case "Sick":
                    TemplateMessageId = Constant.CustomerJobTemplateMessage.ReminderForSick;
                    break;
            }
            return TemplateMessageId;
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult _JobReminderCreate()
        {

            return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> JobSendReminder(DateTime? ReminderDate, Guid ReminderId, string[] JobId, string[] ContactListIds, Nullable<FSMConstant.Constant.CustomerJobTemplateMessage> TempMsgId, string Note, bool HasSMS, bool HasEmail, bool hasSchedule, string fromEmail, string fromEmailVal)
        {

            Customer.ViewModels.CustomerReminderViewModel model = new Customer.ViewModels.CustomerReminderViewModel();

            var CustomerID = Guid.Empty;

            #region update particular contact log and its related data
            if (ReminderId != Guid.Empty)
            {
                Guid jobId;
                var hasJobId = Guid.TryParse(JobId[0], out jobId);

                #region update contact log for a particular job
                // update contact log for a particular job
                var contactLog = CustomercontactLogRepo.FindBy(m => m.CustomerContactId == ReminderId).FirstOrDefault();
                contactLog.LogDate = DateTime.Now;
                contactLog.Note = TempMsgId.Value > 0 ? TempMsgId.GetAttribute<DisplayAttribute>().Name : TempMsgId.Value.ToString();
                contactLog.JobId = JobId.Count() > 0 ? JobId[0] : string.Empty;
                contactLog.ModifiedDate = DateTime.Now;
                contactLog.ModifiedBy = Guid.Parse(base.GetUserId);
                CustomercontactLogRepo.Edit(contactLog);
                CustomercontactLogRepo.Save();
                #endregion

                #region delete ContactLogSiteContactsMapping for a particular contact log
                // delete ContactLogSiteContactsMapping for a particular contact log
                var contactLogSiteContactsMappingList = ContactLogSiteContactsMappingRepo.FindBy(m => m.ContactLogId ==
                                                        contactLog.CustomerContactId).ToList();
                foreach (var contactLogSiteContactsMapping in contactLogSiteContactsMappingList)
                {
                    ContactLogSiteContactsMappingRepo.Delete(contactLogSiteContactsMapping);
                    ContactLogSiteContactsMappingRepo.Save();
                }
                #endregion

                #region delete SchedulerReminder for a particular contact log
                // delete SchedulerReminder for a particular contact log
                var scheduleReminderList = ScheduleReminderRepo.FindBy(m => m.CustomerContactLogId ==
                                           contactLog.CustomerContactId).ToList();
                foreach (var scheduleReminder in scheduleReminderList)
                {
                    ScheduleReminderRepo.Delete(scheduleReminder);
                    ScheduleReminderRepo.Save();
                }
                #endregion

                #region adding schduler reminder and ContactLogSiteContactsMapping for a particular contact
                // adding schduler reminder and ContactLogSiteContactsMapping for a particular contact
                if (ContactListIds.Count() > 0)
                {
                    // find job contacts
                    var jobContacts = Customercontacts.GetJobContactList("'" + jobId + "'");
                    foreach (var contact in jobContacts)
                    {
                        if (Array.IndexOf(ContactListIds, contact.ContactId.ToString()) >= 0)
                        {
                            // saving in ContactLogSiteContactsMapping for each contact
                            ContactLogSiteContactsMapping contactLogSiteContactsMapping = new ContactLogSiteContactsMapping();
                            contactLogSiteContactsMapping.Id = Guid.NewGuid();
                            contactLogSiteContactsMapping.ContactLogId = contactLog.CustomerContactId;
                            contactLogSiteContactsMapping.JobId = jobId;
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
                            scheduleReminder.CustomerContactLogId = contactLog.CustomerContactId;
                            scheduleReminder.Schedule = hasSchedule;
                            scheduleReminder.FromEmail = Convert.ToInt32(fromEmailVal);
                            scheduleReminder.ScheduleDate = ReminderDate;
                            scheduleReminder.CreatedDate = DateTime.Now;
                            scheduleReminder.CreatedBy = Guid.Parse(base.GetUserId);

                            var jobs = JobRepository.FindBy(i => i.Id == jobId).FirstOrDefault();
                            string msgBody = Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);
                            //  if (contact.DateBooked.HasValue)
                            if (jobs != null)
                            {
                                if (jobs.DateBooked.HasValue)
                                {

                                    if (jobs.DateBooked >= DateTime.Now.Date)
                                    {
                                        if (jobs.DateBooked.HasValue)
                                            msgBody = msgBody.Replace("{{DateBooked}}", jobs.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                        else
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                        string startDate = (JobAssignMapping.FindBy(m => m.JobId == jobId && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= jobs.DateBooked).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        // string startDate = (JobAssignMapping.FindBy(m => m.JobId == jobId).OrderBy(m => m.StartTime).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        if (!string.IsNullOrEmpty(startDate))
                                        {
                                            DateTime getDateTime = Convert.ToDateTime(startDate);
                                            string getDate = getDateTime.ToString("dd/MM/yyyy");

                                            DateTime endDate = DateTime.Now;
                                            if (getDateTime != null)
                                            {
                                                endDate = getDateTime.AddMinutes(+30);
                                            }
                                            TimeSpan startTime = new TimeSpan(getDateTime.Hour, getDateTime.Minute, getDateTime.Second);
                                            TimeSpan endTime = new TimeSpan(endDate.Hour, endDate.Minute, endDate.Second);
                                            msgBody = msgBody.Replace("{{DateSend}}", getDate);
                                            msgBody = msgBody.Replace("{{StartEndTime}}", startTime + " - " + endTime);
                                        }
                                        //else
                                        //{
                                        //    msgBody = msgBody.Replace("{{DateSend}}", "today");
                                        //    msgBody = msgBody.Replace("{{StartEndTime}}", "StartEndTime");
                                        //}



                                        if (HasSMS == true || (HasSMS == false && HasEmail == false))
                                        {
                                            // replacing img tag with alt text
                                            var pattern = @"<img.*?alt='(.*?)'[^\>]*>";
                                            var replacePattern = @"$1";
                                            var phoneTemplate = Regex.Replace(msgBody, pattern, replacePattern, RegexOptions.IgnoreCase);
                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                scheduleReminder.Phone = contact.Phone;
                                                scheduleReminder.PhoneTemplate = phoneTemplate;
                                            }

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
                                        if (HasEmail == true)
                                        {
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
                            }
                            ScheduleReminderRepo.Add(scheduleReminder);
                            ScheduleReminderRepo.Save();
                        }
                    }
                }
                #endregion

                CustomerID = contactLog.CustomerGeneralInfoId;
            }
            #endregion

            #region add contact log and its related data
            else
            {
                foreach (var job in JobId)
                {
                    Guid Job_id = Guid.Parse(job);
                    CustomerID = JobRepository.FindBy(m => m.Id == Job_id).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                    var customerInfo = JobRepository.FindBy(m => m.Id == Job_id).Select(m => new
                    {
                        m.CustomerGeneralInfo.CustomerGeneralInfoId,
                        m.CustomerGeneralInfo.CTId
                    }).FirstOrDefault();

                    var JobDetail = JobRepository.FindBy(m => m.Id == Job_id).FirstOrDefault();
                    string SiteAddress = CustomerSiteDetailRepo.GetSiteAddress(JobDetail.SiteId);

                    #region save contactlog for each job
                    // save contactlog for each job
                    CustomerContactLog customerContactLog = new CustomerContactLog();
                    customerContactLog.CustomerContactId = Guid.NewGuid();
                    customerContactLog.CustomerGeneralInfoId = customerInfo.CustomerGeneralInfoId;
                    customerContactLog.CustomerId = customerInfo.CTId.ToString();
                    customerContactLog.JobId = job;
                    customerContactLog.LogDate = DateTime.Now;
                    customerContactLog.Note = TempMsgId.GetAttribute<DisplayAttribute>().Name;
                    customerContactLog.IsDelete = false;
                    customerContactLog.IsReminder = true;
                    customerContactLog.IsScheduler = true;
                    customerContactLog.CreatedDate = DateTime.Now;
                    customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);
                    CustomercontactLogRepo.Add(customerContactLog);
                   CustomercontactLogRepo.Save();
                    #endregion
                    // find job contacts
                    var jobContacts = Customercontacts.GetJobContactList("'" + JobDetail.Id + "'");
                    #region adding ContactLogSiteContactsMapping and scheduler reminder data for each contact 
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
                            scheduleReminder.Schedule = hasSchedule;
                            scheduleReminder.FromEmail = Convert.ToInt32(fromEmailVal);
                            scheduleReminder.ScheduleDate = ReminderDate;
                            scheduleReminder.CreatedDate = DateTime.Now;
                            scheduleReminder.CreatedBy = Guid.Parse(base.GetUserId);
                          //  Jobs jobs = JobRepository.FindBy(i => i.Id == Job_id).FirstOrDefault();
                            string msgBody = Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);
                            if (JobDetail != null)
                            {
                                if (JobDetail.DateBooked.HasValue)
                                {

                                    if (JobDetail.DateBooked >= DateTime.Now.Date)
                                    {
                                        if (JobDetail.DateBooked.HasValue)
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", JobDetail.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        else
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                        string startDate = (JobAssignMapping.FindBy(m => m.JobId == Job_id && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= JobDetail.DateBooked).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        // string startDate = (JobAssignMapping.FindBy(m => m.JobId == Job_id).OrderBy(m => m.StartTime).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        if (!string.IsNullOrEmpty(startDate))
                                        {
                                            DateTime getDateTime = Convert.ToDateTime(startDate);
                                            string getDate = getDateTime.ToString("dd/MM/yyyy");

                                            DateTime endDate = DateTime.Now;
                                            if (getDateTime != null)
                                            {
                                                endDate = getDateTime.AddMinutes(+30);
                                            }
                                            TimeSpan startTime = new TimeSpan(getDateTime.Hour, getDateTime.Minute, getDateTime.Second);
                                            TimeSpan endTime = new TimeSpan(endDate.Hour, endDate.Minute, endDate.Second);
                                            msgBody = msgBody.Replace("{{DateSend}}", getDate);
                                            msgBody = msgBody.Replace("{{StartEndTime}}", startTime + " - " + endTime);
                                        }
                                        //else
                                        //{
                                        //    msgBody = msgBody.Replace("{{DateSend}}", "today");
                                        //    msgBody = msgBody.Replace("{{StartEndTime}}", "");
                                        //}

                                        if (HasSMS == true || (HasSMS == false && HasEmail == false))
                                        {
                                            // replacing img tag with alt text
                                            var pattern = @"<img.*?alt='(.*?)'[^\>]*>";
                                            var replacePattern = @"$1";
                                            var phoneTemplate = Regex.Replace(msgBody, pattern, replacePattern, RegexOptions.IgnoreCase);
                                            if (!string.IsNullOrEmpty(contact.Phone))
                                            {
                                                scheduleReminder.Phone = contact.Phone;
                                                scheduleReminder.PhoneTemplate = phoneTemplate;
                                            }

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
                                        if (HasEmail == true)
                                        {
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
                            }
                            ScheduleReminderRepo.Add(scheduleReminder);
                            ScheduleReminderRepo.Save();
                        }
                    }
                    #endregion
                }
            }
            #endregion

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + "  send Reminder to a customer by email.");
            return Json(new { success = JobId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveSendReminder(Guid ReminderId, int JobNo, FSMConstant.Constant.MessageType MessageTypeId, int MsgTypeId, int TempMsgId, Nullable<FSMConstant.Constant.DashboardTemplateMessage> TemplateMessageId, string Note, bool HasSMS, bool HasEmail)
        {
            {
                var customerGeneralInfoId = JobRepository.FindBy(m => m.JobNo == JobNo).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                var CustomerName = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).Select(m => m.CustomerLastName).FirstOrDefault();
                var InvoiceNo = InvoiceRepo.FindBy(m => m.JobId == JobNo).Select(m => m.InvoiceNo).FirstOrDefault();

                StreamReader reader = null;
                string readFile = "";
                string myString = "";
                int Templateused = Convert.ToInt32(TemplateMessageId);
                if (Templateused != 0)
                {

                    Constant.TemplateMessage template = (Constant.TemplateMessage)Templateused;
                    switch (template)
                    {
                        case Constant.TemplateMessage.Due_to_Rain_Job_is_Postponed_to_tomorrow:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/SmsTemplate/DueToRainJobPostponed.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            myString = myString.Replace("ClientName", CustomerName);
                            break;

                        case Constant.TemplateMessage.Payment_is_Pending:
                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/SmsTemplate/PaymentIsPending.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            myString = myString.Replace("ClientName", CustomerName);
                            break;

                        case Constant.TemplateMessage.Please_Provide_your_feedback_on_the_Job:

                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/SmsTemplate/PleaseProvideFeedbackJob.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            myString = myString.Replace("ClientName", CustomerName);
                            break;

                        case Constant.TemplateMessage.Domestic_Payment_Reminder:

                            reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/SmsTemplate/DomesticPaymentReminder.htm"));
                            readFile = reader.ReadToEnd();
                            myString = readFile;
                            myString = myString.Replace("ClientName", CustomerName);
                            myString = myString.Replace("InvoiceNumber", Convert.ToString(InvoiceNo));
                            break;
                    }
                }

                var TemplateMessage = "";
                Customer.ViewModels.CustomerReminderViewModel model = new Customer.ViewModels.CustomerReminderViewModel();
                CommonMapper<Customer.ViewModels.CustomerReminderViewModel, CustomerReminder> mapper = new CommonMapper<Customer.ViewModels.CustomerReminderViewModel, CustomerReminder>();
                var JobDetail = JobRepository.FindBy(m => m.JobNo == JobNo).FirstOrDefault();
                var customerCTID = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == JobDetail.CustomerGeneralInfoId).Select(m => m.CTId).FirstOrDefault();

                if (ReminderId == Guid.Empty)
                {
                    model.ReminderId = Guid.NewGuid();
                    model.ReminderDate = DateTime.Now;
                    model.CustomerGeneralInfoId = JobDetail.CustomerGeneralInfoId;
                    model.CustomerId = customerCTID.ToString();
                    model.ReContactDate = DateTime.Now;
                    model.IsDelete = false;
                    model.JobId = JobDetail.Id;
                    model.MessageTypeId = MessageTypeId;
                    model.TemplateMessageId = TemplateMessageId;
                    model.Note = Note;
                    model.HasSMS = HasSMS;
                    model.HasEmail = HasEmail;
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = Guid.Parse(base.GetUserId);
                    CustomerReminder dashreminder = mapper.Mapper(model);
                    CustomerReminderRepo.Add(dashreminder);
                    CustomerReminderRepo.Save();

                }
                else
                {
                    CustomerReminder reminderlog = CustomerReminderRepo.FindBy(i => i.ReminderId == ReminderId).FirstOrDefault();
                    reminderlog.ModifiedDate = DateTime.Now;
                    reminderlog.ModeifiedBy = Guid.Parse(base.GetUserId);
                    reminderlog.ReContactDate = DateTime.Now;
                    reminderlog.MessageTypeId = MsgTypeId;
                    reminderlog.TemplateMessageId = TempMsgId;
                    reminderlog.Note = Note;
                    reminderlog.HasSMS = HasSMS;
                    reminderlog.HasEmail = HasEmail;

                    CustomerReminderRepo.DeAttach(reminderlog);
                    CustomerReminderRepo.Edit(reminderlog);
                    CustomerReminderRepo.Save();

                }
                if (HasSMS == true)
                {
                    //var PhoneList = Customercontacts.FindBy(m => m.SiteId == JobDetail.SiteId).Select(m => m.PhoneNo1).ToArray();
                    string[] to = { "+918054008903" };
                    string sendFrom = "+61414363865";
                    TransmitSmsWrapper manager = new TransmitSmsWrapper("23dac442668a809eeaa7d9aaad5f91c7", "clientapisecret", "https://api.transmitsms.com");
                    if (TemplateMessageId != null)
                    {
                        var response = manager.SendSms("" + myString + "", to, sendFrom, null, null, "", "", 0);
                    }
                    else
                    {
                        var response = manager.SendSms("" + Note + "", to, sendFrom, null, null, "", "", 0);
                    }
                }
                if (HasEmail == true)  //Send Email
                {
                    string Job_No = Convert.ToString(JobDetail.JobNo);
                    var ContactList = Customercontacts.FindBy(m => m.SiteId == JobDetail.SiteId).FirstOrDefault();
                    var StreetName = CustomerSiteDetailRepo.FindBy(m => m.SiteDetailId == JobDetail.SiteId).Select(m => m.Street).FirstOrDefault();
                    string SiteAddress = CustomerSiteDetailRepo.GetSiteAddress(JobDetail.SiteId);
                    string EmailID;
                    if (ContactList != null)
                    {
                        EmailID = ContactList.EmailId;
                        if (!string.IsNullOrEmpty(EmailID))
                        {
                            string Date = DateTime.Now.ToShortDateString();

                            var MessageType = ((FSMConstant.Constant.MessageType)MessageTypeId).ToString();
                            if (TemplateMessageId == null)
                            {
                                TemplateMessage = "";
                            }
                            else
                            {
                                TemplateMessage = ((FSMConstant.Constant.TemplateMessage)TemplateMessageId).ToString();
                            }
                            StreamReader Reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/DashboardReminderTemplate.htm"));
                            string ReadFile = Reader.ReadToEnd();
                            string MyString = ReadFile;
                            MyString = MyString.Replace("StreetName_", StreetName.ToString());
                            MyString = MyString.Replace("SiteAddress_", SiteAddress);
                            MyString = MyString.Replace("Date_", Date);
                            MyString = MyString.Replace("Note", Note);
                            MailMessage mmm = new MailMessage();
                            mmm.Subject = "Reminding You";
                            mmm.IsBodyHtml = true;
                            mmm.To.Add(EmailID);
                            mmm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                            string Body = Convert.ToString(MyString);
                            mmm.Body = Body;
                            SmtpClient Smtp = new SmtpClient();
                            Smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                            Smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                            Smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                            Smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            Smtp.Send(mmm);
                        }
                    }


                }
                else
                {

                    var ContactList = Customercontacts.FindBy(m => m.SiteId == JobDetail.SiteId).FirstOrDefault();
                    string EmailID;
                    if (ContactList != null)
                    {
                        EmailID = ContactList.EmailId;
                        if (!string.IsNullOrEmpty(EmailID))
                        {
                            string Date = DateTime.Now.ToShortDateString();

                            var MessageType = ((FSMConstant.Constant.MessageType)MessageTypeId).ToString();
                            if (TemplateMessageId == null)
                            {
                                TemplateMessage = "";
                            }
                            else
                            {
                                TemplateMessage = ((FSMConstant.Constant.TemplateMessage)TemplateMessageId).ToString();
                            }
                            StreamReader Reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplate/DashboardReminderTemplate.htm"));
                            string ReadFile = Reader.ReadToEnd();
                            string MyString = ReadFile;
                            MyString = MyString.Replace("Date_", Date);
                            MyString = MyString.Replace("JobNo", JobNo.ToString());
                            MyString = MyString.Replace("MessageType", MessageType);

                            MyString = MyString.Replace("TemplateMessage", TemplateMessage);
                            MyString = MyString.Replace("Note", Note);
                            MailMessage mmm = new MailMessage();
                            mmm.Subject = "Reminding You";
                            mmm.IsBodyHtml = true;
                            mmm.To.Add(EmailID);
                            mmm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                            string Body = Convert.ToString(MyString);
                            mmm.Body = Body;
                            SmtpClient Smtp = new SmtpClient();
                            Smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                            Smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                            Smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                            Smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            Smtp.Send(mmm);
                        }
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " sent reminder to customer by email.");

                return Json(new { success = JobDetail.Id }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteJobReminder(string ReminderId, string PageNum)
        {
            try
            {
                using (CustomerReminderRepo)
                {
                    Guid reminderId = Guid.Parse(ReminderId);
                    CustomerContactLog logtodelete = CustomercontactLogRepo.FindBy(i => i.CustomerContactId == reminderId).FirstOrDefault();
                    logtodelete.IsDelete = true;
                    CustomercontactLogRepo.Edit(logtodelete);
                    CustomercontactLogRepo.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted job reminder.");

                    return RedirectToAction("ViewCustomerContactLogPartial", "CustomerJob", new { JobId = logtodelete.JobId });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: Employee/CustomerJob/_CallBackJobPopUp
        public ActionResult _CallBackJobPopUp(Guid JobId)
        {
            try
            {
                JobsViewModel empModel = new JobsViewModel();
                empModel.Id = JobId;
                return PartialView(empModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        ////Get:Job Extend

        [HttpGet]
        public ActionResult UpdateCallBackJob(Guid JobId, DateTime UpdateDateBooked, int OTRWRequired)
        {
            try
            {
                EmployeeJobsViewModel empModel = new EmployeeJobsViewModel();
                Jobs jobEntity = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
                var jobAssignMappingEntity = JobAssignMapping.FindBy(m => m.JobId == JobId && m.IsDelete == false).ToList();

                jobEntity.DateBooked = UpdateDateBooked;

                jobEntity.OTRWRequired = OTRWRequired;
                jobEntity.Status = 3;
                jobEntity.Supervisor = null;
                jobEntity.JobType = 5;

                JobRepository.DeAttach(jobEntity);
                JobRepository.Edit(jobEntity);
                JobRepository.Save();

                //Update Site Make Multiple People Need
                CustomerResidenceDetail residenceDetailEntity = CustomerResidence.FindBy(m => m.SiteDetailId == jobEntity.SiteId).FirstOrDefault();
                residenceDetailEntity.NeedTwoPPL = true;
                CustomerResidence.DeAttach(residenceDetailEntity);
                CustomerResidence.Edit(residenceDetailEntity);
                CustomerResidence.Save();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " updated callback job.");

                return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Get maximum job no
        /// <summary>
        /// maximum job no
        /// </summary>
        /// <param name="currentJobNo"></param>
        /// <returns></returns>
        public ActionResult GetMaximumJobNo(int currentJobNo)
        {
            try
            {
                using (JobRepository)
                {
                    int maxJobNo = JobRepository.GetMaxJobNo();
                    return Json(new { result = maxJobNo, status = "false" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult ChangeJobApprovedStatus()
        {
            try
            {
                int result = 1;
                var jobId = !string.IsNullOrEmpty(Request.QueryString["JobId"]) ?
                                       Guid.Parse(Request.QueryString["JobId"]) : Guid.Empty;
                var status = Request.QueryString["JobStatus"];
                var jobStatus = string.Empty;

                var job = JobRepository.FindBy(m => m.Id == jobId).FirstOrDefault();
                var invoiceDataWithQuote = InvoiceRepo.FindBy(m => m.JobId == job.JobNo && m.InvoiceType == "Quote" && m.IsDelete == false).FirstOrDefault();
                var invoiceDataWithInvoice = InvoiceRepo.FindBy(m => m.JobId == job.JobNo && m.InvoiceType == "Invoice" && m.IsDelete == false).FirstOrDefault();

                if (!string.IsNullOrEmpty(status))
                {
                    if (invoiceDataWithQuote != null || job.IsApproved == true || invoiceDataWithInvoice != null)
                    {
                        job.IsApproved = true;
                        jobStatus = "[ Approved ]";

                        if (status == "Approve")
                        {
                            job.IsApproved = true;
                            jobStatus = "[ Approved ]";
                            job.ApprovedDate = DateTime.Now;

                            if (invoiceDataWithInvoice == null)
                            {
                                //pdf genereate with invoice type Quote
                                //QuoteDataPdfGenerate(invoiceDataWithQuote.Id);

                                //created invoice
                                //invoiceDataWithQuote.InvoiceType = "Invoice";
                                invoiceDataWithQuote.ModifiedDate = DateTime.Now;
                                invoiceDataWithQuote.ModifiedBy = Guid.Parse(base.GetUserId);

                                InvoiceRepo.Edit(invoiceDataWithQuote);
                                InvoiceRepo.Save();

                            }
                            else
                            {
                                //Edit invoice
                                invoiceDataWithInvoice.ModifiedDate = DateTime.Now;
                                invoiceDataWithInvoice.ModifiedBy = Guid.Parse(base.GetUserId);

                                InvoiceRepo.Edit(invoiceDataWithInvoice);
                                InvoiceRepo.Save();
                            }
                        }
                        else
                        {
                            job.IsApproved = false;
                            job.ApprovedDate = null;
                            jobStatus = "[ Not Approved ]";
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                }
                job.ModifiedBy = Guid.Parse(base.GetUserId);
                job.ModifiedDate = DateTime.Now;
                JobRepository.Edit(job);
                JobRepository.Save();

                return Json(new { jobstatus = jobStatus, status = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult GetJobPerFormanceData(string Jobid)
        {
            JobsViewModel jobViewModel = new JobsViewModel();
            try
            {
                var jobPerformanceData = JobRepository.GetJobPerFormanceData(Jobid).FirstOrDefault();
                CommonMapper<JobPerFormanceCoreViewModel, JobPerFormanceViewModel> Assignmapper = new CommonMapper<JobPerFormanceCoreViewModel, JobPerFormanceViewModel>();
                JobPerFormanceViewModel jobPerformance = Assignmapper.Mapper(jobPerformanceData);
                //convert Decimal hours into time
                if (jobPerformanceData != null)
                {
                    jobPerformance.Hours = Math.Round(Convert.ToDecimal(jobPerformance.Hours), 2);
                    var hours = (jobPerformance.Hours * 60 * 100) / 100;
                    var fminutes = hours;

                    int fhour = Convert.ToInt32(fminutes / 60);
                    int fminute = Convert.ToInt32(fminutes % 60);

                    string hourtotal = fhour + "." + fminute;
                    jobPerformance.Hours = Convert.ToDecimal(hourtotal);

                    //convert Decimal Lpp hours into time
                    var Lpphours = (jobPerformance.LPPHR * 60 * 100) / 100;
                    int Lpphour = Convert.ToInt32(Lpphours) / 60;
                    int Lppminute = Convert.ToInt32(Lpphours) % 60;

                    string Lpptotal = Lpphour + "." + Lppminute;
                    if (Lpphour > 0 && Lppminute > 0)
                    {
                        jobPerformance.LPPHR = Convert.ToDecimal(Lpptotal);
                    }
                    else
                    {
                        jobPerformance.LPPHR = Convert.ToDecimal("0.00");
                    }

                    //convert Decimal NRL hours into time
                    var NRLhours = (jobPerformance.NRLHours * 60 * 100) / 100;
                    int NRLhour = Convert.ToInt32(NRLhours / 60);
                    int NRLminute = Convert.ToInt32(NRLhours % 60);

                    string NRLtotal = NRLhour + "." + NRLminute;
                    jobPerformance.NRLHours = Convert.ToDecimal(NRLtotal);

                    //convert Decimal LCP hours into time
                    var LCPhours = (jobPerformance.LCPHR * 60 * 100) / 100;
                    int LCPhour = Convert.ToInt32(LCPhours / 60);
                    int LCPminute = Convert.ToInt32(LCPhours % 60);

                    string LCPtotal = LCPhour + "." + LCPminute;
                    jobPerformance.LCPHR = Convert.ToDecimal(LCPtotal);

                    //convert Decimal Rev hours into time
                    var Revhours = (jobPerformance.RevHours * 60 * 100) / 100;
                    int Revhour = Convert.ToInt32(Revhours / 60);
                    int Revminute = Convert.ToInt32(Revhours % 60);

                    string Revtotal = Revhour + "." + Revminute;
                    jobPerformance.RevHours = Convert.ToDecimal(Revtotal);

                    //convert Decimal LIP hours into time
                    var LIPhours = (jobPerformance.LIPHR * 60 * 100) / 100;
                    int LIPhour = Convert.ToInt32(LIPhours / 60);
                    int LIPminute = Convert.ToInt32(LIPhours % 60);

                    string LIPtotal = LIPhour + "." + LIPminute;
                    if (!LIPtotal.Contains("-"))
                    {
                        jobPerformance.LIPHR = Convert.ToDecimal(LIPtotal);
                    }
                    jobViewModel.jobPerFormanceViewModel = jobPerformance;
                }

                return PartialView("_JobPerformanceSnapshot", jobViewModel);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                JobRepository.Dispose();
            }
        }
        // GET: Employee/CustomerJob/_ExtendJob
        public ActionResult _ExtendJob(Guid JobId)
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
                empModel.OTRWList = JobRepository.GetOTRWUserUsingByWorkType(workType).Select(m => new SelectListItem()
                {
                    Text = m.OTRWUserName,
                    Value = m.OTRWID.ToString()
                }).OrderBy(m => m.Text).ToList();


                return PartialView(empModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get:UpdateExtendJob

        [HttpGet]
        public ActionResult UpdateExtendJobPopUp(Guid JobId, DateTime UpdateDateBooked, double EstimatedHours)
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
                var jobAssignMappingEntity = JobAssignMapping.FindBy(m => m.JobId == JobId && m.IsDelete == false).ToList();

                //jobEntity.EstimatedHrsPerUser = jobEntity.EstimatedHours / Convert.ToDouble(OTRWRequired + jobEntity.OTRWRequired);
                jobEntity.EstimatedHrsPerUser = EstimatedHours / Convert.ToDouble(OTRWRequired);

                //if (jobEntity.OTRWRequired + OTRWRequired > 10)
                //{
                //    var MaxSelectOtrwRequired = 10 - jobEntity.OTRWRequired;
                //    return Json(new { Status = "False", Required = "Maximum Select " + MaxSelectOtrwRequired + " OTRW Required !" }, JsonRequestBehavior.AllowGet);
                //}

                //Update Startdate And Enddate With updatedDate 
                //string NewStartDate = UpdateDateBooked.ToString("d/M/yyyy") + ' ' + jobEntity.StartTime.Value.TimeOfDay;
                //DateTime startdate = DateTime.Parse(NewStartDate);

                //DateTime Endtime = startdate.AddHours(Convert.ToDouble(jobEntity.EstimatedHrsPerUser));
                ////string NewEndDate = UpdateDateBooked.ToString("d/M/yyyy") + ' ' + Endtime.Value.TimeOfDay;
                //DateTime enddate = Endtime;

                //check job alredy assign or user work type
                foreach (var key in Employeeid)
                {
                    Guid AssignUser = Guid.Parse(key);
                    var maxEndTime = JobAssignMapping.FindBy(m => m.AssignTo == AssignUser && m.DateBooked == UpdateDateBooked && m.IsDelete == false).Select(m => m.EndTime).Max();

                    if (maxEndTime == null)
                    {
                        maxEndTime = Convert.ToDateTime("06:00:00");
                    }
                    string assignStartDate = UpdateDateBooked.ToString("d/M/yyyy") + ' ' + maxEndTime.Value.TimeOfDay;
                    DateTime startdatetime = DateTime.Parse(assignStartDate);

                    DateTime assignEndtime = startdatetime.AddHours(Convert.ToDouble(jobEntity.EstimatedHrsPerUser));
                    DateTime enddatetime = assignEndtime;

                    var hasJob = JobRepository.OTRWHasJobWithName(AssignUser, (DateTime)startdatetime, (DateTime)enddatetime);
                    foreach (var user in hasJob)
                    {
                        var Error = "Job already assign " + user.UserName + " on given start/end time.";
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
                    var maxEndTime = JobAssignMapping.FindBy(m => m.AssignTo == AssignUser && m.DateBooked == UpdateDateBooked && m.IsDelete == false).Select(m => m.EndTime).Max();

                    if (maxEndTime == null)
                    {
                        maxEndTime = Convert.ToDateTime("06:00:00");
                    }

                    string assignStartDate = UpdateDateBooked.ToString("d/M/yyyy") + ' ' + maxEndTime.Value.TimeOfDay;
                    DateTime startdatetime = DateTime.Parse(assignStartDate);

                    DateTime assignEndtime = startdatetime.AddHours(Convert.ToDouble(jobEntity.EstimatedHrsPerUser));
                    DateTime enddatetime = assignEndtime;

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
                        jobAssignViewModel.StartTime = startdatetime;
                        jobAssignViewModel.EndTime = enddatetime;
                        jobAssignViewModel.EstimatedHrsPerUser = EstimatedHours / Convert.ToDouble(OTRWRequired);
                        CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping> Assignmapper = new CommonMapper<JobAssignToMappingViewModel, JobAssignToMapping>();
                        JobAssignToMapping jobAssignToMapping = Assignmapper.Mapper(jobAssignViewModel);

                        JobAssignMapping.Add(jobAssignToMapping);
                        JobAssignMapping.Save();
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
                CustomerResidenceDetail residenceDetailEntity = CustomerResidence.FindBy(m => m.SiteDetailId == jobEntity.SiteId).FirstOrDefault();
                if (residenceDetailEntity != null)
                {
                    residenceDetailEntity.NeedTwoPPL = true;
                    CustomerResidence.DeAttach(residenceDetailEntity);
                    CustomerResidence.Edit(residenceDetailEntity);
                    CustomerResidence.Save();
                }
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " changed jod's booked date.");
                return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult UploadFiles(string JobId, IEnumerable<HttpPostedFileBase> files)
        {

            try
            {
                JobDocViewModel jobDocViewModel = new JobDocViewModel();
                DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Images/JobDocuments"));
                if (!di.Exists)
                {
                    // create directory
                    Directory.CreateDirectory(Server.MapPath("~/Images/JobDocuments"));

                    // create sub directory
                    di.CreateSubdirectory(JobId.ToString());
                }
                else
                {
                    // create sub directory
                    di.CreateSubdirectory(JobId.ToString());
                }

                // saving doc to server
                for (int i = 0; i < files.Count(); i++)
                {
                    var File = Request.Files[i];
                    if (File != null && File.ContentLength > 0)
                    {
                        Guid attchGuid = Guid.NewGuid();
                        string jobDoc = Path.GetFileNameWithoutExtension(File.FileName);
                        string extension = Path.GetExtension(File.FileName).ToLower();
                        var filepath = Path.Combine(Server.MapPath("~/Images/JobDocuments/" + JobId),
                                          jobDoc + "_" + attchGuid + extension);
                        File.SaveAs(filepath);

                        // saving jobdocument entity
                        JobDocuments jobDocuments = new JobDocuments();
                        jobDocuments.Id = Guid.NewGuid();
                        jobDocuments.JobId = Guid.Parse(JobId);
                        jobDocuments.DocName = System.IO.Path.GetFileName(File.FileName);
                        jobDocuments.DocType = GetDocumentType(extension);
                        jobDocuments.SaveDocName = jobDoc + "_" + attchGuid + extension;
                        jobDocuments.IsDelete = false;
                        jobDocuments.CreatedDate = DateTime.Now;
                        jobDocuments.CreatedBy = Guid.Parse(base.GetUserId);
                        JobDocumentRepo.Add(jobDocuments);
                        JobDocumentRepo.Save();
                        Guid Job_id = Guid.Parse(JobId);
                        // adding job docs list
                        var jobDocList = JobDocumentRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false).ToList();
                        CommonMapper<JobDocuments, JobDocumentList> mapper = new CommonMapper<JobDocuments, JobDocumentList>();
                        var jobdocs = mapper.MapToList(jobDocList);
                        jobDocViewModel.jobDocumentList = jobdocs;
                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " saved job document.");
                        jobDocViewModel.JobId = Job_id;
                        return PartialView("_JobDocumentList", jobDocViewModel);
                    }
                    return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { data = "1" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Employee/CustomerJob/GetOTRWList
        /// <summary>
        /// Get OTRW Users
        /// </summary>
        /// <param name=""></param>
        /// <returns>return json</returns>
        public ActionResult GetOTRWList(int WorkType)
        {
            try
            {
                using (JobRepository)
                {
                    var otrwList = JobRepository.GetOTRWUserForWorkType(Convert.ToInt32(WorkType)).OrderBy(m => m.UserName).Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id

                    }).ToList();

                    var jsonSerialiser = new JavaScriptSerializer();
                    //var json = jsonSerialiser.Serialize(ob);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of JCL items.");

                    return Json(new { data = jsonSerialiser.Serialize(otrwList) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/CustomerJob/InsertAssignToForJob
        /// <summary>
        /// Save Data 
        /// </summary>
        /// <param name=""></param>
        /// <returns>return json</returns>
        [HttpPost]
        public ActionResult InsertAssignToForJob(List<AssignViewModel> assignJobList)
        {
            try
            {
                if (assignJobList != null)
                {
                    TempData["TempAssignJobData"] = null;
                    List<JobAssignToMapping> objtemp = new List<JobAssignToMapping>();
                    foreach (AssignViewModel item in assignJobList)
                    {
                        JobAssignToMapping objJobAssign = new JobAssignToMapping();
                        if (String.IsNullOrEmpty(item.AssignId.ToString()))
                        {
                            objJobAssign.Id = Guid.Empty;
                        }
                        else
                        {
                            objJobAssign.Id = item.AssignId;
                            String Input = item.OTRWNotes;

                            if (!string.IsNullOrEmpty(Input))
                            {
                                Input = item.OTRWNotes.Replace("&lt;", "<");
                                Input = Input.Replace("&gt;", ">");
                                objJobAssign.OTRWNotes = Input;
                            }
                            else { objJobAssign.OTRWNotes = string.Empty; }
                        }

                        objJobAssign.AssignTo = item.AssignTo;
                        objJobAssign.StartTime = item.AssignStartTime;
                        if (item.AssignDateBooked != null)
                        {
                            objJobAssign.DateBooked = item.AssignDateBooked;
                        }
                        else
                        {
                            objJobAssign.DateBooked = item.DateBooked;
                        }
                        if (objJobAssign.AssignTo != null && (item.AssignStartTime != null))
                            objtemp.Add(objJobAssign);
                    }
                    TempData["TempAssignJobData"] = objtemp;
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " Assign user for job.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult DeleteAssignJob(Guid assignUserId)
        {
            try
            {
                if (assignUserId != Guid.Empty)
                {
                    var assignJob = JobAssignMapping.FindBy(i => i.Id == assignUserId).FirstOrDefault();
                    if (assignJob != null)
                    {
                        assignJob.IsDelete = true;
                        assignJob.ModifiedDate = DateTime.Now;
                        assignJob.ModifiedBy = Guid.Parse(base.GetUserId);

                        JobAssignMapping.Edit(assignJob);
                        JobAssignMapping.Save();
                        return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = false }, JsonRequestBehavior.AllowGet);
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted assign job.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult DeleteAssignJobUsingJobId(Guid jobId)
        {
            try
            {
                if (jobId != Guid.Empty)
                {
                    var assignJob = JobAssignMapping.FindBy(i => i.JobId == jobId && i.IsDelete == false).ToList();
                    foreach (var assign in assignJob)
                    {
                        assign.IsDelete = true;
                        assign.ModifiedDate = DateTime.Now;
                        assign.ModifiedBy = Guid.Parse(base.GetUserId);

                        JobAssignMapping.Edit(assign);
                        JobAssignMapping.Save();
                    }
                    return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted assign job.");

                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public ActionResult GetJobAssignEndTime(string assignTo, string jobDateBooked, string JobId)
        {
            try
            {
                DateTime dateBooked = Convert.ToDateTime(jobDateBooked);
                Guid assignUser = Guid.Parse(assignTo);
                var employeeOnLeave = VacationRepo.CheckUserLeave(assignUser, dateBooked);  // Check User On Leave
                Guid jobid = !String.IsNullOrEmpty(JobId) ? Guid.Parse(JobId.ToString()) : Guid.Empty;
                var OTRWNotes = JobAssignMapping.FindBy(i => i.AssignTo == assignUser && i.JobId == jobid && i.IsDelete == false).ToList();
                string notes = "";
                if (OTRWNotes.Count > 0)
                {
                    foreach (var note in OTRWNotes)
                    {
                        notes = notes + note.OTRWNotes;
                    }

                }
                foreach (var user in employeeOnLeave)
                {
                    ModelState.AddModelError("UserLeave", "" + user + " on leave.");

                    return Json(new { error = "0", msg = "" + user + " on leave.", OTRWNotes = notes }, JsonRequestBehavior.AllowGet);
                }

                var maxEndTime = JobAssignMapping.FindBy(m => m.AssignTo == assignUser && m.DateBooked == dateBooked && m.IsDelete == false).Select(m => m.EndTime).Max();

                string endTime = maxEndTime.ToString();

                return Json(new { date = endTime, OTRWNotes = notes }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        //GET: checking Job Status while converting 
        /// <summary>
        ///GetJobStatus
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public ActionResult GetJobStatus(string JobId, string ChangeJobType)
        {
            try
            {
                using (JobRepository)
                {
                    int result = 0;

                    Guid id = Guid.Parse(JobId);
                    //AspNetUsers EmpDetail = NetUsers.FindBy(user => user.UserName == UserName).FirstOrDefault();
                    var JobDetail = JobRepository.FindBy(i => i.Id == id).FirstOrDefault();
                    if (JobDetail != null)
                    {
                        int jobtype = Convert.ToInt32(ChangeJobType);
                        var Jobswithjobno = JobRepository.FindBy(i => i.JobNo == JobDetail.JobNo && i.JobType == jobtype && i.IsDelete == false).FirstOrDefault();

                        if ((JobDetail.IsApproved == null || JobDetail.IsApproved == false) && JobDetail.JobType == 1)
                        {
                            result = 1;
                        }
                        else if (Jobswithjobno != null)
                        {
                            result = 2;
                        }
                        //else if (JobDetail.JobType != 1 && Jobswithjobno == null)
                        //{
                        //    result = 0;
                        //}

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //before accept quote generate pdf 
        public ActionResult QuoteDataPdfGenerate(Guid JobId)
        {
            int CustomerType = 0;
            string CustomerName = "";
            string siteAddress = "";

            List<InvoicePaymentList> QuotepaymentInfo = new List<InvoicePaymentList>();
            Invoice InvoiceData = InvoiceRepo.FindBy(m => m.EmployeeJobId == JobId && m.InvoiceType == "Quote").FirstOrDefault();
            if (InvoiceData != null)
            {
                var customerGeneralInfoId = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                var customerGeneralInfoData = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
                // mapping entity to viewmodel
                CommonMapper<Invoice, CreateInvoiceViewModel> mapper = new CommonMapper<Invoice, CreateInvoiceViewModel>();
                CreateInvoiceViewModel invoiceViewModel = new CreateInvoiceViewModel();
                invoiceViewModel = mapper.Mapper(InvoiceData);
                invoiceViewModel.InvcDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : string.Empty;

                var customer = CustomerGeneralInfoRepo.FindBy(i => i.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

                invoiceViewModel.TradeName = "";
                if (customer != null)
                {
                    CustomerType = customer.CustomerType.HasValue ? Convert.ToInt32(customer.CustomerType) : 0;
                }

                var jobdetail = JobRepository.FindBy(m => m.Id == InvoiceData.EmployeeJobId).FirstOrDefault();
                var sitedetail = CustomerSiteDetailRepo.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
                if (jobdetail != null)
                {
                    invoiceViewModel.WorkOrderNumber = !String.IsNullOrEmpty(jobdetail.WorkOrderNumber) ? jobdetail.WorkOrderNumber : "";
                }
                else
                {
                    invoiceViewModel.WorkOrderNumber = "";
                }
                //var billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
                dynamic billingdetail = null;
                var BillingAddressId = InvoiceData.BillingAddressId;
                if (BillingAddressId != null)
                {
                    if (BillingAddressId != Guid.Empty)
                    {
                        billingdetail = CustomerBilling.FindBy(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                    }
                    else
                    {
                        billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                    }
                }
                else
                {
                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                }

                if (billingdetail == null)
                {

                    billingdetail = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

                }
                if (customer != null)
                {
                    var TradingName = customer.TradingName;
                    if (!string.IsNullOrEmpty(TradingName))
                    {
                        invoiceViewModel.TradeName = !String.IsNullOrEmpty(customer.TradingName) ? customer.TradingName : "";

                    }
                    CustomerName = !String.IsNullOrEmpty(customer.CustomerLastName) ? customer.CustomerLastName : "";
                }

                invoiceViewModel.DisplaysiteAddress =
                                                   // (!string.IsNullOrEmpty(CustomerName) ? CustomerName + "\n" : "") +
                                                   (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                                  (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                                  (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                                  (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                                  (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                if (billingdetail == null)
                {

                    invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else
                {

                    invoiceViewModel.DisplayBillingAddress =
                                                            //(!string.IsNullOrEmpty(invoiceViewModel.TradeName) ? invoiceViewModel.TradeName + "\n" : "") +
                                                            (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                            //(!string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.LastName + "\n" : "") +
                                                            (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                            (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                            (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                            (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                            (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");

                }
                if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Domestic))
                {
                    if (billingdetail == null)
                        invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                }
                else if (CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.RealState) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Strata) || CustomerType == Convert.ToInt32(FSMConstant.Constant.CustomerType.Commercial))
                {
                    invoiceViewModel.DisplaysiteAddress =

                                                 (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                                (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + "\n" : "") +
                                                (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                                (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                                (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                    if (billingdetail == null)
                    {
                        invoiceViewModel.DisplayBillingAddress = invoiceViewModel.DisplaysiteAddress;
                    }
                    else
                    {
                        invoiceViewModel.DisplayBillingAddress = (!string.IsNullOrEmpty(invoiceViewModel.TradeName) ? invoiceViewModel.TradeName + "\n" : "") +
                             (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + "\n" : billingdetail.FirstName + "\n" : billingdetail.FirstName + "\n") +
                                                             (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + "\n" : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                             (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                        invoiceViewModel.DisplaysiteAddress = (!string.IsNullOrEmpty(sitedetail.StrataPlan) ? "SP:" + sitedetail.StrataPlan + "\n" : "") +
                                                                    invoiceViewModel.DisplaysiteAddress;
                    }
                }



                if (InvoiceData.CreatedBy != null)
                {
                    invoiceViewModel.PreparedBy = EmployeeRepo.FindBy(i => i.EmployeeId == InvoiceData.CreatedBy).Select(m => m.FirstName + " " + m.LastName).FirstOrDefault();
                }
                //Get Jcl Item Total Price
                JCLItems jclob = new JCLItems();
                //var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoiceData.EmployeeJobId).ToList();

                var invoiceid = invoiceJCLItemRepo.FindBy(i => i.InvoiceId == InvoiceData.Id).ToList();
                var InvoiceJCLItemlist = (invoiceid.Count > 0) ? invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId && i.InvoiceId == InvoiceData.Id).ToList().OrderBy(i => i.OrderNo) : invoiceJCLItemRepo.FindBy(i => i.JobID == InvoiceData.EmployeeJobId).ToList().OrderBy(i => i.OrderNo);
                List<JcLViewModel> items = new List<JcLViewModel>();
                foreach (var i in InvoiceJCLItemlist)
                {
                    JcLViewModel item = new JcLViewModel();
                    var jclitemifo = JCLRepo.FindBy(k => k.JCLId == i.JCLItemID).FirstOrDefault();
                    item.DefaultQty = i.Quantity;
                    item.Description = i.Description;
                    item.Price = i.Price;
                    item.TotalPrice = Convert.ToInt32(item.DefaultQty) * Convert.ToDecimal(item.Price);
                    items.Add(item);
                }
                jclob.JCLInfo = items;
                //subtotal
                decimal subtotal = Convert.ToDecimal(items.Sum(i => i.TotalPrice));
                invoiceViewModel.Price = subtotal;
                invoiceViewModel.PriceWithGst = (subtotal + (subtotal * 10) / 100);
                //  invoiceViewModel.BalanceDue = (invoiceViewModel.PriceWithGst) - (invoiceViewModel.PriceWithGst * (InvoiceData.AmountPaid)) / 100;
                if (invoiceViewModel.Paid != null)
                {
                    if (invoiceViewModel.Paid > 0)
                    {
                        invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst - invoiceViewModel.Paid;
                    }
                    else
                    {
                        invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
                    }
                }
                else
                {
                    invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
                }
                invoiceViewModel.AmountPay = InvoiceData.AmountPaid;
                invoiceViewModel.GST = Math.Round((Convert.ToDecimal(subtotal * 10) / 100), 2);
                if (invoiceViewModel.Paid != null)
                {
                    if (invoiceViewModel.Paid > 0)
                    {
                        invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst - invoiceViewModel.Paid;
                    }
                    else
                    {
                        invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
                    }
                }
                else
                {
                    invoiceViewModel.BalanceDue = invoiceViewModel.PriceWithGst;
                }
                //Quote payment history

                var QuotePaymenthistory = InvoicePaymentRepo.FindBy(i => i.InvoiceId == InvoiceData.Id).ToList();
                foreach (var payment in QuotePaymenthistory)
                {
                    InvoicePaymentList paymentinfo = new InvoicePaymentList();
                    paymentinfo.Id = payment.Id;
                    paymentinfo.PaymentDate = payment.PaymentDate;
                    paymentinfo.payment_Date = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    paymentinfo.PaymentAmount = payment.PaymentAmount;
                    if (payment.PaymentMethod != null)
                    {
                        Constant.PaymentMethod Durations = (Constant.PaymentMethod)payment.PaymentMethod;
                        var displaynamee = Durations.GetAttribute<DisplayAttribute>();
                        if (displaynamee != null)
                            paymentinfo.payment_Method = displaynamee.Name.ToString();
                    }
                    paymentinfo.Reference = payment.Reference;
                    paymentinfo.PaymentNote = payment.PaymentNote;
                    QuotepaymentInfo.Add(paymentinfo);

                }

                var custoemrInvoiceListViewmodel = new CustoemrInvoiceListViewmodel
                {
                    JclMappingViewModel = jclob,
                    createInvoiceViewModel = invoiceViewModel,
                    InvoicePaymentViewModel = QuotepaymentInfo
                };

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " export pdf of invoice");

                //invoice Root Path
                var root = Server.MapPath("~/QuoteExportFile/");
                var pdfname = "";
                if (InvoiceData.InvoiceType == "Quote")
                {

                    pdfname = String.Format("{0}.pdf", "Quote_No_" + InvoiceData.InvoiceNo);
                }
                else
                {
                    pdfname = String.Format("{0}.pdf", "Invoice_No_" + InvoiceData.InvoiceNo);
                }
                //var pdfname = String.Format("{0}.pdf", InvoiceId.ToString());
                var path = Path.Combine(root, pdfname);
                //  path = Path.GetFullPath(path);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                if (jobdetail.JobType == 1)
                {
                    var Quote = new Rotativa.ViewAsPdf("QuoteExportPreview", custoemrInvoiceListViewmodel)
                    {
                        FileName = "JobQuote.pdf",
                        SaveOnServerPath = path,
                    };
                    CustomerContactLog customerContactLog = new CustomerContactLog();
                    customerContactLog.CustomerContactId = Guid.NewGuid();
                    customerContactLog.CustomerGeneralInfoId = jobdetail.CustomerGeneralInfoId;
                    customerContactLog.CustomerId = (customerGeneralInfoData.CTId).ToString();
                    customerContactLog.JobId = jobdetail.Id.ToString();
                    customerContactLog.Note = "<a href='http://www.srag-portal.com/QuoteExportFile/" + pdfname + "' target=_blank>" + pdfname + "</a>";
                    customerContactLog.LogDate = DateTime.Now;
                    customerContactLog.IsDelete = false;
                    customerContactLog.CreatedDate = DateTime.Now;
                    customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);

                    CustomercontactLogRepo.Add(customerContactLog);
                    CustomercontactLogRepo.Save();
                    return Quote;
                }

                var something = new Rotativa.ViewAsPdf("InvoiceExportPreview", custoemrInvoiceListViewmodel)
                {
                    FileName = "JobInvoice.pdf",
                    SaveOnServerPath = path,
                };
                return something;

            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
        private void SaveDataContactLog(Guid JobId, string Notes)
        {
            var getJobData = JobRepository.FindBy(m => m.Id == JobId).FirstOrDefault();
            var getCustomerData = CustomerGeneralInfoRepo.FindBy(m => m.CustomerGeneralInfoId == getJobData.CustomerGeneralInfoId).FirstOrDefault();

            var ContactlogExist = CustomercontactLogRepo.FindBy(i => i.JobId == JobId.ToString() && i.CustomerGeneralInfoId == getJobData.CustomerGeneralInfoId).FirstOrDefault();

            if (ContactlogExist == null)
            {

                CustomerContactLog customerContactLog = new CustomerContactLog();
                customerContactLog.CustomerContactId = Guid.NewGuid();
                customerContactLog.CustomerGeneralInfoId = getJobData.CustomerGeneralInfoId;
                customerContactLog.CustomerId = getCustomerData.CTId.ToString();
                customerContactLog.JobId = JobId.ToString();

                customerContactLog.LogDate = DateTime.Now;
                customerContactLog.Note = Notes;
                customerContactLog.IsDelete = false;
                customerContactLog.IsReminder = false;
                customerContactLog.IsScheduler = false;
                customerContactLog.CreatedDate = DateTime.Now;
                customerContactLog.CreatedBy = Guid.Parse(base.GetUserId);
                CustomercontactLogRepo.Add(customerContactLog);
                CustomercontactLogRepo.Save();
            }
            else
            {
                ContactlogExist.ModifiedDate = DateTime.Now;
                ContactlogExist.ModifiedBy = Guid.Parse(base.GetUserId);
                ContactlogExist.Note = Notes;
                CustomercontactLogRepo.Save();
            }
        }
    }
}