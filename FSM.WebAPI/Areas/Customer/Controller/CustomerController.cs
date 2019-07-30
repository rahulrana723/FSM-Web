using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Infrastructure;
using FSM.WebAPI.App_Start;
using FSM.WebAPI.Areas.Customer.ViewModels;
using FSM.WebAPI.Areas.Employee.ViewModels;
using FSM.WebAPI.Common;
using FSM.WebAPI.Controllers;
using FSM.WebAPI.Models;
using log4net;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FSM.WebAPI.Areas.Customer.Controller
{

  // [System.Web.Http.Authorize]
    public class CustomerController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private FsmContext db = new FsmContext();
        UnityContainer container = new UnityContainer();

        public ICustomerContactsRepository contactsRepository { get; set; }


        // GET: Customer/GetCustomerList
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Customer/GetCustomerList")]
        public ServiceResponse<GetCustomerListViewModel> GetCustomerList(dynamic json)
        {
            Guid Id = Guid.Parse(json.CreatedBy.Value);
            ServiceResponse<GetCustomerListViewModel> result = new ServiceResponse<GetCustomerListViewModel>();
            try
            {
                IQueryable<GetCustomerListViewModel> lstGetCustomerViewModel = from customer in db.CustomerGeneralInfo
                                                                            join CSD in db.CustomerSiteDetail on customer.CustomerGeneralInfoId equals CSD.CustomerGeneralInfoId
                                                                            join contacts in db.CustomerContacts on customer.CustomerGeneralInfoId equals contacts.CustomerGeneralInfoId
                                                                            where customer.CreatedBy==Id && customer.IsDelete == false && CSD.IsDelete == false && contacts.IsDelete==false 
                                                                            select new GetCustomerListViewModel
                                                                            {
                                                                                CustomerGeneralInfoId = customer.CustomerGeneralInfoId,
                                                                                CustomerLastName = customer.CustomerLastName,
                                                                                CustomerNotes = customer.CustomerNotes,
                                                                                SiteDetailId = CSD.SiteDetailId,
                                                                                Address = CSD.SiteFileName,
                                                                                Unit=CSD.Unit,
                                                                                ContactId = contacts.ContactId,
                                                                                ContactName = contacts.FirstName,
                                                                                PhoneNo = contacts.PhoneNo1,
                                                                                EmailAddress = contacts.EmailId,
                                                                                CreatedDate = customer.CreatedDate
                                                                            };
                lstGetCustomerViewModel = lstGetCustomerViewModel.OrderByDescending(m=>m.CreatedDate);

                result.ResponseList = lstGetCustomerViewModel.ToList().Select(m => new GetCustomerListViewModel
                {
                    CustomerGeneralInfoId = m.CustomerGeneralInfoId,
                    CustomerLastName = m.CustomerLastName,
                    CustomerNotes = m.CustomerNotes,
                    Unit = m.Unit,
                    SiteDetailId = m.SiteDetailId,
                    Address = m.Address,
                    ContactId = m.ContactId,
                    ContactName = m.ContactName,
                    PhoneNo = m.PhoneNo,
                    EmailAddress = m.EmailAddress,
                    CreatedDate = m.CreatedDate
                }).ToList();
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

        // POST: api/AddCustomer
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Customer/AddCustomer")]
        public ServiceResponse<string> AddCustomer(CustomerListViewModel customerViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (customerViewModel.CustomerLastName == null)
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = ("Customer last name is required!");
                    return result;
                }

                //Entry In Customer General Info

                customerViewModel.CustomerGeneralInfoId = Guid.NewGuid();    
                CommonMapper<CustomerListViewModel, CustomerGeneralInfo> mapper = new CommonMapper<CustomerListViewModel, CustomerGeneralInfo>();
                CustomerGeneralInfo generalInfo = mapper.Mapper(customerViewModel);
                generalInfo.Frequency = 0;
                generalInfo.PrefTimeOfDay = 0;
                generalInfo.CustomerType = 1;
                generalInfo.LeadType = 0;
                generalInfo.Terms = 0;
                generalInfo.IsActive = false;
                generalInfo.UmbrellaGroup = false;
                generalInfo.CTId = db.CustomerGeneralInfo.Max(m => m.CTId) + 1;
                generalInfo.CreatedDate = DateTime.Now;
                generalInfo.CreatedBy = customerViewModel.CreatedBy;
                generalInfo.IsDelete = false;
                db.CustomerGeneralInfo.Add(generalInfo);
                db.SaveChanges();

                //Entry In Customer Site Detail

                customerViewModel.SiteDetailId = Guid.NewGuid();
                CommonMapper<CustomerListViewModel, CustomerSiteDetail> mapperSite = new CommonMapper<CustomerListViewModel, CustomerSiteDetail>();
                CustomerSiteDetail siteDetail = mapperSite.Mapper(customerViewModel);
                siteDetail.CustomerGeneralInfoId = customerViewModel.CustomerGeneralInfoId;
                siteDetail.SiteFileName = customerViewModel.Address;
                siteDetail.CreatedDate = DateTime.Now;
                siteDetail.CreatedBy = customerViewModel.CreatedBy;
                siteDetail.IsDelete = false;
                db.CustomerSiteDetail.Add(siteDetail);
                db.SaveChanges();

                //Entry In Customer Residence Detail

                CustomerResidenceDetail residenceDetail =new CustomerResidenceDetail();
                residenceDetail.ResidenceDetailId = Guid.NewGuid();
                residenceDetail.SiteDetailId = customerViewModel.SiteDetailId;
                residenceDetail.GutterGaurd = 0;
                residenceDetail.RoofType = 0;
                residenceDetail.Pitch = 0;
                residenceDetail.Height = 0;
                residenceDetail.SRASinstalled = 0;
                residenceDetail.NeedTwoPPL = true;
                residenceDetail.CreatedDate = DateTime.Now;
                residenceDetail.CreatedBy = customerViewModel.CreatedBy;
                residenceDetail.IsDelete = false;
                db.CustomerResidenceDetail.Add(residenceDetail);
                db.SaveChanges();

                //Entry In Customer Condition Detail
                CustomerConditionReport conditionDetail = new CustomerConditionReport();
                conditionDetail.ConditionReportId = Guid.NewGuid();
                conditionDetail.SiteDetailId = customerViewModel.SiteDetailId;
                conditionDetail.DownPipes = 0;
                conditionDetail.RoofTilesSheets = 0;
                conditionDetail.BargeCappings = 0;
                conditionDetail.RidgeCappings = 0;
                conditionDetail.Valleys = 0;
                conditionDetail.Flashings = 0;
                conditionDetail.Gutters = 0;
                conditionDetail.ConditionRoof = 0;
                conditionDetail.ConditionOfRoof = 0;
                conditionDetail.CreatedDate = DateTime.Now;
                conditionDetail.CreatedBy = customerViewModel.CreatedBy;
                conditionDetail.IsDelete = false;
                db.CustomerConditionReport.Add(conditionDetail);
                db.SaveChanges();

                //Entry In Customer Contacts

                customerViewModel.ContactId = Guid.NewGuid();
                CommonMapper<CustomerListViewModel, CustomerContacts> mapperContacts = new CommonMapper<CustomerListViewModel, CustomerContacts>();
                CustomerContacts contacts = mapperContacts.Mapper(customerViewModel);
                contacts.CustomerGeneralInfoId = customerViewModel.CustomerGeneralInfoId;
                contacts.SiteId = customerViewModel.SiteDetailId;
                contacts.ContactsType = 1;
                contacts.EmailId = customerViewModel.EmailAddress;
                contacts.PhoneNo1 = customerViewModel.PhoneNo;
                contacts.FirstName = customerViewModel.ContactName;
                contacts.CreatedDate = DateTime.Now;
                contacts.CreatedBy = customerViewModel.CreatedBy;
                contacts.IsDelete = false;
                db.CustomerContacts.Add(contacts);
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = customerViewModel.CreatedBy.ToString();
                string userName = "Deepak";


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " add customer.");

                result.ResponseList = new List<string> { "Customer added successfully" };
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

        // POST: api/UpdateCustomer
        [System.Web.Http.HttpPost]
       [System.Web.Http.Route("api/Customer/UpdateCustomer")]
        public ServiceResponse<string> UpdateCustomer(CustomerListViewModel customerViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (customerViewModel.CustomerLastName == null)
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = ("Customer last name is required!");
                    return result;
                }

                //update Customer
                CustomerGeneralInfo generalInfo = new CustomerGeneralInfo();
                generalInfo = db.CustomerGeneralInfo.Where(m => m.CustomerGeneralInfoId == customerViewModel.CustomerGeneralInfoId).FirstOrDefault();
                generalInfo.CustomerLastName = customerViewModel.CustomerLastName;
                generalInfo.CustomerNotes = customerViewModel.CustomerNotes;
                generalInfo.IsDelete = false;
                generalInfo.ModifiedDate = DateTime.Now;
                generalInfo.ModifiedBy = customerViewModel.CreatedBy;
                db.Entry(generalInfo).State = EntityState.Modified;
                db.SaveChanges();

                //update Site
                CustomerSiteDetail siteDetail = new CustomerSiteDetail();
                siteDetail = db.CustomerSiteDetail.Where(m => m.SiteDetailId == customerViewModel.SiteDetailId).FirstOrDefault();
                siteDetail.SiteFileName = customerViewModel.Address;
                siteDetail.Unit = customerViewModel.Unit;
                siteDetail.Street = customerViewModel.Street;
                siteDetail.StreetName = customerViewModel.StreetName;
                siteDetail.Suburb = customerViewModel.Suburb;
                siteDetail.State = customerViewModel.State;
                siteDetail.PostalCode = customerViewModel.PostalCode;
                siteDetail.IsDelete = false;
                siteDetail.ModifiedDate = DateTime.Now;
                siteDetail.ModifiedBy = customerViewModel.CreatedBy;
                db.Entry(siteDetail).State = EntityState.Modified;
                db.SaveChanges();

                //update Contacts
                CustomerContacts contacts = new CustomerContacts();
                contacts = db.CustomerContacts.Where(m => m.ContactId == customerViewModel.ContactId).FirstOrDefault();
                contacts.FirstName = customerViewModel.ContactName;
                contacts.EmailId = customerViewModel.EmailAddress;
                contacts.PhoneNo1 = customerViewModel.PhoneNo;
                contacts.EmailId = customerViewModel.EmailAddress;
                contacts.IsDelete = false;
                contacts.ModifiedDate = DateTime.Now;
                contacts.ModifiedBy = customerViewModel.CreatedBy;
                db.Entry(contacts).State = EntityState.Modified;
                db.SaveChanges();


                result.ResponseList = new List<string> { "Record updated successfully." };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update customer.");


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
       [System.Web.Http.Route("api/Customer/DeleteCustomer")]

        public ServiceResponse<string> DeleteCustomer(dynamic json)
        {
            Guid Id = Guid.Parse(json.CustomerGeneralInfoId.Value);
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (Id != Guid.Empty)
                {
                    var ItemtoDelete = db.CustomerGeneralInfo.Where(i => i.CustomerGeneralInfoId == Id).FirstOrDefault();
                    if (ItemtoDelete != null)
                    {
                        ItemtoDelete.IsDelete = true;
                        db.Entry(ItemtoDelete).State = EntityState.Modified;
                        db.SaveChanges();
                        result.ResponseList = new List<string> { "Customer Deleted successfully" };
                        result.ResponseCode = 1;
                        result.ResponseErrorMessage = null;
                        return result;
                    }
                    result.ResponseList = new List<string> { "Customer does not exist!" };
                    result.ResponseCode = 2;
                    result.ResponseErrorMessage = null;

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " delete customer.");

                    return result;
                }
                result.ResponseList = new List<string> { "Customerm can't deleted .Please try later" };
                result.ResponseCode = 3;
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

        // GET: Customer/GetSiteContactsUsingSiteId
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Customer/GetSiteContactsUsingSiteId")]
        public ServiceResponse<CustomerContactsListViewModel> GetSiteContactsUsingSiteId(dynamic json)
        {
            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " get site contacts using site id");

            Guid SiteId = new Guid(json.SiteDetailId.Value);
            ServiceResponse<CustomerContactsListViewModel> result = new ServiceResponse<CustomerContactsListViewModel>();
            try
            {
                IQueryable<CustomerContactsListViewModel> lstOTRWStock = from contacts in db.CustomerContacts
                                                                  where contacts.SiteId == SiteId && contacts.IsDelete==false
                                                              select new CustomerContactsListViewModel
                                                                  {
                                                                      ContactId = contacts.ContactId,
                                                                      FirstName = contacts.FirstName,
                                                                      LastName = contacts.LastName,
                                                                      EmailId = contacts.EmailId,
                                                                      MobileNo = contacts.PhoneNo1,
                                                                  };

                result.ResponseList = lstOTRWStock.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";
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