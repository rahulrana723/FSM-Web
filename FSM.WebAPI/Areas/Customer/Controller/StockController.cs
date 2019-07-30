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
using FSM.WebAPI.Areas.Customer.ViewModels;
using FSM.WebAPI.Models;
using log4net;

namespace FSM.WebAPI.Areas.Customer.Controller
{
    [System.Web.Http.Authorize]
    public class StockController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        // GET: Customer/OTRWStock
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/stock/GetOTRWStockList")]
        public ServiceResponse<OTRWStockListViewModel> GetOTRWStockList(dynamic json)
        {
            Guid otrwID = new Guid(json.OTRWId.Value);
            ServiceResponse<OTRWStockListViewModel> result = new ServiceResponse<OTRWStockListViewModel>();
            try
            {
                IQueryable<OTRWStockListViewModel> lstOTRWStock = from otrwStock in db.OTRWStock
                                                                  join stock in db.Stock on otrwStock.StockID equals stock.ID
                                                                  where otrwStock.OTRWID == otrwID && (otrwStock.IsDelete == false || otrwStock.IsDelete == null)
                                                                  && (stock.IsDelete == false || stock.IsDelete == null)
                                                                  select new OTRWStockListViewModel
                                                                  {
                                                                      Id = otrwStock.ID,
                                                                      StockId = otrwStock.StockID,
                                                                      StockName = stock.Label,
                                                                      UnitMeasure = otrwStock.UnitMeasure,
                                                                      Quantity = otrwStock.Quantity,
                                                                  };

                result.ResponseList = lstOTRWStock.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get OTRW stock list.");

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
        [System.Web.Http.Route("api/stock/InsertOTRWStockIntoJobStock")]
        public ServiceResponse<string> InsertOTRWStockIntoJobStock(JobStockViewModel objJobStock)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {

                //Insert Into Job Stock
                objJobStock.Id = Guid.NewGuid();
                objJobStock.CreatedDate = DateTime.Now;
                objJobStock.IsDelete = false;

                CommonMapper<JobStockViewModel, JobStock> mapper = new CommonMapper<JobStockViewModel, JobStock>();
                JobStock jobStockEnity = mapper.Mapper(objJobStock);

                db.JobStock.Add(jobStockEnity);
                db.SaveChanges();

                //Update OTRW Stock Quantity
                OTRWStock oTRWStockEntity = db.OTRWStock.Where(m => m.StockID == objJobStock.StockId && m.OTRWID == objJobStock.UserId).FirstOrDefault();
                oTRWStockEntity.Quantity = oTRWStockEntity.Quantity - objJobStock.Quantity;

                db.Entry(oTRWStockEntity).State = EntityState.Modified;
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " insert OTRW stock into job stock.");

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
        [System.Web.Http.Route("api/stock/InsertStockIntoJobStock")]
        public ServiceResponse<string> InsertStockIntoJobStock(JobStockViewModel objJobStock)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {

                //Insert Into Job Stock
                objJobStock.Id = Guid.NewGuid();
                objJobStock.CreatedDate = DateTime.Now;
                objJobStock.IsDelete = false;

                CommonMapper<JobStockViewModel, JobStock> mapper = new CommonMapper<JobStockViewModel, JobStock>();
                JobStock jobStockEnity = mapper.Mapper(objJobStock);

                db.JobStock.Add(jobStockEnity);
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " insert stock into job stock.");

                //Update OTRW Stock Quantity
                Stock StockEntity = db.Stock.Where(m => m.ID == objJobStock.StockId).FirstOrDefault();
                StockEntity.Quantity = StockEntity.Quantity - objJobStock.Quantity;

                db.Entry(StockEntity).State = EntityState.Modified;
                db.SaveChanges();


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
        [System.Web.Http.Route("api/stock/DeleteJobStock")]
        public ServiceResponse<string> DeleteJobStock(dynamic json)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                Guid JobStockId = new Guid(json.Id.Value);
                Guid UserId = new Guid(json.UserId.Value);

                JobStock jobStockEntity = db.JobStock.Where(m => m.ID == JobStockId).FirstOrDefault();

                if (jobStockEntity.AssignedFrom == "OTRW")
                {
                    OTRWStock oTRWStockEntity = db.OTRWStock.Where(m => m.StockID == jobStockEntity.StockID && m.OTRWID == UserId).FirstOrDefault();
                    oTRWStockEntity.Quantity = oTRWStockEntity.Quantity + jobStockEntity.Quantity;

                    db.Entry(oTRWStockEntity).State = EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {
                    Stock StockEntity = db.Stock.Where(m => m.ID == jobStockEntity.StockID).FirstOrDefault();
                    StockEntity.Quantity = StockEntity.Quantity + jobStockEntity.Quantity;

                    db.Entry(StockEntity).State = EntityState.Modified;
                    db.SaveChanges();

                }

                jobStockEntity.IsDelete = true;
                db.Entry(jobStockEntity).State = EntityState.Modified;
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " delete job stock.");

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
    }
}