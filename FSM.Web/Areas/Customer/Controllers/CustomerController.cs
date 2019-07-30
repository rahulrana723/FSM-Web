using AutoMapper;
using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Customer.ViewModels;
using FSM.Web.Common;
using FSM.Web.FSMConstant;
using Microsoft.Practices.Unity;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;
using FSM.Web.Areas.Employee.ViewModels;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading.Tasks;
using log4net;
using TransmitSms;
using System.Text.RegularExpressions;

namespace FSM.Web.Areas.Customer.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                            ().DeclaringType);
        [Dependency]
        public ICustomerGeneralInfoRepository Customer { get; set; }
        [Dependency]
        public ICustomerSiteDetailRepository CustomerSiteDetail { get; set; }
        [Dependency]
        public ICustomerBillingAddressRepository CustomerBilling { get; set; }
        [Dependency]
        public ICustomerResidenceDetailRepository CustomerResidence { get; set; }
        [Dependency]
        public ICustomerConditionReportRepository ConditionReport { get; set; }
        [Dependency]
        public ICustomerSiteDocumentsRepository CustomerSitesDocuments { get; set; }
        [Dependency]
        public ICustomerContactLogRepository CustomercontactLog { get; set; }

        [Dependency]
        public IEmployeeJobRepository CustJobs { get; set; }

        [Dependency]
        public IiNoviceRepository Invrepo { get; set; }
        [Dependency]
        public IJobAssignToMappingRepository JobAssignToMappingRepo { get; set; }
        [Dependency]
        public ICustomerReminderRepository CustomerReminderRepo { get; set; }

        [Dependency]
        public ICustomerContactsRepository Customercontacts { get; set; }
        [Dependency]
        public IEmployeeDetailRepository EmployeeRepo { get; set; }
        [Dependency]
        public ICustomerJobReminderMapping CustomerJobReminderMapping { get; set; }
        [Dependency]
        public IScheduleReminderRepository ScheduleReminderRepo { get; set; }
        [Dependency]
        public IContactLogSiteContactsMappingRepository ContactLogSiteContactsMappingRepo { get; set; }
        //GET: /Customer/Customer
        /// <summary>
        /// Get Customer List
        /// </summary>
        /// <returns>Model</returns>


        [HttpGet]
        public ActionResult Index()
        {

            try
            {
                // logging user info
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed list of all customer.");

                using (Customer)
                {
                    string CustomerLastName = Request.QueryString["CustomerLastName"];
                    var customerList = Customer.GetCustomerListBySearchKeyword(CustomerLastName);
                    Nullable<int> CustomerType = string.IsNullOrEmpty(Request.QueryString["CustomerType"]) ? (int?)null :
                                                 Convert.ToInt32(Request.QueryString["CustomerType"]);

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    CustomerSearchViewModel customerSearchViewModel = new CustomerSearchViewModel
                    {
                        CustomerLastName = string.IsNullOrEmpty(CustomerLastName) ? "" : CustomerLastName,
                        CustomerType = CustomerType.HasValue ? (Constant.CustomerSearchType)CustomerType : 0,
                        GetUserRoles = this.GetUserRoles,
                        PageSize = PageSize
                    };

                    //customerList = string.IsNullOrEmpty(CustomerLastName) ? customerList : customerList.Where(customer => 
                    //               customer.CustomerLastName.Contains(customerSearchViewModel.CustomerLastName) || 
                    //               customer.CTId.ToString().Contains(customerSearchViewModel.CustomerLastName.ToString())
                    //               || ((customer.CustomerContacts.FirstOrDefault().FirstName) + " " + (customer.CustomerContacts.FirstOrDefault().LastName)).Contains(customerSearchViewModel.CustomerLastName.ToString())
                    //               || customer.Jobs.FirstOrDefault().JobNo.ToString().Contains(customerSearchViewModel.CustomerLastName));


                    if (customerSearchViewModel.CustomerType.ToString() == "Contracted")
                    {
                        customerList = (int)customerSearchViewModel.CustomerType > 0 ? (int)customerSearchViewModel.CustomerType != 9 ?
                                         customerList.Where(customer => customer.CustomerType == (int)customerSearchViewModel.CustomerType)
                                   //  : customerList.Where(customer => customer.Contracted != null) : customerList;
                                   : customerList : customerList;
                        var Sitelist = CustomerSiteDetail.GetAll().Where(i => i.Contracted != null).ToList();
                        var contractedcustomer = customerList.Where(p => Sitelist.Any(l => p.CustomerGeneralInfoId == l.CustomerGeneralInfoId))
                             .AsQueryable();
                        customerList = contractedcustomer;
                    }
                    else
                    {
                        customerList = CustomerType.HasValue ? CustomerType > 0 ? (int)customerSearchViewModel.CustomerType != 8 ?
                                       customerList.Where(customer => customer.CustomerType == CustomerType) : customerList.Where(
                                       customer => customer.BlackListed == true) : customerList : customerList;
                    }

                    CommonMapper<CustomerGeneralInfoCoreViewModel, CustomerGeneralInfoViewModel> mapper = new CommonMapper<CustomerGeneralInfoCoreViewModel, CustomerGeneralInfoViewModel>();
                    List<CustomerGeneralInfoViewModel> customerGeneralInfoViewModel = mapper.MapToList(customerList.OrderByDescending(m => m.CTId).ToList());

                    List<FSM.Web.Areas.Customer.ViewModels.CustomerGeneralInfoViewModel> customerSiteCollection =
                        customerGeneralInfoViewModel.Select(m => new FSM.Web.Areas.Customer.ViewModels.CustomerGeneralInfoViewModel
                        {
                            CTId = m.CTId,
                            CustomerLastName = m.CustomerLastName,
                            BlackListed = m.BlackListed,
                            // Contracted = m.Contracted,
                            NextContactDate = m.NextContactDate,
                            DisplayCustomerType = (int)m.CustomerType != 0 ? m.CustomerType.GetAttribute<DisplayAttribute>() != null ? m.CustomerType.GetAttribute<DisplayAttribute>().Name : m.CustomerType.ToString() : string.Empty,
                            CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                            Note = m.Note,
                            CustomerSiteCount = m.CustomerSiteCount,
                            SiteDetailId = m.SiteDetailId,
                            SiteFileName = m.SiteFileName,
                            ViewAddressClass = m.CustomerSiteCount > 0 ? string.Empty : "viewaddress-hidden"
                        }).ToList();

                    CustomerGeneralInfoViewModel customerinfoviewmodel = new CustomerGeneralInfoViewModel();
                    CustomerSitesGeneralInfoViewModel Customerwithsites = new CustomerSitesGeneralInfoViewModel();
                    using (Customer)
                    {
                        var CID = Customer.GetMaxCTId();
                        //if (CID != null)
                        //{
                        customerinfoviewmodel.CID = Convert.ToInt32(CID);

                        //}
                        customerinfoviewmodel.CTId = Customer.GetMaxCTId();
                        Customerwithsites.CID = Convert.ToInt32(CID);
                        Customerwithsites.CTId = Customer.GetMaxCTId();
                    }

                    CustomerSiteDetailViewModel customerSitesViewModel = new CustomerSiteDetailViewModel();

                    CustomerContactsViewModel CustomerContactsViewModel = new CustomerContactsViewModel();
                    customerSitesViewModel.PrefTimeOfDay = FSMConstant.Constant.PrefTimeOfDay.Anytime;
                    Customerwithsites.PrefTimeOfDay = FSMConstant.Constant.PrefTimeOfDay.Anytime;

                    var customerListViewModel = new CustomerListViewModel
                    {
                        CustomerGeneralInfoList = customerSiteCollection,
                        CustomerGeneralInfo = customerSearchViewModel,
                        CustomerGeneralInfoWizard = customerinfoviewmodel,
                        customerSitesViewModelWizard = customerSitesViewModel,
                        CustomerContactsViewModelWizard = CustomerContactsViewModel,
                        Customerwithsites = Customerwithsites
                    };



                    return View(customerListViewModel);
                }
            }
            catch (Exception ex)
            {
                // logging user info
                log.Error(base.GetUserId + ex.Message);
                throw ex;
            }
        }

        //POST:/Customer/Customer
        /// <summary>
        /// Customer searching 
        /// </summary>
        /// <param name="customerSearchViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(CustomerSearchViewModel customerSearchViewModel)
        {
            try
            {
                using (Customer)
                {
                    //var customerList = Customer.GetAll().Include("CustomerSiteDetails").Include("CustomerContacts").Include("Jobs");

                    //customerList = string.IsNullOrEmpty(customerSearchViewModel.CustomerLastName) ? customerList : customerList
                    //         .Where(customer => customer.CustomerLastName.Contains(customerSearchViewModel.CustomerLastName)
                    //         || customer.CTId.ToString().Contains(customerSearchViewModel.CustomerLastName)
                    //         || customer.Note.Contains(customerSearchViewModel.CustomerLastName)
                    //         || ((customer.CustomerContacts.FirstOrDefault().FirstName) + " " + (customer.CustomerContacts.FirstOrDefault().LastName)).Contains(customerSearchViewModel.CustomerLastName.ToString())
                    //         || customer.Jobs.FirstOrDefault().JobNo.ToString().Contains(customerSearchViewModel.CustomerLastName));

                    var customerList = Customer.GetCustomerListBySearchKeyword(customerSearchViewModel.CustomerLastName);
                    if (customerSearchViewModel.CustomerType.ToString() == "Contracted")
                    {
                        customerList = (int)customerSearchViewModel.CustomerType > 0 ? (int)customerSearchViewModel.CustomerType != 9 ?
                                         customerList.Where(customer => customer.CustomerType == (int)customerSearchViewModel.CustomerType)
                                //   : customerList.Where(customer => customer.Contracted != null) : customerList;
                                : customerList : customerList;
                        var Sitelist = CustomerSiteDetail.GetAll().Where(i => i.Contracted != null).ToList();

                        var contractedcustomer = customerList.Where(p => Sitelist.Any(l => p.CustomerGeneralInfoId == l.CustomerGeneralInfoId))
                             .AsQueryable();
                        customerList = contractedcustomer;
                    }
                    else
                    {
                        customerList = (int)customerSearchViewModel.CustomerType > 0 ? (int)customerSearchViewModel.CustomerType != 8 ?
                                         customerList.Where(customer => customer.CustomerType == (int)customerSearchViewModel.CustomerType)
                                       : customerList.Where(customer => customer.BlackListed == true) : customerList;
                    }


                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<CustomerGeneralInfoCoreViewModel, CustomerGeneralInfoViewModel> mapper = new CommonMapper<CustomerGeneralInfoCoreViewModel, CustomerGeneralInfoViewModel>();
                    List<CustomerGeneralInfoViewModel> customerGeneralInfoViewModel = mapper.MapToList(customerList.OrderByDescending(m => m.CTId).ToList());
                    CustomerSitesGeneralInfoViewModel Customerwithsites = new CustomerSitesGeneralInfoViewModel();
                    List<FSM.Web.Areas.Customer.ViewModels.CustomerGeneralInfoViewModel> customerSiteCollection =
                       customerGeneralInfoViewModel.Select(m => new FSM.Web.Areas.Customer.ViewModels.CustomerGeneralInfoViewModel
                       {
                           CTId = m.CTId,
                           CustomerLastName = m.CustomerLastName,
                           BlackListed = m.BlackListed,
                           //  Contracted = m.Contracted,
                           NextContactDate = m.NextContactDate,
                           DisplayCustomerType = (int)m.CustomerType != 0 ? m.CustomerType.GetAttribute<DisplayAttribute>() != null ? m.CustomerType.GetAttribute<DisplayAttribute>().Name : m.CustomerType.ToString() : string.Empty,
                           CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                           Note = m.Note,
                           CustomerSiteCount = m.CustomerSiteCount,
                           SiteDetailId = m.SiteDetailId,
                           SiteFileName = m.SiteFileName
                       }).ToList();


                    customerSearchViewModel.GetUserRoles = this.GetUserRoles;

                    CustomerGeneralInfoViewModel customerinfoviewmodel = new CustomerGeneralInfoViewModel();
                    using (Customer)
                    {
                        //var CID = Customer.GetMaxCID();
                        var CID = Customer.GetMaxCTId();
                        //if (CID != null)
                        //{
                        customerinfoviewmodel.CID = Convert.ToInt32(CID);
                        ////}
                        customerinfoviewmodel.CTId = Customer.GetMaxCTId();
                        Customerwithsites.CID = Convert.ToInt32(CID);
                        Customerwithsites.CTId = Customer.GetMaxCTId();
                    }




                    CustomerSiteDetailViewModel customerSitesViewModel = new CustomerSiteDetailViewModel();
                    CustomerContactsViewModel CustomerContactsViewModel = new CustomerContactsViewModel();
                    customerSitesViewModel.PrefTimeOfDay = FSMConstant.Constant.PrefTimeOfDay.Anytime;
                    Customerwithsites.PrefTimeOfDay = FSMConstant.Constant.PrefTimeOfDay.Anytime;
                    var customerListViewModel = new CustomerListViewModel
                    {
                        CustomerGeneralInfoList = customerSiteCollection,
                        CustomerGeneralInfo = customerSearchViewModel,
                        CustomerGeneralInfoWizard = customerinfoviewmodel,
                        customerSitesViewModelWizard = customerSitesViewModel,
                        CustomerContactsViewModelWizard = CustomerContactsViewModel,
                        Customerwithsites = Customerwithsites
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of all customer based on search.");

                    return View(customerListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: /Customer/Customer/AddCustomer
        /// <summary>
        /// Add New Customer
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult AddCustomer()
        {
            CustomerGeneralInfoViewModel customerinfoviewmodel = new CustomerGeneralInfoViewModel();
            using (Customer)
            {
                var CID = Customer.GetMaxCID();
                if (CID != null)
                {
                    customerinfoviewmodel.CID = Convert.ToInt32(CID);
                }
                customerinfoviewmodel.CTId = Customer.GetMaxCTId();
            }
            return View(customerinfoviewmodel);
        }

        //POST:/Customer/Customer/AddCustomer
        /// <summary>
        /// Add Customer Information
        /// </summary>
        /// <param name="customerGeneralInfoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCustomer(CustomerGeneralInfoViewModel customerGeneralInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerGeneralInfoViewModel.CustomerType == 0)
                    {
                        ModelState.AddModelError("CustomerType", "Customer type is required!");
                        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    }
                    //if (customerGeneralInfoViewModel.Frequency == 0)
                    //{
                    //    ModelState.AddModelError("Frequency", "Frequency is required!");
                    //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //}

                    using (Customer)
                    {
                        if (customerGeneralInfoViewModel.CustomerGeneralInfoId != Guid.Empty)
                        {
                            CustomerGeneralInfo customerGeneralInfo = Customer.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoViewModel.CustomerGeneralInfoId).FirstOrDefault();
                            customerGeneralInfo.ModifiedDate = DateTime.Now;
                            customerGeneralInfo.ModifiedBy = Guid.Parse(base.GetUserId);
                            customerGeneralInfo.IsDelete = false;
                            customerGeneralInfo.CreatedBy = customerGeneralInfo.CreatedBy;
                            customerGeneralInfo.CreatedDate = customerGeneralInfo.CreatedDate;
                            Customer.DeAttach(customerGeneralInfo);
                            CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo>();
                            customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);
                            Customer.Edit(customerGeneralInfo);
                            Customer.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " updated an existing customer of " + customerGeneralInfoViewModel.CustomerType + " customer type.");

                            return Json(new { id = customerGeneralInfoViewModel.CustomerGeneralInfoId.ToString(), activetab = "General Info" });
                        }
                        else
                        {
                            customerGeneralInfoViewModel.CustomerGeneralInfoId = Guid.NewGuid();

                            customerGeneralInfoViewModel.IsDelete = false;
                            customerGeneralInfoViewModel.CreatedDate = DateTime.Now;
                            customerGeneralInfoViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                            // mapping viewmodel to entity
                            CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo>();
                            CustomerGeneralInfo customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);

                            Customer.Add(customerGeneralInfo);
                            Customer.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added new customer of " + customerGeneralInfoViewModel.CustomerType + " customer type.");

                            return Json(new { id = customerGeneralInfoViewModel.CustomerGeneralInfoId.ToString(), activetab = "General Info" });
                        }
                    }
                }

                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }







        //POST:/Customer/Customer/AddCustomer
        /// <summary>
        /// Add Customer Information
        /// </summary>
        /// <param name="customerGeneralInfoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCustomerwithsites(CustomerSitesGeneralInfoViewModel customerGeneralInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerGeneralInfoViewModel.CustomerType == 0)
                    {
                        ModelState.AddModelError("CustomerType", "Customer type is required!");
                        return Json(ModelState.Values.SelectMany(m => m.Errors));


                    }

                    if (String.IsNullOrEmpty(customerGeneralInfoViewModel.SiteFileName))
                    {
                        ModelState.AddModelError("SiteFileName", "File Name is required!");
                        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    }
                    //if (customerGeneralInfoViewModel.Frequency == 0)
                    //{
                    //    ModelState.AddModelError("Frequency", "Frequency is required!");
                    //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //}

                    using (Customer)
                    {
                        if (customerGeneralInfoViewModel.CustomerGeneralInfoId != Guid.Empty)
                        {
                            CustomerGeneralInfo customerGeneralInfo = Customer.FindBy(m => m.CustomerGeneralInfoId == customerGeneralInfoViewModel.CustomerGeneralInfoId).FirstOrDefault();
                            customerGeneralInfo.ModifiedDate = DateTime.Now;
                            customerGeneralInfo.ModifiedBy = Guid.Parse(base.GetUserId);
                            customerGeneralInfo.CreatedBy = customerGeneralInfo.CreatedBy;
                            customerGeneralInfoViewModel.IsDelete = false;
                            customerGeneralInfo.CustomerLastName = customerGeneralInfoViewModel.SiteFileName;
                            customerGeneralInfoViewModel.CustomerLastName = customerGeneralInfoViewModel.SiteFileName;
                            customerGeneralInfo.CreatedDate = customerGeneralInfo.CreatedDate;
                            Customer.DeAttach(customerGeneralInfo);
                            CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerGeneralInfo>();
                            customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);
                            Customer.Edit(customerGeneralInfo);
                            Customer.Save();
                            using (CustomerSiteDetail)
                            {
                                if (customerGeneralInfoViewModel.SiteDetailId != Guid.Empty)
                                {
                                    CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerSiteDetail>();
                                    CustomerSiteDetail customerSite = mapsitedetail.Mapper(customerGeneralInfoViewModel);
                                    customerSite.ModifiedDate = DateTime.Now;
                                    customerSite.ModifiedBy = Guid.Parse(base.GetUserId);
                                    customerSite.IsDelete = false;
                                    CustomerSiteDetail.DeAttach(customerSite);
                                    CustomerSiteDetail.Edit(customerSite);
                                    CustomerSiteDetail.Save();

                                    return Json(new { id = customerGeneralInfoViewModel.CustomerGeneralInfoId.ToString(), siteid = customerGeneralInfoViewModel.SiteDetailId.ToString(), activetab = "General Info" });
                                }
                            }
                        }
                        else
                        {
                            customerGeneralInfoViewModel.CustomerGeneralInfoId = Guid.NewGuid();

                            customerGeneralInfoViewModel.IsDelete = false;
                            customerGeneralInfoViewModel.CreatedDate = DateTime.Now;
                            customerGeneralInfoViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            customerGeneralInfoViewModel.CustomerLastName = customerGeneralInfoViewModel.SiteFileName;

                            // mapping viewmodel to entity
                            CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerGeneralInfo>();
                            CustomerGeneralInfo customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);

                            Customer.Add(customerGeneralInfo);
                            Customer.Save();

                            CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSitesGeneralInfoViewModel, CustomerSiteDetail>();
                            CustomerSiteDetail customerSiteDetail = mapsitedetail.Mapper(customerGeneralInfoViewModel);
                            customerSiteDetail.CustomerGeneralInfoId = customerGeneralInfoViewModel.CustomerGeneralInfoId;
                            customerSiteDetail.SiteDetailId = Guid.NewGuid();
                            customerSiteDetail.CreatedDate = DateTime.Now;
                            customerSiteDetail.CreatedBy = Guid.Parse(base.GetUserId);
                            customerSiteDetail.IsDelete = false;
                            //customerSiteDetail.ContactId = ContactId;

                            // getting lattitude and longitude
                            string address = customerGeneralInfoViewModel.Street + " " + customerGeneralInfoViewModel.StreetName + " " +
                                            customerGeneralInfoViewModel.Suburb + " " + customerGeneralInfoViewModel.State + " " + customerGeneralInfoViewModel.PostalCode;
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

                            CustomerSiteDetail.Add(customerSiteDetail);
                            CustomerSiteDetail.Save();
                            CustomerResidenceDetailViewModel model = new CustomerResidenceDetailViewModel();
                            model.ResidenceDetailId = Guid.NewGuid();
                            model.SiteDetailId = customerSiteDetail.SiteDetailId;
                            model.NeedTwoPPL = true;
                            model.SRASinstalled = FSMConstant.Constant.SRAS.None;
                            model.CreatedBy = Guid.Parse(base.GetUserId);
                            model.CreatedDate = DateTime.Now;
                            model.IsDelete = false;
                            CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                            CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(model);
                            CustomerResidence.Add(customerResidenceDetail);
                            CustomerResidence.Save();
                            return Json(new { id = customerGeneralInfoViewModel.CustomerGeneralInfoId.ToString(), siteid = customerSiteDetail.SiteDetailId.ToString(), activetab = "General Info" });
                        }
                    }
                }
                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }

        }










        //GET:/Customer/Customer/EditCustomer
        /// <summary>
        /// Edit Customer General Info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCustomer(string id)
        {
            try
            {
                using (Customer)
                {
                    Guid CustomerGeneralInfoId;
                    Guid.TryParse(id, out CustomerGeneralInfoId);

                    CustomerGeneralInfo customerGeneralInfo = Customer.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();

                    // mapping entity to viewmodel
                    CommonMapper<CustomerGeneralInfo, CustomerGeneralInfoViewModel> mapper = new CommonMapper<CustomerGeneralInfo, CustomerGeneralInfoViewModel>();
                    CustomerGeneralInfoViewModel customerGeneralInfoViewModel = mapper.Mapper(customerGeneralInfo);
                    var userName = "";
                    if (customerGeneralInfoViewModel.ModifiedBy == null)
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerGeneralInfoViewModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerGeneralInfoViewModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (customerGeneralInfoViewModel.ModifiedDate == null)
                    {
                        customerGeneralInfoViewModel.CreatedDate = customerGeneralInfoViewModel.CreatedDate;
                    }
                    else
                    {
                        customerGeneralInfoViewModel.ModifiedDate = customerGeneralInfoViewModel.ModifiedDate;
                    }
                    customerGeneralInfoViewModel.UserName = userName;

                    return View(customerGeneralInfoViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        //POST:/Customer/Customer/EditCustomer
        /// <summary>
        /// Edit Customer General Info
        /// </summary>
        /// <param name="customerGeneralInfoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCustomer(CustomerGeneralInfoViewModel customerGeneralInfoViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (Customer)
                    {
                        customerGeneralInfoViewModel.ModifiedDate = DateTime.Now;
                        customerGeneralInfoViewModel.ModifiedBy = Guid.Parse(base.GetUserId);


                        // mapping viewmodel to entity
                        CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerGeneralInfoViewModel, CustomerGeneralInfo>();
                        CustomerGeneralInfo customerGeneralInfo = mapper.Mapper(customerGeneralInfoViewModel);
                        customerGeneralInfo.IsDelete = false;
                        Customer.Edit(customerGeneralInfo);
                        Customer.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated General Info of a customer.");

                        return Json(new { id = customerGeneralInfoViewModel.CustomerGeneralInfoId.ToString(), activetab = "General Info" });
                    }
                }

                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }

        }


        //GET:/Customer/Customer/ManageCustomerSites
        /// <summary>
        /// Manage Customer Site Using CustomerGeneralInfoId
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ManageCustomerSites(string CustomerGeneralInfoId)
        {
            try
            {
                CustomerSitesViewModel customerSitesViewModel = new CustomerSitesViewModel();

                Guid customerGeneralInfoId;
                Guid.TryParse(CustomerGeneralInfoId, out customerGeneralInfoId);
                customerSitesViewModel.CustomerGeneralInfoId = customerGeneralInfoId;
                customerSitesViewModel.SiteCount = Request.QueryString["SiteCount"] != null ?
                                                   int.Parse(Request.QueryString["SiteCount"]) : -1;

                // binding grid view
                var CustomerSiteList = CustomerSiteDetail.GetCustomerSiteList(CustomerGeneralInfoId).ToList();


                CommonMapper<FSM.Core.ViewModels.DisplaySitesViewModel, FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.DisplaySitesViewModel, FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel>();
                List<FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel> customerSiteList = mapper.MapToList(CustomerSiteList);

                List<FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel> customerSiteCollection =
                    customerSiteList.Select(m => new FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel
                    {
                        Notes = m.Notes,
                        SiteFileName = m.SiteFileName,
                        StreetName = m.StreetName,
                        DisplayRoofType = m.RoofType != null ? (int)m.RoofType < 5 ? m.RoofType.GetAttribute<DisplayAttribute>() != null ? m.RoofType.GetAttribute<DisplayAttribute>().Name : m.RoofType.ToString() : string.Empty : string.Empty,
                        DisplayResidenceType = m.TypeOfResidence != null ? (int)m.TypeOfResidence != 0 ? m.TypeOfResidence.GetAttribute<DisplayAttribute>() != null ? m.TypeOfResidence.GetAttribute<DisplayAttribute>().Name : m.TypeOfResidence.ToString() : string.Empty : string.Empty,
                        DisplayStreetType = m.StreetType != null ? (int)m.StreetType != 0 ? m.StreetType.GetAttribute<DisplayAttribute>() != null ? m.StreetType.GetAttribute<DisplayAttribute>().Name : m.StreetType.ToString() : string.Empty : string.Empty,
                        IsContracted = m.Contracted != null ? (int)m.Contracted != 0 ? m.Contracted.GetAttribute<DisplayAttribute>() != null ? "Yes" : "Yes" : "No" : "No",
                        SiteDetailId = m.SiteDetailId
                    }).ToList();

                customerSitesViewModel.DisplaySitesViewModel = customerSiteCollection;
                customerSitesViewModel.PageSize = 10; // default page size

                // binding drodownlist
                customerSitesViewModel.ContactList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsContact == true).Select(m =>
                        new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.ContactList.OrderBy(m => m.Text);

                //binding Strata manager drodownlist
                customerSitesViewModel.StrataManagerList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsStrataManager == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.StrataManagerList.OrderBy(m => m.Text);

                //binding Real Estate drodownlist
                customerSitesViewModel.RealEstateList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsRealEstate == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.RealEstateList.OrderBy(m => m.Text);

                if (customerSitesViewModel.SiteCount == 1)
                {
                    customerSitesViewModel.SiteDetailId = customerSiteCollection[0].SiteDetailId;

                    //CustomerSiteDetail customerSiteDetail = CustomerSiteDetail.FindBy(m => m.SiteDetailId == customerSitesViewModel.SiteDetailId).FirstOrDefault();
                    ////Get UserName And Created Or Modified Date
                    //var userName = "";
                    //if (customerSiteDetail.ModifiedBy == null)
                    //{
                    //    userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerSiteDetail.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    //}
                    //else
                    //{
                    //    userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerSiteDetail.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    //}
                    //if (customerSiteDetail.ModifiedDate == null)
                    //{
                    //    customerSitesViewModel.CreatedDate = customerSiteDetail.CreatedDate;
                    //}
                    //else
                    //{
                    //    customerSitesViewModel.ModifiedDate = customerSiteDetail.ModifiedDate;
                    //}
                    //customerSitesViewModel.UserName = userName;
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed customer sites.");

                return View(customerSitesViewModel);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
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
        public ActionResult ManageCustomerSitesPartial(string SiteDetailId)
        {
            try
            {
                CustomerSitesViewModel customerSitesViewModel = new CustomerSitesViewModel();

                Guid siteDetailId;
                Guid.TryParse(SiteDetailId, out siteDetailId);

                CustomerSiteDetail customerSiteDetail = CustomerSiteDetail.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel> mapsitedetail = new CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel>();
                CustomerSiteDetailViewModel customerSiteDetailViewModel = mapsitedetail.Mapper(customerSiteDetail);

                CustomerResidenceDetail customerResidenceDetail = CustomerResidence.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                CommonMapper<CustomerResidenceDetail, CustomerResidenceDetailViewModel> mapresidencedetail = new CommonMapper<CustomerResidenceDetail, CustomerResidenceDetailViewModel>();
                CustomerResidenceDetailViewModel customerResidenceDetailViewModel = mapresidencedetail.Mapper(customerResidenceDetail);

                CustomerConditionReport customerConditionReport = ConditionReport.FindBy(m => m.SiteDetailId == siteDetailId).ToList().FirstOrDefault();
                if (customerConditionReport != null)
                {
                    if (customerConditionReport.ConditionRoof == null)
                    {
                        customerConditionReport.ConditionRoof = 0;
                    }
                }
                CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel> mapconditiondetail = new CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel>();
                CustomerConditionReportViewModel customerConditionReportViewModel = mapconditiondetail.Mapper(customerConditionReport);

                customerSitesViewModel.CustomerSiteDetailViewModel = customerSiteDetailViewModel;
                if (customerResidenceDetail != null)
                {
                    customerSitesViewModel.CustomerResidenceDetailViewModel = customerResidenceDetailViewModel;
                    customerSitesViewModel.ResidenceDetailId = customerResidenceDetail.ResidenceDetailId;
                }
                if (customerConditionReport != null)
                {
                    customerSitesViewModel.CustomerConditionReportViewModel = customerConditionReportViewModel;
                    customerSitesViewModel.ConditionReportId = customerConditionReport.ConditionReportId;
                }
                customerSitesViewModel.CustomerGeneralInfoId = customerSiteDetailViewModel.CustomerGeneralInfoId;
                customerSitesViewModel.SiteDetailId = siteDetailId;



                // binding drodownlist
                customerSitesViewModel.ContactList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerSitesViewModel.CustomerGeneralInfoId && m.IsContact == true).Select(m =>
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

                customerSitesViewModel.SiteCount = CustomerSiteDetail.GetCustomerSiteList(customerSitesViewModel.CustomerGeneralInfoId.ToString()).Count();

                //Get UserName And Created Or Modified Date
                var userName = "";
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
                    customerSitesViewModel.CreatedDate = customerSiteDetail.CreatedDate;
                }
                else
                {
                    customerSitesViewModel.ModifiedDate = customerSiteDetail.ModifiedDate;
                }
                customerSitesViewModel.UserName = userName;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed customer sites of " + SiteDetailId);

                return PartialView("_CustomerSiteForm", customerSitesViewModel);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET:Manage Customer Sites
        /// <summary>
        /// Manage Customer Sites Using CustomerGeneralInfoId
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ManageCustomerSitesAddPartial(string CustomerGeneralInfoId)
        {
            try
            {
                CustomerSitesViewModel customerSitesViewModel = new CustomerSitesViewModel();

                Guid customerGeneralInfoId;
                Guid.TryParse(CustomerGeneralInfoId, out customerGeneralInfoId);
                customerSitesViewModel.CustomerGeneralInfoId = customerGeneralInfoId;

                // binding drodownlist
                customerSitesViewModel.ContactList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsContact == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.ContactList.OrderBy(m => m.Text);
                //binding Strata manager drodownlist
                customerSitesViewModel.StrataManagerList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsStrataManager == true).Select(m =>
                    new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.StrataManagerList.OrderBy(m => m.Text);
                //binding Real Estate drodownlist
                customerSitesViewModel.RealEstateList = Customercontacts.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsRealEstate == true).Select(m =>
                      new SelectListItem { Text = m.FirstName + " " + m.LastName, Value = m.ContactId.ToString() }).ToList();
                customerSitesViewModel.RealEstateList.OrderBy(m => m.Text);

                customerSitesViewModel.HideAddCustomer = Request.QueryString["HideAddCustomer"];

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed customer sites using " + CustomerGeneralInfoId);

                return PartialView("_CustomerSiteForm", customerSitesViewModel);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET:Customer Site List
        /// <summary>
        /// Show List Of Customer Site
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerSiteList()
        {
            CustomerSitesViewModel customerSitesViewModel = new CustomerSitesViewModel();
            string Name = Request.QueryString["Name"] == null ? string.Empty : Request.QueryString["Name"];
            string StreetName = Request.QueryString["StreetName"] == null ? string.Empty : Request.QueryString["StreetName"];
            string CustomerGeneralInfoId = Request.QueryString["CustomerGeneralInfoId"];
            //string ConatctName = Request.QueryString["ConatctName"] == null ? string.Empty : Request.QueryString["ConatctName"];
            //string StrataManager = Request.QueryString["StrataManager"] == null ? string.Empty : Request.QueryString["StrataManager"];
            bool Contracted = Convert.ToBoolean(Request.QueryString["Contracted"]);
            var CustomerSiteList = CustomerSiteDetail.GetCustomerSiteList(CustomerGeneralInfoId);
            var StreetTypes = Enum.GetValues(typeof(Constant.HomeAddressStreetType)).Cast<Constant.HomeAddressStreetType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            var RoofTypes = Enum.GetValues(typeof(Constant.RoofType)).Cast<Constant.RoofType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            var TypeOfResidences = Enum.GetValues(typeof(Constant.ResidenceType)).Cast<Constant.ResidenceType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();


            CustomerSiteList = (from left in CustomerSiteList
                                join right in StreetTypes on left.StreetType.ToString() equals right.Value into joinedList
                                from sub in joinedList.DefaultIfEmpty()
                                join right1 in RoofTypes on left.RoofType.ToString() equals right1.Value into joinedList1
                                from sub1 in joinedList1.DefaultIfEmpty()
                                    //join right2 in TypeOfResidences on left.TypeOfResidence.ToString() equals right2.Value into joinedList2
                                    //from sub2 in joinedList2.DefaultIfEmpty()
                                select new FSM.Core.ViewModels.DisplaySitesViewModel()
                                {
                                    SiteDetailId = left.SiteDetailId,
                                    Notes = left.Notes,
                                    Contracted = left.Contracted,
                                    SiteFileName = left.SiteFileName,
                                    //StreetName = left.StreetName,
                                    //StreetType = left.StreetType,
                                    //State = left.State,
                                    //Suburb = left.Suburb,
                                    TypeOfResidence = left.TypeOfResidence,
                                    RoofType = left.RoofType,
                                    //DownPipe = left.DownPipe,
                                    RoofTypeText = sub1.Text,
                                    //TypeOfResidenceText = sub2.Text
                                });

            if (Name != string.Empty || Name != "")
            {
                CustomerSiteList = CustomerSiteList.Where(m =>
                                    (m.Notes != null && m.Notes.ToLower().Contains(Name.ToLower()) && Name != "") ||
                                    (m.SiteFileName != null && m.SiteFileName.ToLower().Contains(Name.ToLower()) && Name != "") ||
                                           //(m.StrataManagerName != null && m.StrataManagerName.ToLower().Contains(Name.ToLower()) && Name != "") ||
                                           //(m.StreeTypeText != null && m.StreeTypeText.ToLower().Contains(Name.ToLower()) && Name != "") ||
                                           (m.RoofTypeText != null && m.RoofTypeText.ToLower().Contains(Name.ToLower()) && Name != "")
                                //(m.TypeOfResidenceText != null && m.TypeOfResidenceText.ToLower().Contains(Name.ToLower()) && Name != "")
                                );
            }

            if (Contracted == true)
            {
                CustomerSiteList = CustomerSiteList.Where(customer => customer.Contracted != 0 && customer.Contracted != null);
            }

            // mapping list<entity> to list<viewmodel>
            CommonMapper<FSM.Core.ViewModels.DisplaySitesViewModel, FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.DisplaySitesViewModel, FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel>();
            List<FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel> customerSiteList = mapper.MapToList(CustomerSiteList.ToList());



            List<FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel> customerSiteCollection =
                    customerSiteList.Select(m => new FSM.Web.Areas.Customer.ViewModels.DisplaySitesViewModel
                    {
                        Notes = m.Notes,
                        //StrataManagerName = m.StrataManagerName,
                        //StreetName = m.StreetName,
                        //State = m.State,
                        SiteFileName = m.SiteFileName,
                        DisplayRoofType = m.RoofType != null ? (int)m.RoofType < 5 ? m.RoofType.GetAttribute<DisplayAttribute>() != null ? m.RoofType.GetAttribute<DisplayAttribute>().Name : m.RoofType.ToString() : string.Empty : string.Empty,
                        //DisplayRoofType = (int)m.RoofType != 0 ? m.RoofType.GetAttribute<DisplayAttribute>() != null ? m.RoofType.GetAttribute<DisplayAttribute>().Name : m.RoofType.ToString() : string.Empty,
                        //DisplayStreetType = (int)m.StreetType != 0 ? m.StreetType.GetAttribute<DisplayAttribute>() != null ? m.StreetType.GetAttribute<DisplayAttribute>().Name : m.StreetType.ToString() : string.Empty,
                        //DisplayResidenceType = m.TypeOfResidence != null ? (int)m.TypeOfResidence != 0 ? m.TypeOfResidence.GetAttribute<DisplayAttribute>() != null ? m.TypeOfResidence.GetAttribute<DisplayAttribute>().Name : m.TypeOfResidence.ToString() : string.Empty : string.Empty,
                        IsContracted = m.Contracted != null ? (int)m.Contracted != 0 ? m.Contracted.GetAttribute<DisplayAttribute>() != null ? "Yes" : "Yes" : "No" : "No",
                        SiteDetailId = m.SiteDetailId
                    }).ToList();

            customerSitesViewModel.DisplaySitesViewModel = customerSiteCollection;
            customerSitesViewModel.PageSize = Request.QueryString["page_size"] == null ? 2 : Convert.ToInt32(Request.QueryString["page_size"]);
            customerSitesViewModel.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralInfoId);

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " show list of customer site.");

            return PartialView("_CustomerSiteList", customerSitesViewModel);
        }

        //GET:Customer Site Documents List
        /// <summary>
        /// Show List Of Customer Site Documents
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerSiteDocumentsList()
        {
            CustomerSiteDocumentsListViewModel customerSitesViewModel = new CustomerSiteDocumentsListViewModel();
            string Name = Request.QueryString["Name"];
            string custgeninfoid = Request.QueryString["customerGeneralInfoId"];
            Guid Id = Guid.Parse(custgeninfoid);
            var CustomerSiteList = CustomerSitesDocuments.GetSiteDocumentList(Name, Id).ToList();


            // mapping list<entity> to list<viewmodel>
            CommonMapper<FSM.Core.ViewModels.CustomerSitesDocumentsViewModelCore, CustomerSitesDocumentsViewModelCore> mapper = new CommonMapper<FSM.Core.ViewModels.CustomerSitesDocumentsViewModelCore, CustomerSitesDocumentsViewModelCore>();
            List<CustomerSitesDocumentsViewModelCore> customerSiteList = mapper.MapToList(CustomerSiteList.ToList());

            customerSitesViewModel.CustomerSiteDocumentsCoreViewModelList = customerSiteList;
            //customerSitesViewModel.PageSize = Request.QueryString["page_size"] == null ? 2 : Convert.ToInt32(Request.QueryString["page_size"]);

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " show list of customer site documents.");

            return PartialView("_CustomerSiteDocumentList", customerSitesViewModel);
        }


        //GET:AddNewSiteWizard
        /// <summary>
        /// AddNewSiteWizard
        /// </summary>
        /// <param name="AddNewSiteWizard"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewSiteWizard(CustomerSiteDetailViewModel customerSitesViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // saving sitedetail info
                    using (CustomerSiteDetail)
                    {
                        if (customerSitesViewModel.SiteDetailId != Guid.Empty)
                        {
                            CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail>();
                            CustomerSiteDetail customerSite = mapsitedetail.Mapper(customerSitesViewModel);
                            customerSite.ModifiedDate = DateTime.Now;
                            customerSite.ModifiedBy = Guid.Parse(base.GetUserId);
                            customerSite.IsDelete = false;
                            CustomerSiteDetail.DeAttach(customerSite);
                            CustomerSiteDetail.Edit(customerSite);
                            CustomerSiteDetail.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " updated an existing site wizard.");

                            return Json(new { id = customerSitesViewModel.CustomerGeneralInfoId.ToString(), SiteId = customerSite.SiteDetailId.ToString(), activetab = "Customer Sites" });
                        }
                        else
                        {
                            CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail>();
                            CustomerSiteDetail customerSiteDetail = mapsitedetail.Mapper(customerSitesViewModel);
                            customerSiteDetail.CustomerGeneralInfoId = customerSitesViewModel.CustomerGeneralInfoId;
                            customerSiteDetail.SiteDetailId = Guid.NewGuid();
                            customerSiteDetail.CreatedDate = DateTime.Now;
                            customerSiteDetail.CreatedBy = Guid.Parse(base.GetUserId);
                            customerSiteDetail.IsDelete = false;
                            //customerSiteDetail.ContactId = ContactId;

                            // getting lattitude and longitude
                            string address = customerSitesViewModel.Street + " " + customerSitesViewModel.StreetName + " " +
                                            customerSitesViewModel.Suburb + " " + customerSitesViewModel.State + " " + customerSitesViewModel.PostalCode;

                            string[] arrLatLong = { };

                            for (int Index = 0; Index > 4; Index++)
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

                            CustomerSiteDetail.Add(customerSiteDetail);
                            CustomerSiteDetail.Save();
                            CustomerResidenceDetailViewModel model = new CustomerResidenceDetailViewModel();
                            model.ResidenceDetailId = Guid.NewGuid();
                            model.SiteDetailId = customerSiteDetail.SiteDetailId;
                            model.NeedTwoPPL = true;
                            model.SRASinstalled = FSMConstant.Constant.SRAS.None;
                            model.CreatedBy = Guid.Parse(base.GetUserId);
                            model.CreatedDate = DateTime.Now;
                            model.IsDelete = false;
                            CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                            CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(model);
                            CustomerResidence.Add(customerResidenceDetail);
                            CustomerResidence.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added new site wizard.");

                            return Json(new { id = customerSitesViewModel.CustomerGeneralInfoId.ToString(), SiteId = customerSiteDetail.SiteDetailId.ToString(), activetab = "Customer Sites" });
                        }
                    }

                }

                return Json(ModelState.Values.SelectMany(m => m.Errors));
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
                    //        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //    }
                    //}
                    //if (customerSitesViewModel.CustomerResidenceDetailViewModel.TypeOfResidence == null)
                    //{
                    //    ModelState.AddModelError("TypeOfResidence", @"Residence type is required  !");
                    //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //}
                    //if (customerSitesViewModel.CustomerSiteDetailViewModel.Contracted != 0 && customerSitesViewModel.CustomerSiteDetailViewModel.Contracted != null)
                    //{
                    //    if (customerSitesViewModel.CustomerSiteDetailViewModel.ScheduledPrice == null)
                    //    {
                    //        ModelState.AddModelError("ScheduledPrice", "Scheduled Price is required!");
                    //        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //    }
                    //}

                    CustomerSiteDetailViewModel customerSiteDetailViewModel = customerSitesViewModel.CustomerSiteDetailViewModel;
                    customerSiteDetailViewModel.SiteDetailId = Guid.NewGuid();
                    customerSiteDetailViewModel.IsDelete = false;
                    customerSiteDetailViewModel.CreatedDate = DateTime.Now;
                    customerSiteDetailViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    CustomerResidenceDetailViewModel customerResidenceDetailViewModel = customerSitesViewModel.CustomerResidenceDetailViewModel;
                    customerResidenceDetailViewModel.ResidenceDetailId = Guid.NewGuid();
                    customerResidenceDetailViewModel.IsDelete = false;
                    customerResidenceDetailViewModel.CreatedDate = DateTime.Now;
                    customerResidenceDetailViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    CustomerConditionReportViewModel customerConditionReportViewModel = customerSitesViewModel.CustomerConditionReportViewModel;
                    customerConditionReportViewModel.IsDelete = false;
                    customerConditionReportViewModel.ConditionReportId = Guid.NewGuid();
                    customerConditionReportViewModel.CreatedDate = DateTime.Now;
                    customerConditionReportViewModel.CreatedBy = Guid.Parse(base.GetUserId);

                    // saving sitedetail info
                    using (CustomerSiteDetail)
                    {
                        CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail>();
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

                        CustomerSiteDetail.Add(customerSiteDetail);
                        CustomerSiteDetail.Save();
                    }

                    // saving residencedetail info
                    using (CustomerResidence)
                    {
                        CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                        CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(customerResidenceDetailViewModel);
                        customerResidenceDetail.SiteDetailId = customerSiteDetailViewModel.SiteDetailId;
                        customerResidenceDetail.NeedTwoPPL = true;
                        CustomerResidence.Add(customerResidenceDetail);
                        CustomerResidence.Save();
                    }

                    // saving conditionreport info
                    using (ConditionReport)
                    {
                        CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport> mapper = new CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport>();
                        CustomerConditionReport customerConditionReport = mapper.Mapper(customerConditionReportViewModel);
                        customerConditionReport.SiteDetailId = customerSiteDetailViewModel.SiteDetailId;
                        ConditionReport.Add(customerConditionReport);
                        ConditionReport.Save();
                    }


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " saved customer list.");

                    //return Json(new { Status = "ok" });
                    return Json(new { id = customerSitesViewModel.CustomerGeneralInfoId.ToString(), activetab = "Customer Sites" });
                }

                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }

        }

        //GET:Update Customer 
        /// <summary>
        /// Update List Of Customer
        /// </summary>
        /// <param name="customerSitesViewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult EditCustomerList(CustomerSitesViewModel customerSitesViewModel)
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
                    //        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //    }
                    //}
                    //if (customerSitesViewModel.CustomerResidenceDetailViewModel.TypeOfResidence == null)
                    //{
                    //    ModelState.AddModelError("TypeOfResidence", @"Residence type is required  !");
                    //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //}
                    //if (customerSitesViewModel.CustomerSiteDetailViewModel.Contracted != 0 && customerSitesViewModel.CustomerSiteDetailViewModel.Contracted != null)
                    //{
                    //    if (customerSitesViewModel.CustomerSiteDetailViewModel.ScheduledPrice == null)
                    //    {
                    //        ModelState.AddModelError("ScheduledPrice", "Scheduled Price is required!");
                    //        return Json(ModelState.Values.SelectMany(m => m.Errors));
                    //    }
                    //}

                    CustomerSiteDetailViewModel customerSiteDetailViewModel = customerSitesViewModel.CustomerSiteDetailViewModel;
                    customerSiteDetailViewModel.SiteDetailId = customerSitesViewModel.SiteDetailId;
                    customerSiteDetailViewModel.CustomerGeneralInfoId = customerSitesViewModel.CustomerGeneralInfoId;
                    customerSiteDetailViewModel.ModifiedDate = DateTime.Now;
                    customerSiteDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    CustomerResidenceDetailViewModel customerResidenceDetailViewModel = customerSitesViewModel.CustomerResidenceDetailViewModel;
                    customerResidenceDetailViewModel.SiteDetailId = customerSitesViewModel.SiteDetailId;
                    customerResidenceDetailViewModel.ResidenceDetailId = customerSitesViewModel.ResidenceDetailId;
                    customerResidenceDetailViewModel.NeedTwoPPL = true;
                    customerResidenceDetailViewModel.ModifiedDate = DateTime.Now;
                    customerResidenceDetailViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    CustomerConditionReportViewModel customerConditionReportViewModel = customerSitesViewModel.CustomerConditionReportViewModel;
                    customerConditionReportViewModel.SiteDetailId = customerSitesViewModel.SiteDetailId;
                    customerConditionReportViewModel.ConditionReportId = customerSitesViewModel.ConditionReportId;
                    customerConditionReportViewModel.ModifiedDate = DateTime.Now;
                    customerConditionReportViewModel.ModifiedBy = Guid.Parse(base.GetUserId);

                    // saving sitedetail info
                    using (CustomerSiteDetail)
                    {
                        CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail> mapsitedetail = new CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail>();
                        CustomerSiteDetail customerSiteDetail = mapsitedetail.Mapper(customerSiteDetailViewModel);

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

                        CustomerSiteDetail.Edit(customerSiteDetail);
                        CustomerSiteDetail.Save();
                    }

                    // saving residencedetail info
                    using (CustomerResidence)
                    {
                        CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail> mapresidence = new CommonMapper<CustomerResidenceDetailViewModel, CustomerResidenceDetail>();
                        CustomerResidenceDetail customerResidenceDetail = mapresidence.Mapper(customerResidenceDetailViewModel);
                        if (customerResidenceDetail.ResidenceDetailId != Guid.Empty)
                        {
                            CustomerResidence.Edit(customerResidenceDetail);
                            CustomerResidence.Save();
                        }
                    }

                    // saving conditionreport info
                    using (ConditionReport)
                    {
                        CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport> mapper = new CommonMapper<CustomerConditionReportViewModel, CustomerConditionReport>();
                        CustomerConditionReport customerConditionReport = mapper.Mapper(customerConditionReportViewModel);
                        if (customerConditionReport.ConditionReportId != Guid.Empty)
                        {
                            ConditionReport.Edit(customerConditionReport);
                            ConditionReport.Save();
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " updated customer list.");

                    //return Json(new { Status = "ok" });
                    return Json(new { id = customerSitesViewModel.CustomerGeneralInfoId.ToString(), activetab = "Customer Sites" });
                }

                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
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

        //POST:Delete Customer Site
        /// <summary>
        /// Delete Customer Site Using SiteDetailId
        /// </summary>
        /// <param name="SiteDetailId"></param>
        /// <returns></returns>
        public ActionResult DeleteCustomerSite(string SiteDetailId)
        {
            try
            {
                Guid siteDetailId;
                Guid.TryParse(SiteDetailId, out siteDetailId);

                CustomerSiteDetail customerSiteDetail = CustomerSiteDetail.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                CustomerResidenceDetail customerResidenceDetail = CustomerResidence.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();
                CustomerConditionReport customerConditionReport = ConditionReport.FindBy(m => m.SiteDetailId == siteDetailId).FirstOrDefault();

                using (CustomerResidence)
                {
                    customerResidenceDetail.IsDelete = true;
                    CustomerResidence.Edit(customerResidenceDetail);
                    CustomerResidence.Save();
                }

                using (ConditionReport)
                {
                    customerConditionReport.IsDelete = true;
                    ConditionReport.Edit(customerConditionReport);
                    ConditionReport.Save();
                }

                using (CustomerSiteDetail)
                {
                    customerSiteDetail.IsDelete = true;
                    CustomerSiteDetail.Edit(customerSiteDetail);
                    CustomerSiteDetail.Save();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted customer site " + SiteDetailId);

                return Json(new { id = customerSiteDetail.CustomerGeneralInfoId.ToString(), activetab = "Customer Sites" });
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET:/Customer/Customer/AddCustomerSiteDetail
        /// <summary>
        /// Add Customer SiteDetail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddCustomerSiteDetail(string id)
        {
            try
            {
                CustomerSiteDetailViewModel customerSiteDetailViewModel = new CustomerSiteDetailViewModel();
                Guid CustomerGeneralInfoId;

                Guid.TryParse(id, out CustomerGeneralInfoId);
                customerSiteDetailViewModel.CustomerGeneralInfoId = CustomerGeneralInfoId;


                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added new customer details");

                return View(customerSiteDetailViewModel);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET:/Customer/Customer/EditCustomerSiteDetail
        /// <summary>
        /// Edit CustomerSite Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCustomerSiteDetail(string id)
        {
            Guid SiteDetailId;
            Guid.TryParse(id, out SiteDetailId);

            try
            {
                using (CustomerSiteDetail)
                {
                    CustomerSiteDetail customerSiteDetail = CustomerSiteDetail.FindBy(m => m.SiteDetailId == SiteDetailId).FirstOrDefault();

                    // mapping entity to viewmodel
                    CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel> mapper = new CommonMapper<CustomerSiteDetail, CustomerSiteDetailViewModel>();
                    CustomerSiteDetailViewModel customerSiteDetailViewModel = mapper.Mapper(customerSiteDetail);



                    return View(customerSiteDetailViewModel);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //POST:/Customer/Customer/EditCustomerSiteDetail
        /// <summary>
        /// Edit customer SiteDetail
        /// </summary>
        /// <param name="customerSiteDetailViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCustomerSiteDetail(CustomerSiteDetailViewModel customerSiteDetailViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (CustomerSiteDetail)
                    {
                        // mapping viewmodel to entity
                        CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail> mapper = new CommonMapper<CustomerSiteDetailViewModel, CustomerSiteDetail>();
                        CustomerSiteDetail customerSiteDetail = mapper.Mapper(customerSiteDetailViewModel);

                        CustomerSiteDetail.Edit(customerSiteDetail);
                        CustomerSiteDetail.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated customer sit details");

                        return Json(new { id = customerSiteDetailViewModel.CustomerGeneralInfoId.ToString(), activetab = "Site Details" });
                    }
                }
                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET: Customer/Customer/AddBillingAddress
        /// <summary>
        /// Add Billing Address
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddBillingAddress(string CustomerGeneralInfoId)
        {
            try
            {
                CustomerBillingAddressViewModel model = new CustomerBillingAddressViewModel();

                Guid customerGeneralInfoId;
                Guid.TryParse(CustomerGeneralInfoId, out customerGeneralInfoId);

                var contacts = Customercontacts.FindBy(m => m.IsBillingContact == true && m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
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

                model.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralInfoId);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added billing address.");


                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }

        }

        //POST: Customer/Customer/AddBillingAddress
        /// <summary>
        /// Adding Billing Address 
        /// </summary>
        /// <param name="CustomerBillingAddressModel"></param>
        /// <returns></returns>
        //[HttpPost]
        [HttpPost, ValidateInput(false)]
        public ActionResult AddBillingAddress(CustomerBillingAddressListViewModel customerBillingListModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerBillingListModel.customerBillingAddressViewModel.PO == false)
                    {
                        //if (customerBillingListModel.customerBillingAddressViewModel.Unit == null)
                        //{
                        //    ModelState.AddModelError("Unit", "Unit is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.StreetNo == null)
                        //{
                        //    ModelState.AddModelError("StreetNo", "Street Number is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.StreetName == null)
                        //{
                        //    ModelState.AddModelError("StreetName", "Street Name is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}

                        //if (customerBillingListModel.customerBillingAddressViewModel.StreetType == null)
                        //{
                        //    ModelState.AddModelError("StreetType", "Street Type is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.Suburb == null)
                        //{
                        //    ModelState.AddModelError("Suburb", "Suburb is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.State == null)
                        //{
                        //    ModelState.AddModelError("State", "State is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.PostalCode == null)
                        //{
                        //    ModelState.AddModelError("PostalCode", "Postal Code is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}

                    }

                    else
                    {
                        //if (customerBillingListModel.customerBillingAddressViewModel.POAddress == null)
                        //{
                        //    ModelState.AddModelError("PoAddress", "PO Address is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}

                        //if (customerBillingListModel.customerBillingAddressViewModel.Suburb == null)
                        //{
                        //    ModelState.AddModelError("Suburb", "Suburb is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.State == null)
                        //{
                        //    ModelState.AddModelError("State", "State is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        //if (customerBillingListModel.customerBillingAddressViewModel.PostalCode == null)
                        //{
                        //    ModelState.AddModelError("PostalCode", "Postal Code is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}

                    }
                    if (customerBillingListModel.customerBillingAddressViewModel.CustomerTitle == 0)
                    {
                        //ModelState.AddModelError("CustomerTitle", "Title is required!");
                        //return Json(ModelState.Values.SelectMany(m => m.Errors));
                    }
                    //using (CustomerBilling)
                    //{
                    if (customerBillingListModel.customerBillingAddressViewModel.IsDefault)
                    {
                        CustomerBilling.UpdateDefaultAddress(customerBillingListModel.CustomerGeneralInfoId);

                    }
                    //}
                    customerBillingListModel.customerBillingAddressViewModel.BillingAddressId = Guid.NewGuid();
                    customerBillingListModel.customerBillingAddressViewModel.IsDelete = false;
                    customerBillingListModel.customerBillingAddressViewModel.CreatedDate = DateTime.Now;
                    customerBillingListModel.customerBillingAddressViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                    customerBillingListModel.customerBillingAddressViewModel.CustomerGeneralInfoId = customerBillingListModel.CustomerGeneralInfoId;

                    // mapping viewmodel to entity
                    CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress> mapper = new CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress>();
                    CustomerBillingAddress customerBillingAdderss = mapper.Mapper(customerBillingListModel.customerBillingAddressViewModel);

                    CustomerBilling.Add(customerBillingAdderss);
                    CustomerBilling.Save();
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added billing address.");
                    return Json(new { id = customerBillingListModel.customerBillingAddressViewModel.CustomerGeneralInfoId.ToString(), activetab = "Billing Address" });
                }
                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);

                throw;
            }
        }


        //GET: Customer/Customer/EditBillingAddress
        /// <summary>
        /// Edit BillingAddress of Customer
        /// </summary>
        /// <param name="BillingAddressId"></param>
        /// <returns></returns>
        public ActionResult EditBillingAddress(string BillingAddressId)
        {
            try
            {
                CustomerBillingAddressViewModel model = new CustomerBillingAddressViewModel();
                if (!string.IsNullOrEmpty(BillingAddressId))
                {
                    using (CustomerBilling)
                    {
                        string CustomerGeneralInfoId = Request.QueryString["CustomerGeneralInfoId"];
                        Guid customerGeneralInfoId;
                        Guid.TryParse(CustomerGeneralInfoId, out customerGeneralInfoId);

                        Guid custbilId = Guid.Parse(BillingAddressId);
                        CustomerBillingAddress CustomerBillingAdderss = CustomerBilling.FindBy(i => i.BillingAddressId == custbilId).FirstOrDefault();
                        if (CustomerBillingAdderss != null)
                        {
                            //mapping entity to viewmodel
                            CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel> mapper = new CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel>();
                            model = mapper.Mapper(CustomerBillingAdderss);


                            var contacts = Customercontacts.FindBy(m => m.IsBillingContact == true && m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();
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
                            return View(model);
                        }
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
        public ActionResult EditBillingAddress(CustomerBillingAddressListViewModel customerBillingListModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerBillingListModel.customerBillingAddressViewModel.PO == false)
                    {
                        //if (customerBillingListModel.customerBillingAddressViewModel.Unit == null)
                        //{
                        //    ModelState.AddModelError("Unit", "Unit is required!");
                        //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                        //}
                        if (customerBillingListModel.customerBillingAddressViewModel.StreetNo == null)
                        {
                            ModelState.AddModelError("StreetNo", "Street Number is required!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }
                        if (customerBillingListModel.customerBillingAddressViewModel.StreetName == null)
                        {
                            ModelState.AddModelError("StreetName", "Street Name is required!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }

                        if (customerBillingListModel.customerBillingAddressViewModel.Suburb == null)
                        {
                            ModelState.AddModelError("Suburb", "Suburb is required!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }
                        if (customerBillingListModel.customerBillingAddressViewModel.State == null)
                        {
                            ModelState.AddModelError("State", "State is required!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }
                        if (customerBillingListModel.customerBillingAddressViewModel.PostalCode == null)
                        {
                            ModelState.AddModelError("PostalCode", "Postal Code is required!");
                            return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }

                    }
                    else
                    {
                        if (customerBillingListModel.customerBillingAddressViewModel.POAddress == null)
                        {
                            //  ModelState.AddModelError("PoAddress", "PO Address is required!");
                            //if (customerBillingListModel.customerBillingAddressViewModel.StreetType == null)
                            //{
                            //    ModelState.AddModelError("StreetType", "Street Type is required!");
                            //    return Json(ModelState.Values.SelectMany(m => m.Errors));
                            //}
                            if (customerBillingListModel.customerBillingAddressViewModel.Suburb == null)
                            {
                                ModelState.AddModelError("Suburb", "Suburb is required!");
                                return Json(ModelState.Values.SelectMany(m => m.Errors));
                            }
                            if (customerBillingListModel.customerBillingAddressViewModel.State == null)
                            {
                                ModelState.AddModelError("State", "State is required!");
                                return Json(ModelState.Values.SelectMany(m => m.Errors));
                            }
                            if (customerBillingListModel.customerBillingAddressViewModel.PostalCode == null)
                            {
                                ModelState.AddModelError("PostalCode", "Postal Code is required!");
                                return Json(ModelState.Values.SelectMany(m => m.Errors));
                            }
                            //   return Json(ModelState.Values.SelectMany(m => m.Errors));
                        }
                    }
                    using (CustomerBilling)
                    {
                        if (customerBillingListModel.customerBillingAddressViewModel.IsDefault)
                        {
                            var CustomersAddress = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == customerBillingListModel.customerBillingAddressViewModel.CustomerGeneralInfoId).ToList();

                            foreach (var address in CustomersAddress)
                            {
                                address.IsDefault = false;
                                //  CustomerBilling.DeAttach(address);
                                CustomerBilling.Edit(address);
                                CustomerBilling.Save();
                            }
                        }

                        customerBillingListModel.customerBillingAddressViewModel.ModifiedDate = DateTime.Now;
                        customerBillingListModel.customerBillingAddressViewModel.ModifiedBy = Guid.Parse(base.GetUserId);
                        customerBillingListModel.customerBillingAddressViewModel.CustomerGeneralInfoId = customerBillingListModel.CustomerGeneralInfoId;
                        customerBillingListModel.customerBillingAddressViewModel.BillingAddressId = customerBillingListModel.BillingAddressId;

                        //mapping viewmodel to entity
                        CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress> mapper = new CommonMapper<CustomerBillingAddressViewModel, CustomerBillingAddress>();
                        CustomerBillingAddress customerBillingAdderss = mapper.Mapper(customerBillingListModel.customerBillingAddressViewModel);
                        customerBillingAdderss.IsDelete = false;
                        CustomerBilling.Edit(customerBillingAdderss);
                        CustomerBilling.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated billing address.");

                        return Json(new { id = customerBillingListModel.customerBillingAddressViewModel.CustomerGeneralInfoId.ToString(), activetab = "Billing Address" });
                    }
                }
                return Json(ModelState.Values.SelectMany(m => m.Errors));
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET:  /Customer/Customer/ViewCustomerContactLog
        /// <summary>
        /// View Customer Contact Log 
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult ViewCustomerContactLog(string customerGeneralinfoid)
        {
            try
            {
                using (CustomercontactLog)
                {
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    Guid Customercontactid = Guid.Empty;
                    //var CustomerContactList = CustomercontactLog.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid);
                    var CustomerContactList = CustomercontactLog.GetCustomercontactLogs(customerGeneralinfoid).ToList();
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
                    customerContractListViewModel.CustomerContactLog.CustomerGeneralInfoId = custGeneralinfoid;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed customer contact log.");

                    return View(customerContractListViewModel);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// ViewCustomer ContactLog Partial
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewCustomerContactLogPartial(string customerGeneralinfoid, string Keyword)
        {
            try
            {
                using (CustomercontactLog)
                {
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    Guid Customercontactid = Guid.Empty;
                    var CustomerContactList = CustomercontactLog.GetCustomercontactLogs(customerGeneralinfoid, Keyword).ToList();
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
                    customerContractListViewModel.CustomerContactLog.CustomerGeneralInfoId = custGeneralinfoid;

                    return PartialView("_CustomerContactLogList", customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST /Customer/Customer/ViewCustomerContactLog
        /// <summary>
        /// View CustomerContactLog
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewCustomerContactLog(CustomerContactLogViewModel customerContactLogViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (CustomercontactLog)
                    {
                        //mapping of viewmodel to entity
                        CommonMapper<CustomerContactLogViewModel, CustomerContactLog> mapper = new CommonMapper<CustomerContactLogViewModel, CustomerContactLog>();
                        if (customerContactLogViewModel.CustomerContactId == Guid.Empty)
                        {
                            customerContactLogViewModel.CustomerContactId = Guid.NewGuid();
                            CustomerContactLog customerContactLog = mapper.Mapper(customerContactLogViewModel);
                            CustomercontactLog.Add(customerContactLog);
                            CustomercontactLog.Save();
                            return RedirectToAction("AddCustomerInfo", new { id = customerContactLogViewModel.CustomerGeneralInfoId.ToString(), activetab = "Contact" });
                        }
                        else
                        {
                            CustomerContactLog customerContactLog = mapper.Mapper(customerContactLogViewModel);
                            CustomercontactLog.Edit(customerContactLog);
                            CustomercontactLog.Save();
                            return RedirectToAction("AddCustomerInfo", new { id = customerContactLogViewModel.CustomerGeneralInfoId, activetab = "Contact" });
                        }
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
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
        public ActionResult CustomerContactLogSearch(string CustomerGeneralinfoid, string Keyword, string PageNum)
        {
            try
            {
                using ((Customercontacts))
                {
                    if (Keyword == null)
                        Keyword = "";

                    Guid Customerid = Guid.Parse(CustomerGeneralinfoid);
                    var CustomerContactList = CustomercontactLog.GetCustomercontactLogs(CustomerGeneralinfoid, Keyword).ToList();
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
                    customerContractListViewModel.CustomerContactLog.CustomerGeneralInfoId = Customerid;

                    return PartialView("_CustomerContactLogList", customerContractListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:  /Customer/Customer/DeleteCustomerContactLog
        /// <summary>
        /// Delete customer Contact Log
        /// </summary>
        /// <param name="Customercontactid"></param>
        /// <returns></returns>
        public ActionResult DeleteCustomerContactLog(string Customercontactid, string PageNum)
        {
            try
            {
                using (CustomercontactLog)
                {
                    Guid contactid = Guid.Parse(Customercontactid);
                    CustomerContactLog logtodelete = CustomercontactLog.FindBy(i => i.CustomerContactId == contactid).FirstOrDefault();
                    logtodelete.IsDelete = true;
                    CustomercontactLog.Edit(logtodelete);
                    CustomercontactLog.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted customer contact log.");

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET: /Customer/Customer/AddCustomerInfo
        /// <summary>
        /// Add Customer Info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activetab"></param>
        /// <param name="success"></param>
        /// <param name="pagenum"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddCustomerInfo(string id, string activetab, string success, string pagenum, string JobId)
        {
            Guid CustomerGeneralInfoId;
            Guid.TryParse(id, out CustomerGeneralInfoId);

            CustomerTabPanelViewModel customerTabPanelViewModel = new CustomerTabPanelViewModel();
            customerTabPanelViewModel.CustomerGeneralInfoId = id;
            customerTabPanelViewModel.ActiveTab = activetab;
            customerTabPanelViewModel.Success = success;
            customerTabPanelViewModel.PageNum = pagenum;
            customerTabPanelViewModel.JobId = JobId;

            // getting site count
            customerTabPanelViewModel.SiteCount = CustomerSiteDetail.GetCustomerSiteList(id).Count().ToString();

            customerTabPanelViewModel.BillingCount = CustomerBilling.GetBillingAddressList(CustomerGeneralInfoId).Count().ToString();

            customerTabPanelViewModel.SitesDocumentCount = CustomerSitesDocuments.GetCustomerSiteDocumentsList(id).Count().ToString();
            customerTabPanelViewModel.CustomerContactsCount = Customercontacts.GetCustomerContactsList(id).Count().ToString();

            var CustomerName = Customer.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();
            if (CustomerName != null)
            {
                customerTabPanelViewModel.CustomerName = CustomerName.CustomerLastName;
            }

            var customerContacts = Customercontacts.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();
            if (customerContacts != null)
            {
                customerTabPanelViewModel.ContactId = customerContacts.ContactId.ToString();
            }

            var customerSiteDetail = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();
            if (customerSiteDetail != null)
            {
                customerTabPanelViewModel.SiteDetailId = customerSiteDetail.SiteDetailId.ToString();
            }

            var customerBilling = CustomerBilling.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();
            if (customerBilling != null)
            {
                customerTabPanelViewModel.BillingAddressId = customerBilling.BillingAddressId.ToString();
            }

            var customerSiteDocuments = CustomerSitesDocuments.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();
            if (customerSiteDocuments != null)
            {
                customerTabPanelViewModel.DocumentId = customerSiteDocuments.DocumentId.ToString();
            }

            var customercontactLog = CustomercontactLog.FindBy(m => m.CustomerGeneralInfoId == CustomerGeneralInfoId).FirstOrDefault();
            if (customercontactLog != null)
            {
                customerTabPanelViewModel.CustomerContactId = customercontactLog.CustomerContactId.ToString();
            }

            return View(customerTabPanelViewModel);
        }

        //GET:/Customer/Customer/CustomerGeneralInfoSearch
        /// <summary>
        /// Customer GeneralInfo Search
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerGeneralInfoSearch()
        {
            CustomerSearchViewModel customerSearchViewModel = new CustomerSearchViewModel();
            return PartialView("_CustomerGeneralInfoSearch", customerSearchViewModel);
        }

        //GET: /Customer/Customer/_CustomercontactAddEdit
        /// <summary>
        ///Get Customer contact
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="Customercontactid"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public PartialViewResult _CustomercontactAddEdit(string CustomerGeneralinfoid, string Customercontactid, string PageNum, string SiteName)
        {
            try
            {
                CustomerContactLogViewModel model = new CustomerContactLogViewModel();
                var Customerjobs = GetCustomerjobsByCustomerid(CustomerGeneralinfoid);

                model.PageNum = PageNum;
                if (!string.IsNullOrEmpty(Customercontactid))
                {
                    using (CustomercontactLog)
                    {
                        Guid custContactid = Guid.Parse(Customercontactid);
                        CustomerContactLog cotactlog = CustomercontactLog.FindBy(i => i.CustomerContactId == custContactid).FirstOrDefault();

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

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " updated customer contact.");

                            return PartialView(model);
                        }
                        else
                        {

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added customer contact.");

                            return PartialView(model);
                        }
                    }
                }
                else
                {
                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (Customer)
                    {
                        Guid customerGeneralinfoid = Guid.Parse(CustomerGeneralinfoid);
                        CustomerGeneralinfo = Customer.FindBy(i => i.CustomerGeneralInfoId == customerGeneralinfoid).FirstOrDefault();
                    }
                    Guid custContactid = Guid.Parse(CustomerGeneralinfoid);
                    model = new CustomerContactLogViewModel();
                    model.Customerjobs = Customerjobs;
                    model.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);
                    model.CustomerId = CustomerGeneralinfo.CTId.ToString();
                    model.SiteName = SiteName;
                    model.LogDate = DateTime.Now.Date;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added customer contact.");

                    return PartialView(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Get Customer Jobs Using Customerinfoid
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <returns></returns>
        private List<CustomerJobs> GetCustomerjobsByCustomerid(string Customerinfoid)
        {
            try
            {
                using (CustJobs)
                {
                    Guid Customerid = Guid.Parse(Customerinfoid);
                    List<CustomerJobs> joblist = new List<CustomerJobs>();
                    var jobs = CustJobs.FindBy(i => i.CustomerGeneralInfoId == Customerid).ToList();
                    foreach (var job in jobs)
                    {
                        CustomerJobs obj = new CustomerJobs();
                        obj.CustJobId = job.Id;
                        obj.Jobtext = job.JobNo.ToString();
                        joblist.Add(obj);
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets customer jobs of " + Customerid);

                    return joblist;
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }


        //POST:/Customer/Customer/_CustomercontactAddEdit
        /// <summary>
        /// Post Customer contact
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                            CustomercontactLog.Edit(customerContactLog);
                            CustomercontactLog.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " updated customer info");

                            return RedirectToAction("AddCustomerInfo", new { id = model.CustomerGeneralInfoId.ToString(), activetab = "Contact", success = "ok", pagenum = model.PageNum });
                        }
                        else
                        {
                            model.IsDelete = false;
                            model.CreatedDate = DateTime.Now;
                            model.CreatedBy = Guid.Parse(base.GetUserId);
                            model.CustomerContactId = Guid.NewGuid();
                            CustomerContactLog customerContactLog = mapper.Mapper(model);
                            CustomercontactLog.Add(customerContactLog);
                            CustomercontactLog.Save();

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added customer info");

                            return RedirectToAction("AddCustomerInfo", new { id = model.CustomerGeneralInfoId.ToString(), activetab = "Contact", success = "ok", pagenum = model.PageNum });
                        }
                    }
                    else
                    {
                        return PartialView(model);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(base.GetUserName + ex.Message);
                    throw;
                }

            }
            return PartialView(model);
        }


        //GET:Customer/Customer/ExportCustomerContact
        /// <summary>
        /// Export Customer Contact
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportCustomerContact(string CustomerGeneralInfoId)
        {
            try
            {
                Guid customerId = Guid.Parse(CustomerGeneralInfoId);
                List<CustomerContactExport> Customercontactexport = new List<CustomerContactExport>();
                using (CustomercontactLog)
                {
                    var customecontactlog = CustomercontactLog.FindBy(i => i.CustomerGeneralInfoId == customerId).ToList();
                    if (customecontactlog != null)
                    {
                        foreach (var cust in customecontactlog)
                        {
                            CustomerContactExport obj = new CustomerContactExport();
                            obj.CustomerId = cust.CustomerId;
                            obj.JobId = cust.JobId;
                            if (cust.LogDate != null)
                                obj.LogDate = Convert.ToDateTime(cust.LogDate).Date.ToString("d");
                            if (cust.ReContactDate != null)
                                obj.ReContactDate = Convert.ToDateTime(cust.ReContactDate).Date.ToString("d");
                            obj.Note = cust.Note;
                            Customercontactexport.Add(obj);
                        }
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " download pdf file of customer contact log.");

                    return new ViewAsPdf("ExportCustomerContact", Customercontactexport)
                    {
                        FileName = "CustomerContactList.pdf",
                        CustomSwitches =
                       "--footer-center \"  Dated: " +
                       DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                       " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\""
                    };
                }

            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //GET:Customer/Customer/AddDocuments
        /// <summary>
        /// Add New Documents For Customer
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult AddDocuments(string customerGeneralinfoid)
        {
            try
            {
                var keyWordSearch = "";
                Guid id = Guid.Parse(customerGeneralinfoid);
                var customerSiteDetaillist = CustomerSitesDocuments.GetSiteCountForCustomer(id).ToList();
                var customerSiteDocumentsist = CustomerSitesDocuments.GetSiteDocumentList(keyWordSearch, id).ToList();


                CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                List<ViewModels.SiteDetail> li = new List<ViewModels.SiteDetail>();
                foreach (var i in customerSiteDetaillist)
                {
                    ViewModels.SiteDetail obj = new ViewModels.SiteDetail();
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
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.CustomerGeneralInfoId = id;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added document");


                return View(customerSiteDocumentsListViewModel);
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);

                throw ex;
            }
        }

        /// <summary>
        /// Seacrh  Documents
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        public ActionResult AddDocumentsPartial(string customerGeneralinfoid)
        {
            try
            {
                var keyWordSearch = "";
                Guid id = Guid.Parse(customerGeneralinfoid);
                var customerSiteDetaillist = CustomerSitesDocuments.GetSiteCountForCustomer(id).ToList();
                var customerSiteDocumentsist = CustomerSitesDocuments.GetSiteDocumentList(keyWordSearch, id).ToList();

                CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                List<ViewModels.SiteDetail> li = new List<ViewModels.SiteDetail>();
                foreach (var i in customerSiteDetaillist)
                {
                    ViewModels.SiteDetail obj = new ViewModels.SiteDetail();
                    obj.SiteDetailId = i.SiteDetailId;
                    obj.SiteAddress = i.SiteAddress;
                    li.Add(obj);
                }
                customersitemodel.siteDetail = li;

                var customerSiteDocumentsListViewModel = new CustomerSiteDocumentsListViewModel
                {
                    CustomerSiteDocumentsCoreViewModelList = customerSiteDocumentsist,
                    CustomerSiteDocuments = new CustomerSitesDocuments()
                };
                customerSiteDocumentsListViewModel.CustomerSiteDocuments.CustomerGeneralInfoId = id;



                return PartialView("_CustomerSiteDocumentList", customerSiteDocumentsListViewModel);
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw ex;
            }
        }
        //GET:  /Customer/Customer/ViewCustomerContacts
        /// <summary>
        /// view Customer Contact 
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns></returns>
        public ActionResult ViewCustomerContacts(string CustomerGeneralInfoId)
        {
            try
            {
                using (Customercontacts)
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    string Searchstring = Request.QueryString["searchkeyword"];
                    Guid custGeneralinfoid = Guid.Parse(CustomerGeneralInfoId);
                    Guid Customercontactid = Guid.Empty;
                    //var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();
                    var CustomerContactsList = Customercontacts.GetContactsInfo(custGeneralinfoid, Searchstring).ToList();
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

                        var userName = "";
                        if (customerContacts.ModifiedBy == null)
                        {
                            userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerContacts.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                        }
                        else
                        {
                            userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerContacts.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                        }
                        model.UserName = userName;
                    }

                    model.SiteList = CustomerSiteDetail.GetAll().Where(m => m.CustomerGeneralInfoId == custGeneralinfoid).Select(m =>
                    new SelectListItem { Text = m.StreetName, Value = m.SiteDetailId.ToString() }).ToList();
                    model.SiteList.OrderBy(m => m.Text);

                    var CustomerSiteList = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == custGeneralinfoid).Select(m => new SelectListItem()
                    {
                        Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode,
                        Value = m.SiteDetailId.ToString()
                    }).ToList();

                    contactsSearchViewModel.SiteSearch = CustomerSiteList;
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

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of customer contact");


                    return View(customerContactsListViewModel);
                }
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }

        //POST:  /Customer/Customer/ViewCustomerContacts
        /// <summary>
        /// Search Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="name"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ViewCustomerContacts(string CustomerGeneralinfoid, string SiteId, int ContactType, string name, string PageNum)
        {
            try
            {
                using ((Customercontacts))
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    if (name == null)
                        name = "";

                    Guid Customerid = Guid.Parse(CustomerGeneralinfoid);
                    var contactsList = Customercontacts.GetCustomerContactsInfo(Customerid, SiteId, ContactType, name).ToList();

                    //Guid custGeneralinfoid = Guid.Parse(CustomerGeneralInfoId);
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(contactsList.ToList());

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

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of customer contact");

                    return PartialView("_CustomerContactsList", customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public PartialViewResult _CustomercontactsAddWizard(string CustomerGeneralinfoid)
        {
            try
            {
                CustomerContactsViewModel model = new CustomerContactsViewModel();
                Guid customerGeneralinfoid = Guid.Parse(CustomerGeneralinfoid);

                CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                using (Customer)
                {
                    CustomerGeneralinfo = Customer.FindBy(i => i.CustomerGeneralInfoId == customerGeneralinfoid).FirstOrDefault();
                }
                Guid custContactid = Guid.Parse(CustomerGeneralinfoid);
                model = new CustomerContactsViewModel();
                model.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);

                model.SiteList = CustomerSiteDetail.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralinfoid && m.IsDelete == false).Select(m =>
                     new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).ToList();
                model.SiteList.OrderBy(m => m.Text);
                Guid SiteId = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == customerGeneralinfoid && m.IsDelete == false).Select(m => m.SiteDetailId).FirstOrDefault();
                model.SiteId = SiteId.ToString();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added customer wizard.");

                return PartialView("_AddContactWizard", model);

            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }



        /// <summary>
        ///Post: Add New Customer Contacts
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects AddCustomerInfo</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult _CustomercontactsAddWizard(CustomerContactsViewModel model)
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
                            model.IsDelete = false;
                            model.CreatedDate = DateTime.Now;
                            model.CreatedBy = Guid.Parse(base.GetUserId);
                            model.ContactId = Guid.NewGuid();
                            CustomerContacts customerContacts = mapper.Mapper(model);
                            Customercontacts.Add(customerContacts);
                            Customercontacts.Save();


                            //Add default Billing Address If DefaultContact Checked
                            if (model.DefaultContact == true)
                            {
                                Guid siteid = Guid.Parse(model.SiteId);
                                var sitedetail = CustomerSiteDetail.FindBy(i => i.SiteDetailId == siteid).FirstOrDefault();
                                CustomerBillingAddress billingDetail = new CustomerBillingAddress();

                                billingDetail.BillingAddressId = Guid.NewGuid();
                                billingDetail.CreatedDate = DateTime.Now;
                                billingDetail.CreatedBy = Guid.Parse(base.GetUserId);
                                billingDetail.CustomerGeneralInfoId = model.CustomerGeneralInfoId;
                                billingDetail.FirstName = model.FirstName;
                                billingDetail.LastName = model.LastName;
                                billingDetail.PhoneNo1 = model.PhoneNo1;
                                billingDetail.PhoneNo2 = model.PhoneNo2;
                                billingDetail.PhoneNo3 = model.PhoneNo3;
                                billingDetail.EmailId = model.EmailId;
                                billingDetail.Spare1 = model.Spare1;  //Note
                                billingDetail.IsDelete = false;
                                billingDetail.IsDefault = true;
                                CustomerBilling.Add(billingDetail);
                                CustomerBilling.Save();
                            }

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added new customer contacts.");

                            return Json(new { success = "1" }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            foreach (ModelState modelState in ViewData.ModelState.Values)
                            {
                                foreach (ModelError error in modelState.Errors)
                                {
                                    //   DoSomethingWith(error);
                                }
                            }
                            model.SiteList = CustomerSiteDetail.GetAll().Where(m => m.CustomerGeneralInfoId == model.CustomerGeneralInfoId).Select(m =>
                 new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).ToList();
                            model.SiteList.OrderBy(m => m.Text);
                            return PartialView("_AddContactWizard", model);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return PartialView(model);
        }


        /// <summary>
        /// Add New Customer Contacts
        /// </summary>
        /// <param name="CustomerGeneralinfoid"></param>
        /// <param name="customercontactid"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public PartialViewResult _CustomercontactsAddEdit(string CustomerGeneralinfoid, string customercontactid, string PageNum)
        {
            try
            {
                CustomerContactsViewModel model = new CustomerContactsViewModel();
                Guid customerGeneralinfoid = Guid.Parse(CustomerGeneralinfoid);
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
                            var userName = "";
                            if (cotactlog.ModifiedBy == null)
                            {
                                userName = EmployeeRepo.FindBy(m => m.EmployeeId == cotactlog.CreatedBy).Select(m => m.UserName).FirstOrDefault();
                            }
                            else
                            {
                                userName = EmployeeRepo.FindBy(m => m.EmployeeId == cotactlog.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                            }
                            if (cotactlog.ModifiedDate == null)
                            {
                                cotactlog.CreatedDate = cotactlog.CreatedDate;
                            }
                            else
                            {
                                cotactlog.ModifiedDate = cotactlog.ModifiedDate;
                            }


                            model.SiteList = CustomerSiteDetail.GetAll().Where(m => m.CustomerGeneralInfoId == customerGeneralinfoid && m.IsDelete == false).Select(m =>
                            new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).OrderBy(m => m.Text).ToList();

                            model.SiteList.OrderBy(m => m.Text);
                            model.UserName = userName;
                            model.CreatedDate = cotactlog.CreatedDate;
                            model.ModifiedDate = cotactlog.ModifiedDate;
                            model.IsDelete = false;



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
                    using (Customer)
                    {
                        CustomerGeneralinfo = Customer.FindBy(i => i.CustomerGeneralInfoId == customerGeneralinfoid).FirstOrDefault();
                    }
                    Guid custContactid = Guid.Parse(CustomerGeneralinfoid);
                    model = new CustomerContactsViewModel();
                    model.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);

                    var siteList = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == customerGeneralinfoid).ToList();
                    if (siteList.Count == 1)
                    {
                        foreach (var val in siteList)
                        {
                            model.SiteId = val.SiteDetailId.ToString();
                        }
                    }

                    model.SiteList = siteList.Select(m =>
                       new SelectListItem { Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode, Value = m.SiteDetailId.ToString() }).OrderBy(m => m.Text).ToList();
                    model.HideAddContacts = Request.QueryString["HideAddContacts"];

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
        public ActionResult _CustomercontactsAddEdit(CustomerContactsViewModel model)
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
                                log.Info(base.GetUserName + " updated customer contact.");

                                return RedirectToAction("AddCustomerInfo", new { id = model.CustomerGeneralInfoId.ToString(), activetab = "Contacts", success = "ok", pagenum = model.PageNum });
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

                                //Add default Billing Address If DefaultContact Checked
                                if (model.DefaultContact == true)
                                {
                                    CustomerBillingAddress billingDetail = new CustomerBillingAddress();

                                    billingDetail.BillingAddressId = Guid.NewGuid();
                                    billingDetail.IsDelete = false;
                                    billingDetail.CreatedDate = DateTime.Now;
                                    billingDetail.CreatedBy = Guid.Parse(base.GetUserId);
                                    billingDetail.CustomerGeneralInfoId = model.CustomerGeneralInfoId;
                                    billingDetail.FirstName = model.FirstName;
                                    billingDetail.LastName = model.LastName;
                                    billingDetail.PhoneNo1 = model.PhoneNo1;
                                    billingDetail.PhoneNo2 = model.PhoneNo2;
                                    billingDetail.PhoneNo3 = model.PhoneNo3;
                                    billingDetail.EmailId = model.EmailId;
                                    billingDetail.Spare1 = model.Spare1;  //Note

                                    CustomerBilling.Add(billingDetail);
                                    CustomerBilling.Save();
                                }

                                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                                log.Info(base.GetUserName + " added new customer contact.");

                                return RedirectToAction("AddCustomerInfo", new { id = model.CustomerGeneralInfoId.ToString(), activetab = "Contacts", success = "ok", pagenum = model.PageNum });
                            }
                        }
                        else
                        {
                            return PartialView(model);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return PartialView(model);
        }

        /// <summary>
        ///Post: View All Customer Contacts
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewCustomerContactsPartial(string customerGeneralinfoid, string SiteId, int ContactType, string Keyword)
        {
            try
            {
                using (Customercontacts)
                {
                    ContactsSearchViewModel contactsSearchViewModel = new ContactsSearchViewModel();
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    Guid Customercontactid = Guid.Empty;
                    //var CustomerContactList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == custGeneralinfoid).ToList();
                    var CustomerContactList = Customercontacts.GetCustomerContactsInfo(custGeneralinfoid, SiteId, ContactType, Keyword).ToList();
                    CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel> mapper = new CommonMapper<CustomerContactsCoreViewModel, CustomerContactsViewModel>();
                    List<CustomerContactsViewModel> customerContactsCoreViewModel = mapper.MapToList(CustomerContactList.ToList());

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
                    var CustomerSiteList = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == custGeneralinfoid).Select(m => new SelectListItem()
                    {
                        Text = m.Street + m.StreetName + "," + m.Suburb + m.State + m.PostalCode,
                        Value = m.SiteDetailId.ToString()
                    }).ToList();

                    contactsSearchViewModel.SiteSearch = CustomerSiteList;

                    var customerContactsListViewModel = new CustomerContactsListViewModel
                    {
                        CustomerContactsViewModelList = customerSiteCollection,
                        ContactsDetailInfo = contactsSearchViewModel,
                        CustomerContacts = new CustomerContacts(),
                        customerContactsViewModel = new CustomerContactsViewModel()
                    };
                    customerContactsListViewModel.CustomerContacts.CustomerGeneralInfoId = custGeneralinfoid;


                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed all customer contact.");

                    return PartialView("_CustomerContactsList", customerContactsListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:  /Customer/Customer/ViewCustomerContacts
        /// <summary>
        /// Delete customer Contact 
        /// </summary>
        /// <param name="Customercontactid"></param>
        /// <returns></returns>
        public ActionResult DeleteCustomerContacts(string Customercontactid)
        {
            try
            {
                using (Customercontacts)
                {
                    Guid contactid = Guid.Parse(Customercontactid);
                    CustomerContacts contactdelete = Customercontacts.FindBy(i => i.ContactId == contactid).FirstOrDefault();
                    contactdelete.IsDelete = true;
                    Customercontacts.Edit(contactdelete);
                    Customercontacts.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted customer contact.");

                    return Json(contactdelete.CustomerGeneralInfoId.ToString(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:  /Customer/Customer/AddDocuments
        /// <summary>
        /// Add New Documents
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDocuments(CustomerSiteDocumentsListViewModel model, IEnumerable<HttpPostedFileBase> filename)
        {
            if (model != null)
            {
                try
                {
                    CustomerSitesDocumentsViewModel customerSiteDocumentsViewModel = new CustomerSitesDocumentsViewModel();

                    if (ModelState.IsValid)
                    {
                        using (CustomerSitesDocuments)
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
                                    CustomerSitesDocuments.Add(customerSiteDocuments);
                                    CustomerSitesDocuments.Save();
                                }
                            }

                            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                            log.Info(base.GetUserName + " added new document.");

                            return RedirectToAction("AddCustomerInfo", new { id = model.CustomerSiteDocuments.CustomerGeneralInfoId.ToString(), activetab = "Documents", success = "ok" });
                        }
                    }
                    else
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(base.GetUserName + ex.Message);
                    throw;
                }
            }
            return View(model);
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
        public ActionResult CustomerDocQuickView(Guid siteId)
        {
            var SiteDocList = CustomerSitesDocuments.FindBy(m => m.SiteId == siteId).ToList();

            CommonMapper<CustomerSitesDocuments, CustomerSitesDocuments> mapper = new CommonMapper<CustomerSitesDocuments, CustomerSitesDocuments>();
            var sitedocs = mapper.MapToList(SiteDocList);


            CustomerSiteDocumentsListViewModel DocViewModel = new CustomerSiteDocumentsListViewModel();
            DocViewModel.CustomerSiteDocumentsViewModelList = sitedocs;

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed document by site id.");

            return PartialView("_QuickCustSiteView", DocViewModel);
        }

        /// <summary>
        /// Add Site Documents Using SiteDetailId
        /// </summary>
        /// <param name="SiteDetailId"></param>
        /// <param name="PageNum"></param>
        /// <returns></returns>
        public ActionResult DeleteSiteDocuments(string SiteDetailId, string PageNum)
        {
            try
            {
                using (CustomerSitesDocuments)
                {
                    Guid sitedetailid = Guid.Parse(SiteDetailId);
                    List<CustomerSitesDocuments> docdelete = CustomerSitesDocuments.FindBy(i => i.SiteId == sitedetailid).ToList();
                    foreach (CustomerSitesDocuments Doc in docdelete)
                    {
                        Doc.IsDelete = true;
                        CustomerSitesDocuments.Edit(Doc);
                        CustomerSitesDocuments.Save();
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " deleted site document.");

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Download Attach File
        /// </summary>
        /// <param name="DocumentId"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string DocumentId)
        {
            try
            {
                Guid docId = Guid.Parse(DocumentId);
                var ImageName = CustomerSitesDocuments.FindBy(i => i.DocumentId == docId).FirstOrDefault();
                var FileVirtualPath = "/Images/CustomerDocs/" + docId + '/' + ImageName.DocumentName;
                //byte[] fileBytes = System.IO.File.ReadAllBytes(FileVirtualPath);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted document " + ImageName);

                return Json(FileVirtualPath, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult _ViewSitesDocuments(string CustomerGeneralinfoid, string Sitedetailid, string PageNum)
        {
            try
            {
                Guid siteDetailId = Guid.Parse(Sitedetailid);
                using (CustomerSitesDocuments)
                {
                    var docs = CustomerSitesDocuments.FindBy(i => i.SiteId == siteDetailId).ToList();
                    CommonMapper<CustomerSitesDocuments, CustomerSitesDocumentsViewModel> mapper = new CommonMapper<CustomerSitesDocuments, CustomerSitesDocumentsViewModel>();
                    List<CustomerSitesDocumentsViewModel> customerGeneralInfoViewModel = mapper.MapToList(docs.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(customerGeneralInfoViewModel);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed site document");

                    return Json(new { list = json, length = customerGeneralInfoViewModel.Count() });
                }
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);
                throw;
            }
        }
        //POST: /Customer/Customer/GetCustomerSiteByJobid
        /// <summary>
        /// GetCustomerSiteByJobid
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult GetCustomerSiteByJobid(string JobId)
        {
            try
            {
                Guid jobid = Guid.Parse(JobId);
                Guid siteid = Guid.Empty;
                using (CustJobs)
                {
                    var sitedetail = CustJobs.FindBy(i => i.Id == jobid).FirstOrDefault();
                    siteid = Guid.Parse(sitedetail.SiteId.ToString());
                }
                using (CustomerSiteDetail)
                {
                    CustomerSiteDetail sitedetail = CustomerSiteDetail.FindBy(i => i.SiteDetailId == siteid).FirstOrDefault();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets customer site by job id.");

                    return Json(new { sitename = sitedetail.StreetName, siteid = sitedetail.SiteDetailId, JsonRequestBehavior.AllowGet });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //GET:  /Customer/Customer/JobHistory
        /// <summary>
        /// view Job History
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns></returns>
        public ActionResult JobHistory(string CustomerGeneralInfoId)
        {
            try
            {
                using (CustJobs)
                {
                    Guid custGeneralinfoid = Guid.Parse(CustomerGeneralInfoId);
                    var CustomerJobList = CustJobs.GetEmployeeCustomerJobs(custGeneralinfoid);
                    var customerSiteDetaillist = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == custGeneralinfoid);

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    EmployeejobSearchViewModel employeejobSearchViewModel = new EmployeejobSearchViewModel
                    {
                        PageSize = PageSize
                    };

                    CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                    List<ViewModels.SiteDetail> li = new List<ViewModels.SiteDetail>();
                    foreach (var i in customerSiteDetaillist)
                    {
                        ViewModels.SiteDetail obj = new ViewModels.SiteDetail();
                        obj.SiteDetailId = i.SiteDetailId;
                        obj.StreetName = i.StreetName;
                        li.Add(obj);
                    }
                    customersitemodel.siteDetail = li;

                    //CustomerJobList = CustomerJobList.Where(customer => customer.Status == 15);
                    //CustomerJobList = CustomerJobList.Where(customer => customer.CustomerGeneralInfoId == custGeneralinfoid);

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(CustomerJobList.OrderByDescending(i => i.JobNo).ToList());

                    var Employeelistviewmodel = new EmployeeJobListViewModel
                    {
                        EmployeeJoblist = EmployeejobViewModel,
                        SiteCountviewModel = customersitemodel,
                        Employeejobsearchmodel = employeejobSearchViewModel
                    };
                    Employeelistviewmodel.Employeejobsearchmodel.CustomerGeneralInfoId = custGeneralinfoid;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets job history.");

                    return View(Employeelistviewmodel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Post: View All Job history
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult JobHistoryPartial(string customerGeneralinfoid, string Keyword)
        {
            try
            {
                using (CustJobs)
                {
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    var CustomerJobList = CustJobs.GetEmployeeCustomerJobs(custGeneralinfoid);
                    var customerSiteDetaillist = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == custGeneralinfoid);


                    //string keyword = string.IsNullOrEmpty(Request.QueryString["Keyword"]) ? "" :
                    //                            (Request.QueryString["Keyword"]);
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    EmployeejobSearchViewModel employeejobSearchViewModel = new EmployeejobSearchViewModel
                    {
                        Keyword = Keyword,
                        PageSize = PageSize
                    };

                    CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                    List<ViewModels.SiteDetail> li = new List<ViewModels.SiteDetail>();
                    foreach (var i in customerSiteDetaillist)
                    {
                        ViewModels.SiteDetail obj = new ViewModels.SiteDetail();
                        obj.SiteDetailId = i.SiteDetailId;
                        obj.StreetName = i.StreetName;
                        li.Add(obj);
                    }
                    customersitemodel.siteDetail = li;

                    //CustomerJobList = CustomerJobList.Where(customer => customer.Status == 15);
                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        CustomerJobList = CustomerJobList.Where(customer =>
                        (customer.JobNo != null && customer.JobNo.ToString().ToLower().Contains(Keyword.ToLower())) ||
                        (customer.CustomerName != null && customer.CustomerName.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.BookedByName != null && customer.BookedByName.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.StrataPlan != null && customer.StrataPlan.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.StrataNumber != null && customer.StrataNumber.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.SiteAddress != null && customer.SiteAddress.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.AssignUser != null && customer.AssignUser.ToLower().Contains(Keyword.ToLower())) ||
                        (customer.ContactName != null && customer.ContactName.ToLower().Contains(Keyword.ToLower()))
                        );
                    }
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(CustomerJobList.OrderByDescending(i => i.JobNo).ToList());

                    var Employeelistviewmodel = new EmployeeJobListViewModel
                    {
                        EmployeeJoblist = EmployeejobViewModel,
                        SiteCountviewModel = customersitemodel,
                        Employeejobsearchmodel = employeejobSearchViewModel
                    };
                    Employeelistviewmodel.Employeejobsearchmodel.CustomerGeneralInfoId = custGeneralinfoid;

                    return PartialView("_CustomerJobList", Employeelistviewmodel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST: Customer/Customer/JobHistory
        /// <summary>
        /// View  Jobs lists 
        /// </summary>
        /// <param name="CustomerGeneralinfoid,Keyword,PageNum,SiteId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JobHistory(string CustomerGeneralinfoid, string Keyword, int PageNum, string SiteId)
        {
            try
            {
                using (CustJobs)
                {
                    Guid CustGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);
                    var EmployeeList = CustJobs.GetEmployeeCustomerJobs(CustGeneralInfoId);
                    var customerSiteDetaillist = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == CustGeneralInfoId);

                    if (SiteId != "")
                    {
                        EmployeeList = EmployeeList.Where(customer => customer.SiteId == Guid.Parse(SiteId));
                    }
                    //EmployeeList = EmployeeList.Where(customer => customer.Status == 15);
                    //EmployeeList = EmployeeList.Where(customer => customer.CustomerGeneralInfoId == CustGeneralInfoId);
                    string keyword = Keyword;
                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        EmployeeList = EmployeeList.Where(customer =>
                        (customer.JobNo != null && customer.JobNo.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.CustomerName != null && customer.CustomerName.ToLower().Contains(keyword.ToLower())) ||
                        (customer.BookedByName != null && customer.BookedByName.ToLower().Contains(keyword.ToLower())) ||
                        (customer.StrataPlan != null && customer.StrataPlan.ToLower().Contains(keyword.ToLower())) ||
                        (customer.StrataNumber != null && customer.StrataNumber.ToLower().Contains(keyword.ToLower())) ||
                        (customer.SiteAddress != null && customer.SiteAddress.ToLower().Contains(keyword.ToLower())) ||
                        (customer.AssignUser != null && customer.AssignUser.ToLower().Contains(keyword.ToLower())) ||
                        (customer.ContactName != null && customer.ContactName.ToLower().Contains(keyword.ToLower()))
                        );
                    }

                    CustomerSiteCountViewModel customersitemodel = new CustomerSiteCountViewModel();
                    List<ViewModels.SiteDetail> li = new List<ViewModels.SiteDetail>();
                    foreach (var i in customerSiteDetaillist)
                    {
                        ViewModels.SiteDetail obj = new ViewModels.SiteDetail();
                        obj.SiteDetailId = i.SiteDetailId;
                        obj.StreetName = i.StreetName;
                        li.Add(obj);
                    }
                    customersitemodel.siteDetail = li;

                    EmployeejobSearchViewModel employeejobSearchViewModel = new EmployeejobSearchViewModel
                    {
                        PageSize = PageNum
                    };

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel> mapper = new CommonMapper<EmployeeJobVieweModel, EmployeeJobsViewModel>();
                    List<EmployeeJobsViewModel> EmployeejobViewModel = mapper.MapToList(EmployeeList.ToList());
                    var EmployeejobListViewModel = new EmployeeJobListViewModel
                    {
                        EmployeeJoblist = EmployeejobViewModel,
                        SiteCountviewModel = customersitemodel,
                        Employeejobsearchmodel = employeejobSearchViewModel,
                    };
                    EmployeejobListViewModel.Employeejobsearchmodel.CustomerGeneralInfoId = CustGeneralInfoId;
                    return PartialView("_CustomerJobList", EmployeejobListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:  /Customer/Customer/InvoiceHistory
        /// <summary>
        /// view Invoice History
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns></returns>
        public ActionResult InvoiceHistory(string CustomerGeneralInfoId)
        {
            try
            {
                using (Invrepo)
                {
                    Guid custGeneralinfoid = Guid.Parse(CustomerGeneralInfoId);
                    var CustomerInvoiceList = Invrepo.GetEmployeeInvoice();

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);

                    InvoiceSearchViewModel invoiceSearchViewModel = new InvoiceSearchViewModel
                    {
                        PageSize = PageSize
                    };


                    CustomerInvoiceList = CustomerInvoiceList.Where(customer => customer.CustomerGeneralInfoId == custGeneralinfoid);

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<InvoiceHistoryCoreVieweModel, InvoiceHistoryviewModel> mapper = new CommonMapper<InvoiceHistoryCoreVieweModel, InvoiceHistoryviewModel>();
                    List<InvoiceHistoryviewModel> InvoiceViewModel = mapper.MapToList(CustomerInvoiceList.ToList());

                    var invoiceHistoryListViewModel = new InvoiceHistoryListViewModel
                    {
                        invoiceHistoryViewModel = InvoiceViewModel,
                        invoiceSearchViewModel = invoiceSearchViewModel
                    };
                    invoiceHistoryListViewModel.invoiceSearchViewModel.CustomerGeneralInfoId = custGeneralinfoid;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed invoice history.");

                    return View(invoiceHistoryListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Post: View All Invoice history
        /// </summary>
        /// <param name="customerGeneralinfoid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult InvoiceHistoryPartial(string customerGeneralinfoid, string Keyword)
        {
            try
            {
                using (Invrepo)
                {
                    Guid custGeneralinfoid = Guid.Parse(customerGeneralinfoid);
                    var CustomerInvoiceList = Invrepo.GetEmployeeInvoice();

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);


                    InvoiceSearchViewModel invoiceSearchViewModel = new InvoiceSearchViewModel
                    {
                        PageSize = PageSize,
                        searchkeyword = Keyword
                    };


                    CustomerInvoiceList = CustomerInvoiceList.Where(customer => customer.CustomerGeneralInfoId == custGeneralinfoid);

                    string keyword = Keyword;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        CustomerInvoiceList = CustomerInvoiceList.Where(customer =>
                        (customer.JobNo != null && customer.JobNo.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.CustomerName != null && customer.CustomerName.ToLower().Contains(keyword.ToLower())) ||
                        (customer.InvoiceNo != null && customer.InvoiceNo.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.SiteAddress != null && customer.SiteAddress.ToLower().Contains(keyword.ToLower())) ||
                        (customer.AssignUser != null && customer.AssignUser.ToLower().Contains(keyword.ToLower()))
                        );
                    }

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<InvoiceHistoryCoreVieweModel, InvoiceHistoryviewModel> mapper = new CommonMapper<InvoiceHistoryCoreVieweModel, InvoiceHistoryviewModel>();
                    List<InvoiceHistoryviewModel> InvoiceViewModel = mapper.MapToList(CustomerInvoiceList.ToList());

                    var invoiceHistoryListViewModel = new InvoiceHistoryListViewModel
                    {
                        invoiceHistoryViewModel = InvoiceViewModel,
                        invoiceSearchViewModel = invoiceSearchViewModel
                    };
                    invoiceHistoryListViewModel.invoiceSearchViewModel.CustomerGeneralInfoId = custGeneralinfoid;

                    return PartialView("_CustomerInvoiceList", invoiceHistoryListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST: Customer/Customer/InvoiceHistoryPartial
        /// <summary>
        /// View  Jobs lists 
        /// </summary>
        /// <param name="CustomerGeneralinfoid,Keyword,PageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InvoiceHistory(string CustomerGeneralinfoid, string Keyword, int PageNum)
        {
            try
            {
                using (CustJobs)
                {
                    Guid CustGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);
                    var CustomerInvoiceList = Invrepo.GetEmployeeInvoice();

                    CustomerInvoiceList = CustomerInvoiceList.Where(customer => customer.CustomerGeneralInfoId == CustGeneralInfoId);

                    string keyword = Keyword;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        CustomerInvoiceList = CustomerInvoiceList.Where(customer =>
                        (customer.JobNo != null && customer.JobNo.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.CustomerName != null && customer.CustomerName.ToLower().Contains(keyword.ToLower())) ||
                        (customer.InvoiceNo != null && customer.InvoiceNo.ToString().ToLower().Contains(keyword.ToLower())) ||
                        (customer.SiteAddress != null && customer.SiteAddress.ToLower().Contains(keyword.ToLower())) ||
                        (customer.AssignUser != null && customer.AssignUser.ToLower().Contains(keyword.ToLower()))
                        );
                    }
                    InvoiceSearchViewModel invoiceSearchViewModel = new InvoiceSearchViewModel
                    {
                        PageSize = PageNum
                    };

                    CommonMapper<InvoiceHistoryCoreVieweModel, InvoiceHistoryviewModel> mapper = new CommonMapper<InvoiceHistoryCoreVieweModel, InvoiceHistoryviewModel>();
                    List<InvoiceHistoryviewModel> InvoiceViewModel = mapper.MapToList(CustomerInvoiceList.ToList());

                    var invoiceHistoryListViewModel = new InvoiceHistoryListViewModel
                    {
                        invoiceHistoryViewModel = InvoiceViewModel,
                        invoiceSearchViewModel = invoiceSearchViewModel
                    };
                    invoiceHistoryListViewModel.invoiceSearchViewModel.CustomerGeneralInfoId = CustGeneralInfoId;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed invoice history.");

                    return PartialView("_CustomerInvoiceList", invoiceHistoryListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET: checking Customer Job Exist 
        /// <summary>
        /// Check CustomerJob Exist
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns></returns>
        public ActionResult CheckCustomerJob(string CustomerGeneralInfoId)
        {
            try
            {
                using (CustJobs)
                {
                    int result = 0;
                    Guid id = Guid.Parse(CustomerGeneralInfoId);
                    Jobs JobDetail = CustJobs.FindBy(user => user.CustomerGeneralInfoId == id).FirstOrDefault();
                    if (JobDetail != null)
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
        //POST:  /Customer/Customer/DeleteCustomer
        /// <summary>
        /// Delete customer  
        /// </summary>
        /// <param name="Customercontactid"></param>
        /// <returns></returns>
        public ActionResult DeleteCustomer(string CustomerGeneralInfoId)
        {
            try
            {
                Guid Id = Guid.Parse(CustomerGeneralInfoId);
                //var CustomerContactsList = Customercontacts.FindBy(i => i.CustomerGeneralInfoId == Id).ToList();
                //var CustomerSiteDocumentList = CustomerSitesDocuments.FindBy(i => i.CustomerGeneralInfoId == Id).ToList();
                //var CustomerContactLogList = CustomercontactLog.FindBy(i => i.CustomerGeneralInfoId == Id).ToList();
                //var CustomerSiteList = CustomerSiteDetail.FindBy(i => i.CustomerGeneralInfoId == Id).ToList();
                //var CustomerBillingList = CustomerBilling.FindBy(i => i.CustomerGeneralInfoId == Id).ToList();
                CustomerGeneralInfo CustomerDetail = Customer.FindBy(i => i.CustomerGeneralInfoId == Id).FirstOrDefault();

                ////delete data in customer contacts
                //using (Customercontacts)
                //{
                //    foreach (var key in CustomerContactsList)
                //    {
                //        Customercontacts.Delete(key);
                //        Customercontacts.Save();
                //    }
                //}
                ////delete data in customer site documents
                //using (CustomerSitesDocuments)
                //{
                //    foreach (var key in CustomerSiteDocumentList)
                //    {
                //        CustomerSitesDocuments.Delete(key);
                //        CustomerSitesDocuments.Save();
                //    }
                //}

                ////delete data in customer contact log
                //using (CustomercontactLog)
                //{
                //    foreach (var key in CustomerContactLogList)
                //    {
                //        CustomercontactLog.Delete(key);
                //        CustomercontactLog.Save();
                //    }
                //}

                ////delete data in customer site
                //using (CustomerSiteDetail)
                //{
                //    foreach (var key in CustomerSiteList)
                //    {
                //        using (ConditionReport)
                //        {
                //            CustomerConditionReport conditionReport = ConditionReport.FindBy(i => i.SiteDetailId == key.SiteDetailId).FirstOrDefault();
                //            if (conditionReport != null)
                //            {
                //                ConditionReport.Delete(conditionReport);
                //                ConditionReport.Save();
                //            }
                //        }
                //        using (CustomerResidence)
                //        {
                //            CustomerResidenceDetail residenceDetail = CustomerResidence.FindBy(i => i.SiteDetailId == key.SiteDetailId).FirstOrDefault();
                //            if (residenceDetail != null)
                //            {
                //                CustomerResidence.Delete(residenceDetail);
                //                CustomerResidence.Save();
                //            }
                //        }
                //        CustomerSiteDetail.Delete(key);
                //        CustomerSiteDetail.Save();
                //    }
                //}

                ////delete data in customer billing
                //using (CustomerBilling)
                //{
                //    foreach (var key in CustomerBillingList)
                //    {
                //        CustomerBilling.Delete(key);
                //        CustomerBilling.Save();
                //    }
                //}

                //delete data in customer 
                using (Customer)
                {
                    CustomerDetail.IsDelete = true;
                    Customer.Edit(CustomerDetail);
                    Customer.Save();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted a customer.");

                return Json(new { id = CustomerGeneralInfoId.ToString() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Customer/Customer/ViewCustomerBillingAddress
        /// <summary>
        /// Show all Billing Address
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ViewCustomerBillingAddress(string customerGeneralinfoid)
        {
            try
            {
                using (CustomerBilling)
                {
                    BillingSearchViewModel billingSearchViewModel = new BillingSearchViewModel();
                    CustomerBillingAddressViewModel CustomerBillingAddressViewModel = new CustomerBillingAddressViewModel();
                    Guid CustInfoId = Guid.Parse(customerGeneralinfoid);
                    var BillingList = CustomerBilling.GetBillingAddressList(CustInfoId).ToList();

                    CommonMapper<CustomerBillingAddressCoreViewModel, CustomerBillingAddressViewModel> mapper = new CommonMapper<CustomerBillingAddressCoreViewModel, CustomerBillingAddressViewModel>();
                    List<CustomerBillingAddressViewModel> customerBillingViewModel = mapper.MapToList(BillingList.ToList());

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]); ; // default page size;
                    billingSearchViewModel.PageSize = PageSize;

                    var customerBillingAddressListViewModel = new CustomerBillingAddressListViewModel
                    {
                        CustomerBillingViewModelList = customerBillingViewModel,
                        BillingSearchInfo = billingSearchViewModel,
                        CustomerGeneralInfoId = CustInfoId,
                        customerBillingAddressViewModel = CustomerBillingAddressViewModel,
                        CustomerBillingAddress = new CustomerBillingAddress(),
                        BillingCount = BillingList.Count,
                        BillingAddressId = BillingList.Count == 1 ? BillingList[0].BillingAddressId : Guid.Empty
                    };
                    customerBillingAddressListViewModel.CustomerGeneralInfoId = CustInfoId;

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed customer billing address.");

                    return View(customerBillingAddressListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Manage Customer Billings
        /// <summary>
        /// Manage Customer Sites Using CustomerGeneralInfoId
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ManageCustomerBillingsAddPartial(string CustomerGeneralInfoId)
        {
            try
            {
                CustomerBillingAddressListViewModel customerBillingAddressListViewModel = new CustomerBillingAddressListViewModel();
                Guid customerGeneraId = Guid.Parse(CustomerGeneralInfoId);

                customerBillingAddressListViewModel.CustomerGeneralInfoId = customerGeneraId;
                customerBillingAddressListViewModel.HideBillingBtn = Request.QueryString["HideBillingBtn"];

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed customer sites using customer general info ids.");

                return PartialView("_CustomerBillingForm", customerBillingAddressListViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POST:Delete Customer Billing
        /// <summary>
        /// Delete Customer Billing Using BillingAddressId
        /// </summary>
        /// <param name="SiteDetailId"></param>
        /// <returns></returns>
        public ActionResult DeleteCustomerBilling(string BillingAddressId)
        {
            try
            {
                Guid billingDetailId;
                Guid.TryParse(BillingAddressId, out billingDetailId);

                CustomerBillingAddress customerBillingDetail = CustomerBilling.FindBy(m => m.BillingAddressId == billingDetailId).FirstOrDefault();

                using (CustomerBilling)
                {
                    customerBillingDetail.IsDelete = true;
                    CustomerBilling.Edit(customerBillingDetail);
                    CustomerBilling.Save();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + "  deleted customer billing using billing address id.");

                return Json(new { id = customerBillingDetail.CustomerGeneralInfoId.ToString(), activetab = "Billing Address" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Manage Customer Billing
        /// <summary>
        /// Manage Customer Billing Using BillingAddressId
        /// </summary>
        /// <param name="CustomerGeneralInfoId"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ManageCustomerBillingPartial(string BillingAddressId)
        {
            try
            {
                CustomerBillingAddressListViewModel customerBillingViewModel = new CustomerBillingAddressListViewModel();

                Guid billingDetailId;
                Guid.TryParse(BillingAddressId, out billingDetailId);

                CustomerBillingAddress customerBillingDetail = CustomerBilling.FindBy(m => m.BillingAddressId == billingDetailId).FirstOrDefault();
                CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel> mapbillingdetail = new CommonMapper<CustomerBillingAddress, CustomerBillingAddressViewModel>();
                CustomerBillingAddressViewModel customerBillingDetailViewModel = mapbillingdetail.Mapper(customerBillingDetail);
                var userName = "";
                if (customerBillingDetailViewModel.ModifiedBy == null)
                {
                    userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerBillingDetailViewModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                }
                else
                {
                    userName = EmployeeRepo.FindBy(m => m.EmployeeId == customerBillingDetailViewModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                }
                if (customerBillingDetailViewModel.ModifiedDate == null)
                {
                    customerBillingDetailViewModel.CreatedDate = customerBillingDetailViewModel.CreatedDate;
                }
                else
                {
                    customerBillingDetailViewModel.ModifiedDate = customerBillingDetailViewModel.ModifiedDate;
                }

                customerBillingViewModel.customerBillingAddressViewModel = customerBillingDetailViewModel;
                customerBillingViewModel.CustomerGeneralInfoId = customerBillingDetailViewModel.CustomerGeneralInfoId;
                customerBillingViewModel.BillingAddressId = billingDetailId;
                customerBillingViewModel.UserName = userName;
                customerBillingViewModel.ModifiedDate = customerBillingDetailViewModel.ModifiedDate;
                customerBillingViewModel.CreatedDate = customerBillingDetailViewModel.CreatedDate;

                return PartialView("_CustomerBillingForm", customerBillingViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Customer Billing List
        /// <summary>
        /// Show List Of Customer Billing
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerBillingList()
        {
            BillingSearchViewModel billingSearchViewModel = new BillingSearchViewModel();
            CustomerBillingAddressViewModel CustomerBillingAddressViewModel = new CustomerBillingAddressViewModel();

            string Name = Request.QueryString["Name"] == null ? string.Empty : Request.QueryString["Name"];
            string CustomerGeneralInfoId = Request.QueryString["CustomerGeneralInfoId"];
            var BillingList = CustomerBilling.GetBillingAddressList(Guid.Parse(CustomerGeneralInfoId), Name).ToList();

            //Mapping
            CommonMapper<CustomerBillingAddressCoreViewModel, CustomerBillingAddressViewModel> mapper = new CommonMapper<CustomerBillingAddressCoreViewModel, CustomerBillingAddressViewModel>();
            List<CustomerBillingAddressViewModel> customerBillingViewModel = mapper.MapToList(BillingList.ToList());

            int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                       Convert.ToInt32(Request.QueryString["page_size"]); // default page size;
            billingSearchViewModel.PageSize = PageSize;

            var customerBillingAddressListViewModel = new CustomerBillingAddressListViewModel
            {
                CustomerBillingViewModelList = customerBillingViewModel,
                BillingSearchInfo = billingSearchViewModel,
                CustomerGeneralInfoId = Guid.Parse(CustomerGeneralInfoId),
                customerBillingAddressViewModel = CustomerBillingAddressViewModel,
                CustomerBillingAddress = new CustomerBillingAddress()
            };
            customerBillingAddressListViewModel.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralInfoId);

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + "  view customer billing list.");

            return PartialView("_CustomerBillingAddressList", customerBillingAddressListViewModel);
        }


        public ActionResult GetSiteAddress()
        {
            var id = !string.IsNullOrEmpty(Request.QueryString["id"]) ? Guid.Parse(Request.QueryString["id"]) : Guid.Empty;
            var addressList = CustomerSiteDetail.FindBy(m => m.CustomerGeneralInfoId == id).Select(m => new SiteAddressViewModel()
            {
                Street = m.Street,
                StreetName = m.StreetName,
                StreetType = (Constant.HomeAddressStreetType)m.StreetType,
                Suburb = m.Suburb,
                State = m.State,
                PostalCode = m.PostalCode
            }).ToList();

            return PartialView("_GetSiteAddress", addressList);
        }



        //GET:  /Customer/Customer/DeleteCustomerReminder
        /// <summary>
        /// Delete customer Contact Log
        /// </summary>
        /// <param name="ReminderId"></param>
        /// <returns></returns>
        public ActionResult DeleteCustomerReminder(string ReminderId, string PageNum)
        {
            try
            {
                using (CustomerReminderRepo)
                {
                    Guid reminderId = Guid.Parse(ReminderId);
                    CustomerContactLog logtodelete = CustomercontactLog.FindBy(i => i.CustomerContactId == reminderId).FirstOrDefault();
                    logtodelete.IsDelete = true;
                    CustomercontactLog.Edit(logtodelete);
                    CustomercontactLog.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + "  deleted customer Reminder.");

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PartialViewResult _CustomerReminderCreate(Guid customerGeneralinfoid, string Customercontactid, string PageNum, string SiteName)
        {
            try
            {
                ViewModels.CustomerReminderVM model = new ViewModels.CustomerReminderVM();
                List<SelectListItem> JobList = null;

                if (!string.IsNullOrEmpty(Customercontactid))
                {
                    Guid contactLogId = Guid.Parse(Customercontactid);
                    var customercontactLogInfo = CustomercontactLog.FindBy(m => m.CustomerContactId == contactLogId).
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
                        model.ReminderDate = scheduleReminder.ScheduleDate;
                        model.Schedule = scheduleReminder.Schedule;
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

                    JobList = CustomercontactLog.GetJobByContactLog(Customercontactid).Select(m =>
                              new SelectListItem
                              {
                                  Text = m.SiteFileName.Length > 33 ? (m.SiteFileName).Substring(0, 33) + "...." + "(JobNo :" + m.JobNo.ToString() + ")" : m.SiteFileName + "(JobNo :" + m.JobNo.ToString(),
                                  Value = m.Id.ToString()
                              }).ToList();
                    model.JobList = JobList;
                    model.JobId2 = CustomercontactLog.GetJobByContactLog(Customercontactid).Select(m =>
                                   new JobDataVM
                                   {
                                       Id = m.Id
                                   }).Select(m => m.Id).ToList();

                    model.ContactList = Customercontacts.GetJobContactList("'" + JobList[0].Value + "'").Select(m => new
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
                    model.ReminderId = Guid.Parse(Customercontactid);
                    return PartialView(model);
                }
                else
                {
                    JobList = CustJobs.FindBy(m => m.CustomerGeneralInfoId == customerGeneralinfoid)
                              .Where(m => m.Status != 15).Select(m =>
                             new SelectListItem
                             {
                                 Text = m.CustomerSiteDetail.SiteFileName.Length > 33 ? (m.CustomerSiteDetail.SiteFileName).Substring(0, 33) + "...." + "(JobNo :" + m.JobNo.ToString() + ")" : m.CustomerSiteDetail.SiteFileName + "(JobNo :" + m.JobNo.ToString(),
                                 Value = m.Id.ToString()
                             }).ToList();
                    Guid CustomerGeneralinfoid = customerGeneralinfoid;

                    CustomerGeneralInfo CustomerGeneralinfo = new CustomerGeneralInfo();
                    using (Customer)
                    {
                        CustomerGeneralinfo = Customer.FindBy(i => i.CustomerGeneralInfoId == CustomerGeneralinfoid).FirstOrDefault();
                    }
                    model = new ViewModels.CustomerReminderVM();
                    model.CustomerGeneralInfoId = customerGeneralinfoid;

                    model.CustomerId = CustomerGeneralinfo.CTId.ToString();
                    model.JobList = JobList;
                    model.ContactList = new List<SelectListItem>();

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
        public ActionResult _GetJobContacts(string[] jobIds)
        {
            FSM.Web.Areas.Customer.ViewModels.CustomerReminderVM customerReminderViewModel =
                new FSM.Web.Areas.Customer.ViewModels.CustomerReminderVM();
            if (jobIds != null)
            {
                string strJobIds = string.Join("','", jobIds); // separating each element by "','"
                strJobIds = strJobIds.Insert(0, "'"); // putting "'" at 0 index
                strJobIds = strJobIds.Insert(strJobIds.Count(), "'"); // putting "'" at last index

                var jobContacts = Customercontacts.GetJobContactList(strJobIds).Select(m => new
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
                }).ToList();
            }
            else
            {
                customerReminderViewModel.ContactList = new List<SelectListItem>();
            }

            return PartialView(customerReminderViewModel);
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

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> CustomerSendReminder(DateTime? ReminderDate, Guid ReminderId, string[] JobId, string[] ContactListIds, Nullable<FSMConstant.Constant.CustomerJobTemplateMessage> TempMsgId, string Note, bool HasSMS, bool HasEmail, bool hasSchedule, string fromEmail, string fromEmailVal)
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
                var contactLog = CustomercontactLog.FindBy(m => m.CustomerContactId == ReminderId).FirstOrDefault();
                contactLog.LogDate = DateTime.Now;
                contactLog.Note = TempMsgId.Value > 0 ? TempMsgId.GetAttribute<DisplayAttribute>().Name : TempMsgId.Value.ToString();
                contactLog.JobId = JobId.Count() > 0 ? JobId[0] : string.Empty;
                contactLog.ModifiedDate = DateTime.Now;
                contactLog.ModifiedBy = Guid.Parse(base.GetUserId);
                CustomercontactLog.Edit(contactLog);
                CustomercontactLog.Save();
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

                            var jobs = CustJobs.FindBy(i => i.Id == jobId).FirstOrDefault();
                            string msgBody = Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);
                            //if (contact.DateBooked.HasValue)
                            if (jobs != null)
                            {
                                if (jobs.DateBooked.HasValue)
                                {
                                    if (jobs.DateBooked >= DateTime.Now.Date)
                                    {
                                        if (jobs.DateBooked.HasValue)
                                        {

                                            msgBody = msgBody.Replace("{{DateBooked}}", jobs.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        else
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                                        }
                                        int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                        string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == jobId && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= jobs.DateBooked).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        //string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == jobId && m.IsDelete==false).OrderBy(m => m.StartTime).Select(m => m.StartTime).FirstOrDefault()).ToString();
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
                    CustomerID = CustJobs.FindBy(m => m.Id == Job_id).Select(m => m.CustomerGeneralInfoId).FirstOrDefault();
                    var customerInfo = CustJobs.FindBy(m => m.Id == Job_id).Select(m => new
                    {
                        m.CustomerGeneralInfo.CustomerGeneralInfoId,
                        m.CustomerGeneralInfo.CTId
                    }).FirstOrDefault();

                    var JobDetail = CustJobs.FindBy(m => m.Id == Job_id).FirstOrDefault();
                    string SiteAddress = CustomerSiteDetail.GetSiteAddress(JobDetail.SiteId);

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
                    CustomercontactLog.Add(customerContactLog);
                    CustomercontactLog.Save();
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
                            var jobs = CustJobs.FindBy(i => i.Id == Job_id).FirstOrDefault();
                            string msgBody = Note;
                            msgBody = msgBody.Replace("{{ClientName}}", contact.FirstName + " " + contact.LastName);
                            msgBody = msgBody.Replace("{{SiteAdress}}", contact.SiteFileName);

                            if (jobs != null)
                            {
                                if (jobs.DateBooked.HasValue)
                                {
                                    if (jobs.DateBooked >= DateTime.Now.Date)
                                    {

                                        if (contact.DateBooked.HasValue)
                                            msgBody = msgBody.Replace("{{DateBooked}}", jobs.DateBooked.Value.ToString("dddd, dd MMMM yyyy"));
                                        else
                                        {
                                            msgBody = msgBody.Replace("{{DateBooked}}", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
                                        }

                                        int completestatus = Convert.ToInt16(Constant.JobStatus.Completed);
                                        string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete == false && m.Status != completestatus).OrderBy(m => m.StartTime).Where(i => i.DateBooked >= jobs.DateBooked).Select(m => m.StartTime).FirstOrDefault()).ToString();
                                        // string startDate = (JobAssignToMappingRepo.FindBy(m => m.JobId == Job_id && m.IsDelete==false).OrderBy(m => m.StartTime).Select(m => m.StartTime).FirstOrDefault()).ToString();
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
            return Json(new { success = CustomerID }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SendSupportMail()
        {
            try
            {
                CustomerEmailSendListViewModel model = new CustomerEmailSendListViewModel();
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<ActionResult> SendSupportMail(List<CustomerEmailSendViewModel> model)
        {
            try
            {
                // for from
                //  var From = Request.Form.Get("from");
                var Severity = Request.Form.Get("Severity");
                Guid LoggedId = Guid.Parse(base.GetUserId);
                string msg = "";
                var employee = EmployeeRepo.FindBy(i => i.EmployeeId == LoggedId).FirstOrDefault();
                var FromEmail = employee.Email;
                var From = FromEmail;
                msg = "Hi " + employee.UserName + " ,";
                var To = "support@srag-portal.com";
                var Subject = Request.Form.Get("subject");
                var CC = Request.Form.Get("Cc");
                var BCC = Request.Form.Get("Bcc");
                var message = Request.Form.Get("Message");
                using (MailMessage mm = new MailMessage())
                {
                    mm.Subject = "FSM :" + Severity;
                    mm.IsBodyHtml = true;
                    string lastname = !String.IsNullOrEmpty(employee.LastName) ? employee.LastName : "";
                    mm.Body = message + @"<br/><br/></br>Thanks, <br/>" + employee.FirstName + " " + lastname + " <br/>" + employee.Email + "<br/>";
                    mm.From = new MailAddress(FromEmail);
                    string[] TOId = To.Split(',');
                    foreach (string ToEmail in TOId)
                    {
                        mm.To.Add(new MailAddress(ToEmail)); //Adding Multiple To email Id
                    }
                    string[] CCId = CC.Split(',');

                    if (CCId[0] != null && CCId[0] != "")
                    {
                        foreach (string CCEmail in CCId)
                        {
                            mm.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                        }
                    }
                    mm.Bcc.Add(new MailAddress("fsmsupport@seasiainfotech.com")); //Adding Multiple Bcc email Id

                    //For File Uploade
                    if (Request.Files != null && Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            HttpPostedFileBase file = Request.Files[i];
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var attachment = new Attachment(file.InputStream, fileName);
                                mm.Attachments.Add(attachment);
                            }
                        }
                    }
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                        smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                        smtp.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                        smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtp.SendMailAsync(mm);


                        //send mail to the sender

                        using (MailMessage mmrev = new MailMessage())
                        {
                            mmrev.Subject = "Thanks for Contacting us";
                            mmrev.IsBodyHtml = true;
                            mmrev.Body = msg + @"<br/><br/>Thanks for your support request, we will respond to your issue asap.
                                            <br/><br/><br/> <br/>
                                            Thanks, <br/>
                                            Support Team<br/>
                                            <a href='www.srag-portal.com'>www.srag-portal.com<a>";
                            //mm.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Username"]);
                            mmrev.From = new MailAddress(To);
                            mmrev.To.Add(new MailAddress(FromEmail)); //Adding Multiple To email Id

                            using (SmtpClient smtprev = new SmtpClient())
                            {
                                smtprev.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
                                smtprev.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                                smtprev.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
                                smtprev.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
                                smtprev.DeliveryMethod = SmtpDeliveryMethod.Network;
                                await smtp.SendMailAsync(mmrev);

                            }
                        }

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + "  sent support mail.");

                        return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }
        //GET:  /Customer/Customer/ConditionalReportExport
        /// <summary>
        /// Conditional Report Export
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ConditionalReportExportPreview()
        {
            try
            {
                Guid? Id = Guid.Parse(Request.RequestContext.RouteData.Values["Id"].ToString());
                CustomerConditionReportViewModel customerConditionReportViewModel = null;
                if (Id != Guid.Empty)
                {
                    CustomerConditionReport customerConditionReport = ConditionReport.FindBy(m => m.ConditionReportId == Id).FirstOrDefault();
                    CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel> mapconditiondetail = new CommonMapper<CustomerConditionReport, CustomerConditionReportViewModel>();
                    customerConditionReportViewModel = mapconditiondetail.Mapper(customerConditionReport);
                }
                else
                {
                    customerConditionReportViewModel = new CustomerConditionReportViewModel();
                    customerConditionReportViewModel.RoofTilesSheets = Constant.RoofType.Undefined;
                    customerConditionReportViewModel.RidgeCappings = Constant.RidgeCappings.Unknown;
                    customerConditionReportViewModel.BargeCappings = Constant.BargeCappings.Unknown;
                    customerConditionReportViewModel.Valleys = Constant.Valleys.Unknown;
                    customerConditionReportViewModel.Flashings = Constant.flashings.Unknown;
                    customerConditionReportViewModel.Gutters = Constant.GutarGuard.Unknown;
                    customerConditionReportViewModel.DownPipes = Constant.DownPipe.Unknown;
                }

                return new ViewAsPdf("ConditionalReportExportPreview", customerConditionReportViewModel)
                {
                    FileName = "ConditionalReport.pdf"
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult UploadFiles(string CustomerGeneralinfoid, string SiteId, IEnumerable<HttpPostedFileBase> files)
        {
            CustomerSitesDocumentsViewModel customerSiteDocumentsViewModel = new CustomerSitesDocumentsViewModel();
            //foreach (var file in files)
            //{
            //    //string filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
            //file.SaveAs(Path.Combine(Server.MapPath("~/Images/EmployeeJobs/" + Jobid), filePath));
            customerSiteDocumentsViewModel.IsDelete = false;
            customerSiteDocumentsViewModel.CreatedDate = DateTime.Now;
            customerSiteDocumentsViewModel.CreatedBy = Guid.Parse(base.GetUserId);

            for (int i = 0; i < files.Count(); i++)
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
                    customerSiteDocumentsViewModel.SiteId = Guid.Parse(SiteId);
                    customerSiteDocumentsViewModel.CustomerGeneralInfoId = Guid.Parse(CustomerGeneralinfoid);
                    customerSiteDocumentsViewModel.DocumentName = fileName.ToString();
                    customerSiteDocumentsViewModel.DocType = GetDocumentType(extension);
                    CommonMapper<CustomerSitesDocumentsViewModel, CustomerSitesDocuments> mapperdoc = new CommonMapper<CustomerSitesDocumentsViewModel, CustomerSitesDocuments>();
                    CustomerSitesDocuments customerSiteDocuments = mapperdoc.Mapper(customerSiteDocumentsViewModel);
                    CustomerSitesDocuments.Add(customerSiteDocuments);
                    CustomerSitesDocuments.Save();
                }
            }
            //Here you can write code for save this information in your database if you want

            return Json("file uploaded successfully");
        }
    }
}