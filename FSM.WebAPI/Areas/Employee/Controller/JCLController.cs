using FSM.Core.Entities;
using FSM.Infrastructure;
using FSM.WebAPI.Areas.Employee.ViewModels;
using FSM.WebAPI.Common;
using FSM.WebAPI.Models;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FSM.WebAPI.Areas.Employee.Controller
{
   [System.Web.Http.Authorize]
    public class JCLController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();
        UnityContainer container = new UnityContainer();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/JCL/GetJCLItemList")]
        public ServiceResponse<JCLItemViewModel> GetJCLItemList(dynamic json)
        {
            ServiceResponse<JCLItemViewModel> result = new ServiceResponse<JCLItemViewModel>();
            try
            {
                IQueryable<JCLItemViewModel> lstJCLList = from jcl in db.JCL
                                                          where jcl.IsDelete == false
                                                          select new JCLItemViewModel
                                                          {
                                                              JCLId = jcl.JCLId,
                                                              ItemName = jcl.ItemName,
                                                              Price = jcl.Price,
                                                              Quantity = jcl.DefaultQty,
                                                              description = jcl.Description
                                                          };

                result.ResponseList = lstJCLList.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get list of JCL items.");

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
        [System.Web.Http.Route("api/JCL/GetColorsListByJCLId")]
        public ServiceResponse<JCLColorViewModel> GetColorsListByJCLId(dynamic json)
        {
            Guid JclId = Guid.Parse(json.JCLId.Value);
            ServiceResponse<JCLColorViewModel> result = new ServiceResponse<JCLColorViewModel>();
            try
            {
                IQueryable<JCLColorViewModel> lstJCLList = from jclcolor in db.JCLColor_Mapping
                                                           where jclcolor.JCLId == JclId && jclcolor.IsDelete == false
                                                           select new JCLColorViewModel
                                                           {
                                                               Id = jclcolor.ColorId,
                                                               JCLId = jclcolor.JCLId,
                                                               ColorName = jclcolor.ColorName
                                                           };

                result.ResponseList = lstJCLList.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get color list by JCL id.");

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
        [System.Web.Http.Route("api/JCL/GetSizeListByJCLId")]
        public ServiceResponse<JCLSizeViewModel> GetSizeListByJCLId(dynamic json)
        {
            Guid JclId = Guid.Parse(json.JCLId.Value);
            ServiceResponse<JCLSizeViewModel> result = new ServiceResponse<JCLSizeViewModel>();
            try
            {
                IQueryable<JCLSizeViewModel> lstJCLList = from jclsize in db.JCLSize_Mapping
                                                          where jclsize.JCLId == JclId && jclsize.IsDelete == false
                                                          select new JCLSizeViewModel
                                                          {
                                                              Id = jclsize.SizeId,
                                                              JCLId = jclsize.JCLId,
                                                              Size = jclsize.Size
                                                          };

                result.ResponseList = lstJCLList.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get size list by JCL id.");

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
        [System.Web.Http.Route("api/JCL/GetProductsListByJCLId")]
        public ServiceResponse<JCLProductsViewModel> GetProductsListByJCLId(dynamic json)
        {
            Guid JclId = Guid.Parse(json.JCLId.Value);
            ServiceResponse<JCLProductsViewModel> result = new ServiceResponse<JCLProductsViewModel>();
            try
            {
                IQueryable<JCLProductsViewModel> lstJCLList = from jclproducts in db.JCLProducts_Mapping
                                                              where jclproducts.JCLId == JclId && jclproducts.IsDelete == false
                                                              select new JCLProductsViewModel
                                                              {
                                                                  Id = jclproducts.ProductId,
                                                                  JCLId = jclproducts.JCLId,
                                                                  ProductName = jclproducts.ProductName
                                                              };

                result.ResponseList = lstJCLList.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get product list by JCL id.");

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
        [System.Web.Http.Route("api/JCL/GetExtraProductListByJCLId")]
        public ServiceResponse<JCLExtraViewModel> GetExtraProductListByJCLId(dynamic json)
        {
            Guid JclId = Guid.Parse(json.JCLId.Value);
            ServiceResponse<JCLExtraViewModel> result = new ServiceResponse<JCLExtraViewModel>();
            try
            {
                IQueryable<JCLExtraViewModel> lstJCLList = from jclextraa in db.JcLExtraproductMapping
                                                           where jclextraa.JCLId == JclId && jclextraa.IsDelete == false
                                                           select new JCLExtraViewModel
                                                           {
                                                               Id = jclextraa.ProductId,
                                                               JCLId = jclextraa.JCLId,
                                                               ExtraProduct = jclextraa.ProductName
                                                           };

                result.ResponseList = lstJCLList.ToList();
                result.ResponseCode = 1;
                result.ResponseErrorMessage = "";

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " get color list by JCL id.");

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
        [System.Web.Http.Route("api/JCL/AddInvoiceJCLItems")]

        public ServiceResponse<string> AddInvoiceJCLItems(JCLInvoiceITemsModel objJCLItems)
        {
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                foreach (JCLInvoiceITems jclitem in objJCLItems.JcLinvoiceitems)
                {
                    jclitem.ID = Guid.NewGuid();
                    jclitem.CreateDate = DateTime.Now;
                    if (jclitem.InvoiceId == null)
                    {
                        var isinvoiceexist = db.InvoicedJCLItemMapping.Where(i => i.JobID == jclitem.JobID).ToList();
                        if (isinvoiceexist.Count() > 0)
                        {
                            var invoiceDetail = isinvoiceexist.Where(i => i.InvoiceId != null).FirstOrDefault();
                            if (invoiceDetail != null)
                            {
                                jclitem.InvoiceId = invoiceDetail.InvoiceId;
                            }
                        }
                    }
                    CommonMapper<JCLInvoiceITems, InvoicedJCLItemMapping> mapper = new CommonMapper<JCLInvoiceITems, InvoicedJCLItemMapping>();
                    InvoicedJCLItemMapping JCLITem = mapper.Mapper(jclitem);
                    db.InvoicedJCLItemMapping.Add(JCLITem);
                    db.SaveChanges();

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " add invoice JCL items.");

                }
                result.ResponseList = new List<string> { "Jcl Invoice Items added successfully" };
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
        [System.Web.Http.Route("api/JCL/DeleteInvoiceJCLItem")]

        public ServiceResponse<string> DeleteInvoiceJCLItem(dynamic json)
        {
            Guid Id = Guid.Parse(json.ID.Value);
            ServiceResponse<string> result = new ServiceResponse<string>();
            try
            {
                if (Id != Guid.Empty)
                {
                    var ItemtoDelete = db.InvoicedJCLItemMapping.Where(i => i.ID == Id).FirstOrDefault();
                    if (ItemtoDelete != null)
                    {
                        db.InvoicedJCLItemMapping.Remove(ItemtoDelete);
                        db.SaveChanges();
                        result.ResponseList = new List<string> { "Jcl Invoice Item Deleted successfully" };
                        result.ResponseCode = 1;
                        result.ResponseErrorMessage = null;
                        return result;
                    }
                    result.ResponseList = new List<string> { "Jcl Invoice Item does not Exist" };
                    result.ResponseCode = 2;
                    result.ResponseErrorMessage = null;

                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " delete invoice by JCL item.");

                    return result;
                }
                result.ResponseList = new List<string> { "Jcl Invoice Item can't deleted .Please try later" };
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



        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/JCL/GetInvoiceJCLItemByJob")]

        public ServiceResponse<JCLInvoiceITems> GetInvoiceJCLItemByJob(dynamic json)
        {
            ServiceResponse<JCLInvoiceITems> result = new ServiceResponse<JCLInvoiceITems>();
            try
            {
                Guid JobID = new Guid(json.JobID.Value);
                int? jobNo = db.Jobs.Where(m => m.Id == JobID).Select(m=>m.JobNo).FirstOrDefault();
                if (JobID != Guid.Empty)
                {
                    var jclitems = from Intm in db.InvoicedJCLItemMapping
                                   join j in db.JCL on Intm.JCLItemID equals j.JCLId
                                         into Lj
                                   from j in Lj.DefaultIfEmpty()                                    //Left Join For JCL 
                                   join cm in db.JCLColor_Mapping on Intm.ColorID equals cm.ColorId
                                         into Lcm
                                   from cm in Lcm.DefaultIfEmpty()                                    //Left Join For JCLColor 
                                   join sm in db.JCLSize_Mapping on Intm.SizeID equals sm.SizeId
                                    into Lsm
                                   from sm in Lsm.DefaultIfEmpty()                                    //Left Join For JCLSize
                                   join pm in db.JCLProducts_Mapping on Intm.ProductStyleID equals pm.ProductId
                                    into Lpm
                                   from pm in Lpm.DefaultIfEmpty()
                                   join em in db.JcLExtraproductMapping on Intm.ExtraID equals em.ProductId
                                        into Lem
                                   from em in Lem.DefaultIfEmpty()               //Left Join For JCLExtra
                                   join jobs in db.Jobs on Intm.JobID equals jobs.Id
                                   into jb
                                   from jobs in jb.DefaultIfEmpty()
                                   where jobs.JobNo == jobNo
                                   select new JCLInvoiceITems
                                   {
                                       ID = Intm.ID,
                                       JCLItemID = Intm.JCLItemID,
                                       ColorID = Intm.ColorID,
                                       SizeID = Intm.SizeID,
                                       ProductStyleID = Intm.ProductStyleID,
                                       ExtraID= Intm.ExtraID,
                                       JobID = Intm.JobID,
                                       Quantity = Intm.Quantity,
                                       description = Intm.Description,
                                       Price = Intm.Price,
                                       TotalPrice = Intm.TotalPrice,
                                       CreatedBy = Intm.CreatedBy,
                                       ModifiedBy = Intm.ModifiedBy,
                                       CreateDate = Intm.CreateDate,
                                       ModifiedDate = Intm.ModifiedDate,
                                       ItemName = j.ItemName,
                                       ColorName = cm.ColorName,
                                       Size = sm.Size,
                                       ProductName = pm.ProductName,
                                       ExtraProduct=em.ProductName,
                                       InvoiceId = Intm.InvoiceId
                                   };
                    result.ResponseList = jclitems.Select(m => new JCLInvoiceITems
                    {
                        ID = m.ID,
                        JCLItemID = m.JCLItemID,
                        ColorID = m.ColorID,
                        SizeID = m.SizeID,
                        ProductStyleID = m.ProductStyleID,
                        ExtraID=m.ExtraID,
                        JobID = m.JobID,
                        Quantity = m.Quantity,
                        description = m.description,
                        Price = m.Price,
                        TotalPrice = m.TotalPrice,
                        CreatedBy = m.CreatedBy,
                        ModifiedBy = m.ModifiedBy,
                        CreateDate = m.CreateDate,
                        ModifiedDate = m.ModifiedDate,
                        ItemName = m.ItemName,
                        ColorName = m.ColorName,
                        Size = m.Size,
                        ProductName = m.ProductName,
                        ExtraProduct=m.ExtraProduct,
                        InvoiceId = m.InvoiceId
                    }).ToList();


                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = null;


                    ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                    response = CommonFunctions.GetUserInfoByToken();
                    string userId = response.ResponseList[0].UserId;
                    string userName = response.ResponseList[0].UserName;


                    log4net.ThreadContext.Properties["UserId"] = userId;
                    log.Info(userName + " get invoice JCL item by job.");

                    return result;
                }
                else
                {
                    result.ResponseList = null;
                    result.ResponseCode = 1;
                    result.ResponseErrorMessage = "Please provide jobID";
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
