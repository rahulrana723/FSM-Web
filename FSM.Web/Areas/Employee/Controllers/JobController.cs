using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using FSM.Web.FSMConstant;
using log4net;
using Microsoft.Practices.Unity;
using Rotativa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace FSM.Web.Areas.Employee
{
    [Authorize]
    public class JobController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                     ().DeclaringType);
        [Dependency]
        public ICustomerGeneralInfoRepository Customers { get; set; }

        [Dependency]
        public ISupportdojobMapping SupportJoblink { get; set; }

        [Dependency]
        public ISupplier Supplier { get; set; }

        [Dependency]
        public IPurchaseOrderByJobRepository JobPurchaseOrder { get; set; }

        [Dependency]
        public IPurchaseorderItemJobRepository JobPurchaseOrderitem { get; set; }

        [Dependency]
        public ICustomerSiteDetailRepository CustomerSiteDetail { get; set; }

        [Dependency]
        public ICustomerResidenceDetailRepository CustomerResidenceDetail { get; set; }
        [Dependency]
        public ICustomerContactsRepository Customercontacts { get; set; }

        [Dependency]
        public ICustomerConditionReportRepository CustomerconditionDetail { get; set; }

        [Dependency]
        public ICustomerSiteDocumentsRepository CustomerSiteDocuments { get; set; }

        [Dependency]
        public IEmployeeJobRepository Employeejob { get; set; }

        [Dependency]
        public IiNoviceRepository InvoiceRep { get; set; }

        [Dependency]
        public IEmployeeJobDocumentRepository EmployeeJobDoc { get; set; }

        [Dependency]
        public IStockRepository Stock { get; set; }

        [Dependency]
        public IJobStockRepository JobStock { get; set; }

        [Dependency]
        public IEmployeeDetailRepository Employee { get; set; }
        [Dependency]
        public IAspNetUsersRepository AspNetUser { get; set; }
        [Dependency]
        public IUserTimeSheetRepository TimeSheet { get; set; }
        [Dependency]

        public ISupportJobRepository SupportJob { get; set; }
        [Dependency]
        public IiNoviceRepository Invoice { get; set; }
        [Dependency]
        public IJobAssignToMappingRepository JobAssignMapping { get; set; }

        // GET: Employee/Job
        /// <summary>
        /// GEt Job list 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        //GET: Employee/Job/AddEmployeeJobs
        /// <summary>
        /// Add Employee Jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddEmployeeJobs()
        {
            try
            {
                EmployeeJobsViewModel employeeJobsViewModel = new EmployeeJobsViewModel();
                employeeJobsViewModel.GetUserRoles = this.GetUserRoles;
                if (employeeJobsViewModel.GetUserRoles[0] == "ACCOUNTS")
                {
                    employeeJobsViewModel.Status = (Constant.JobStatus)1; // setting default for Account user
                }
                employeeJobsViewModel.BookedBy = Guid.Parse(base.GetUserId);

                using (AspNetUser)
                {
                    var User = AspNetUser.FindBy(i => i.Id == employeeJobsViewModel.BookedBy.ToString());
                    employeeJobsViewModel.BookedByName = User.FirstOrDefault().UserName;
                }

                employeeJobsViewModel.JobId = Employeejob.GetMaxJobID();

                //Customer list binding
                employeeJobsViewModel.CustomerCOLastName = GetCustomerList();
                //OTRW Employee Binding
                employeeJobsViewModel.OTRWList = GetOtrwEmployeesList();
                employeeJobsViewModel.JobList = GetJoblist();
                List<SiteDetail> listsites = new List<SiteDetail>();
                employeeJobsViewModel.LstCustomerSiteDetail = listsites;
                TempData["Message"] = 0;
                return View(employeeJobsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Employeejob.Dispose();
            }
        }
        //POST: Employee/Job/AddEmployeeJobs
        /// <summary>
        /// Add Employee sJobs
        /// </summary>
        /// <param name="employeeJobsViewModel"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEmployeeJobs(EmployeeJobsViewModel employeeJobsViewModel, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);

                using (AspNetUser)
                {
                    var User = AspNetUser.FindBy(i => i.Id == employeeJobsViewModel.BookedBy.ToString());
                    if (User != null) { employeeJobsViewModel.BookedByName = User.FirstOrDefault().UserName; }
                }
                if (this.GetUserRoles[0] == "ACCOUNTS")
                {
                    employeeJobsViewModel.Status = (Constant.JobStatus)1; // setting default for Account user
                    employeeJobsViewModel.JobType = (Constant.JobType)employeeJobsViewModel.AccountJobType;
                }

                if (ModelState.IsValid)
                {

                    employeeJobsViewModel.Id = Guid.NewGuid();
                    if (employeeJobsViewModel.JobType == FSMConstant.Constant.JobType.Support && employeeJobsViewModel.SupportjobSiteId != null)
                    {
                        employeeJobsViewModel.SiteId = employeeJobsViewModel.SupportjobSiteId;
                    }
                    employeeJobsViewModel.IsDelete = false;
                    employeeJobsViewModel.CreatedDate = DateTime.Now;
                    employeeJobsViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                    CommonMapper<EmployeeJobsViewModel, Jobs> mapper = new CommonMapper<EmployeeJobsViewModel, Jobs>();
                    Jobs employeejob = mapper.Mapper(employeeJobsViewModel);
                    Employeejob.Add(employeejob);
                    Employeejob.Save();
                    using (EmployeeJobDoc)
                    {
                        for (int i = 0; i < file.Count(); i++)
                        {
                            var File = Request.Files[i];
                            if (File != null && File.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(File.FileName);
                                string Jobid = employeeJobsViewModel.Id.ToString();
                                Directory.CreateDirectory(Server.MapPath("~/Images/EmployeeJobs/" + Jobid));
                                File.SaveAs(Path.Combine(Server.MapPath("~/Images/EmployeeJobs/" + Jobid), fileName));
                                EmployeeJobDocumentViewModel employeeJobDocumentViewModel = new EmployeeJobDocumentViewModel();
                                employeeJobDocumentViewModel.CreatedDate = DateTime.Now;
                                employeeJobDocumentViewModel.DocName = fileName.ToString();
                                employeeJobDocumentViewModel.Id = Guid.NewGuid();
                                employeeJobDocumentViewModel.JobId = employeeJobsViewModel.Id;
                                CommonMapper<EmployeeJobDocumentViewModel, JobDocuments> mapperdoc = new CommonMapper<EmployeeJobDocumentViewModel, JobDocuments>();
                                JobDocuments employeeJobDocuments = mapperdoc.Mapper(employeeJobDocumentViewModel);
                                EmployeeJobDoc.Add(employeeJobDocuments);
                                EmployeeJobDoc.Save();
                            }
                        }
                    }

                    //check if support job linking 
                    if (employeeJobsViewModel.LinkedJobId != null && employeeJobsViewModel.LinkedJobId != Guid.Empty)
                    {

                        var jobStatus = SupportJoblink.FindBy(i => i.LinkedJobId == employeeJobsViewModel.Id).FirstOrDefault();
                        if (jobStatus != null)
                        {
                            jobStatus.ModifiedBy = Guid.Parse(base.GetUserId);
                            jobStatus.ModifiedDate = DateTime.Now;
                            SupportJoblink.Edit(jobStatus);
                            SupportJoblink.Save();

                        }
                        else
                        {
                            SupportdojobMappingViewModel
                                jobstatusmodel = new SupportdojobMappingViewModel();
                            jobstatusmodel.ID = Guid.NewGuid();
                            jobstatusmodel.LinkedJobId = employeeJobsViewModel.LinkedJobId;
                            jobstatusmodel.SupportJobId = employeeJobsViewModel.Id;
                            jobstatusmodel.CreatedBy = Guid.Parse(base.GetUserId);
                            jobstatusmodel.CreatedDate = DateTime.Now;
                            CommonMapper<SupportdojobMappingViewModel, SupportdojobMapping> mapperlinkjob = new CommonMapper<SupportdojobMappingViewModel, SupportdojobMapping>();
                            SupportdojobMapping employeeJoblink = mapperlinkjob.Mapper(jobstatusmodel);
                            SupportJoblink.Add(employeeJoblink);
                            SupportJoblink.Save();
                        }
                    }

                    List<SiteDetail> listsites = new List<SiteDetail>();
                    employeeJobsViewModel.LstCustomerSiteDetail = BindsiteDetail(employeeJobsViewModel.CustomerGeneralInfoId.ToString()); ;
                    employeeJobsViewModel.CustomerCOLastName = GetCustomerList();
                    employeeJobsViewModel.OTRWList = GetOtrwEmployeesList();
                    TempData["Message"] = 1;

                    employeeJobsViewModel.GetUserRoles = this.GetUserRoles;
                    if (employeeJobsViewModel.GetUserRoles[0] == "ACCOUNTS")
                    {
                        employeeJobsViewModel.Status = (Constant.JobStatus)1; // setting default for Account user
                    }

                    employeeJobsViewModel.JobList = GetJoblist();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added employee job.");

                    return RedirectToAction("EditEmployeeJob", new { id = Convert.ToString(employeeJobsViewModel.Id) });
                }
                else
                {
                    employeeJobsViewModel.GetUserRoles = this.GetUserRoles;
                    if (employeeJobsViewModel.GetUserRoles[0] == "ACCOUNTS")
                    {
                        employeeJobsViewModel.Status = (Constant.JobStatus)1; // setting default for Account user
                    }
                    List<SiteDetail> listsites = new List<SiteDetail>();
                    employeeJobsViewModel.LstCustomerSiteDetail = BindsiteDetail(employeeJobsViewModel.CustomerGeneralInfoId.ToString());
                    employeeJobsViewModel.CustomerCOLastName = GetCustomerList();
                    employeeJobsViewModel.OTRWList = GetOtrwEmployeesList();
                    employeeJobsViewModel.JobList = GetJoblist();
                    return View(employeeJobsViewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET: Employee/Job/GetSiteDetailById
        /// <summary>
        /// Get Site Detail By CustomerGeneralinfoId
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSiteDetailById(string CustomerGeneralinfoid)
        {
            try
            {
                Guid Customerid = Guid.Parse(CustomerGeneralinfoid);
                using (CustomerSiteDetail)
                {
                    List<SiteDetail> li = new List<SiteDetail>();
                    var Customersitedetail = CustomerSiteDetail.FindBy(i => i.CustomerGeneralInfoId == Customerid);
                    foreach (var cust in Customersitedetail)
                    {
                        if (!string.IsNullOrEmpty(cust.StreetName))
                        {
                            SiteDetail obj = new SiteDetail();
                            obj.SiteName = cust.StreetName;
                            obj.SitesId = cust.SiteDetailId;
                            li.Add(obj);
                        }
                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(li);
                    return Json(new { list = json, length = li.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET: Employee/Job/ViewEmployeeJobs
        /// <summary>
        /// View Employee Jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewEmployeeJobs()
        {
            try
            {
                using (Employeejob)
                {

                    string keyword = string.IsNullOrEmpty(Request.QueryString["Keyword"]) ? "" :
                                                (Request.QueryString["Keyword"]);           //get search keyword
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        keyword = keyword.Trim();
                    }

                    Nullable<int> JobType = string.IsNullOrEmpty(Request.QueryString["JobType"]) ? (int?)null :
                                                 Convert.ToInt32(Request.QueryString["JobType"]);     //get job type value

                    Nullable<bool> Contracted = string.IsNullOrEmpty(Request.QueryString["Contracted"]) ? (bool?)null :
                                             Convert.ToBoolean(Request.QueryString["Contracted"]);    //get contracted check unchecked


                    var CustomerId = Request.RequestContext.RouteData.Values["id"] != null ?
                                     Guid.Parse(Request.RequestContext.RouteData.Values["id"].ToString()) : Guid.Empty;   //get customer general info id

                    var Employeelist = Employeejob.GetEmployeeJobsWithKeyword(keyword,JobType,Contracted, CustomerId).ToList();
                    


                    //Nullable<bool> SOL = string.IsNullOrEmpty(Request.QueryString["SOL"]) ? (bool?)null :
                    //                             Convert.ToBoolean(Request.QueryString["SOL"]);
                    //Nullable<bool> LNC = string.IsNullOrEmpty(Request.QueryString["LNC"]) ? (bool?)null :
                    //                             Convert.ToBoolean(Request.QueryString["LNC"]);
                    //Nullable<bool> UnsentInv = string.IsNullOrEmpty(Request.QueryString["UnsentInv"]) ? (bool?)null :
                    //                             Convert.ToBoolean(Request.QueryString["UnsentInv"]);

                  


                    var CustomerName = Customers.FindBy(m => m.CustomerGeneralInfoId == CustomerId).Select(m => m.CustomerLastName).FirstOrDefault();
                    if (CustomerId != null)
                    {
                        ViewBag.CustomerId = CustomerId;
                        ViewBag.CustomerName = CustomerName;
                    }

                    

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    EmployeejobSearchViewModel employeejobSearchViewModel = new EmployeejobSearchViewModel
                    {
                        JobType = JobType.HasValue ? (Constant.JobType)JobType : 0,
                        //SOL = SOL.HasValue ? (bool)SOL : false,
                        //LNC = LNC.HasValue ? (bool)LNC : false,
                        //UnsentInv = UnsentInv.HasValue ? (bool)UnsentInv : false,
                        TotalCount= Employeelist.Count>0?Employeelist.FirstOrDefault().TotalCount:0,
                        Contracted = Contracted.HasValue ? (bool)Contracted : false,
                        Keyword = keyword,
                        PageSize = PageSize
                    };

                                    
                    //// mapping list<entity> to list<viewmodel>
                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(Employeelist.OrderByDescending(i => i.JobNo).ToList());
                    if (EmployeejobViewModel.Count > 0)
                    {
                        foreach (EmployeeJobsViewModel Employee in EmployeejobViewModel)
                        {
                            Employee.DisplayJobType = (int)Employee.JobType > 0 ? Convert.ToString((Constant.JobType)Employee.JobType) : " ";
                            Employee.DisplayStatus = Convert.ToString((Constant.JobStatus)Employee.Status) == "OnHold" ? "Stand-By" : Convert.ToString((Constant.JobStatus)Employee.Status);
                        }
                    }

                    var customerList = Customers.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.CustomerLastName,
                        Value = m.CustomerGeneralInfoId.ToString()
                    }).ToList();

                    employeejobSearchViewModel.CustomerList = customerList;

                    var Employeelistviewmodel = new EmployeeJobListViewModel
                    {
                        EmployeeJoblist = EmployeejobViewModel,
                        Employeejobsearchmodel = employeejobSearchViewModel,
                        CustomerName = CustomerName
                    };
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed employee job list.");
                    return View(Employeelistviewmodel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //POST: Employee/Job/ViewEmployeeJobs
        /// <summary>
        /// View  Jobs lists 
        /// </summary>
        /// <param name="employeesearchviewmodel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewEmployeeJobs(EmployeejobSearchViewModel employeesearchviewmodel)

        {
            try
            {
                using (Employeejob)
                {
                    string Keyword = employeesearchviewmodel.Keyword;  
                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        Keyword = employeesearchviewmodel.Keyword.Trim();
                    }

                    int jobType = (int)employeesearchviewmodel.JobType;

                    bool contracted = employeesearchviewmodel.Contracted;

                    Guid customerId = !string.IsNullOrEmpty(employeesearchviewmodel.CustomerInfoId) ? Guid.Parse(employeesearchviewmodel.CustomerInfoId) : Guid.Empty;

                    var EmployeeList = Employeejob.GetEmployeeJobsWithKeyword(Keyword,jobType,contracted,customerId);

                    //if (customerId != Guid.Empty)
                    //{
                    //    EmployeeList = EmployeeList.Where(customer => customer.CustomerGeneralInfoId == customerId);
                    //}

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(EmployeeList.ToList());
                    if (EmployeejobViewModel.Count > 0)
                    {
                        foreach (EmployeeJobsViewModel Employee in EmployeejobViewModel)
                        {
                            Employee.DisplayJobType = (int)Employee.JobType > 0 ? Convert.ToString((Constant.JobType)Employee.JobType) : " ";
                            Employee.DisplayStatus = Convert.ToString((Constant.JobStatus)Employee.Status) == "OnHold" ? "Stand-By" : Convert.ToString((Constant.JobStatus)Employee.Status);
                        }
                    }

                    var customerList = Customers.GetAll().Select(m => new SelectListItem()
                    {
                        Text = m.CustomerLastName,
                        Value = m.CustomerGeneralInfoId.ToString()
                    }).ToList();

                    employeesearchviewmodel.CustomerList = customerList;
                    if (EmployeejobViewModel.Count > 0)
                    {
                        employeesearchviewmodel.TotalCount = EmployeeList.FirstOrDefault().TotalCount;
                    }

                    var EmployeejobListViewModel = new EmployeeJobListViewModel
                    {
                        EmployeeJoblist = EmployeejobViewModel,
                        Employeejobsearchmodel = employeesearchviewmodel,
                    };
                    return View(EmployeejobListViewModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SiteDetail> BindsiteDetail(string CustomerGeneralinfoid)
        {
            try
            {
                Guid id = Guid.Parse(CustomerGeneralinfoid);
                List<SiteDetail> li = new List<SiteDetail>();
                var Customersitedetail = CustomerSiteDetail.FindBy(i => i.CustomerGeneralInfoId == id);
                foreach (var cust in Customersitedetail)
                {
                    if (!string.IsNullOrEmpty(cust.StreetName))
                    {
                        SiteDetail obj = new SiteDetail();
                        obj.SiteName = cust.StreetName;
                        obj.SitesId = cust.SiteDetailId;
                        li.Add(obj);
                    }
                }
                return li;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/Job/EditEmployeejob
        /// <summary>
        /// Edit Employee Job
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditEmployeeJob(string id)
        {
            try
            {
                Guid jobid;
                string Empid = string.Empty;
                Guid.TryParse(id, out jobid);
                EmployeeJobsViewModel employeeJobsViewModel = new EmployeeJobsViewModel();

                Jobs employeejob = Employeejob.FindBy(m => m.Id == jobid).FirstOrDefault();
                // mapping entity to viewmodel
                Empid = employeejob.BookedBy.ToString();
                CommonMapper<Jobs, EmployeeJobsViewModel> mapper = new CommonMapper<Jobs, EmployeeJobsViewModel>();
                employeeJobsViewModel = mapper.Mapper(employeejob);
                List<SiteDetail> listsites = new List<SiteDetail>();
                employeeJobsViewModel.LstCustomerSiteDetail = BindsiteDetail(employeejob.CustomerGeneralInfoId.ToString()); ;
                employeeJobsViewModel.CustomerCOLastName = GetCustomerList();
                employeeJobsViewModel.OTRWList = GetOtrwEmployeesList();
                employeeJobsViewModel.GetUserRoles = this.GetUserRoles;
                employeeJobsViewModel.JobList = GetJoblist();
                //check if job is support job 
                if (employeeJobsViewModel.JobType == FSM.Web.FSMConstant.Constant.JobType.Support)
                {

                    var joblinking = SupportJoblink.FindBy(i => i.SupportJobId == jobid).FirstOrDefault();
                    if (joblinking != null)
                    {
                        employeeJobsViewModel.LinkedJobId = joblinking.LinkedJobId;
                        var jobId = Employeejob.FindBy(i => i.Id == employeeJobsViewModel.LinkedJobId).FirstOrDefault().JobId;
                        employeeJobsViewModel.JobList.Insert(0, new Joblist { Jobid = Guid.Parse(joblinking.LinkedJobId.ToString()), jobnumeric = "Job_" + jobId });
                    }
                }
                if (this.GetUserRoles[0] == "ACCOUNTS")
                {
                    employeeJobsViewModel.AccountJobType = (Constant.AccountJobType)employeeJobsViewModel.JobType;
                }
                var User = AspNetUser.FindBy(i => i.Id == Empid.ToString());
                if (User != null) { employeeJobsViewModel.BookedByName = User.FirstOrDefault().UserName; }
                TempData["Message"] = 0;


                return View(employeeJobsViewModel);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

                AspNetUser.Dispose();
                Employeejob.Dispose();
                SupportJoblink.Dispose();
            }
        }

        //POST: Employee/Job/EditEmployeejob
        /// <summary>
        /// Edit Employee job
        /// </summary>
        /// <param name="employeeJobsViewModel"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEmployeeJob(EmployeeJobsViewModel employeeJobsViewModel, IEnumerable<HttpPostedFileBase> file)
        {
            employeeJobsViewModel.GetUserRoles = this.GetUserRoles;
            if (this.GetUserRoles[0] == "ACCOUNTS")
            {
                employeeJobsViewModel.Status = (Constant.JobStatus)1; // setting default for Account user
                employeeJobsViewModel.JobType = (Constant.JobType)employeeJobsViewModel.AccountJobType;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (Employeejob)
                    {
                        employeeJobsViewModel.ModifiedDate = DateTime.Now;
                        employeeJobsViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                        if (employeeJobsViewModel.JobType == FSMConstant.Constant.JobType.Support && employeeJobsViewModel.SupportjobSiteId != null)
                        {
                            employeeJobsViewModel.SiteId = employeeJobsViewModel.SupportjobSiteId;
                        }
                        // mapping entity to viewmodel
                        CommonMapper<EmployeeJobsViewModel, Jobs> mapper = new CommonMapper<EmployeeJobsViewModel, Jobs>();
                        Jobs employeejobs = mapper.Mapper(employeeJobsViewModel);
                        Employeejob.Edit(employeejobs);
                        Employeejob.Save();
                        using (EmployeeJobDoc)
                        {
                            for (int i = 0; i < file.Count(); i++)
                            {
                                var File = Request.Files[i];
                                if (File != null && File.ContentLength > 0)
                                {
                                    var fileName = Path.GetFileName(File.FileName);
                                    string Jobid = employeeJobsViewModel.Id.ToString();

                                    //check directory  exists or not 
                                    if (Directory.Exists(Server.MapPath("~/Images/EmployeeJobs/" + Jobid)))
                                    {
                                        File.SaveAs(Path.Combine(Server.MapPath("~/Images/EmployeeJobs/" + Jobid), fileName));
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(Server.MapPath("~/Images/EmployeeJobs/" + Jobid));
                                        File.SaveAs(Path.Combine(Server.MapPath("~/Images/EmployeeJobs/" + Jobid), fileName));
                                    }
                                    EmployeeJobDocumentViewModel employeeJobDocumentViewModel = new EmployeeJobDocumentViewModel();
                                    employeeJobDocumentViewModel.CreatedDate = DateTime.Now;
                                    employeeJobDocumentViewModel.DocName = fileName.ToString();
                                    employeeJobDocumentViewModel.Id = Guid.NewGuid();
                                    employeeJobDocumentViewModel.JobId = employeeJobsViewModel.Id;
                                    CommonMapper<EmployeeJobDocumentViewModel, JobDocuments> mapperdoc = new CommonMapper<EmployeeJobDocumentViewModel, JobDocuments>();
                                    JobDocuments employeeJobDocuments = mapperdoc.Mapper(employeeJobDocumentViewModel);
                                    EmployeeJobDoc.Add(employeeJobDocuments);
                                    EmployeeJobDoc.Save();
                                }
                            }
                        }

                        //check if support job linking 
                        if (employeeJobsViewModel.LinkedJobId != null && employeeJobsViewModel.LinkedJobId != Guid.Empty)
                        {
                            using (SupportJoblink)
                            {
                                var jobStatus = SupportJoblink.FindBy(i => i.SupportJobId == employeeJobsViewModel.Id).FirstOrDefault();
                                if (jobStatus != null)
                                {
                                    jobStatus.ModifiedBy = Guid.Parse(base.GetUserId);
                                    jobStatus.ModifiedDate = DateTime.Now;
                                    jobStatus.LinkedJobId = employeeJobsViewModel.LinkedJobId;
                                    SupportJoblink.Edit(jobStatus);
                                    SupportJoblink.Save();

                                }
                                else
                                {
                                    SupportdojobMappingViewModel
                                        jobstatusmodel = new SupportdojobMappingViewModel();
                                    jobstatusmodel.ID = Guid.NewGuid();
                                    jobstatusmodel.LinkedJobId = employeeJobsViewModel.LinkedJobId;
                                    jobstatusmodel.SupportJobId = employeeJobsViewModel.Id;
                                    jobstatusmodel.CreatedBy = Guid.Parse(base.GetUserId);
                                    jobstatusmodel.CreatedDate = DateTime.Now;
                                    CommonMapper<SupportdojobMappingViewModel, SupportdojobMapping> mapperlinkjob = new CommonMapper<SupportdojobMappingViewModel, SupportdojobMapping>();
                                    SupportdojobMapping employeeJoblink = mapperlinkjob.Mapper(jobstatusmodel);
                                    SupportJoblink.Add(employeeJoblink);
                                    SupportJoblink.Save();
                                }

                            }
                        }
                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated employee job.");
                        TempData["Message"] = 2;
                        return RedirectToAction("ViewEmployeeJobs");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                List<SiteDetail> listsites = new List<SiteDetail>();
                employeeJobsViewModel.LstCustomerSiteDetail = listsites;
                employeeJobsViewModel.CustomerCOLastName = GetCustomerList();
                employeeJobsViewModel.OTRWList = GetOtrwEmployeesList();
                return View(employeeJobsViewModel);
            }
        }

        /// <summary>
        /// Delete Employee Job By jobid
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Redirects ViewEmployeeJobs</returns>
        [HttpGet]
        public ActionResult DeleteEmployeeJob(string id)
        {
            Guid jobid = Guid.Parse(id);
            try
            {
                //using (EmployeeJobDoc)
                //{
                //    var docs = EmployeeJobDoc.FindBy(i => i.JobId == jobid);
                //    foreach (var doc in docs)
                //    {
                //        EmployeeJobDoc.Delete(doc);
                //    }
                //    EmployeeJobDoc.Save();
                //}
                //using (TimeSheet)
                //{
                //    var timeSheet = TimeSheet.FindBy(m => m.JobId == jobid).ToList();
                //    if (timeSheet != null)
                //    {
                //        foreach (var item in timeSheet)
                //        {
                //            TimeSheet.Delete(item);
                //            TimeSheet.Save();
                //        }
                //    }
                //}
                //deleting the files and folder  of the jobs
                //if (Directory.Exists(Server.MapPath("~/Images/EmployeeJobs/" + jobid)))
                //{
                //    System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Images/EmployeeJobs/" + jobid));
                //    foreach (FileInfo file in di.GetFiles())
                //    {
                //        file.Delete();
                //    }
                //    //delete directory
                //    di.Delete();
                //}
                //using (SupportJoblink)
                //{
                //    SupportdojobMapping supportdojobMapping = SupportJoblink.FindBy(i => i.SupportJobId == jobid).FirstOrDefault();
                //    if (supportdojobMapping != null)
                //    {
                //        SupportJoblink.Delete(supportdojobMapping);
                //        SupportJoblink.Save();
                //    }
                //}

                using (Employeejob)
                {
                    int result = 0;
                    Jobs empjob = Employeejob.FindBy(i => i.Id == jobid).FirstOrDefault();
                    string DisplayStatus = ((Constant.JobStatus)empjob.Status).ToString();

                   
                    if (empjob.Status != 11 || empjob.Status != 15)
                    {
                        //Delete data in job Assign to Maaping
                        var jobAssignEntity = JobAssignMapping.FindBy(m => m.JobId == jobid && m.IsDelete==false).ToList();
                        if (jobAssignEntity != null)
                        {
                            foreach (var assign in jobAssignEntity)
                            {
                                assign.IsDelete = true;
                                JobAssignMapping.Edit(assign);
                                JobAssignMapping.Save();
                            }
                        }

                        //Delete data in jobs
                        result = 0;
                        empjob.IsDelete = true;
                        Employeejob.Edit(empjob);
                        Employeejob.Save();

                        // Delete Invoice While deleting job (19-2-2018 as per discussion with tejpal sir) 
                        FSM.Core.Entities.Invoice invoice = InvoiceRep.FindBy(i => i.EmployeeJobId == jobid).FirstOrDefault();
                        if (invoice != null)
                        {
                            invoice.IsDelete = true;
                            InvoiceRep.Edit(invoice);
                            InvoiceRep.Save();
                        }

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

                
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/Job/CancelJob
        /// <summary>
        /// Canecel Job
        /// </summary>
        /// <returns>Redirects ViewEmployeeJobs</returns>
        [HttpGet]
        public ActionResult CanecelJob()
        {
            try
            {
                TempData["Message"] = "";
                return RedirectToAction("ViewEmployeeJobs");
            }
            catch
            {
            }
            return RedirectToAction("ViewEmployeeJobs");
        }

        //GET:Employee/Job/DeletejobDocumentByDocId
        /// <summary>
        /// Delete job Document By DocId
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeletejobDocumentByDocId(string docid)
        {
            try
            {
                using (EmployeeJobDoc)
                {
                    Guid docmentid = Guid.Parse(docid);
                    var docs = EmployeeJobDoc.FindBy(i => i.Id == docmentid).FirstOrDefault();
                    EmployeeJobDoc.Delete(docs);
                    EmployeeJobDoc.Save();
                    if ((System.IO.File.Exists(Server.MapPath("~/Images/EmployeeJobs/" + docs.JobId + '/' + docs.DocName))))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Images/EmployeeJobs/" + docs.JobId + '/' + docs.DocName));
                    }
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted job document.");
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// ViewJobDocuments
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewJobDocuments(string id)
        {
            try
            {
                Guid jobid = Guid.Parse(id);
                using (EmployeeJobDoc)
                {
                    var docs = EmployeeJobDoc.FindBy(i => i.JobId == jobid).ToList();
                    CommonMapper<JobDocuments, EmployeeJobDocumentViewModel> mapper = new CommonMapper<JobDocuments, EmployeeJobDocumentViewModel>();
                    List<EmployeeJobDocumentViewModel> customerGeneralInfoViewModel = mapper.MapToList(docs.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(customerGeneralInfoViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job document.");
                    return Json(new { list = json, length = customerGeneralInfoViewModel.Count() });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Downloadthe job documents
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        public ActionResult DownloadDocuments(string docid)
        {
            using (EmployeeJobDoc)
            {
                Guid Docid = Guid.Parse(docid);
                var file = EmployeeJobDoc.FindBy(i => i.Id == Docid).FirstOrDefault();
                var FileVirtualPath = Server.MapPath("~/Images/EmployeeJobs/" + file.JobId + '/' + file.DocName);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " download documents.");

                return File(FileVirtualPath, MimeMapping.GetMimeMapping(FileVirtualPath), file.DocName);
            }
        }

        //GET: Employee/Job/Stockinfo
        /// <summary>
        /// StockInfo by jobid
        /// </summary>
        /// <param name="Jobid"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult StockInfo(string Jobid)
        {
            try
            {
                Guid jid = Guid.Parse(Jobid);
                var StockList = Stock.GetAll();
                StockList = StockList.Where(i => i.Available > 0 && i.IsDelete==false);
                var JobStockList = JobStock.GetJobStockList().Where(i => i.Jobid == jid).ToList();
                int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 5 :
                               Convert.ToInt32(Request.QueryString["page_size"]);
                DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                displayJobStocksViewModel.JobId = jid;
                List<StockDetail> li = new List<StockDetail>();
                foreach (var i in StockList)
                {
                    StockDetail obj = new StockDetail();
                    obj.StockID = i.ID;
                    obj.Label = i.Label;
                    li.Add(obj);
                }
                displayJobStocksViewModel.stockDetail = li;
                displayJobStocksViewModel.PageSize = PageSize;

                var displayJobStocksListViewModel = new DisplayJobStocksListViewModel
                {
                    DisplayJobStocksViewModel = displayJobStocksViewModel,
                    DisplayJobStocksList = JobStockList,
                    JobStock = new JobStock()
                };
                return View(displayJobStocksListViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/job/StockInfo
        /// <summary>
        ///Save stockinfo 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects StockInfo</returns>
        [HttpPost]
        public ActionResult StockInfo(DisplayJobStocksListViewModel model)
        {
            if (model != null)
            {
                try
                {
                    DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                    if (ModelState.IsValid)
                    {
                        displayJobStocksViewModel.JobId = model.DisplayJobStocksViewModel.JobId;
                        displayJobStocksViewModel.StockID = model.DisplayJobStocksViewModel.StockID;
                        displayJobStocksViewModel.UnitMeasure = model.DisplayJobStocksViewModel.UnitMeasure;
                        displayJobStocksViewModel.Price = model.DisplayJobStocksViewModel.Price;
                        displayJobStocksViewModel.Quantity = model.DisplayJobStocksViewModel.Quantity;
                        int available = (Convert.ToInt32(model.DisplayJobStocksViewModel.AvailableQuantity) - Convert.ToInt32(displayJobStocksViewModel.Quantity));
                        using (Stock)
                        {
                            var stock = Stock.FindBy(i => i.ID == displayJobStocksViewModel.StockID).FirstOrDefault();
                            stock.Available = available;
                            stock.ModifiedBy = Guid.Parse(base.GetUserId);
                            stock.ModifiedDate = DateTime.Now;
                            Stock.Edit(stock);
                            Stock.Save();
                        }
                        if (model.DisplayJobStocksViewModel.ID == Guid.Empty)
                        {
                            displayJobStocksViewModel.IsDelete = false;
                            displayJobStocksViewModel.CreatedDate = DateTime.Now;
                            displayJobStocksViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            displayJobStocksViewModel.ID = Guid.NewGuid();
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            JobStock.Add(jobStock);
                            JobStock.Save();
                        }
                        else
                        {
                            displayJobStocksViewModel.ID = model.DisplayJobStocksViewModel.ID;
                            displayJobStocksViewModel.ModifiedDate = DateTime.Now;
                            displayJobStocksViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            JobStock.Edit(jobStock);
                            JobStock.Save();
                        }

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " stock info.");

                        return RedirectToAction("StockInfo", new { Jobid = displayJobStocksViewModel.JobId });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("StockInfo", new { Jobid = model.DisplayJobStocksViewModel.JobId });
        }

        //POST: Employee/Job/EditStockInfo
        /// <summary>
        /// EditStockInfo by stockid 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditStockInfo(string Id, string stockID)
        {
            try
            {
                using (JobStock)
                {
                    Guid JobStockID = Guid.Parse(Id);
                    Guid StockID = Guid.Parse(stockID);

                    var stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    int available = Convert.ToInt32(stock.Available);

                    JobStock jobStock = JobStock.FindBy(m => m.ID == JobStockID).FirstOrDefault();
                    int quantityauail = Convert.ToInt32(jobStock.Quantity);

                    // mapping entity to viewmodel
                    CommonMapper<JobStock, DisplayJobStocksViewModel> mapper = new CommonMapper<JobStock, DisplayJobStocksViewModel>();
                    DisplayJobStocksViewModel displayJobStocksViewModel = mapper.Mapper(jobStock);

                    displayJobStocksViewModel.AvailableQuantity = available + quantityauail;

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(displayJobStocksViewModel);
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated stock info.");


                    return Json(new { json, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/Job/DeleteStockInfo
        /// <summary>
        /// DeleteStockInfo By stockid
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="stockID"></param>
        /// <returns>Redirects StockInfo</returns>
        public ActionResult DeleteStockInfo(string Id, string stockID)
        {
            try
            {
                int newavailablestock;
                Guid Jobid;
                Guid jobStockID = Guid.Parse(Id);
                Guid StockID = Guid.Parse(stockID);
                using (JobStock)
                {
                    JobStock jobStock = JobStock.FindBy(m => m.ID == jobStockID).FirstOrDefault();
                    int quantityauail = Convert.ToInt32(jobStock.Quantity);
                    newavailablestock = quantityauail;
                    Jobid = jobStock.JobId;

                    JobStock stockDelete = JobStock.FindBy(i => i.ID == jobStockID).FirstOrDefault();
                    stockDelete.IsDelete = true;
                    JobStock.Edit(stockDelete);
                    JobStock.Save();
                }
                using (Stock)
                {
                    var stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    int available = Convert.ToInt32(stock.Available);
                    stock.Available = available + newavailablestock;
                    Stock.Edit(stock);
                    Stock.Save();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted stock info.");

                return Json(Jobid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST: Employee/Job/GetStockData
        /// <summary>
        /// GetStockData By Stockid
        /// </summary>
        /// <param name="StockId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStockData(string StockId)
        {
            try
            {
                using (Stock)
                {
                    Guid StockID = Guid.Parse(StockId);
                    Stock stock = Stock.FindBy(i => i.ID == StockID).FirstOrDefault();
                    CommonMapper<Stock, FSM.Web.Areas.Employee.ViewModels.StockViewModel> mapper = new CommonMapper<Stock, FSM.Web.Areas.Employee.ViewModels.StockViewModel>();
                    FSM.Web.Areas.Employee.ViewModels.StockViewModel displayStocksViewModel = mapper.Mapper(stock);
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(displayStocksViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets stock data.");

                    return Json(new { json, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/job/ViewJobSiteDetail
        /// <summary>
        /// ViewJobSiteDetail of job
        /// </summary>
        /// <param name="jobid"></param>
        /// <param name="SiteId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ViewJobSiteDetail(string jobid, string SiteId)
        {
            try
            {
                using (Employeejob)
                {
                    CustomerSiteDetailListViewModel Sitedetailmodel = new CustomerSiteDetailListViewModel();

                    Guid JobId = Guid.Parse(jobid);
                    var jobdetail = Employeejob.FindBy(i => i.Id == JobId).FirstOrDefault();
                    if (jobdetail != null)
                    {
                        Guid contactid = Guid.Empty;
                        using (CustomerSiteDetail)
                        {
                            var SiteDetail = CustomerSiteDetail.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
                            if (SiteDetail != null)
                            {
                                CommonMapper<CustomerSiteDetail, FSM.Web.Areas.Customer.ViewModels.CustomerSiteDetailViewModel> mapper = new CommonMapper<CustomerSiteDetail, FSM.Web.Areas.Customer.ViewModels.CustomerSiteDetailViewModel>();
                                FSM.Web.Areas.Customer.ViewModels.CustomerSiteDetailViewModel CustomerSitedetailmodel = mapper.Mapper(SiteDetail);
                                Sitedetailmodel.CustomerSiteDetailmodel = CustomerSitedetailmodel;
                                //contactid = !(String.IsNullOrEmpty(CustomerSitedetailmodel.ContactId)) ? Guid.Parse(CustomerSitedetailmodel.ContactId.ToString()) : Guid.Empty;
                            }
                        }

                        using (Customers)
                        {
                            var contactName = Customers.FindBy(i => i.CustomerGeneralInfoId == jobdetail.CustomerGeneralInfoId).FirstOrDefault();
                            if (contactName != null)
                            {
                                Sitedetailmodel.CustomerSiteDetailmodel.CustomerName = (!String.IsNullOrEmpty(contactName.CustomerLastName) ? contactName.CustomerLastName : "");
                            }
                        }

                        using (CustomerResidenceDetail)
                        {
                            var ResidenceDetail = CustomerResidenceDetail.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
                            if (ResidenceDetail != null)
                            {
                                CommonMapper<CustomerResidenceDetail, FSM.Web.Areas.Customer.ViewModels.CustomerResidenceDetailViewModel> mapper = new CommonMapper<CustomerResidenceDetail, FSM.Web.Areas.Customer.ViewModels.CustomerResidenceDetailViewModel>();
                                FSM.Web.Areas.Customer.ViewModels.CustomerResidenceDetailViewModel CustomerResidencedetailmodel = mapper.Mapper(ResidenceDetail);

                                if (CustomerResidencedetailmodel.TypeOfResidence.HasValue)
                                {
                                    CustomerResidencedetailmodel.DisplayResidenceType = CustomerResidencedetailmodel.TypeOfResidence != 0 ?
                                                    CustomerResidencedetailmodel.TypeOfResidence.GetAttribute<DisplayAttribute>() != null ?
                                                    CustomerResidencedetailmodel.TypeOfResidence.GetAttribute<DisplayAttribute>().Name :
                                                    CustomerResidencedetailmodel.TypeOfResidence.ToString() : "Not Available";
                                }
                                else
                                {
                                    CustomerResidencedetailmodel.DisplayResidenceType = "Not Available";
                                }

                                CustomerResidencedetailmodel.DisplayResidenceHeight = CustomerResidencedetailmodel.Height != 0 ?
                                                    CustomerResidencedetailmodel.Height.GetAttribute<DisplayAttribute>() != null ?
                                                    CustomerResidencedetailmodel.Height.GetAttribute<DisplayAttribute>().Name :
                                                    CustomerResidencedetailmodel.Height.ToString() : "Not Available";

                                CustomerResidencedetailmodel.DisplayRoofPitch = CustomerResidencedetailmodel.Pitch != 0 ?
                                                    CustomerResidencedetailmodel.Pitch.GetAttribute<DisplayAttribute>() != null ?
                                                    CustomerResidencedetailmodel.Pitch.GetAttribute<DisplayAttribute>().Name :
                                                    CustomerResidencedetailmodel.Pitch.ToString() : "Not Available";

                                CustomerResidencedetailmodel.DisplayRoofType = CustomerResidencedetailmodel.RoofType != 0 ?
                                                    CustomerResidencedetailmodel.RoofType.GetAttribute<DisplayAttribute>() != null ?
                                                    CustomerResidencedetailmodel.RoofType.GetAttribute<DisplayAttribute>().Name :
                                                    CustomerResidencedetailmodel.RoofType.ToString() : "Not Available";

                                CustomerResidencedetailmodel.DisplayGutterGaurd = CustomerResidencedetailmodel.GutterGaurd != 0 ?
                                                    CustomerResidencedetailmodel.GutterGaurd.GetAttribute<DisplayAttribute>() != null ?
                                                    CustomerResidencedetailmodel.GutterGaurd.GetAttribute<DisplayAttribute>().Name :
                                                    CustomerResidencedetailmodel.GutterGaurd.ToString() : "Not Available";

                                Sitedetailmodel.CustomerResidenceDetailmodel = CustomerResidencedetailmodel;
                            }
                        }

                        using (CustomerconditionDetail)
                        {
                            var conditiondetail = CustomerconditionDetail.FindBy(i => i.SiteDetailId == jobdetail.SiteId).FirstOrDefault();
                            if (conditiondetail != null)
                            {
                                CommonMapper<CustomerConditionReport, FSM.Web.Areas.Customer.ViewModels.CustomerConditionReportViewModel> mapper = new CommonMapper<CustomerConditionReport, FSM.Web.Areas.Customer.ViewModels.CustomerConditionReportViewModel>();
                                FSM.Web.Areas.Customer.ViewModels.CustomerConditionReportViewModel Customerconditiondetailmodel = mapper.Mapper(conditiondetail);

                                Customerconditiondetailmodel.DisplayRoofTilesSheet = Customerconditiondetailmodel.RoofTilesSheets != 0 ?
                                                    Customerconditiondetailmodel.RoofTilesSheets.GetAttribute<DisplayAttribute>() != null ?
                                                    Customerconditiondetailmodel.RoofTilesSheets.GetAttribute<DisplayAttribute>().Name :
                                                    Customerconditiondetailmodel.RoofTilesSheets.ToString() : "Not Available";

                                Customerconditiondetailmodel.DisplayValley = Customerconditiondetailmodel.Valleys != 0 ?
                                                    Customerconditiondetailmodel.Valleys.GetAttribute<DisplayAttribute>() != null ?
                                                    Customerconditiondetailmodel.Valleys.GetAttribute<DisplayAttribute>().Name :
                                                    Customerconditiondetailmodel.Valleys.ToString() : "Not Available";

                                Customerconditiondetailmodel.DisplayFlashing = Customerconditiondetailmodel.Flashings != 0 ?
                                                    Customerconditiondetailmodel.Flashings.GetAttribute<DisplayAttribute>() != null ?
                                                    Customerconditiondetailmodel.Flashings.GetAttribute<DisplayAttribute>().Name :
                                                    Customerconditiondetailmodel.Flashings.ToString() : "Not Available";

                                Customerconditiondetailmodel.DisplayGutter = Customerconditiondetailmodel.Gutters != 0 ?
                                                    Customerconditiondetailmodel.Gutters.GetAttribute<DisplayAttribute>() != null ?
                                                    Customerconditiondetailmodel.Gutters.GetAttribute<DisplayAttribute>().Name :
                                                    Customerconditiondetailmodel.Gutters.ToString() : "Not Available";

                                Customerconditiondetailmodel.DisplayDownPipe = Customerconditiondetailmodel.DownPipes != 0 ?
                                                    Customerconditiondetailmodel.DownPipes.GetAttribute<DisplayAttribute>() != null ?
                                                    Customerconditiondetailmodel.DownPipes.GetAttribute<DisplayAttribute>().Name :
                                                    Customerconditiondetailmodel.DownPipes.ToString() : "Not Available";

                                Sitedetailmodel.CustomerconditionDetailmodel = Customerconditiondetailmodel;
                            }
                        }

                        using (CustomerSiteDocuments)
                        {
                            var Sitedocuments = CustomerSiteDocuments.FindBy(i => i.SiteId == jobdetail.SiteId).ToList();
                            if (Sitedocuments != null)
                            {
                                CommonMapper<CustomerSitesDocuments, FSM.Web.Areas.Customer.ViewModels.CustomerSitesDocumentsViewModel> mapper = new CommonMapper<CustomerSitesDocuments, FSM.Web.Areas.Customer.ViewModels.CustomerSitesDocumentsViewModel>();
                                List<FSM.Web.Areas.Customer.ViewModels.CustomerSitesDocumentsViewModel> Customersitedocumentmodel = mapper.MapToList(Sitedocuments.ToList());
                                Sitedetailmodel.CustomerSitedocumentviewmodel = Customersitedocumentmodel;
                            }
                        }

                        using (Customercontacts)
                        {
                            var customercontacts = Customercontacts.FindBy(i => i.ContactId == contactid).FirstOrDefault();
                            if (customercontacts != null)
                            {
                                CommonMapper<CustomerContacts, FSM.Web.Areas.Customer.ViewModels.CustomerContactsViewModel> mapper = new CommonMapper<CustomerContacts, FSM.Web.Areas.Customer.ViewModels.CustomerContactsViewModel>();
                                FSM.Web.Areas.Customer.ViewModels.CustomerContactsViewModel Customercontactmodel = mapper.Mapper(customercontacts);
                                Sitedetailmodel.Customercontactsmodel = Customercontactmodel;
                            }
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job site details.");

                    return View(Sitedetailmodel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET: Employee/Job/DownloadSiteDocuments
        /// <summary>
        /// DownloadSiteDocuments
        /// </summary>
        /// <param name="SiteDocid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DownloadSiteDocuments(string SiteDocid)
        {
            try
            {
                using (CustomerSiteDocuments)
                {
                    Guid Docid = Guid.Parse(SiteDocid);
                    var file = CustomerSiteDocuments.FindBy(i => i.DocumentId == Docid).FirstOrDefault();
                    var FileVirtualPath = "/Images/CustomerDocs/" + file.DocumentId + '/' + file.DocumentName;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " download site document.");

                    return Json(FileVirtualPath, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<OTRWEmployee> GetOtrwEmployeesList()
        {
            List<OTRWEmployee> list = new List<OTRWEmployee>();
            var role = Guid.Parse("31cf918d-b8fe-4490-b2d7-27324bfe89b4");
            var employee = Employee.FindBy(i => i.Role == role);
            foreach (var emp in employee)
            {
                OTRWEmployee obj = new OTRWEmployee();
                obj.EmployeeId = emp.EmployeeId;
                obj.EmployeeName = emp.FirstName;
                list.Add(obj);
            }
            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed otrw list.");
            return list;
        }

        public List<CustomerCoName> GetCustomerList()
        {

            List<CustomerCoName> li = new List<CustomerCoName>();

            var Customerdetail = Customers.GetAll().OrderBy(i => i.CustomerLastName);
            foreach (var cust in Customerdetail)
            {
                if (!string.IsNullOrEmpty(cust.CustomerLastName))
                {
                    CustomerCoName obj = new CustomerCoName();
                    obj.CustomerGeneralInfoId = cust.CustomerGeneralInfoId;
                    obj.LastName = cust.CustomerLastName;
                    li.Add(obj);
                }
            }

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed customer list.");

            return li;
        }
        public ActionResult GetCustomerListforauto()
        {
            List<CustomerCoName> li = new List<CustomerCoName>();
            using (Customers)
            {
                var Customerdetail = Customers.GetAll();
                foreach (var cust in Customerdetail)
                {
                    if (!string.IsNullOrEmpty(cust.CustomerLastName))
                    {
                        CustomerCoName obj = new CustomerCoName();
                        obj.CustomerGeneralInfoId = cust.CustomerGeneralInfoId;
                        obj.LastName = cust.CustomerLastName;
                        li.Add(obj);
                    }
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(li);
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed customer list for auto.");

                return Json(new { list = json, length = li.Count() }, JsonRequestBehavior.AllowGet);
            }

        }
        public List<Joblist> GetJoblist()
        {
            List<Joblist> li = new List<Joblist>();

            var jobs = Employeejob.FindBy(i => i.JobType == (int)FSMConstant.Constant.JobType.Do).OrderBy(i => i.JobId);
            foreach (var job in jobs)
            {
                var Linkedid = SupportJoblink.FindBy(i => i.LinkedJobId == job.Id).FirstOrDefault();
                if (Linkedid == null)
                {
                    Joblist obj = new Joblist();
                    obj.Jobid = job.Id;
                    obj.jobnumeric = "Job_" + job.JobId;
                    li.Add(obj);
                }
            }

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed job list.");


            return li;
        }

        //GET:Employee/Job/CustomerJobInfoList
        /// <summary>
        /// Show all Job list
        /// </summary>
        /// <returns>Model</returns>
        public ActionResult CustomerJobInfoList()
        {
            using (Employeejob)
            {
                string Searchstring = Request.QueryString["searchkeyword"];

                Nullable<int> Invoicestatus = string.IsNullOrEmpty(Request.QueryString["InvoiceStatus"]) ? (int?)null :
                                                 Convert.ToInt32(Request.QueryString["InvoiceStatus"]);

                EmployeeJobVieweModel employeeJobVieweModel = new EmployeeJobVieweModel();
                var CustomerjobInfoGridList = Employeejob.GetJobInfoList().Where(j => j.JobType != (int)FSM.Web.FSMConstant.Constant.JobType.Support).ToList();

                if (!string.IsNullOrEmpty(Invoicestatus.ToString()))
                {
                    CustomerjobInfoGridList = CustomerjobInfoGridList.Where(i => i.InvoiceStatus == Invoicestatus).ToList();
                }
                // mapping list<Coreviewmodel> to list<viewmodel>
                CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(CustomerjobInfoGridList.OrderByDescending(i => i.JobId).ToList());

                int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);

                EmployeejobSearchViewModel employeejobViewModelPageSize = new EmployeejobSearchViewModel
                {
                    PageSize = PageSize,
                    searchkeyword = string.IsNullOrEmpty(Searchstring) ? "" : Searchstring,
                    InvoiceStatus = Invoicestatus != null ? Convert.ToString(Invoicestatus.ToString()) : ""
                };

                var Employeelistviewmodel = new EmployeeJobListViewModel
                {
                    EmployeeJoblist = EmployeejobViewModel,
                    Employeejobsearchmodel = employeejobViewModelPageSize
                };
                return View(Employeelistviewmodel);
            }
        }


        //POST:Employee/Job/CustomerJobInfoList
        /// <summary>
        /// Search Record 
        /// </summary>
        /// <param name="jobInfosearchViewmodel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult CustomerJobInfoList(EmployeejobSearchViewModel jobInfosearchViewmodel)
        {
            try
            {
                using (Employeejob)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    var Jobs = Employeejob.GetJobInfoListBySearchkeyword(jobInfosearchViewmodel.searchkeyword, jobInfosearchViewmodel.InvoiceStatus).Where(j => j.JobType != (int)FSM.Web.FSMConstant.Constant.JobType.Support).ToList();
                    if (!string.IsNullOrEmpty(jobInfosearchViewmodel.InvoiceStatus))
                    {
                        int invoiceStatus = Convert.ToInt32(jobInfosearchViewmodel.InvoiceStatus);
                        //var stocks =Stock.GetAll();
                        Jobs = Jobs.Where(i => i.InvoiceStatus == invoiceStatus).ToList();
                    }

                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> jobviewmodel = mapper.MapToList(Jobs.ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    //var EmployeesearchViewmodel = new EmployeejobSearchViewModel
                    //{
                    //    PageSize = PageSize,
                    //    searchkeyword = Searchstring
                    //};
                    var Employeelistviewmodel = new EmployeeJobListViewModel
                    {
                        EmployeeJoblist = jobviewmodel,
                        Employeejobsearchmodel = jobInfosearchViewmodel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of customer job info.");


                    return View(Employeelistviewmodel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        //GET: Employee/Job/GetUserTimeSheetData
        /// <summary>
        /// Show List of User time Sheet Data
        /// </summary>
        /// <returns>Model</returns>
        public ActionResult GetUserTimeSheetData()
        {
            TimeSheetDataViewModel timeSheetDataViewModel = new TimeSheetDataViewModel();

            try
            {
                var sheetTotalHrs = TimeSheet.GetSheetTotalHrs(null, null, null, null);
                var userTimeSheetList = TimeSheet.GetSheetAll(null);
                // mapping list<entity> to list<viewmodel>
                CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel> mapper = new CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel>();
                List<UserTimeSheetViewModel> userTimeSheetViewModel = mapper.MapToList(userTimeSheetList.ToList());

                var userList = AspNetUser.GetOTRWUser().Select(m => new SelectListItem { Text = m.UserName, Value = m.Id }).ToList();

                timeSheetDataViewModel.UserTimeSheetList = userTimeSheetViewModel;
                timeSheetDataViewModel.Users = userList;

                timeSheetDataViewModel.Job = sheetTotalHrs.Where(m => m.Reason == "Job").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Job))
                {
                    timeSheetDataViewModel.Job = "00:00:00";
                }

                timeSheetDataViewModel.Lunch = sheetTotalHrs.Where(m => m.Reason == "Lunch").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Lunch))
                {
                    timeSheetDataViewModel.Lunch = "00:00:00";
                }

                timeSheetDataViewModel.Personal = sheetTotalHrs.Where(m => m.Reason == "Personal").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Personal))
                {
                    timeSheetDataViewModel.Personal = "00:00:00";
                }

                timeSheetDataViewModel.Travelling = sheetTotalHrs.Where(m => m.Reason == "Travelling").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Travelling))
                {
                    timeSheetDataViewModel.Travelling = "00:00:00";
                }

                timeSheetDataViewModel.TotalHrs = TimeSheet.GetSheetHrs(null, null, null, null).FirstOrDefault();

                timeSheetDataViewModel.PageSize = 10;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed user's time sheet data.");


                return View(timeSheetDataViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                TimeSheet.Dispose();
                AspNetUser.Dispose();
            }
        }

        public ActionResult GetUserTimeSheetPartial()
        {
            TimeSheetDataViewModel timeSheetDataViewModel = new TimeSheetDataViewModel();
            var regex = new Regex(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$");

            try
            {
                DateTime? JobStartDate = (DateTime?)null;
                if (Request.QueryString["JobStartDate"] != null)
                {
                    if (regex.IsMatch(Request.QueryString["JobStartDate"]))
                    {
                        JobStartDate = Request.QueryString["JobStartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobStartDate"].ToString()) ?
                                    DateTime.Parse(Request.QueryString["JobStartDate"]) : (DateTime?)null : (DateTime?)null;
                    }
                }

                DateTime? JobEndDate = (DateTime?)null;
                if (Request.QueryString["JobEndDate"] != null)
                {
                    if (regex.IsMatch(Request.QueryString["JobEndDate"]))
                    {
                        JobEndDate = Request.QueryString["JobEndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobEndDate"].ToString()) ?
                                    DateTime.Parse(Request.QueryString["JobEndDate"]) : (DateTime?)null : (DateTime?)null;
                    }
                }


                string UserIds = Request.QueryString["UserId"];
                if (UserIds == null)
                {
                    UserIds = "";
                }


                string Keyword = Request.QueryString["Keyword"];

                int PageSize = Request.QueryString["PageSize"] != null ? int.Parse(Request.QueryString["PageSize"]) : 10;


                //sheet total hours
                var sheetTotalHrs = TimeSheet.GetSheetTotalHrs(JobStartDate, JobEndDate, UserIds, Keyword);
                //sheet all
                var userTimeSheetList = TimeSheet.GetSheetAll(UserIds);

                if (JobStartDate.HasValue && JobEndDate.HasValue)
                {
                    userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate && m.JobDate <= JobEndDate));
                }
                else if (JobStartDate.HasValue)
                {
                    userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate));
                }
                else if (JobEndDate.HasValue)
                {
                    userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate <= JobEndDate));
                }

                if (!string.IsNullOrEmpty(Keyword))
                {
                    Keyword = Keyword.Replace("'", "''");

                    userTimeSheetList = userTimeSheetList.Where(m => (m.JobNo.ToString().Contains(Keyword) || (m.CustomerLastName != null && m.CustomerLastName.ToLower().Contains(Keyword.ToLower()))));
                }

                // mapping list<entity> to list<viewmodel>
                CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel> mapper = new CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel>();
                List<UserTimeSheetViewModel> userTimeSheetViewModel = mapper.MapToList(userTimeSheetList.ToList());

                timeSheetDataViewModel.UserTimeSheetList = userTimeSheetViewModel.OrderByDescending(m => m.JobDate).ThenByDescending(m => m.StartTime);

                timeSheetDataViewModel.Job = sheetTotalHrs.Where(m => m.Reason == "Job").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Job))
                {
                    timeSheetDataViewModel.Job = "00:00:00";
                }

                timeSheetDataViewModel.Lunch = sheetTotalHrs.Where(m => m.Reason == "Lunch").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Lunch))
                {
                    timeSheetDataViewModel.Lunch = "00:00:00";
                }

                timeSheetDataViewModel.Personal = sheetTotalHrs.Where(m => m.Reason == "Personal").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Personal))
                {
                    timeSheetDataViewModel.Personal = "00:00:00";
                }

                timeSheetDataViewModel.Travelling = sheetTotalHrs.Where(m => m.Reason == "Travelling").Select(m => m.CalculatedHour).FirstOrDefault();
                if (string.IsNullOrEmpty(timeSheetDataViewModel.Travelling))
                {
                    timeSheetDataViewModel.Travelling = "00:00:00";
                }
                //sheet hours
                timeSheetDataViewModel.TotalHrs = TimeSheet.GetSheetHrs(JobStartDate, JobEndDate, UserIds, Keyword).FirstOrDefault();

                timeSheetDataViewModel.PageSize = PageSize;

                return PartialView("_UserTimeSheetList", timeSheetDataViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                TimeSheet.Dispose();
            }
        }


        //GET: Employee/Job/_EditUserTimeSheet
        /// <summary>
        ///Update User Time Sheet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _EditUserTimeSheet(string id, string EmpUserId)
        {
            try
            {
                Guid Id;
                Guid.TryParse(id, out Id);

                Guid UserId;
                Guid.TryParse(EmpUserId, out UserId);

                UserTimeSheet userTimeSheet = TimeSheet.FindBy(m => m.Id == Id).FirstOrDefault();
                // mapping entity to viewmodel
                CommonMapper<UserTimeSheet, UserTimeSheetViewModel> mapper = new CommonMapper<UserTimeSheet, UserTimeSheetViewModel>();
                UserTimeSheetViewModel userTimeSheetViewModel = mapper.Mapper(userTimeSheet);
                userTimeSheetViewModel.pagenum = Request.QueryString["grid-page"];
                userTimeSheetViewModel.jobstartdateSearch = Request.QueryString["JobStartDate"];
                userTimeSheetViewModel.jobenddateSearch = Request.QueryString["JobEndDate"];
                userTimeSheetViewModel.useridSearch = Request.QueryString["UserId"];
                userTimeSheetViewModel.Keyword = Request.QueryString["Keyword"];
                userTimeSheetViewModel.PageSize = Request.QueryString["PageSize"];
                var employeerJobList = Employeejob.GetOtrwAssignJobNo(UserId).AsEnumerable();
                List<EmployeeJobTimeSheetDetail> li = new List<EmployeeJobTimeSheetDetail>();
                foreach (var i in employeerJobList)
                {
                    EmployeeJobTimeSheetDetail obj = new EmployeeJobTimeSheetDetail();
                    obj.EmployeeJobId = i.JobId;
                    obj.JobNo = i.JobNo;
                    obj.Description = "JobNo_" + obj.JobNo;
                    li.Add(obj);
                }
                userTimeSheetViewModel.employeeJobTimeSheetDetail = li;



                return PartialView(userTimeSheetViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                TimeSheet.Dispose();
            }
        }

        //POST:Employee/Job/_EditUserTimeSheet
        /// <summary>
        /// Update User time Sheet
        /// </summary>
        /// <param name="userTimeSheetViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult _EditUserTimeSheet(UserTimeSheetViewModel userTimeSheetViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int errCnt = 0;
                    if (userTimeSheetViewModel.StartTime.Hours < 0)
                    {
                        ModelState.AddModelError("StartTime", "Start Time is invalid !");
                        errCnt = errCnt + 1;
                    }
                    if (userTimeSheetViewModel.EndTime.Hours < 0)
                    {
                        ModelState.AddModelError("EndTime", "End Time is invalid !");
                        errCnt = errCnt + 1;
                    }
                    if (userTimeSheetViewModel.StartTime > userTimeSheetViewModel.EndTime)
                    {
                        ModelState.AddModelError("", "Start Time should be less than equal to End Time !");
                        errCnt = errCnt + 1;
                    }
                    if (errCnt > 0)
                    {
                        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    }
                    else
                    {
                        userTimeSheetViewModel.TimeSpent = userTimeSheetViewModel.EndTime - userTimeSheetViewModel.StartTime;
                        CommonMapper<UserTimeSheetViewModel, UserTimeSheet> mapper = new CommonMapper<UserTimeSheetViewModel, UserTimeSheet>();
                        UserTimeSheet userTimeSheet = mapper.Mapper(userTimeSheetViewModel);
                        TimeSheet.DeAttach(userTimeSheet);
                        TimeSheet.Edit(userTimeSheet);
                        TimeSheet.Save();


                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated user's time sheet.");

                        var routeValues = new RouteValueDictionary();
                        if (!string.IsNullOrEmpty(userTimeSheetViewModel.jobstartdateSearch))
                        {
                            routeValues.Add("JobStartDate", userTimeSheetViewModel.jobstartdateSearch);
                        }
                        if (!string.IsNullOrEmpty(userTimeSheetViewModel.jobenddateSearch))
                        {
                            routeValues.Add("JobEndDate", userTimeSheetViewModel.jobenddateSearch);
                        }
                        if (!string.IsNullOrEmpty(userTimeSheetViewModel.useridSearch))
                        {
                            routeValues.Add("UserId", userTimeSheetViewModel.useridSearch);
                        }

                        if (!string.IsNullOrEmpty(userTimeSheetViewModel.pagenum))
                        {
                            routeValues.Add("grid-page", userTimeSheetViewModel.pagenum);
                        }
                        if (!string.IsNullOrEmpty(userTimeSheetViewModel.Keyword))
                        {
                            routeValues.Add("Keyword", userTimeSheetViewModel.Keyword);
                        }
                        if (!string.IsNullOrEmpty(userTimeSheetViewModel.PageSize))
                        {
                            routeValues.Add("PageSize", userTimeSheetViewModel.PageSize);
                        }
                        if (routeValues.Count > 0)
                        {
                            return RedirectToAction("GetUserTimeSheetPartial", routeValues);
                        }
                        else
                        {
                            return RedirectToAction("GetUserTimeSheetPartial");
                        }
                    }
                }
                else
                {
                    var errStartTime = ModelState.Where(m => m.Key == "StartTime").Select(m => m.Value.Errors).FirstOrDefault();
                    if (errStartTime.Count != 0 && errStartTime[0].ErrorMessage != "The Start Time field is required.")
                    {
                        ModelState.Remove("StartTime");
                        ModelState.AddModelError("StartTime", "Start Time is invalid !");
                    }
                    var errEndTime = ModelState.Where(m => m.Key == "EndTime").Select(m => m.Value.Errors).FirstOrDefault();
                    if (errEndTime.Count != 0 && errEndTime[0].ErrorMessage != "The End Time field is required.")
                    {
                        ModelState.Remove("EndTime");
                        ModelState.AddModelError("EndTime", "End Time is invalid !");
                    }
                    return Json(ModelState.Values.SelectMany(m => m.Errors));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                TimeSheet.Dispose();
            }
        }

        [HttpGet]
        public ActionResult TimeSheetExport()
        {
            TimeSheetDataViewModel timeSheetDataViewModel = new TimeSheetDataViewModel();

            DateTime? JobStartDate = Request.QueryString["JobStartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobStartDate"].ToString()) ?
                                DateTime.Parse(Request.QueryString["JobStartDate"]) : (DateTime?)null : (DateTime?)null;
            DateTime? JobEndDate = Request.QueryString["JobEndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobEndDate"].ToString()) ?
                                DateTime.Parse(Request.QueryString["JobEndDate"]) : (DateTime?)null : (DateTime?)null;
            string UserId = Request.QueryString["UserId"];
            string Keyword = Request.QueryString["Keyword"];

            var sheetTotalHrs = TimeSheet.GetSheetTotalHrs(JobStartDate, JobEndDate, UserId, Keyword);


            var userTimeSheetList = TimeSheet.GetSheetAll("");
            if (UserId != null)
            {
                userTimeSheetList = TimeSheet.GetSheetAll(UserId);
            }



            if (JobStartDate.HasValue && JobEndDate.HasValue)
            {
                userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate && m.JobDate <= JobStartDate));
                timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
                timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
            }
            else if (JobStartDate.HasValue)
            {
                userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate));
                timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
            }
            else if (JobEndDate.HasValue)
            {
                userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate <= JobEndDate));
                timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
            }

            // mapping list<entity> to list<viewmodel>
            CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel> mapper = new CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel>();
            List<UserTimeSheetViewModel> userTimeSheetViewModel = mapper.MapToList(userTimeSheetList.ToList());

            timeSheetDataViewModel.UserTimeSheetList = userTimeSheetViewModel;
            timeSheetDataViewModel.Job = sheetTotalHrs.Where(m => m.Reason == "Job").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Job))
            {
                timeSheetDataViewModel.Job = "00:00:00";
            }

            timeSheetDataViewModel.Lunch = sheetTotalHrs.Where(m => m.Reason == "Lunch").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Lunch))
            {
                timeSheetDataViewModel.Lunch = "00:00:00";
            }

            timeSheetDataViewModel.Personal = sheetTotalHrs.Where(m => m.Reason == "Personal").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Personal))
            {
                timeSheetDataViewModel.Personal = "00:00:00";
            }

            timeSheetDataViewModel.Travelling = sheetTotalHrs.Where(m => m.Reason == "Travelling").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Travelling))
            {
                timeSheetDataViewModel.Travelling = "00:00:00";
            }

            timeSheetDataViewModel.TotalHrs = TimeSheet.GetSheetHrs(JobStartDate, JobEndDate, UserId, Keyword).FirstOrDefault();


            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " exported time sheet.");

            return View(timeSheetDataViewModel);
        }
        public ActionResult Export()
        {
            try
            {
                TimeSheetDataViewModel timeSheetDataViewModel = new TimeSheetDataViewModel();

                string ReportType = Request.QueryString["ReportType"];
                DateTime? JobStartDate = Request.QueryString["JobStartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobStartDate"].ToString()) ?
                                DateTime.Parse(Request.QueryString["JobStartDate"]) : (DateTime?)null : (DateTime?)null;
                DateTime? JobEndDate = Request.QueryString["JobEndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobEndDate"].ToString()) ?
                                    DateTime.Parse(Request.QueryString["JobEndDate"]) : (DateTime?)null : (DateTime?)null;
                string UserId = Request.QueryString["UserId"];
                string Keyword = Request.QueryString["Keyword"];
                if (ReportType == "1")
                {
                    var sheetTotalHrs = TimeSheet.GetSheetTotalHrs(JobStartDate, JobEndDate, UserId, Keyword);

                    var userTimeSheetList = TimeSheet.GetSheetAll("");
                    if (UserId != null)
                    {
                        userTimeSheetList = TimeSheet.GetSheetAll(UserId);
                    }


                    if (JobStartDate == null)
                    {
                        timeSheetDataViewModel.JobStartDate = userTimeSheetList.Any() ? userTimeSheetList.Min(i => i.JobDate) : (DateTime?)null;
                    }
                    if (JobEndDate == null)
                    {
                        timeSheetDataViewModel.JobEndDate = userTimeSheetList.Any() ? userTimeSheetList.Max(i => i.JobDate) : (DateTime?)null;
                    }
                    if (JobStartDate.HasValue && JobEndDate.HasValue)
                    {
                        userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate && m.JobDate <= JobEndDate));
                        timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
                        timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
                    }
                    else if (JobStartDate.HasValue)
                    {
                        userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate));
                        timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
                    }
                    else if (JobEndDate.HasValue)
                    {
                        userTimeSheetList = userTimeSheetList.Where(m => (m.JobDate != null && m.JobDate <= JobEndDate));
                        timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
                    }

                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        userTimeSheetList = userTimeSheetList.Where(m => (m.Job.ToString().Contains(Keyword) || (m.CustomerLastName != null && m.CustomerLastName.ToLower().Contains(Keyword.ToLower()))));
                    }


                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel> mapper = new CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel>();
                    List<UserTimeSheetViewModel> userTimeSheetViewModel = mapper.MapToList(userTimeSheetList.ToList());

                    timeSheetDataViewModel.UserTimeSheetList = userTimeSheetViewModel;
                    timeSheetDataViewModel.Job = sheetTotalHrs.Where(m => m.Reason == "Job").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Job))
                    {
                        timeSheetDataViewModel.Job = "00:00:00";
                    }

                    timeSheetDataViewModel.Lunch = sheetTotalHrs.Where(m => m.Reason == "Lunch").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Lunch))
                    {
                        timeSheetDataViewModel.Lunch = "00:00:00";
                    }

                    timeSheetDataViewModel.Personal = sheetTotalHrs.Where(m => m.Reason == "Personal").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Personal))
                    {
                        timeSheetDataViewModel.Personal = "00:00:00";
                    }

                    timeSheetDataViewModel.Travelling = sheetTotalHrs.Where(m => m.Reason == "Travelling").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Travelling))
                    {
                        timeSheetDataViewModel.Travelling = "00:00:00";
                    }
                    timeSheetDataViewModel.TotalHrs = TimeSheet.GetSheetHrs(JobStartDate, JobEndDate, UserId, Keyword).FirstOrDefault();


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " exported pdf file of Detail Report.");

                    return new ViewAsPdf("TimeSheetExport", timeSheetDataViewModel)
                    {
                        FileName = "UserTimeSheet.pdf",
                        CustomSwitches =
                            "--footer-center \"  Dated: " +
                            DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                            " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\""
                    };
                }
                else
                {
                    var sheetTotalHrs = TimeSheet.GetSheetTotalHrs(JobStartDate, JobEndDate, UserId, Keyword);

                    var userAggregateSheetList = TimeSheet.GetAggregateSheetAll("");

                    if (UserId != null)
                    {
                        userAggregateSheetList = TimeSheet.GetAggregateSheetAll(UserId);
                    }

                    if (JobStartDate == null)
                    {
                        timeSheetDataViewModel.JobStartDate = userAggregateSheetList.Any() ? userAggregateSheetList.Min(i => i.JobDate) : (DateTime?)null;
                    }
                    if (JobEndDate == null)
                    {
                        timeSheetDataViewModel.JobEndDate = userAggregateSheetList.Any() ? userAggregateSheetList.Max(i => i.JobDate) : (DateTime?)null;
                    }
                    if (JobStartDate.HasValue && JobEndDate.HasValue)
                    {
                        userAggregateSheetList = userAggregateSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate && m.JobDate <= JobEndDate));
                        timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
                        timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
                    }
                    else if (JobStartDate.HasValue)
                    {
                        userAggregateSheetList = userAggregateSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate));
                        timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
                    }
                    else if (JobEndDate.HasValue)
                    {
                        userAggregateSheetList = userAggregateSheetList.Where(m => (m.JobDate != null && m.JobDate <= JobEndDate));
                        timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
                    }

                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        userAggregateSheetList = userAggregateSheetList.Where(m => (m.Job.ToString().Contains(Keyword) || (m.CustomerLastName != null && m.CustomerLastName.ToLower().Contains(Keyword.ToLower()))));
                    }



                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel> mapper = new CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel>();
                    List<UserTimeSheetViewModel> userTimeSheetViewModel = mapper.MapToList(userAggregateSheetList.ToList());

                    timeSheetDataViewModel.UserTimeSheetList = userTimeSheetViewModel;
                    timeSheetDataViewModel.Job = sheetTotalHrs.Where(m => m.Reason == "Job").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Job))
                    {
                        timeSheetDataViewModel.Job = "00:00:00";
                    }

                    timeSheetDataViewModel.Lunch = sheetTotalHrs.Where(m => m.Reason == "Lunch").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Lunch))
                    {
                        timeSheetDataViewModel.Lunch = "00:00:00";
                    }

                    timeSheetDataViewModel.Personal = sheetTotalHrs.Where(m => m.Reason == "Personal").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Personal))
                    {
                        timeSheetDataViewModel.Personal = "00:00:00";
                    }

                    timeSheetDataViewModel.Travelling = sheetTotalHrs.Where(m => m.Reason == "Travelling").Select(m => m.CalculatedHour).FirstOrDefault();
                    if (string.IsNullOrEmpty(timeSheetDataViewModel.Travelling))
                    {
                        timeSheetDataViewModel.Travelling = "00:00:00";
                    }
                    timeSheetDataViewModel.TotalHrs = TimeSheet.GetSheetHrs(JobStartDate, JobEndDate, UserId, Keyword).FirstOrDefault();


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " exported pdf file of Aggregate Report.");

                    return new ViewAsPdf("AggregateSheetExport", timeSheetDataViewModel)
                    {
                        FileName = "UserAggregateTimeSheet.pdf",
                        CustomSwitches =
                            "--footer-center \"  Dated: " +
                            DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                            " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\""
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                TimeSheet.Dispose();
            }
        }
        [HttpGet]
        public ActionResult AggregateSheetExport()
        {
            TimeSheetDataViewModel timeSheetDataViewModel = new TimeSheetDataViewModel();

            DateTime? JobStartDate = Request.QueryString["JobStartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobStartDate"].ToString()) ?
                                DateTime.Parse(Request.QueryString["JobStartDate"]) : (DateTime?)null : (DateTime?)null;
            DateTime? JobEndDate = Request.QueryString["JobEndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["JobEndDate"].ToString()) ?
                                DateTime.Parse(Request.QueryString["JobEndDate"]) : (DateTime?)null : (DateTime?)null;
            string UserId = Request.QueryString["UserId"];
            string Keyword = Request.QueryString["Keyword"];

            var sheetTotalHrs = TimeSheet.GetSheetTotalHrs(JobStartDate, JobEndDate, UserId, Keyword);

            var userAggregateSheetList = TimeSheet.GetAggregateSheetAll("");

            if (JobStartDate.HasValue && JobEndDate.HasValue)
            {
                userAggregateSheetList = userAggregateSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate && m.JobDate <= JobStartDate));
                timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
                timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
            }
            else if (JobStartDate.HasValue)
            {
                userAggregateSheetList = userAggregateSheetList.Where(m => (m.JobDate != null && m.JobDate >= JobStartDate));
                timeSheetDataViewModel.JobStartDate = JobStartDate.Value;
            }
            else if (JobEndDate.HasValue)
            {
                userAggregateSheetList = userAggregateSheetList.Where(m => (m.JobDate != null && m.JobDate <= JobEndDate));
                timeSheetDataViewModel.JobEndDate = JobEndDate.Value;
            }

            // mapping list<entity> to list<viewmodel>
            CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel> mapper = new CommonMapper<TimeSheetViewModel, UserTimeSheetViewModel>();
            List<UserTimeSheetViewModel> userTimeSheetViewModel = mapper.MapToList(userAggregateSheetList.ToList());

            timeSheetDataViewModel.UserTimeSheetList = userTimeSheetViewModel;
            timeSheetDataViewModel.Job = sheetTotalHrs.Where(m => m.Reason == "Job").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Job))
            {
                timeSheetDataViewModel.Job = "00:00:00";
            }

            timeSheetDataViewModel.Lunch = sheetTotalHrs.Where(m => m.Reason == "Lunch").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Lunch))
            {
                timeSheetDataViewModel.Lunch = "00:00:00";
            }

            timeSheetDataViewModel.Personal = sheetTotalHrs.Where(m => m.Reason == "Personal").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Personal))
            {
                timeSheetDataViewModel.Personal = "00:00:00";
            }

            timeSheetDataViewModel.Travelling = sheetTotalHrs.Where(m => m.Reason == "Travelling").Select(m => m.CalculatedHour).FirstOrDefault();
            if (string.IsNullOrEmpty(timeSheetDataViewModel.Travelling))
            {
                timeSheetDataViewModel.Travelling = "00:00:00";
            }

            timeSheetDataViewModel.TotalHrs = TimeSheet.GetSheetHrs(JobStartDate, JobEndDate, UserId, Keyword).FirstOrDefault();

            return View(timeSheetDataViewModel);
        }

        [HttpPost]
        public ActionResult AddPurchaseOrder(List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values)
        {
            //TempData["PurchaseOrderDetail"] = values.ToList();
            //TempData.Keep("PurchaseOrderDetail");
            SavePurchaseDetail(values, Convert.ToString(""));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //AddPurchase items 
        public void SavePurchaseDetail(List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values, string InvoiceId)
        {
            try
            {
                string Purchaseid = values[0].purchaseId;
                PurchaseOrderByJobviewmodel purchaseOrderByJobviewmodel = new PurchaseOrderByJobviewmodel();
                if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                {
                    Guid id = Guid.Parse(Purchaseid);
                    var purchasestock = JobPurchaseOrder.FindBy(i => i.ID == id).FirstOrDefault();
                    purchasestock.SupplierID = Guid.Parse(values[0].Supplierid);
                    purchasestock.Description = values[0].Description;
                    if (!String.IsNullOrEmpty(InvoiceId))
                    {
                        purchasestock.InvoiceId = Guid.Parse(InvoiceId);
                    }
                    purchasestock.Cost = Convert.ToDecimal(values[0].Cost);
                    if (!String.IsNullOrEmpty(values[0].JobId))
                        purchasestock.JobID = Guid.Parse(values[0].JobId);
                    purchasestock.ModifiedBy = Guid.Parse(base.GetUserId);
                    purchasestock.ModifiedDate = DateTime.Now;
                    purchasestock.IsDelete = false;
                    JobPurchaseOrder.Edit(purchasestock);
                    JobPurchaseOrder.Save();


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated purchase order.");

                    TempData["Message"] = 2;
                }

                else
                {
                    purchaseOrderByJobviewmodel.ID = Guid.NewGuid();
                    purchaseOrderByJobviewmodel.SupplierID = Guid.Parse(values[0].Supplierid);
                    purchaseOrderByJobviewmodel.Description = values[0].Description;
                    if (!(String.IsNullOrEmpty(InvoiceId)))
                    {
                        purchaseOrderByJobviewmodel.InvoiceId = Guid.Parse(InvoiceId);
                    }
                    purchaseOrderByJobviewmodel.Cost = Convert.ToDecimal(values[0].Cost);
                    if (!String.IsNullOrEmpty(values[0].JobId))
                        purchaseOrderByJobviewmodel.JobID = Guid.Parse(values[0].JobId);
                    purchaseOrderByJobviewmodel.CreatedBy = Guid.Parse(base.GetUserId);
                    purchaseOrderByJobviewmodel.CreatedDate = DateTime.Now;
                    CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob> mapper = new CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob>();
                    PurchaseOrderByJob purchasejobinfo = mapper.Mapper(purchaseOrderByJobviewmodel);
                    purchasejobinfo.IsDelete = false;
                    JobPurchaseOrder.Add(purchasejobinfo);
                    JobPurchaseOrder.Save();
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added purchase order.");

                    TempData["Message"] = 1;
                }

                //delete item if not exist while updating
                Guid purchaseitemId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                var pitems = JobPurchaseOrderitem.FindBy(i => i.PurchaseOrderID == purchaseitemId).ToList();
                foreach (var item in pitems)
                {
                    string iteminfoid = Convert.ToString(item.ID.ToString());
                    bool pos = Array.Exists(values.ToArray(), element => element.ItemId == Guid.Parse(iteminfoid));
                    if (!pos)
                    {
                        if (!String.IsNullOrEmpty(iteminfoid))
                        {
                            var item_id = Guid.Parse(iteminfoid);
                            var itemtodelete = JobPurchaseOrderitem.FindBy(i => i.ID == item_id && i.PurchaseOrderID == purchaseitemId).FirstOrDefault();
                            JobPurchaseOrderitem.Delete(itemtodelete);
                            JobPurchaseOrderitem.Save();
                        }
                    }
                }
                foreach (var stockitem in values)
                {
                    if (stockitem.StockId == null)
                    {
                        stockitem.StockId = Guid.Empty.ToString();
                    }
                    Guid itemorderId=Guid.Empty;
                    Guid itempurchasdId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                    if (stockitem.ItemId != null) {
                        itemorderId = Guid.Parse(stockitem.ItemId.ToString());
                    }
                    var checkExist = JobPurchaseOrderitem.FindBy(i => i.ID == itemorderId && i.PurchaseOrderID == itempurchasdId).FirstOrDefault();
                    if (checkExist != null)
                    {
                        var itemtoupdate = checkExist;
                        itemtoupdate.PurchaseItem = Convert.ToString(stockitem.PurchaseItem);
                        itemtoupdate.UnitOfMeasure = Convert.ToString(stockitem.UnitMeasure);
                        itemtoupdate.Price = Convert.ToDecimal(stockitem.Price);
                        itemtoupdate.Quantity = Convert.ToInt32(stockitem.Quantity);
                        itemtoupdate.ModifiedBy = Guid.Parse(base.GetUserId);
                        itemtoupdate.ModifiedDate = DateTime.Now;
                        JobPurchaseOrderitem.Edit(itemtoupdate);
                        JobPurchaseOrderitem.Save();
                    }
                    else
                    {
                        PurchaseorderItemJobViewModel purchaseOrderITemByjobViewModel = new PurchaseorderItemJobViewModel();
                        purchaseOrderITemByjobViewModel.ID = Guid.NewGuid();
                        if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                        {
                            purchaseOrderITemByjobViewModel.PurchaseOrderID = Guid.Parse(Purchaseid);
                        }
                        else
                        {
                            purchaseOrderITemByjobViewModel.PurchaseOrderID = purchaseOrderByJobviewmodel.ID;
                        }
                        purchaseOrderITemByjobViewModel.StockID = Guid.Parse(stockitem.StockId);
                        purchaseOrderITemByjobViewModel.PurchaseItem = stockitem.PurchaseItem;
                        purchaseOrderITemByjobViewModel.UnitOfMeasure = stockitem.UnitMeasure;
                        purchaseOrderITemByjobViewModel.Quantity = Convert.ToInt32(stockitem.Quantity);
                        purchaseOrderITemByjobViewModel.Price = Convert.ToDecimal(stockitem.Price);
                        purchaseOrderITemByjobViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        purchaseOrderITemByjobViewModel.CreatedDate = DateTime.Now;
                        CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob> mapper = new CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob>();
                        PurchaseorderItemJob purchasejobiteminfo = mapper.Mapper(purchaseOrderITemByjobViewModel);
                        JobPurchaseOrderitem.Add(purchasejobiteminfo);
                        JobPurchaseOrderitem.Save();
                    }
                }

                TempData["PurchaseOrderDetail"] = null;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                JobPurchaseOrderitem.Dispose();
            }
        }

        [HttpPost]
        public ActionResult GetSuppliers()
        {
            try
            {
                using (Supplier)
                {
                    var supplier = Supplier.GetAll();
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<Supplier, SupplierViewModel> mapper = new CommonMapper<Supplier, SupplierViewModel>();
                    List<SupplierViewModel> supplierlist = mapper.MapToList(supplier.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(supplierlist);
                    return Json(new { list = json, length = supplierlist.Count() });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult SupportJobList(string Id)
        {
            try
            {
                using (SupportJob)
                {
                    Guid Supportid = Guid.Parse(Id);

                    var SupportjobGridList = SupportJob.GetJobsForSupport().ToList();

                    string Searchstring = Request.QueryString["searchkeyword"];

                    Nullable<int> JobType = string.IsNullOrEmpty(Request.QueryString["JobType"]) ? (int?)null :
                                                Convert.ToInt32(Request.QueryString["JobType"]);

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                          Convert.ToInt32(Request.QueryString["page_size"]);

                    SupportJobSearchViewModel SupportSearchViewModelPageSize = new SupportJobSearchViewModel
                    {
                        PageSize = PageSize,
                        searchkeyword = string.IsNullOrEmpty(Searchstring) ? "" : Searchstring,
                        JobType = JobType.HasValue ? (Constant.JobType)JobType : 0
                    };

                    SupportjobGridList = (JobType.HasValue ? JobType > 0 ? SupportjobGridList.Where(customer => customer.JobType == JobType) : SupportjobGridList : SupportjobGridList).ToList();

                    // mapping list<coreviewmodel> to list<viewmodel>
                    CommonMapper<SupportJobCoreViewModel, SupportJobViewModel> mapper = new CommonMapper<SupportJobCoreViewModel, SupportJobViewModel>();
                    List<SupportJobViewModel> Supportlist = mapper.MapToList(SupportjobGridList.ToList());
                    SupportSearchViewModelPageSize.SupportJobid = Supportid;
                    var supportListViewModel = new SupportListViewModel
                    {
                        supportJobViewModel = Supportlist,
                        supportJobSearchViewModel = SupportSearchViewModelPageSize
                    };

                   

                    return View(supportListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult SupportJobList(SupportJobSearchViewModel supportSearchViewModel)
        {
            try
            {
                using (SupportJob)
                {
                    var SupportjobGridList = SupportJob.GetJobsForSupport().ToList();

                    SupportjobGridList = ((int)supportSearchViewModel.JobType > 0 ? SupportjobGridList.Where(customer => customer.JobType ==
                                   (int)supportSearchViewModel.JobType) : SupportjobGridList).ToList();

                    string Keyword = supportSearchViewModel.searchkeyword;

                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        SupportjobGridList = SupportjobGridList.Where(customer =>
                        (customer.JobId != null && customer.JobId.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (customer.InvoiceNo != null && customer.InvoiceNo.ToString().ToLower().Contains(Keyword.ToLower()))).ToList();

                    }

                    CommonMapper<SupportJobCoreViewModel, SupportJobViewModel> mapper = new CommonMapper<SupportJobCoreViewModel, SupportJobViewModel>();
                    List<SupportJobViewModel> SupportjobViewModel = mapper.MapToList(SupportjobGridList.ToList());

                    var supportListViewModel = new SupportListViewModel
                    {
                        supportJobViewModel = SupportjobViewModel,
                        supportJobSearchViewModel = supportSearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of support job.");

                    return View(supportListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult SaveSupportJob(string jobid, string supportJobId)
        {
            try
            {
                int job_id = Convert.ToInt32(jobid);
                Guid supportJobid = Guid.Parse(supportJobId);
                var job = Employeejob.FindBy(i => i.JobId == job_id).FirstOrDefault();
                var invoice = Invoice.FindBy(i => i.JobId == job_id).FirstOrDefault();
                using (SupportJob)
                {
                    var checkExist = SupportJob.FindBy(i => i.JobId == job_id && i.SupportJobId == supportJobid).FirstOrDefault();

                    if (checkExist == null)
                    {
                        SupportJobViewModel model = new SupportJobViewModel();
                        model.JobId = job_id;


                        if (!string.IsNullOrEmpty(Convert.ToString(invoice)))
                        {
                            model.InvoiceNo = invoice.InvoiceNo;
                        }
                        model.JobType = (FSMConstant.Constant.JobType)job.JobType;
                        model.BookedDate = job.DateBooked;
                        model.Id = Guid.NewGuid();
                        model.SupportJobId = supportJobid;
                        CommonMapper<SupportJobViewModel, SupportJob> mapper = new CommonMapper<SupportJobViewModel, SupportJob>();
                        SupportJob Supportjob = mapper.Mapper(model);
                        SupportJob.Add(Supportjob);
                        SupportJob.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " saved support job.");
                    }
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // //POST:Employee/Job/GetJobDetailByJobID
        /// <summary>
        /// Get JobDetail By JobID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetJobDetailByJobID(string id)
        {
            try
            {
                Guid jobid;
                string Empid = string.Empty;
                Guid.TryParse(id, out jobid);
                EmployeeJobsViewModel employeeJobsViewModel = new EmployeeJobsViewModel();
                Jobs employeejob = Employeejob.FindBy(m => m.Id == jobid).FirstOrDefault();
                // mapping entity to viewmodel
                Empid = employeejob.BookedBy.ToString();
                var customer = Customers.FindBy(i => i.CustomerGeneralInfoId == employeejob.CustomerGeneralInfoId).FirstOrDefault();
                CommonMapper<Jobs, EmployeeJobsViewModel> mapper = new CommonMapper<Jobs, EmployeeJobsViewModel>();
                employeeJobsViewModel = mapper.Mapper(employeejob);
                List<SiteDetail> listsites = new List<SiteDetail>();
                employeeJobsViewModel.CustomerLastName = customer.CustomerLastName.ToString();
                employeeJobsViewModel.LstCustomerSiteDetail = BindsiteDetail(employeejob.CustomerGeneralInfoId.ToString());
                employeeJobsViewModel.CustomerCOLastName = GetCustomerList();
                employeeJobsViewModel.OTRWList = GetOtrwEmployeesList();
                employeeJobsViewModel.GetUserRoles = this.GetUserRoles;
                if (this.GetUserRoles[0] == "ACCOUNTS")
                {
                    employeeJobsViewModel.AccountJobType = (Constant.AccountJobType)employeeJobsViewModel.JobType;
                }
                var User = AspNetUser.FindBy(i => i.Id == Empid).FirstOrDefault();
                if (User != null)
                {
                    if (!string.IsNullOrEmpty(User.UserName))
                    { employeeJobsViewModel.BookedByName = User.UserName; }
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(employeeJobsViewModel);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed job details by job id.");

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                AspNetUser.Dispose();
                Employeejob.Dispose();
                Customers.Dispose();

            }

        }

        [HttpGet]
        public PartialViewResult _ViewjobSiteDetail(string SiteId, string jobid = "")
        {
            try
            {
                CustomerSiteDetailListViewModel Sitedetailmodel = new CustomerSiteDetailListViewModel();

                Guid SitedDetailId = Guid.Parse(SiteId);
                var SiteDetail = CustomerSiteDetail.FindBy(i => i.SiteDetailId == SitedDetailId).FirstOrDefault();
                Guid contactid = Guid.Empty;
                using (CustomerSiteDetail)
                {
                    if (SiteDetail != null)
                    {
                        CommonMapper<CustomerSiteDetail, FSM.Web.Areas.Customer.ViewModels.CustomerSiteDetailViewModel> mapper = new CommonMapper<CustomerSiteDetail, FSM.Web.Areas.Customer.ViewModels.CustomerSiteDetailViewModel>();
                        FSM.Web.Areas.Customer.ViewModels.CustomerSiteDetailViewModel CustomerSitedetailmodel = mapper.Mapper(SiteDetail);
                        Sitedetailmodel.CustomerSiteDetailmodel = CustomerSitedetailmodel;
                        //contactid = !(String.IsNullOrEmpty(CustomerSitedetailmodel.ContactId)) ? Guid.Parse(CustomerSitedetailmodel.ContactId.ToString()) : Guid.Empty;
                    }
                    using (Customers)
                    {
                        var contactName = Customers.FindBy(i => i.CustomerGeneralInfoId == SiteDetail.CustomerGeneralInfoId).FirstOrDefault();
                        if (contactName != null)
                        {
                            Sitedetailmodel.CustomerSiteDetailmodel.CustomerName = (!String.IsNullOrEmpty(contactName.CustomerLastName) ? contactName.CustomerLastName : "");
                        }
                    }

                    using (CustomerResidenceDetail)
                    {
                        var ResidenceDetail = CustomerResidenceDetail.FindBy(i => i.SiteDetailId == SiteDetail.SiteDetailId).FirstOrDefault();
                        if (ResidenceDetail != null)
                        {
                            CommonMapper<CustomerResidenceDetail, FSM.Web.Areas.Customer.ViewModels.CustomerResidenceDetailViewModel> mapper = new CommonMapper<CustomerResidenceDetail, FSM.Web.Areas.Customer.ViewModels.CustomerResidenceDetailViewModel>();
                            FSM.Web.Areas.Customer.ViewModels.CustomerResidenceDetailViewModel CustomerResidencedetailmodel = mapper.Mapper(ResidenceDetail);

                            if (CustomerResidencedetailmodel.TypeOfResidence.HasValue)
                            {
                                CustomerResidencedetailmodel.DisplayResidenceType = CustomerResidencedetailmodel.TypeOfResidence != 0 ?
                                                CustomerResidencedetailmodel.TypeOfResidence.GetAttribute<DisplayAttribute>() != null ?
                                                CustomerResidencedetailmodel.TypeOfResidence.GetAttribute<DisplayAttribute>().Name :
                                                CustomerResidencedetailmodel.TypeOfResidence.ToString() : "Not Available";
                            }
                            else
                            {
                                CustomerResidencedetailmodel.DisplayResidenceType = "Not Available";
                            }

                            CustomerResidencedetailmodel.DisplayResidenceHeight = CustomerResidencedetailmodel.Height != 0 ?
                                                CustomerResidencedetailmodel.Height.GetAttribute<DisplayAttribute>() != null ?
                                                CustomerResidencedetailmodel.Height.GetAttribute<DisplayAttribute>().Name :
                                                CustomerResidencedetailmodel.Height.ToString() : "Not Available";

                            CustomerResidencedetailmodel.DisplayRoofPitch = CustomerResidencedetailmodel.Pitch != 0 ?
                                                CustomerResidencedetailmodel.Pitch.GetAttribute<DisplayAttribute>() != null ?
                                                CustomerResidencedetailmodel.Pitch.GetAttribute<DisplayAttribute>().Name :
                                                CustomerResidencedetailmodel.Pitch.ToString() : "Not Available";

                            CustomerResidencedetailmodel.DisplayRoofType = CustomerResidencedetailmodel.RoofType != 0 ?
                                                CustomerResidencedetailmodel.RoofType.GetAttribute<DisplayAttribute>() != null ?
                                                CustomerResidencedetailmodel.RoofType.GetAttribute<DisplayAttribute>().Name :
                                                CustomerResidencedetailmodel.RoofType.ToString() : "Not Available";

                            CustomerResidencedetailmodel.DisplayGutterGaurd = CustomerResidencedetailmodel.GutterGaurd != 0 ?
                                                CustomerResidencedetailmodel.GutterGaurd.GetAttribute<DisplayAttribute>() != null ?
                                                CustomerResidencedetailmodel.GutterGaurd.GetAttribute<DisplayAttribute>().Name :
                                                CustomerResidencedetailmodel.GutterGaurd.ToString() : "Not Available";

                            Sitedetailmodel.CustomerResidenceDetailmodel = CustomerResidencedetailmodel;
                        }
                    }

                    using (CustomerconditionDetail)
                    {
                        var conditiondetail = CustomerconditionDetail.FindBy(i => i.SiteDetailId == SiteDetail.SiteDetailId).FirstOrDefault();
                        if (conditiondetail != null)
                        {
                            CommonMapper<CustomerConditionReport, FSM.Web.Areas.Customer.ViewModels.CustomerConditionReportViewModel> mapper = new CommonMapper<CustomerConditionReport, FSM.Web.Areas.Customer.ViewModels.CustomerConditionReportViewModel>();
                            FSM.Web.Areas.Customer.ViewModels.CustomerConditionReportViewModel Customerconditiondetailmodel = mapper.Mapper(conditiondetail);

                            Customerconditiondetailmodel.DisplayRoofTilesSheet = Customerconditiondetailmodel.RoofTilesSheets != 0 ?
                                                Customerconditiondetailmodel.RoofTilesSheets.GetAttribute<DisplayAttribute>() != null ?
                                                Customerconditiondetailmodel.RoofTilesSheets.GetAttribute<DisplayAttribute>().Name :
                                                Customerconditiondetailmodel.RoofTilesSheets.ToString() : "Not Available";

                            Customerconditiondetailmodel.DisplayValley = Customerconditiondetailmodel.Valleys != 0 ?
                                                Customerconditiondetailmodel.Valleys.GetAttribute<DisplayAttribute>() != null ?
                                                Customerconditiondetailmodel.Valleys.GetAttribute<DisplayAttribute>().Name :
                                                Customerconditiondetailmodel.Valleys.ToString() : "Not Available";

                            Customerconditiondetailmodel.DisplayFlashing = Customerconditiondetailmodel.Flashings != 0 ?
                                                Customerconditiondetailmodel.Flashings.GetAttribute<DisplayAttribute>() != null ?
                                                Customerconditiondetailmodel.Flashings.GetAttribute<DisplayAttribute>().Name :
                                                Customerconditiondetailmodel.Flashings.ToString() : "Not Available";

                            Customerconditiondetailmodel.DisplayGutter = Customerconditiondetailmodel.Gutters != 0 ?
                                                Customerconditiondetailmodel.Gutters.GetAttribute<DisplayAttribute>() != null ?
                                                Customerconditiondetailmodel.Gutters.GetAttribute<DisplayAttribute>().Name :
                                                Customerconditiondetailmodel.Gutters.ToString() : "Not Available";

                            Customerconditiondetailmodel.DisplayDownPipe = Customerconditiondetailmodel.DownPipes != 0 ?
                                                Customerconditiondetailmodel.DownPipes.GetAttribute<DisplayAttribute>() != null ?
                                                Customerconditiondetailmodel.DownPipes.GetAttribute<DisplayAttribute>().Name :
                                                Customerconditiondetailmodel.DownPipes.ToString() : "Not Available";

                            Sitedetailmodel.CustomerconditionDetailmodel = Customerconditiondetailmodel;
                        }
                    }

                    using (CustomerSiteDocuments)
                    {
                        var Sitedocuments = CustomerSiteDocuments.FindBy(i => i.SiteId == SiteDetail.SiteDetailId).ToList();
                        if (Sitedocuments != null)
                        {
                            CommonMapper<CustomerSitesDocuments, FSM.Web.Areas.Customer.ViewModels.CustomerSitesDocumentsViewModel> mapper = new CommonMapper<CustomerSitesDocuments, FSM.Web.Areas.Customer.ViewModels.CustomerSitesDocumentsViewModel>();
                            List<FSM.Web.Areas.Customer.ViewModels.CustomerSitesDocumentsViewModel> Customersitedocumentmodel = mapper.MapToList(Sitedocuments.ToList());
                            Sitedetailmodel.CustomerSitedocumentviewmodel = Customersitedocumentmodel;
                        }
                    }

                    using (Customercontacts)
                    {
                        var customercontacts = Customercontacts.FindBy(i => i.ContactId == contactid).FirstOrDefault();
                        if (customercontacts != null)
                        {
                            CommonMapper<CustomerContacts, FSM.Web.Areas.Customer.ViewModels.CustomerContactsViewModel> mapper = new CommonMapper<CustomerContacts, FSM.Web.Areas.Customer.ViewModels.CustomerContactsViewModel>();
                            FSM.Web.Areas.Customer.ViewModels.CustomerContactsViewModel Customercontactmodel = mapper.Mapper(customercontacts);
                            Sitedetailmodel.Customercontactsmodel = Customercontactmodel;
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job site details.");

                    return PartialView("_ViewjobSiteDetail", Sitedetailmodel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult AddEditJobPurchaseOrder(string Purchaseorderid, string JobId)
        {
            List<SupplierJobItem> SupplierJobList = new List<SupplierJobItem>();
            using (Supplier)
            {
                var suppliers = Supplier.GetAll();
                foreach (var supplier in suppliers)
                {
                    SupplierJobItem obj = new SupplierJobItem();
                    obj.ID = supplier.ID;
                    obj.Name = supplier.Name;
                    SupplierJobList.Add(obj);
                }
            }

            List<EmployeeJobDetail> empjobdetail = new List<EmployeeJobDetail>();
            using (Employeejob)
            {
                Guid id = Guid.Parse(JobId);
                var employeerJobList = Employeejob.FindBy(i => i.Id == id).AsEnumerable();

                GetJobViewModel getJobViewModel = new GetJobViewModel();
                foreach (var i in employeerJobList)
                {
                    EmployeeJobDetail obj = new EmployeeJobDetail();
                    obj.EmployeeJobId = i.Id;
                    obj.JobNo = i.JobNo;
                    obj.Description = "JobNo_" + obj.JobNo;
                    empjobdetail.Add(obj);
                }
            }

            using (Stock)
            {
                var stocks = Stock.GetAll();
                List<StockJobList> StockJobList = new List<StockJobList>();
                foreach (var stock in stocks)
                {
                    StockJobList sitem = new StockJobList();
                    sitem.StockId = stock.ID;
                    sitem.StockName = stock.Label;
                    StockJobList.Add(sitem);
                }

                JobPurChaseViewModel model = new JobPurChaseViewModel
                {
                    PurchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel(),
                    PurchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel(),
                    getjobviewmodel = new GetJobViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    Guid id = Guid.Parse(JobId);
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                    model.PurchaseOrderByJobViewModel.PurchaseOrderNo = JobPurchaseOrder.FindBy(i => i.JobID == id && i.ID== model.PurchaseOrderByJobViewModel.ID).FirstOrDefault().PurchaseOrderNo;
                }
                model.PurchaseOrderITemByJobViewModel.StockJoblist = StockJobList.OrderBy(i => i.StockName).ToList();
                model.PurchaseOrderByJobViewModel.SupplierJobList = SupplierJobList.OrderBy(i => i.Name).ToList();
                model.getjobviewmodel.employeeJobDetail = empjobdetail.OrderBy(i => i.EmployeeJobId).ToList();
                ViewBag.JobId = JobId;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added job purchage order."); 
                return View(model);
            }

        }
        [HttpGet]

        public ActionResult AddEditJobPurchaseOrderWithJobId()
        {
            try
            {
                using (Employeejob)
                {
                    var employeerJobList = Employeejob.GetAll().AsEnumerable();

                    GetJobViewModel getJobViewModel = new GetJobViewModel();
                    List<EmployeeJobDetail> li = new List<EmployeeJobDetail>();
                    foreach (var i in employeerJobList)
                    {
                        EmployeeJobDetail obj = new EmployeeJobDetail();
                        obj.EmployeeJobId = i.Id;
                        obj.JobNo = i.JobNo;
                        obj.Description = "JobNo_" + obj.JobNo;
                        li.Add(obj);
                    }
                    getJobViewModel.employeeJobDetail = li;
                    return View(getJobViewModel);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpGet]
        public ActionResult ViewJobspurchaseOrder(string Jobid)
        {
            try
            {
                using (JobPurchaseOrder)
                {
                    ///Check if the do type job exist for the jobno 
                    Guid _jobid = Guid.Parse(Jobid);
                    string jobno ;
                    
                    var JobDetail = Employeejob.FindBy(i => i.Id == _jobid ).FirstOrDefault();
                    if(JobDetail!=null)
                    {
                        jobno = JobDetail.JobNo.ToString();
                        var dojob = Employeejob.FindBy(i => i.JobNo.ToString() == jobno && i.JobType==2).FirstOrDefault();
                        if(dojob!=null)
                        {
                            
                            Jobid = dojob.Id.ToString();
                        }

                    }
                    ///

                    var purchaseorders = JobPurchaseOrder.GetjobPurchaseOrdersByJobId(Jobid);
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());

                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = "" }
                    };

                    ViewBag.JobId = Jobid;

                   

                    return View(model);
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ViewJobspurchaseOrder(PurchaseOrderjobsearchviewModel purchaseOrderjobsearchviewModel)
        {
            try
            {
                using (JobPurchaseOrder)
                {
                    string jobId = (purchaseOrderjobsearchviewModel.JobId).ToString();
                    string Searchstring = purchaseOrderjobsearchviewModel.SearchKeyword;
                    //var stocks =Stock.GetAll();
                    var purchaseorders = JobPurchaseOrder.GetjobPurchaseOrders(Searchstring);
                    purchaseorders = string.IsNullOrEmpty(jobId) ? purchaseorders : purchaseorders.Where(i => (i.JobID).ToString().Contains(jobId));
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };
                    ViewBag.JobId = jobId;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed job purchase order.");

                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult CheckPurchaseNumber(string PurchaseNo)
        {
            try
            {
                int maxpurchaseno = JobPurchaseOrder.GetMaxPurchaseNo();
                return Json(new { result = maxpurchaseno, status = "false" }, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                throw ex;

                throw;
            }
        }
        [HttpGet]

        public ActionResult AddJobPurchaseOrder(string JobId)
        {
            try
            {
                Guid id = Guid.Parse(JobId);
                var employeerJobList = Employeejob.FindBy(i => i.Id == id).AsEnumerable();
                GetJobViewModel getJobViewModel = new GetJobViewModel();
                List<EmployeeJobDetail> li = new List<EmployeeJobDetail>();
                foreach (var i in employeerJobList)
                {
                    EmployeeJobDetail obj = new EmployeeJobDetail();
                    obj.EmployeeJobId = i.Id;
                    obj.JobNo = i.JobNo;
                    obj.Description = "JobNo_" + obj.JobNo;
                    li.Add(obj);
                }
                getJobViewModel.employeeJobDetail = li;
                ViewBag.JobId = JobId;
                getJobViewModel.SupplierList = Supplier.GetAll().Select(m => new SelectListItem()
                {
                    Text = m.Name,
                    Value = m.ID.ToString()
                }).ToList();
                //pur
              //  var Jobpurchaseorderno = JobPurchaseOrder.FindBy(i => i.JobID == id).FirstOrDefault().PurchaseOrderNo;
                //if (Jobpurchaseorderno == null)
                //{
                    getJobViewModel.PurchaseOrderNo = JobPurchaseOrder.GetMaxPurchaseNo();
                //}
                //else
                //{
                //    getJobViewModel.PurchaseOrderNo =Convert.ToInt32( Jobpurchaseorderno);
                //}
                return View(getJobViewModel);


            }
            catch (Exception)
            {

                throw;
            }

        }

        //GET: checking Job Invoice Exist 
        /// <summary>
        /// Check JobInvoice Exist
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        public ActionResult CheckJobInvoice(string Id)
        {
            try
            {
                using (Invoice)
                {
                    int result = 0;
                    Guid id = Guid.Parse(Id);
                    Jobs JobDetail = Employeejob.FindBy(user => user.Id == id).FirstOrDefault();
                    Invoice InvoiceDetail = Invoice.FindBy(user => user.JobId == JobDetail.JobNo && user.IsDelete==false).FirstOrDefault();
                    if (InvoiceDetail != null)
                    {
                        result = 0;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = 1;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: Employee/Job/ReportTypeGenerate
        [ValidateInput(false)]
        [HttpGet]
        public ActionResult _ReportTypeGenerate(string EmployeeId)
        {
            try
            {
                TimeSheetDataViewModel timeSheetDataViewModel = new TimeSheetDataViewModel();

                //Get OTRW User List
                var userList = AspNetUser.GetOTRWUser().Select(m => new SelectListItem { Text = m.UserName, Value = m.Id }).ToList();
                timeSheetDataViewModel.Users = userList;
                return PartialView(timeSheetDataViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}