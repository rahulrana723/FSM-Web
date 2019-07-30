using FSM.Core.Entities;
using FSM.Infrastructure;
using FSM.WebAPI.App_Start;
using FSM.WebAPI.Areas.Employee.ViewModels;
using FSM.WebAPI.Common;
using FSM.WebAPI.Models;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace FSM.WebAPI.Areas.Employee.Controller
{
    [System.Web.Http.Authorize]
    public class PurchaseOrderController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        UnityContainer container = new UnityContainer();
        public PurchaseOrderController()
        {
            container = (UnityContainer)UnityConfig.GetConfiguredContainer();
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/PurchaseOrder/CreatePurchaseOrder")]
        public ServiceResponse<string> CreatePurchaseOrder(PurchaseOrderByJobViewModel objPurchaseOrder)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                objPurchaseOrder.ID = Guid.NewGuid();

                CommonMapper<PurchaseOrderByJobViewModel, PurchaseOrderByJob> mapper = new CommonMapper<PurchaseOrderByJobViewModel, PurchaseOrderByJob>();
                PurchaseOrderByJob PurchaseOrderByJobEnity = mapper.Mapper(objPurchaseOrder);
                PurchaseOrderByJobEnity.IsApprove = false;
                PurchaseOrderByJobEnity.ApprovedBy = null;
                PurchaseOrderByJobEnity.IsDelete = false;

                db.PurchaseOrderByJob.Add(PurchaseOrderByJobEnity);
                db.SaveChanges();

                foreach (PurchaseOrderItemsViewModel objPurchaseOrderItemsViewModel in objPurchaseOrder.lstPurchaseOrderItem)
                {
                    objPurchaseOrderItemsViewModel.ID = Guid.NewGuid();
                    objPurchaseOrderItemsViewModel.PurchaseOrderID = objPurchaseOrder.ID;

                    CommonMapper<PurchaseOrderItemsViewModel, PurchaseorderItemJob> mapperItem = new CommonMapper<PurchaseOrderItemsViewModel, PurchaseorderItemJob>();
                    PurchaseorderItemJob PurchaseorderItemJobEnity = mapperItem.Mapper(objPurchaseOrderItemsViewModel);
                    PurchaseorderItemJobEnity.StockID = new Guid();
                    db.PurchaseorderItemJob.Add(PurchaseorderItemJobEnity);
                    db.SaveChanges();
                }

                // adding notifications
                string sql = @"DECLARE @UserId uniqueidentifier
                                DECLARE @PurchaseOrderId uniqueidentifier ='" + PurchaseOrderByJobEnity.ID + @"'
                                
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
                                      ,'New purchase order #" + PurchaseOrderByJobEnity.PurchaseOrderNo + " has been created on "
                                      + DateTime.Now.ToShortDateString() + @"'
                                      ,'PurchaseOrder'
                                      ,@PurchaseOrderId
                                      ,@UserId
                                      ,0
                                      ,GETDATE()
                                END";


                db.Database.ExecuteSqlCommand(sql);

                result.ResponseList = new List<string> { "New purchase order #" + PurchaseOrderByJobEnity.PurchaseOrderNo +
                                        " sent successfully for approval !" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " create purchage order.");
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
        [System.Web.Http.Route("api/PurchaseOrder/UpdatePurchaseOrder")]
        public ServiceResponse<string> UpdatePurchaseOrder(PurchaseOrderByJobViewModel objPurchaseOrder)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                CommonMapper<PurchaseOrderByJobViewModel, PurchaseOrderByJob> mapper = new CommonMapper<PurchaseOrderByJobViewModel, PurchaseOrderByJob>();
                PurchaseOrderByJob PurchaseOrderByJobEnity = mapper.Mapper(objPurchaseOrder);
                db.Entry(PurchaseOrderByJobEnity).State = EntityState.Modified;
                PurchaseOrderByJobEnity.IsApprove = false;
                PurchaseOrderByJobEnity.ApprovedBy = null;
                PurchaseOrderByJobEnity.IsDelete = false;
                db.SaveChanges();

                db.PurchaseorderItemJob.RemoveRange(db.PurchaseorderItemJob.Where(c => c.PurchaseOrderID == objPurchaseOrder.ID));
                db.SaveChanges();

                foreach (PurchaseOrderItemsViewModel objPurchaseOrderItemsViewModel in objPurchaseOrder.lstPurchaseOrderItem)
                {
                    objPurchaseOrderItemsViewModel.ID = Guid.NewGuid();
                    objPurchaseOrderItemsViewModel.PurchaseOrderID = objPurchaseOrder.ID;

                    CommonMapper<PurchaseOrderItemsViewModel, PurchaseorderItemJob> mapperItem = new CommonMapper<PurchaseOrderItemsViewModel, PurchaseorderItemJob>();
                    PurchaseorderItemJob PurchaseorderItemJobEnity = mapperItem.Mapper(objPurchaseOrderItemsViewModel);

                    db.PurchaseorderItemJob.Add(PurchaseorderItemJobEnity);
                    db.SaveChanges();
                }

                // adding notifications
                string sql = @"DECLARE @UserId uniqueidentifier
                                DECLARE @PurchaseOrderId uniqueidentifier ='" + PurchaseOrderByJobEnity.ID + @"'
                                
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
                                      ,'Purchase order #" + PurchaseOrderByJobEnity.PurchaseOrderNo + " has been updated on "
                                      + DateTime.Now.ToShortDateString() + @"'
                                      ,'PurchaseOrder'
                                      ,@PurchaseOrderId
                                      ,@UserId
                                      ,0
                                      ,GETDATE()
                                END";


                db.Database.ExecuteSqlCommand(sql);

                result.ResponseList = new List<string> { "Purchase order #" + PurchaseOrderByJobEnity.PurchaseOrderNo +
                                        " updation successfully sent for approval !" };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " update purchage order.");

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
        [System.Web.Http.Route("api/PurchaseOrder/GetPurchaseOrderByJob")]
        public ServiceResponse<GetPurchaseOrderByJob> GetPurchaseOrderByJob(dynamic json)
        {
            ServiceResponse<GetPurchaseOrderByJob> result = new ServiceResponse<GetPurchaseOrderByJob>();
            List<GetPurchaseOrderByJob> getPurchaseOrderByJobList = new List<GetPurchaseOrderByJob>();
            GetPurchaseOrderByJob getPurchaseOrderByJob = null;
            try
            {
                Guid JobID = new Guid(json.JobID.Value);
                var resultObject = db.PurchaseOrderByJob.Where(m => m.JobID == JobID && (m.IsDelete == false ||m.IsDelete ==null))
                    .Select(m => new PurchaseOrderByJobViewModel
                    {
                        ID = m.ID,
                        SupplierID = m.SupplierID,
                        SupplierName = m.Supplier.Name,
                        PurchaseOrderNo = m.PurchaseOrderNo,
                        Description = m.Description,
                        Cost = m.Cost,
                        JobID = m.JobID,
                        InvoiceId = m.InvoiceId,
                        CreatedBy = m.CreatedBy,
                        ModifiedBy = m.ModifiedBy,
                        CreatedDate = m.CreatedDate,
                        ModifiedDate = m.ModifiedDate,
                        PurchaseorderItemJobs = m.PurchaseorderItemJobs,
                        PONumber= m.PurchaseOrderNo.ToString()
                    })
                        .ToList();

                foreach (var item in resultObject)
                {
                    var purchaseOrderItemsViewModel = item.PurchaseorderItemJobs.Select(m => new PurchaseOrderItemsViewModel
                    {
                        ID = m.ID,
                        PurchaseOrderID = m.PurchaseOrderID,
                        PurchaseItem = m.PurchaseItem,
                        UnitOfMeasure = m.UnitOfMeasure,
                        Quantity = m.Quantity,
                        Price = m.Price,
                        CreatedBy = m.CreatedBy,
                        ModifiedBy = m.ModifiedBy,
                        CreatedDate = m.CreatedDate,
                        ModifiedDate = m.ModifiedDate,
                     
                        
                    }).ToList();

                    CommonMapper<PurchaseOrderByJobViewModel, GetPurchaseOrderByJob> mapper2 = new CommonMapper<PurchaseOrderByJobViewModel, GetPurchaseOrderByJob>();
                    getPurchaseOrderByJob = mapper2.Mapper(item);
                    getPurchaseOrderByJob.PONumber = item.PONumber;
                    getPurchaseOrderByJob.PurchaseorderItemJob = purchaseOrderItemsViewModel;

                    getPurchaseOrderByJobList.Add(getPurchaseOrderByJob);
                }
                result.ResponseList = getPurchaseOrderByJobList;
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get purchage order by job.");

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
        [System.Web.Http.Route("api/PurchaseOrder/GetPurchaseOrders")]
        public ServiceResponse<PurchaseOrderByUserVM> GetPurchaseOrders(dynamic json)
        {
            ServiceResponse<PurchaseOrderByUserVM> result = new ServiceResponse<PurchaseOrderByUserVM>();
            try
            {
                Guid UserId = new Guid(json.UserId.Value);
                IQueryable<PurchaseOrderByUserVM> resultObject = db.PurchaseOrderByJob.Where(m => m.CreatedBy == UserId && m.IsDelete == false).
                                        Select(m => new PurchaseOrderByUserVM
                                        {
                                            Id = m.ID.ToString(),
                                            JobId = m.Jobs != null ? m.Jobs.JobId.ToString() : "",
                                            JobKey = m.Jobs != null ? m.Jobs.Id.ToString() : "",
                                            PO = m.PurchaseOrderNo.ToString(),
                                            SupplierName = m.Supplier != null ? m.Supplier.Name : ""
                                        }).AsQueryable();

                result.ResponseList = resultObject.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get purchage order.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.Route("api/PurchaseOrder/GetStockLists")]
        public ServiceResponse<StockListViewModel> GetStockLists()
        {
            ServiceResponse<StockListViewModel> result = new ServiceResponse<StockListViewModel>();
            try
            {
                StockListViewModel stockLists = new StockListViewModel();
                stockLists.Stock = db.Stock.Where(m => m.IsDelete == false).Select(m => new SelectListItem { Text = m.Label, Value = m.ID.ToString() }).ToList();
                stockLists.Stock.OrderBy(m => m.Text);

                result.ResponseList = new List<StockListViewModel>();
                result.ResponseList.Add(stockLists);
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "null";
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get stock list.");

                return result;
            }
            catch (Exception ex)
            {
                result.ResponseCode = 0;
                result.ResponseErrorMessage = ex.Message + " " + ex.InnerException;
                return result;
            }
        }

        [System.Web.Http.Route("api/PurchaseOrder/GetSupplierLists")]
        public ServiceResponse<SupplierListViewModel> GetSupplierLists()
        {
            ServiceResponse<SupplierListViewModel> result = new ServiceResponse<SupplierListViewModel>();
            try
            {
                SupplierListViewModel supplierLists = new SupplierListViewModel();
                supplierLists.SupplierName = db.Supplier.Select(m => new SelectListItem { Text = m.Name, Value = m.ID.ToString() }).ToList();
                supplierLists.SupplierName.OrderBy(m => m.Text);

                result.ResponseList = new List<SupplierListViewModel>();
                result.ResponseList.Add(supplierLists);
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "null";
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get supplier list.");

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
        [System.Web.Http.Route("api/PurchaseOrder/DeletePurchaseOrder")]
        public ServiceResponse<string> DeletePurchaseOrder(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid PurchaseOrderId = new Guid(json.PurchaseOrderId.Value);
                string message = "PO doesn't exist !";

                var purchaseOrder = db.PurchaseOrderByJob.Find(PurchaseOrderId);
                var purchaseOrderItems = db.PurchaseorderItemJob.Where(m => m.PurchaseOrderID == PurchaseOrderId).ToList();

                // removing all purchase order items
                if (purchaseOrderItems.Count > 0)
                {
                    db.PurchaseorderItemJob.RemoveRange(purchaseOrderItems);
                    db.SaveChanges();
                }

                // removing purchase order
                if (purchaseOrder != null)
                {
                    db.PurchaseOrderByJob.Remove(purchaseOrder);
                    db.SaveChanges();

                    message = "PO " + purchaseOrder.PurchaseOrderNo + " deleted successfully !";
                }

                result.ResponseList = new List<string>() { message };
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "null";
                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " delete purchage order.");
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
        [System.Web.Http.Route("api/PurchaseOrder/GetJobsForPurchaseOrders")]
        public ServiceResponse<PurchaseOrderVM> GetJobsForPurchaseOrders(dynamic json)
        {
            ServiceResponse<PurchaseOrderVM> result = new ServiceResponse<PurchaseOrderVM>();
            try
            {
                Guid UserId = new Guid(json.UserId.Value);
                string sql = @"select distinct PurchaseOrderByJob.JobID 'JobId' ,Jobs.JobNo 'JobKey'
                               ,Jobs.JobType from PurchaseOrderByJob inner join Jobs on Jobs.Id = PurchaseOrderByJob.JobID 
                                inner join JobAssignToMapping on JobAssignToMapping.JobId= Jobs.Id where JobAssignToMapping.AssignTo 
                                = '" + UserId + "'";

                IQueryable<PurchaseOrderVM> resultObject = db.Database.SqlQuery<PurchaseOrderVM>(sql).AsQueryable();

                result.ResponseList = resultObject.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = null;

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get job for purchage order.");

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
