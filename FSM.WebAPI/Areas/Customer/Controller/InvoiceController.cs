using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Infrastructure;
using FSM.WebAPI.App_Start;
using FSM.WebAPI.Areas.Customer.ViewModels;
using FSM.WebAPI.Areas.Employee.ViewModels;
using FSM.WebAPI.Common;
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


namespace FSM.WebAPI.Areas.Customer.Controller
{

    // [Authorize]
    public class InvoiceController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        UnityContainer container = new UnityContainer();
        public IiNoviceRepository InvoiceRepository { get; set; }

        public InvoiceController()
        {
            container = (UnityContainer)UnityConfig.GetConfiguredContainer();
            InvoiceRepository = container.Resolve<IiNoviceRepository>();
        }
        [HttpGet]
        [Route("api/Invoice/GetInvoiceDetailByJobId")]
        public ServiceResponse<CreateInvoiceCoreViewModel> GetInvoiceDetailByJobId(Guid Jobid)
        {
            try
            {
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get site contacts using site id");

                ServiceResponse<CreateInvoiceCoreViewModel> result = new ServiceResponse<CreateInvoiceCoreViewModel>();
                List<CreateInvoiceCoreViewModel> resultList = InvoiceRepository.GetCreateInvoiceList(Convert.ToString(Jobid)).ToList();
                if (resultList != null)
                {
                    foreach (var val in resultList)
                    {
                        if (val.Id != Guid.Empty)
                        {
                            Guid invoiceId = val.Id;
                            var invoiceData = db.Invoice.Where(m => m.Id == invoiceId).FirstOrDefault();
                            Jobs jobData = db.Jobs.Where(m => m.Id == invoiceData.EmployeeJobId).FirstOrDefault();
                            Guid customerGeneralInfoId;
                            if (invoiceData.CustomerGeneralInfoId != null && invoiceData.CustomerGeneralInfoId != Guid.Empty)
                            {
                                customerGeneralInfoId = invoiceData.CustomerGeneralInfoId ?? Guid.NewGuid();
                            }
                            else
                            {
                                customerGeneralInfoId = jobData.CustomerGeneralInfoId;
                            }
                            if (jobData.WorkOrderNumber != null)
                            {
                                val.WorkOrderNumber = jobData.WorkOrderNumber;
                            }
                            else
                            {
                                val.WorkOrderNumber = "";
                            }


                            CustomerGeneralInfo customerData = db.CustomerGeneralInfo.Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();    //Get Customer Detail
                                                                                                                                                                        //Get Job Data
                            CustomerSiteDetail sitedetail = db.CustomerSiteDetail.Where(i => i.SiteDetailId == jobData.SiteId).FirstOrDefault();                       //Get Site Data

                            dynamic billingdetail = null;
                            var BillingAddressId = invoiceData.BillingAddressId;
                            if (BillingAddressId != null)
                            {
                                if (BillingAddressId != Guid.Empty)
                                {
                                    billingdetail = db.CustomerBillingAddress.Where(m => m.BillingAddressId == BillingAddressId).FirstOrDefault();
                                }
                                else
                                {
                                    billingdetail = db.CustomerBillingAddress.Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                                }
                            }
                            else
                            {
                                billingdetail = db.CustomerBillingAddress.Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId && m.IsDefault == true).FirstOrDefault();
                            }

                            if (billingdetail == null)
                            {

                                billingdetail = db.CustomerBillingAddress.Where(m => m.CustomerGeneralInfoId == customerGeneralInfoId).FirstOrDefault();

                            }


                            val.DisplaySiteAddress = //(!string.IsNullOrEmpty(customerData.CustomerLastName) ? customerData.CustomerLastName + " " : "") +
                                                          (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                                         (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + " " : "") +
                                                         (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                                         (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");

                            if (billingdetail == null)
                            {

                                val.DisplayBillingAddress = val.DisplaySiteAddress;
                            }
                            else
                            {

                                val.DisplayBillingAddress =
                                                                          // (!string.IsNullOrEmpty(customerData.TradingName) ? customerData.TradingName + " " : "") +
                                                                          (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + " " : billingdetail.FirstName + " " : billingdetail.FirstName + " ") +
                                                                          (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + " " : "") +
                                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                            }

                            if (customerData.CustomerType == Convert.ToInt32(Constants.CustomerType.Domestic))
                            {
                                if (billingdetail == null)
                                    val.DisplayBillingAddress = val.DisplaySiteAddress;
                            }
                            else if (customerData.CustomerType == Convert.ToInt32(Constants.CustomerType.RealState) || customerData.CustomerType == Convert.ToInt32(Constants.CustomerType.Strata) || customerData.CustomerType == Convert.ToInt32(Constants.CustomerType.Commercial))
                            {
                                val.DisplaySiteAddress = (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                           (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + " " : "") +
                                           (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                           (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                           (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");


                                if (billingdetail == null)
                                {
                                    val.DisplayBillingAddress = val.DisplaySiteAddress;
                                }
                                else
                                {
                                    val.DisplayBillingAddress = (!string.IsNullOrEmpty(customerData.TradingName) ? customerData.TradingName + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.FirstName) ? !string.IsNullOrEmpty(billingdetail.LastName) ? billingdetail.FirstName + " " + billingdetail.LastName + " " : billingdetail.FirstName + " " : billingdetail.FirstName + " ") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetNo) ? billingdetail.StreetNo + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.StreetName) ? billingdetail.StreetName + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.Suburb) ? billingdetail.Suburb + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.State) ? billingdetail.State + " " : "") +
                                                         (!string.IsNullOrEmpty(billingdetail.PostalCode) ? billingdetail.PostalCode : "");
                                }
                                val.DisplaySiteAddress = (!string.IsNullOrEmpty(sitedetail.StrataPlan) ? "SP:" + sitedetail.StrataPlan + " " : "") +
                                                          (!string.IsNullOrEmpty(sitedetail.Street) ? sitedetail.Street + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.StreetName) ? sitedetail.StreetName + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.Suburb) ? sitedetail.Suburb + " " : "") +
                                              (!string.IsNullOrEmpty(sitedetail.State) ? sitedetail.State + " " + "" : "") +
                                              (!string.IsNullOrEmpty(sitedetail.PostalCode.ToString()) ? sitedetail.PostalCode.ToString() : "");
                            }
                        }
                    }
                }
                result.ResponseList = resultList.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        [Route("api/Invoice/SaveInvoiceDetail")]
        public ServiceResponse<string> SaveInvoiceDetail(ViewModels.InvoiceViewModel invoiceViewModel)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                
                invoiceViewModel.SentStatus = Convert.ToInt32(Constants.InvoiceSentStatus.Unsent);
                invoiceViewModel.CreatedDate = DateTime.Now;
                invoiceViewModel.CreatedBy = Guid.Parse(invoiceViewModel.LoginId.ToString());
                invoiceViewModel.InvoiceStatus = (int)Constants.InvoiceStatus.Submitted;
                invoiceViewModel.OTRWNotes = invoiceViewModel.DesriptionServicesPerformed;
                if (invoiceViewModel.InvoiceDate == null)
                {
                    invoiceViewModel.InvoiceDate = DateTime.Now.Date;
                }
                // mapping viewmodel to entity
                CommonMapper<ViewModels.InvoiceViewModel, Invoice> mapper = new CommonMapper<ViewModels.InvoiceViewModel, Invoice>();
                Invoice invoice = mapper.Mapper(invoiceViewModel);
                invoice.OTRWNotes = invoiceViewModel.DesriptionServicesPerformed;
                invoice.IsApproved = false;
                invoice.ApprovedBy = null;
                invoice.IsDelete = false;
                bool IsNewInvoice = false;
                if (invoiceViewModel.Id == null || invoiceViewModel.Id == Guid.Empty)
                {
                    invoice.Id = Guid.NewGuid();
                    db.Invoice.Add(invoice);
                    IsNewInvoice = true;
                }
                else
                {
                    db.Entry(invoice).State = EntityState.Modified;
                }

                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;

                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " saves invoice details");

                if (IsNewInvoice == true)
                {
                    // adding notifications
                    string sql = @"DECLARE @UserId uniqueidentifier
                                DECLARE @InvoiceId uniqueidentifier ='" + invoice.Id + @"'
                                
                                DECLARE @tmpTableUsers as table
                                (
                                RowId int identity(1,1)
                                ,UserId uniqueidentifier
                                ) 
                                DECLARE @CurrentRowUsers int = 0
                                DECLARE @TotalRowsUsers int = 0 
                                
                                INSERT INTO @tmpTableUsers
                                SELECT EmployeeDetail.EmployeeId
                                from EmployeeDetail
                                INNER JOIN AspNetUserRoles ON AspNetUserRoles.UserId = EmployeeDetail.EmployeeId
                                INNER JOIN AspNetRoles ON AspNetRoles.Id = AspNetUserRoles.RoleId
                                WHERE AspNetRoles.Name like '%admin%' OR AspNetRoles.Name like '%operation%'
                                
                                SET @TotalRowsUsers = @@ROWCOUNT
                                
                                WHILE (@CurrentRowUsers < @TotalRowsUsers)
                                                              
                                BEGIN
                                    SET @CurrentRowUsers+=1
                                    
                                    SELECT @UserId = UserId FROM @tmpTableUsers WHERE RowId = @CurrentRowUsers
                                    
                                    INSERT INTO WebNotifications
                                    (
                                    Id
                                    ,NotificationMessage
                                    ,NotificationType
                                    ,NotificationTypeId
                                    ,UserId
                                    ,IsRead
                                    ,CreatedDate
                                    )
                                    SELECT 
                                       NEWID()
                                      ,'New invoice #" + invoice.InvoiceNo + " has been created on "
                                      + DateTime.Now.ToShortDateString() + @"'
                                      ,'Invoice'
                                      ,@InvoiceId
                                      ,@UserId
                                      ,0
                                      ,GETDATE()
                                END";


                    db.Database.ExecuteSqlCommand(sql);
                }
                result.ResponseList = new List<string> { invoice.Id.ToString() };
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
        [System.Web.Http.Route("api/Invoice/SaveInvoicePayment")]
        public ServiceResponse<string> SaveInvoicePayment(InvoicePaymentVM invoicePaymentVM)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (invoicePaymentVM.InvoiceId == Guid.Empty && !invoicePaymentVM.PaymentAmount.HasValue &&
                    !invoicePaymentVM.PaymentMethod.HasValue)
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "InvoiceId, PaymentMethod and PaymentAmount are Required";
                    return result;
                }
                else if (invoicePaymentVM.InvoiceId == Guid.Empty)
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "InvoiceId is Required";
                    return result;
                }
                else if (!invoicePaymentVM.PaymentMethod.HasValue)
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "PaymentMethod is Required";
                    return result;
                }
                else if (!invoicePaymentVM.PaymentAmount.HasValue)
                {
                    result.ResponseCode = 0;
                    result.ResponseErrorMessage = "PaymentAmount is Required";
                    return result;
                }
                else
                {
                    invoicePaymentVM.Id = Guid.NewGuid();
                    invoicePaymentVM.CreatedDate = DateTime.Now;
                    CommonMapper<InvoicePaymentVM, InvoicePayment> mapper = new CommonMapper<InvoicePaymentVM, InvoicePayment>();
                    InvoicePayment invoicePayment = mapper.Mapper(invoicePaymentVM);
                    db.InvoicePayment.Add(invoicePayment);
                    db.SaveChanges();

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " Invoice paid successfully.");

                    result.ResponseCode = 1;
                    result.ResponseList = new List<string> { "Invoice paid successfully !" };
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

    }
}
