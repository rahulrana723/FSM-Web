using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using log4net;
using Microsoft.Practices.Unity;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Contracts.Version2.Contact;
using MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger;
using MYOB.AccountRight.SDK.Contracts.Version2.Purchase;
using MYOB.AccountRight.SDK.Contracts.Version2.Sale;
using MYOB.AccountRight.SDK.Services;
using MYOB.AccountRight.SDK.Services.GeneralLedger;
using MYOB.AccountRight.SDK.Services.Purchase;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FSM.Web.Areas.Employee.Controllers
{
    public class PurchaseController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                 ().DeclaringType);
        [Dependency]
        public IStockRepository Stock { get; set; }

        [Dependency]
        public ISupplier Supplier { get; set; }

        [Dependency]
        public IPurchaseorderItemJobRepository PurchaseOrderitemJob { get; set; }
        [Dependency]
        public IPurchaseOrderByJobRepository PurchaseOrderJob { get; set; }
        [Dependency]
        public IPurchaseorderItemsStock PurchaseStock { get; set; }
        [Dependency]
        public IPurchaseOrderByStock PurchaseStockorder { get; set; }
        [Dependency]
        public IEmployeeJobRepository EmployeeJob { get; set; }
        [Dependency]
        public IiNoviceRepository InvoiceRep { get; set; }
        [Dependency]
        public IEmployeeDetailRepository EmployeeRepo { get; set; }
        
        [Dependency]
        public IiNvoicePaymentRepository InvoicePaymentRepo { get; set; }
        [Dependency]
        public IPurchaseorderItemJobRepository JobPurchaseOrderitem { get; set; }
        [Dependency]
        public IEmployeeJobRepository JobRepository { get; set; }

        public string _Apiode = "";
        //GET:Employee/Purchase
        public ActionResult Index()
        {
            return View();
        }

        //GET:Employee/Purchase/ViewPurchaseOrder
        /// <summary>
        /// Show list Of Purchase Order
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ViewPurchaseOrder()
        {
            try
            {
                using (PurchaseStockorder)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    var purchaseorders = PurchaseStockorder.GetStockPurchaseOrders(Searchstring);
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    CommonMapper<FSM.Core.ViewModels.PurchaseOrderByStockCoreViewModel, PurchaseOrderByStockViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseOrderByStockCoreViewModel, PurchaseOrderByStockViewModel>();
                    List<PurchaseOrderByStockViewModel> purchaseOrderByStockViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    PurcahseOrderListviewmodel model = new PurcahseOrderListviewmodel
                    {
                        Purchaseorderviewmodel = purchaseOrderByStockViewModel,
                        Purchasesearchorderviewmodel = new PurchaseorderSearchViewmodel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };
                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Purchase/ViewPurchaseOrder
        /// <summary>
        /// Search Record In Purchase Order
        /// </summary>
        /// <param name="purchaseorderSearchViewmodel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult ViewPurchaseOrder(PurchaseorderSearchViewmodel purchaseorderSearchViewmodel)
        {
            try
            {
                using (PurchaseStockorder)
                {
                    string Searchstring = purchaseorderSearchViewmodel.SearchKeyword;
                    //var stocks =Stock.GetAll();
                    var purchaseorders = PurchaseStockorder.GetStockPurchaseOrders(Searchstring);
                    CommonMapper<FSM.Core.ViewModels.PurchaseOrderByStockCoreViewModel, PurchaseOrderByStockViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseOrderByStockCoreViewModel, PurchaseOrderByStockViewModel>();
                    List<PurchaseOrderByStockViewModel> purchaseOrderByStockViewModel = mapper.MapToList(purchaseorders.ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);

                    PurcahseOrderListviewmodel model = new PurcahseOrderListviewmodel
                    {
                        Purchaseorderviewmodel = purchaseOrderByStockViewModel,
                        Purchasesearchorderviewmodel = new PurchaseorderSearchViewmodel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of purchase order.");

                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET:Employee/Purchase/ViewJobspurchaseOrder
        /// <summary>
        /// Show list Of Jobs Purchase Order
        /// </summary>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ViewJobspurchaseOrder()
        {
            try
            {
                using (PurchaseOrderJob)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    var purchaseorders = PurchaseOrderJob.GetjobPurchaseOrders(Searchstring);
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };
                    return View(model);
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Purchase/ViewJobspurchaseOrder
        /// <summary>
        /// Search Record In Job Purchase Order
        /// </summary>
        /// <param name="purchaseOrderjobsearchviewModel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult ViewJobspurchaseOrder(PurchaseOrderjobsearchviewModel purchaseOrderjobsearchviewModel)
        {
            try
            {
                using (PurchaseOrderJob)
                {
                    string Searchstring = purchaseOrderjobsearchviewModel.SearchKeyword;
                    //var stocks =Stock.GetAll();
                    var purchaseorders = PurchaseOrderJob.GetjobPurchaseOrders(Searchstring);
                    CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaserOrderByJobCoreViewModel, PurchaseOrderByJobviewmodel>();
                    List<PurchaseOrderByJobviewmodel> purchaseOrderByjobViewModel = mapper.MapToList(purchaseorders.OrderByDescending(i => i.PurchaseOrderNo).ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    PurchaseOrderjobListviewModel model = new PurchaseOrderjobListviewModel
                    {
                        PurchaseorderjobViewmodel = purchaseOrderByjobViewModel,
                        Purchasejobsearchorderviewmodel = new PurchaseOrderjobsearchviewModel() { PageSize = PageSize, SearchKeyword = Searchstring }
                    };

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

        //POST:Employee/Purchase/DeleteJobPurchaseorder
        /// <summary>
        /// Delete JobPurchaseOrder Record
        /// </summary>
        /// <param name="jobid"></param>
        /// <returns>Redirect ViewJobspurchaseOrder</returns>
        public ActionResult DeleteJobPurchaseorder(string purchaserId)
        {
            try
            {
                Guid PurchaseId = Guid.Parse(purchaserId);
                string PurchaseorderId = "";

                var purchaseorder = PurchaseOrderJob.FindBy(i => i.ID == PurchaseId).FirstOrDefault();
                if (purchaseorder != null)
                {
                    PurchaseorderId = purchaseorder.ID.ToString();
                }

                if (!string.IsNullOrEmpty(PurchaseorderId))
                {
                    Guid purchaseId = Guid.Parse(PurchaseorderId);
                    var purchaseorderitems = PurchaseOrderitemJob.FindBy(i => i.PurchaseOrderID == purchaseId).ToList();
                    if (purchaseorderitems != null && purchaseorderitems.Count > 0)
                    {
                        foreach (var pitem in purchaseorderitems)
                        {
                            PurchaseOrderitemJob.Delete(pitem);
                            PurchaseOrderitemJob.Save();
                        }
                    }
                    var porder = PurchaseOrderJob.FindBy(i => i.ID == purchaseId).FirstOrDefault();
                    porder.IsDelete = true;
                    PurchaseOrderJob.Edit(porder);
                    PurchaseOrderJob.Save();
                    TempData["Message"] = "3";
                }
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted job purchase order.");

                return RedirectToAction("ViewJobspurchaseOrder");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                PurchaseOrderJob.Dispose();
            }
        }

        //GET:Employee/Purchase/AddEditPurchaseOrder
        /// <summary>
        /// Add New Purchase Order
        /// </summary>
        /// <param name="Purchaseorderid"></param>
        /// <returns>Model</returns>
        public ActionResult AddEditPurchaseOrder(string Purchaseorderid)

        {
           
            List<SupplierItem> Supplierlist = new List<SupplierItem>();
            using (Supplier)
            {
                var suppliers = Supplier.GetAll();
                foreach (var supplier in suppliers)
                {
                    SupplierItem obj = new SupplierItem();
                    obj.ID = supplier.ID;
                    obj.Name = supplier.Name;
                    Supplierlist.Add(obj);
                }
            }

            using (Stock)
            {
                var stocks = Stock.GetAll();
                List<StockList> stocklist = new List<StockList>();
                foreach (var stock in stocks)
                {
                    StockList sitem = new StockList();
                    sitem.StockId = stock.ID;
                    sitem.StockName = stock.Label;
                    stocklist.Add(sitem);
                }

                StockPurChaseViewModel model = new StockPurChaseViewModel
                {
                    PurchaseOrderByStockViewModel = new PurchaseOrderByStockViewModel(),
                    PurchaseOrderITemByStockViewModel = new PurchaseOrderITemByStockViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    model.PurchaseOrderByStockViewModel.ID = Guid.Parse(Purchaseorderid);
                }
                model.PurchaseOrderITemByStockViewModel.Stocklist = stocklist.OrderBy(i => i.StockName).ToList();
                model.PurchaseOrderByStockViewModel.SupplierList = Supplierlist.OrderBy(i => i.Name).ToList();

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " added purchase order.");

                return View(model);
            }
        }


        [HttpGet]
        public ActionResult GetPurchaseItemByOrderId(string Id)
        {
            try
            {
                using (PurchaseStock)
                {
                    var purchaseitem = PurchaseStock.GetPurchaseitembyPurchaseorder(Id);
                    CommonMapper<FSM.Core.ViewModels.PurchaseDataCoreviewModel, PurchaseDataViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseDataCoreviewModel, PurchaseDataViewModel>();
                    List<PurchaseDataViewModel> purchaseitemviewmodel = mapper.MapToList(purchaseitem.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(purchaseitemviewmodel);


                    return Json(new { list = json, length = purchaseitemviewmodel.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //POST:Employee/Purchase/DeletePurchaseorder
        /// <summary>
        /// Delete PurchaseOrder Record
        /// </summary>
        /// <param name="Purchaseid"></param>
        /// <returns>Redirect ViewPurchaseOrder</returns>
        [HttpGet]
        public ActionResult DeletePurchaseorder(string Purchaseid)
        {
            try
            {
                Dictionary<Guid, int> DeletedStock = new Dictionary<Guid, int>();
                using (PurchaseStock)
                {
                    Guid orderid = Guid.Parse(Purchaseid);
                    var stocksitem = PurchaseStock.FindBy(i => i.PurchaseOrderID == orderid).ToList();
                    foreach (var item in stocksitem)
                    {
                        DeletedStock.Add(item.StockID, Convert.ToInt32(item.Quantity));
                        PurchaseStock.Delete(item);
                        PurchaseStock.Save();
                    }
                }

                using (PurchaseStockorder)
                {
                    Guid orderid = Guid.Parse(Purchaseid);
                    var stocksitem = PurchaseStockorder.FindBy(i => i.ID == orderid).FirstOrDefault();
                    PurchaseStockorder.Delete(stocksitem);
                    PurchaseStockorder.Save();
                }
                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted purchase order.");
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                TempData["Message"] = 0;
                throw;
            }
        }

        [HttpPost]
        public ActionResult SavePurchaseData(List<FSM.Core.ViewModels.PurchaseDataCoreviewModel> values)
        {
            try
            {
                Dictionary<Guid, int> dictionarystock = new Dictionary<Guid, int>();
                Dictionary<Guid, string> dictionaryItemstopurchase = new Dictionary<Guid, string>();

                string Purchaseid = values[0].purchaseId;
                PurchaseOrderByStockViewModel purchaseOrderByStockViewModel = new PurchaseOrderByStockViewModel();
                #region Add-Edit Purchaseorderstock
                using (PurchaseStockorder)
                {
                    if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                    {
                        Guid id = Guid.Parse(Purchaseid);
                        var purchasestock = PurchaseStockorder.FindBy(i => i.ID == id).FirstOrDefault();
                        purchasestock.supplierID = Guid.Parse(values[0].Supplierid);
                        purchasestock.Description = values[0].Description;
                        purchasestock.Cost = Convert.ToDecimal(values[0].Cost);
                        purchasestock.ModifiedBy = Guid.Parse(base.GetUserId);
                        purchasestock.ModifedDate = DateTime.Now;
                        PurchaseStockorder.Edit(purchasestock);
                        PurchaseStockorder.Save();
                        TempData["Message"] = 2;
                    }
                    else
                    {
                        purchaseOrderByStockViewModel.ID = Guid.NewGuid();
                        purchaseOrderByStockViewModel.supplierID = Guid.Parse(values[0].Supplierid);
                        purchaseOrderByStockViewModel.Description = values[0].Description;
                        purchaseOrderByStockViewModel.Cost = Convert.ToDecimal(values[0].Cost);
                        purchaseOrderByStockViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        purchaseOrderByStockViewModel.CreatedDate = DateTime.Now;
                        CommonMapper<PurchaseOrderByStockViewModel, PurchaseOrderByStock> mapper = new CommonMapper<PurchaseOrderByStockViewModel, PurchaseOrderByStock>();
                        PurchaseOrderByStock purchasestockinfo = mapper.Mapper(purchaseOrderByStockViewModel);
                        PurchaseStockorder.Add(purchasestockinfo);
                        PurchaseStockorder.Save();
                        TempData["Message"] = 1;
                    }
                }
                #endregion

                #region Add-Edit Purchaseitemstock
                using (PurchaseStock)
                {
                    foreach (var stockitem in values)
                    {
                        Guid stockid = Guid.Parse(stockitem.StockId);
                        Guid itempurchasdId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                        var checkExist = PurchaseStock.FindBy(i => i.StockID == stockid && i.PurchaseOrderID == itempurchasdId).FirstOrDefault();
                        if (checkExist != null)
                        {
                            var itemtoupdate = checkExist;
                            itemtoupdate.UnitOfMeasure = Convert.ToString(stockitem.UnitMeasure);
                            itemtoupdate.Price = Convert.ToDecimal(stockitem.Price);
                            itemtoupdate.Quantity = Convert.ToInt32(stockitem.Quantity);
                            itemtoupdate.ModifiedBy = Guid.Parse(base.GetUserId);
                            itemtoupdate.ModifiedDate = DateTime.Now;
                            PurchaseStock.Edit(itemtoupdate);
                            PurchaseStock.Save();
                        }
                        else
                        {
                            PurchaseOrderITemByStockViewModel purchaseOrderITemByStockViewModel = new PurchaseOrderITemByStockViewModel();
                            purchaseOrderITemByStockViewModel.ID = Guid.NewGuid();
                            if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                            {
                                purchaseOrderITemByStockViewModel.PurchaseOrderID = Guid.Parse(Purchaseid);
                            }
                            else { purchaseOrderITemByStockViewModel.PurchaseOrderID = purchaseOrderByStockViewModel.ID; }
                            purchaseOrderITemByStockViewModel.StockID = Guid.Parse(stockitem.StockId);
                            purchaseOrderITemByStockViewModel.UnitOfMeasure = stockitem.UnitMeasure;
                            purchaseOrderITemByStockViewModel.Quantity = Convert.ToInt32(stockitem.Quantity);
                            purchaseOrderITemByStockViewModel.Price = Convert.ToDecimal(stockitem.Price);
                            purchaseOrderITemByStockViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            purchaseOrderITemByStockViewModel.CreatedDate = DateTime.Now;
                            CommonMapper<PurchaseOrderITemByStockViewModel, PurchaseorderItemsStock> mapper = new CommonMapper<PurchaseOrderITemByStockViewModel, PurchaseorderItemsStock>();
                            PurchaseorderItemsStock purchasestockiteminfo = mapper.Mapper(purchaseOrderITemByStockViewModel);
                            PurchaseStock.Add(purchasestockiteminfo);
                            PurchaseStock.Save();
                        }
                    }
                    #region Delete item on update 
                    Guid itemId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                    var items = PurchaseStock.FindBy(i => i.PurchaseOrderID == itemId).ToList();

                    foreach (var item in items)
                    {
                        dictionaryItemstopurchase.Add(item.StockID, item.Quantity.ToString());
                        string itemid = Convert.ToString(item.StockID.ToString());
                        bool pos = Array.Exists(values.ToArray(), element => element.StockId == itemid);
                        if (!pos)
                        {
                            var stockid = Convert.ToString(item.StockID.ToString());
                            if (!String.IsNullOrEmpty(stockid))
                            {
                                var stock_id = Guid.Parse(stockid);
                                dictionarystock.Add(stock_id, Convert.ToInt32(item.Quantity));
                                var itemtodelete = PurchaseStock.FindBy(i => i.StockID == stock_id).FirstOrDefault();
                                PurchaseStock.Delete(itemtodelete);
                                PurchaseStock.Save();
                            }
                        }
                    }
                }
                #endregion
                #endregion

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " saved purchase data.");

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                TempData["Message"] = 0;
                throw;
            }
        }
        public ActionResult AddEditJobPurchaseOrder(string Purchaseorderid)
        {
            List<SelectListItem> JobList = new List<SelectListItem>();
            List<SelectListItem> QuoteDoJobList = new List<SelectListItem>();
            List<SelectListItem> QuoteJobList = new List<SelectListItem>();
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
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                }
                model.PurchaseOrderITemByJobViewModel.StockJoblist = StockJobList.OrderBy(i => i.StockName).ToList();
                model.PurchaseOrderByJobViewModel.SupplierJobList = SupplierJobList.OrderBy(i => i.Name).ToList();

                if (Purchaseorderid != null)
                {
                    Guid Id = Guid.Parse(Purchaseorderid);
                    var purchaselist = PurchaseOrderJob.FindBy(m => m.ID == Id).FirstOrDefault();
                    model.PurchaseOrderByJobViewModel.PurchaseOrderNo = purchaselist.PurchaseOrderNo;
                    model.PurchaseOrderByJobViewModel.ApproveStatus = purchaselist.IsApprove != null ?
                                purchaselist.IsApprove == true ? "[ Approved ]" : "[ Not Approved ]" : string.Empty;
                    var userName = "";
                    if (purchaselist.ModifiedBy == null)
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == purchaselist.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = EmployeeRepo.FindBy(m => m.EmployeeId == purchaselist.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (purchaselist.ModifiedDate == null)
                    {
                        purchaselist.CreatedDate = purchaselist.CreatedDate;
                    }
                    else
                    {
                        purchaselist.ModifiedDate = purchaselist.ModifiedDate;
                    }
                    model.UserName = userName;
                    model.CreatedDate = purchaselist.CreatedDate;
                    model.ModifiedDate = purchaselist.ModifiedDate;
                    if (purchaselist.JobID != null)
                    {
                        model.getjobviewmodel.EmployeeJobId = Guid.Parse(purchaselist.JobID.ToString());
                    }
                }
                if(model.getjobviewmodel.EmployeeJobId !=Guid.Empty)
                {

                    JobList = EmployeeJob.GetAll().Where(m => m.IsDelete == false && m.Id == model.getjobviewmodel.EmployeeJobId).Where(i=>i.JobType==2 ).AsEnumerable().OrderBy(m => m.JobNo).Select(m => new SelectListItem()
                    {
                        Text = "JobNo_" + m.JobNo,
                        Value = m.Id.ToString()
                    }).ToList();

                    QuoteJobList = EmployeeJob.GetAll().Where(m => m.IsDelete == false && m.Id == model.getjobviewmodel.EmployeeJobId).Where(i => i.JobType == 1).AsEnumerable().OrderBy(m => m.JobNo).Select(m => new SelectListItem()
                    {
                        Text = "JobNo_" + m.JobNo,
                        Value = m.Id.ToString()
                    }).ToList();

                    QuoteDoJobList = JobList.Except(QuoteJobList).Union(QuoteJobList.Except(JobList)).ToList();
                }
                

                model.getjobviewmodel.JobDetailsList = QuoteDoJobList;
                model.UserRole = base.GetUserRoles[0];
                if (Purchaseorderid == null)
                {
                    model.PurchaseOrderByJobViewModel.PurchaseOrderNo = PurchaseOrderJob.GetMaxPurchaseNo();
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;

                log.Info(base.GetUserName + " added job purchase order.");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult CheckPurchaseNumber(string PurchaseNo)
        {
            try
            {
                int maxpurchaseno = PurchaseOrderJob.GetMaxPurchaseNo();
                return Json(new { result = maxpurchaseno, status = "false" }, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                throw ex;

                throw;
            }
        }

        public ActionResult ChangePurchaseOrderStatus()
        {
            try
            {
                var purchaseOrderId = !string.IsNullOrEmpty(Request.QueryString["PurchaseOrderId"]) ?
                                       Guid.Parse(Request.QueryString["PurchaseOrderId"]) : Guid.Empty;
                var status = Request.QueryString["PurchaseStatus"];
                var purchaseStatus = string.Empty;

                var purchaseOrderByJob = PurchaseOrderJob.FindBy(m => m.ID == purchaseOrderId).FirstOrDefault();
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "Approve")
                    {
                        purchaseOrderByJob.IsApprove = true;
                        purchaseStatus = "[ Approved ]";

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " approved purchage order.");
                    }
                    else
                    {
                        purchaseOrderByJob.IsApprove = false;
                        purchaseStatus = "[ Not Approved ]";

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " not approved purchase order.");
                    }
                }
                purchaseOrderByJob.ApprovedBy = Guid.Parse(base.GetUserId);
                PurchaseOrderJob.Edit(purchaseOrderByJob);
                PurchaseOrderJob.Save();

                return Json(new { purchaseStatus = purchaseStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetJobPurchaseItemByOrderId(string Id)
        {
            try
            {
                using (PurchaseOrderitemJob)
                {
                    var purchaseitem = PurchaseOrderitemJob.GetPurchaseorderItemJobs(Id);
                    CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel>();
                    List<PurchaseDatajobviewModel> purchaseitemviewmodel = mapper.MapToList(purchaseitem.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(purchaseitemviewmodel);
                    return Json(new { list = json, length = purchaseitemviewmodel.Count() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult SaveJobPurchaseData(List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> values)
        {
            try
            {
                Dictionary<Guid, int> dictionaryjob = new Dictionary<Guid, int>();
                Dictionary<Guid, string> dictionaryItemstoJobpurchase = new Dictionary<Guid, string>();

                string Purchaseid = values[0].purchaseId;
                PurchaseOrderByJobviewmodel purchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel();
                #region Add-Edit Purchaseorderstock
                using (PurchaseOrderJob)
                {
                    if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                    {
                        Guid id = Guid.Parse(Purchaseid);
                        var purchasestock = PurchaseOrderJob.FindBy(i => i.ID == id).FirstOrDefault();
                        purchasestock.SupplierID = Guid.Parse(values[0].Supplierid);
                        if (values[0].JobId != null)
                        {
                            purchasestock.JobID = Guid.Parse(values[0].JobId);
                            purchasestock.InvoiceId = InvoiceRep.FindBy(m => m.EmployeeJobId == purchasestock.JobID).Select(m => m.Id).FirstOrDefault();
                            if (purchasestock.InvoiceId == Guid.Empty)
                            {
                                purchasestock.InvoiceId = null;
                            }
                        }
                        purchasestock.Description = values[0].Description;
                        purchasestock.Cost = Convert.ToDecimal(values[0].Cost);
                        purchasestock.ModifiedBy = Guid.Parse(base.GetUserId);
                        purchasestock.ModifiedDate = DateTime.Now;
                        PurchaseOrderJob.Edit(purchasestock);
                        PurchaseOrderJob.Save();
                        TempData["Message"] = 2;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated job purchase data.");

                    }
                    else
                    {
                        purchaseOrderByJobViewModel.ID = Guid.NewGuid();
                        purchaseOrderByJobViewModel.SupplierID = Guid.Parse(values[0].Supplierid);
                        purchaseOrderByJobViewModel.Description = values[0].Description;
                        if (values[0].JobId != null)
                        {
                            purchaseOrderByJobViewModel.JobID = Guid.Parse(values[0].JobId);
                            purchaseOrderByJobViewModel.InvoiceId = InvoiceRep.FindBy(m => m.EmployeeJobId == purchaseOrderByJobViewModel.JobID).Select(m => m.Id).FirstOrDefault();
                            if (purchaseOrderByJobViewModel.InvoiceId==Guid.Empty)
                            {
                                purchaseOrderByJobViewModel.InvoiceId = null;
                            }
                        }
                        purchaseOrderByJobViewModel.IsDelete = false;
                        purchaseOrderByJobViewModel.Cost = Convert.ToDecimal(values[0].Cost);
                        purchaseOrderByJobViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                        purchaseOrderByJobViewModel.CreatedDate = DateTime.Now;
                        CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob> mapper = new CommonMapper<PurchaseOrderByJobviewmodel, PurchaseOrderByJob>();
                        PurchaseOrderByJob purchasestockinfo = mapper.Mapper(purchaseOrderByJobViewModel);
                        PurchaseOrderJob.Add(purchasestockinfo);
                        PurchaseOrderJob.Save();
                        TempData["Message"] = 1;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " saved job purchase data.");

                    }
                }
                #endregion
                #region Delete item on update 
                Guid itemId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;

                var items = PurchaseOrderitemJob.FindBy(i => i.PurchaseOrderID == itemId).ToList();

                foreach (var item in items)
                {
                    dictionaryItemstoJobpurchase.Add((item.ID), item.Quantity.ToString());
                    string iteminfoid = Convert.ToString(item.ID.ToString());
                    //int pos = Array.IndexOf(values.ToArray(), itemid);
                    bool pos = Array.Exists(values.ToArray(), element => element.ItemId == Guid.Parse(iteminfoid));
                    if (!pos)
                    {
                        var itemdetailid = Convert.ToString(item.ID.ToString());
                        if (!String.IsNullOrEmpty(itemdetailid))
                        {
                            var item_id = Guid.Parse(itemdetailid);
                            dictionaryjob.Add(item_id, Convert.ToInt32(item.Quantity));
                            var itemtodelete = PurchaseOrderitemJob.FindBy(i => i.ID == item_id).FirstOrDefault();
                            PurchaseOrderitemJob.Delete(itemtodelete);
                            PurchaseOrderitemJob.Save();
                        }
                    }
                }
                int tempcount = 0;
                #endregion
                #region Add-Edit Purchaseitemstock
                using (PurchaseOrderitemJob)
                {
                    foreach (var stockitem in values)
                    {
                        if (stockitem.StockId == null)
                        {
                            stockitem.StockId = Guid.Empty.ToString();
                        }
                        Guid stockid = Guid.Parse(stockitem.StockId);
                        Guid itemorderId = Guid.Empty;
                        if (stockitem.ItemId!=null)
                        {
                            itemorderId= Guid.Parse(stockitem.ItemId.ToString());
                        }
                       
                        Guid itempurchasdId = !(String.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString()) ? Guid.Parse(Purchaseid) : Guid.Empty;
                        var checkExist = PurchaseOrderitemJob.FindBy(i => i.PurchaseOrderID == itempurchasdId && i.ID == itemorderId).FirstOrDefault();
                        if (checkExist != null)
                        {
                            var itemtoupdate = checkExist;
                            itemtoupdate.PurchaseItem = Convert.ToString(stockitem.PurchaseItem);
                            itemtoupdate.UnitOfMeasure = Convert.ToString(stockitem.UnitMeasure);
                            itemtoupdate.Price = Convert.ToDecimal(stockitem.Price);
                            itemtoupdate.Quantity = Convert.ToInt32(stockitem.Quantity);
                            itemtoupdate.ModifiedBy = Guid.Parse(base.GetUserId);
                            itemtoupdate.ModifiedDate = DateTime.Now;
                            PurchaseOrderitemJob.Edit(itemtoupdate);
                            PurchaseOrderitemJob.Save();
                            tempcount = 1;

                        }
                        else
                        {
                            PurchaseorderItemJobViewModel purchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel();
                            purchaseOrderITemByJobViewModel.ID = Guid.NewGuid();
                            if (!string.IsNullOrEmpty(Purchaseid) && Purchaseid != Guid.Empty.ToString())
                            {
                                purchaseOrderITemByJobViewModel.PurchaseOrderID = Guid.Parse(Purchaseid);
                            }
                            else
                            {
                                purchaseOrderITemByJobViewModel.PurchaseOrderID = purchaseOrderByJobViewModel.ID;
                            }
                            purchaseOrderITemByJobViewModel.StockID = Guid.Parse(stockitem.StockId);
                            purchaseOrderITemByJobViewModel.PurchaseItem = stockitem.PurchaseItem;
                            purchaseOrderITemByJobViewModel.UnitOfMeasure = stockitem.UnitMeasure;
                            purchaseOrderITemByJobViewModel.Quantity = Convert.ToInt32(stockitem.Quantity);
                            purchaseOrderITemByJobViewModel.Price = Convert.ToDecimal(stockitem.Price);
                            purchaseOrderITemByJobViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            purchaseOrderITemByJobViewModel.CreatedDate = DateTime.Now;
                            CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob> mapper = new CommonMapper<PurchaseorderItemJobViewModel, PurchaseorderItemJob>();
                            PurchaseorderItemJob purchasestockiteminfo = mapper.Mapper(purchaseOrderITemByJobViewModel);
                            PurchaseOrderitemJob.Add(purchasestockiteminfo);
                            PurchaseOrderitemJob.Save();
                            tempcount = 2;

                        }
                    }
                    if (tempcount == 2)
                    {
                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " saved purchase item stock.");
                    }
                    else
                    {

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated purchase item stock.");
                    }



                }
                #endregion

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                TempData["Message"] = 0;
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult PurchaseExport(List<FSM.Core.ViewModels.PurchaseDataCoreviewModel> values)
        {
            try
            {
                if (values != null)
                {
                    TempData["TempExportPurchaseList"] = null;
                    List<FSM.Core.ViewModels.PurchaseDataCoreviewModel> PurchaseList = new List<Core.ViewModels.PurchaseDataCoreviewModel>();
                    Guid tempPurcahse;
                    if (values[0].purchaseId == null)
                    {
                        tempPurcahse = Guid.NewGuid();
                    }
                    else
                    {
                        tempPurcahse = Guid.Parse(values[0].purchaseId);
                    }
                    foreach (var item in values)
                    {
                        FSM.Core.ViewModels.PurchaseDataCoreviewModel obj = new FSM.Core.ViewModels.PurchaseDataCoreviewModel();
                        obj.purchaseId = tempPurcahse.ToString();
                        obj.Supplierid = (values[0].Supplierid);
                        obj.Description = values[0].Description;
                        obj.Cost = values[0].Cost;

                        //Add Item
                        obj.PurchaseItem = item.PurchaseItem;
                        obj.UnitMeasure = item.UnitMeasure;
                        obj.Quantity = item.Quantity;
                        obj.Price = item.Price;

                        PurchaseList.Add(obj);
                    }
                    TempData["TempExportPurchaseList"] = PurchaseList;
                    return Json(new { TempPurchaseOrderId = tempPurcahse.ToString() }, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                TempData["Message"] = 0;
                throw;
            }
        }
        [HttpGet]
        public ActionResult PurchaseExport(string Purchaseorderid)
        {
            using (Stock)
            {
                JobPurChaseViewModel model = new JobPurChaseViewModel
                {
                    PurchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel(),
                    PurchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel(),
                    getjobviewmodel = new GetJobViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                }
                int purchaseNo=PurchaseOrderJob.FindBy(m => m.ID == model.PurchaseOrderByJobViewModel.ID).Select(m=>m.PurchaseOrderNo).FirstOrDefault();
                if(purchaseNo==0)
                {
                    purchaseNo = PurchaseOrderJob.GetMaxPurchaseNo();
                }
                model.PurchaseOrderByJobViewModel.PurchaseOrderNo = purchaseNo;   //Get purchase Order No

                List<FSM.Core.ViewModels.PurchaseDataCoreviewModel> invoicePayments = (List<FSM.Core.ViewModels.PurchaseDataCoreviewModel>)TempData["TempExportPurchaseList"];

                if (invoicePayments != null)
                {
                    //Guid Id = Guid.Parse(Purchaseorderid);
                    var purchaselist = invoicePayments.Where(m => m.purchaseId == Purchaseorderid).FirstOrDefault();
                    //model.PurchaseOrderByJobViewModel.ApproveStatus = purchaselist.IsApprove != null ?
                    //            purchaselist.IsApprove == true ? "[ Approved ]" : "[ Not Approved ]" : string.Empty;
                    Guid supplierId = Guid.Parse(purchaselist.Supplierid);
                    
                    model.PurchaseOrderByJobViewModel.SupplierName = Supplier.FindBy(m => m.ID == supplierId).Select(i => i.Name).FirstOrDefault();
                    model.PurchaseOrderByJobViewModel.Description = purchaselist.Description;
                    model.PurchaseOrderByJobViewModel.Cost = Convert.ToDecimal(purchaselist.Cost);
                    model.PurchaseOrderByJobViewModel.GST = (model.PurchaseOrderByJobViewModel.Cost * 10) / 100; 
                    model.PurchaseOrderByJobViewModel.PriceWithGst= (model.PurchaseOrderByJobViewModel.Cost + (model.PurchaseOrderByJobViewModel.Cost * 10) / 100);
                    //model.getjobviewmodel.EmployeeJobId = Guid.Parse(purchaselist.JobID.ToString());

                    List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> purcahseitem = new List<Core.ViewModels.PurchaseDatajobCoreviewModel>();
                    foreach (var i in invoicePayments)
                    {
                        FSM.Core.ViewModels.PurchaseDatajobCoreviewModel item = new FSM.Core.ViewModels.PurchaseDatajobCoreviewModel();
                        item.PurchaseItem = i.PurchaseItem;
                        item.UnitMeasure = i.UnitMeasure;
                        item.Quantity = i.Quantity;
                        item.Price = i.Price;
                        item.Cost = i.Cost;

                        purcahseitem.Add(item);
                    }

                    //var purchaseitem = PurchaseOrderitemJob.GetPurchaseorderItemJobs(Purchaseorderid);
                    CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel>();
                    List<PurchaseDatajobviewModel> purchaseitemviewmodel = mapper.MapToList(purcahseitem);

                    model.PurchaseDataListviewModel = purchaseitemviewmodel;
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " exported purchase to pdf file.");

                model.UserRole = base.GetUserRoles[0];
                TempData["TempExportPurchaseList"] = null;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult PurchaseExportPreview(string Purchaseorderid)
        {
            using (Stock)
            {
                JobPurChaseViewModel model = new JobPurChaseViewModel
                {
                    PurchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel(),
                    PurchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel(),
                    getjobviewmodel = new GetJobViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                }

                int purchaseNo = PurchaseOrderJob.FindBy(m => m.ID == model.PurchaseOrderByJobViewModel.ID).Select(m => m.PurchaseOrderNo).FirstOrDefault();
                if (purchaseNo == 0)
                {
                    purchaseNo = PurchaseOrderJob.GetMaxPurchaseNo();
                }
                model.PurchaseOrderByJobViewModel.PurchaseOrderNo = purchaseNo;    //Get purchase Order No

                List<FSM.Core.ViewModels.PurchaseDataCoreviewModel> invoicePayments = (List<FSM.Core.ViewModels.PurchaseDataCoreviewModel>)TempData["TempExportPurchaseList"];

                if (invoicePayments != null)
                {
                    //Guid Id = Guid.Parse(Purchaseorderid);
                    var purchaselist = invoicePayments.Where(m => m.purchaseId == Purchaseorderid).FirstOrDefault();
                    //model.PurchaseOrderByJobViewModel.ApproveStatus = purchaselist.IsApprove != null ?
                    //            purchaselist.IsApprove == true ? "[ Approved ]" : "[ Not Approved ]" : string.Empty;
                    Guid supplierId = Guid.Parse(purchaselist.Supplierid);
                    model.PurchaseOrderByJobViewModel.SupplierName = Supplier.FindBy(m => m.ID == supplierId).Select(i => i.Name).FirstOrDefault();
                    model.PurchaseOrderByJobViewModel.Description = purchaselist.Description;
                    model.PurchaseOrderByJobViewModel.Cost = Convert.ToDecimal(purchaselist.Cost);
                    model.PurchaseOrderByJobViewModel.GST = (model.PurchaseOrderByJobViewModel.Cost * 10) / 100;
                    model.PurchaseOrderByJobViewModel.PriceWithGst = (model.PurchaseOrderByJobViewModel.Cost + (model.PurchaseOrderByJobViewModel.Cost * 10) / 100);
                    //model.getjobviewmodel.EmployeeJobId = Guid.Parse(purchaselist.JobID.ToString());

                    List<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel> purcahseitem = new List<Core.ViewModels.PurchaseDatajobCoreviewModel>();
                    foreach (var i in invoicePayments)
                    {
                        FSM.Core.ViewModels.PurchaseDatajobCoreviewModel item = new FSM.Core.ViewModels.PurchaseDatajobCoreviewModel();
                        item.PurchaseItem = i.PurchaseItem;
                        item.UnitMeasure = i.UnitMeasure;
                        item.Quantity = i.Quantity;
                        item.Price = i.Price;
                        item.Cost = i.Cost;

                        purcahseitem.Add(item);
                    }

                    //var purchaseitem = PurchaseOrderitemJob.GetPurchaseorderItemJobs(Purchaseorderid);
                    CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel>();
                    List<PurchaseDatajobviewModel> purchaseitemviewmodel = mapper.MapToList(purcahseitem);

                    model.PurchaseDataListviewModel = purchaseitemviewmodel;
                }
                model.UserRole = base.GetUserRoles[0];

                TempData["TempExportPurchaseList"] = null;

                return new ViewAsPdf("PurchaseExportPreview", model)
                {
                    FileName = "PurchaseExportFile.pdf"
                };
            }
        }
        public ActionResult Export(string Purchaseorderid)
        {
            try
            {
                JobPurChaseViewModel model = new JobPurChaseViewModel
                {
                    PurchaseOrderByJobViewModel = new PurchaseOrderByJobviewmodel(),
                    PurchaseOrderITemByJobViewModel = new PurchaseorderItemJobViewModel(),
                    getjobviewmodel = new GetJobViewModel()
                };
                if (!(String.IsNullOrEmpty(Purchaseorderid)))
                {
                    model.PurchaseOrderByJobViewModel.ID = Guid.Parse(Purchaseorderid);
                }

                if (Purchaseorderid != null)
                {
                    Guid Id = Guid.Parse(Purchaseorderid);
                    var purchaselist = PurchaseOrderJob.FindBy(m => m.ID == Id).FirstOrDefault();
                    model.PurchaseOrderByJobViewModel.ApproveStatus = purchaselist.IsApprove != null ?
                                purchaselist.IsApprove == true ? "[ Approved ]" : "[ Not Approved ]" : string.Empty;
                    model.PurchaseOrderByJobViewModel.SupplierName = purchaselist.Supplier.Name;
                    model.PurchaseOrderByJobViewModel.Description = purchaselist.Description;
                    model.PurchaseOrderByJobViewModel.Cost = purchaselist.Cost;
                    model.getjobviewmodel.EmployeeJobId = purchaselist.JobID == null ? Guid.Empty : Guid.Parse(purchaselist.JobID.ToString());

                    var purchaseitem = PurchaseOrderitemJob.GetPurchaseorderItemJobs(Purchaseorderid);
                    CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel> mapper = new CommonMapper<FSM.Core.ViewModels.PurchaseDatajobCoreviewModel, PurchaseDatajobviewModel>();
                    List<PurchaseDatajobviewModel> purchaseitemviewmodel = mapper.MapToList(purchaseitem.ToList());

                    model.PurchaseDataListviewModel = purchaseitemviewmodel;
                }
                model.UserRole = base.GetUserRoles[0];

                return new ViewAsPdf("PurchaseExportPreview", model)
                {
                    FileName = "PurchaseExportFile.pdf"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetJobList(string SearchTerm)
        {
            var JobList = EmployeeJob.GetAll().Where(m => m.IsDelete == false && m.JobNo.ToString().Contains(SearchTerm)).Take(20).Distinct().Select(m => new SelectListItem()
            {
                Text = "JobNo_" + m.JobNo,
                Value = m.Id.ToString()
            }).ToList();

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " viewed all job list.");

            return Json(new { customerData = JobList }, JsonRequestBehavior.AllowGet);
        }


        //Sync Purchase order to the myob
        [HttpGet]
        public ActionResult SyncmyobPurchase()
        {
            var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlMyobpurchase"];

            _Apiode = !string.IsNullOrEmpty(Request.QueryString["code"]) ? Request.QueryString["code"].ToString() : String.Empty;
            if (string.IsNullOrEmpty(_Apiode))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var t = new Thread(SyncPurchaseordermyob);
                t.SetApartmentState(ApartmentState.STA);
                if (t.IsAlive)
                { }
                else
                {
                    t.Start();
                }
            }
            return RedirectToAction("ViewJobspurchaseOrder");
        }

        public void SyncPurchaseordermyob()
        {

            #region GetCompanyFile
            var s = "";
            CustomerGeneralInfo customerData = new Core.Entities.CustomerGeneralInfo();
            var developerKey = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperKey"]; //"jgnsyvj7brdpw7kb4ftpz3rk";
            var developerSecret = System.Configuration.ConfigurationManager.AppSettings["MyobDeveloperSecret"];// "PqbpPYmxr2UcCfScHxrkEYSz";
            var UserName = System.Configuration.ConfigurationManager.AppSettings["CompanyUserName"];
            var Password = System.Configuration.ConfigurationManager.AppSettings["CompanyPassword"];
            var CompanyFilename = System.Configuration.ConfigurationManager.AppSettings["CompanyFileName"];
            var RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrlMyobpurchase"];
            CompanyFileCredentials _CompanyFile = new CompanyFileCredentials(UserName, Password);
            var configuration = new ApiConfiguration(developerKey, developerSecret, RedirectUrl);
            var oauthService = new OAuthService(configuration);
            string code = _Apiode;
            var tokens = oauthService.GetTokens(code);
            tokens.ExpiresIn = 1;
            var mytoken = tokens;
            if (tokens.HasExpired)
            {
                mytoken = oauthService.RenewTokens(tokens);
            }
            var keystore = new SimpleOAuthKeyService();
            keystore.OAuthResponse = mytoken;
            //Get Company Files
            var cfService = new CompanyFileService(configuration, null, keystore);
            var companyFiles = cfService.GetRange();
            companyFiles = companyFiles.Where(i => i.Name == CompanyFilename).ToArray();
            #endregion

            #region GetITemService (From My Ob)
            var itemservice = new MYOB.AccountRight.SDK.Services.Inventory.ItemService(configuration, null, keystore); // invoice service
            #endregion
            var supService = new MYOB.AccountRight.SDK.Services.Contact.SupplierService(configuration, null, keystore);
            // purchase order service
            var poService = new ItemPurchaseOrderService(configuration, null, keystore);
            var taxcodes = new TaxCodeService(configuration, null, keystore);
            var taxcodeservices = taxcodes.GetRange(companyFiles[0], null, _CompanyFile);
            var tax_Gstcodes = taxcodeservices.Items.ToList().Where(i => i.Code == "GST").FirstOrDefault();
            var Purchaseorders = InvoiceRep.GetallPurchaseOrderformyob();
            List<ItemPurchaseOrderLine> purchaseline = new List<ItemPurchaseOrderLine>();
            foreach (var order in Purchaseorders)
            {
                try
                {
                    //check purchase order is existed on myob or not
                    string pageFilters = String.Format("$filter=Number eq '{0}'", order.Purchaseorderno.ToString());
                    var purchaseOrdersitem = poService.GetRange(companyFiles[0], pageFilters, _CompanyFile);
                    if (purchaseOrdersitem.Items.Count() == 0)
                    {
                        string jobno = "";
                        string filtername = order.Name.ToString();
                        var list = supService.GetRange(companyFiles[0], null, _CompanyFile);
                        var supList = list.Items.Where(i => i.CompanyName == order.Name).FirstOrDefault();
                        if (supList != null)
                        {
                            var itemservicepurchase = new MYOB.AccountRight.SDK.Services.Inventory.ItemService(configuration, null, keystore); // invoice service
                            var itemfilter = String.Format("$filter=Number eq '{0}'", "MATERIALS");//For Most Direct Deposit payments go straight into the NAB Account 1-1111 – NAB NSGC Account 1943
                            var itemss = itemservice.GetRange(companyFiles[0], itemfilter, _CompanyFile);
                            //Get Total items of the purchaseorder
                            var invoiceno = "";
                            if (order.InvoiceId != null)
                            {
                                var invoice = InvoiceRep.FindBy(i => i.Id == order.InvoiceId).FirstOrDefault();
                                if(invoice!=null)
                                {
                                    invoiceno = invoice.InvoiceNo.ToString();
                                }
                            }
                            var orderitems = JobPurchaseOrderitem.FindBy(i => i.PurchaseOrderID == order.ID && i.IsSyncedToMyob == null).ToList();
                            if (orderitems.Count > 0)
                            {
                                purchaseline = new List<ItemPurchaseOrderLine>();
                                foreach (var item in orderitems)
                                {
                                    ItemPurchaseOrderLine line = new ItemPurchaseOrderLine();
                                    line.Item = new MYOB.AccountRight.SDK.Contracts.Version2.Inventory.ItemLink()
                                    {
                                        UID = itemss.Items[0].UID,
                                        URI = itemss.Items[0].URI,
                                        Name = itemss.Items[0].Name,
                                        Number = itemss.Items[0].Number
                                    };

                                    if (!string.IsNullOrEmpty(item.PurchaseItem))
                                    {
                                        if (item.PurchaseItem.Length > 255)
                                        {
                                            line.Description = item.PurchaseItem
                                                .Substring(0, 254);
                                        }
                                        else
                                        {
                                            line.Description = item.PurchaseItem;
                                        }
                                    }
                                    else
                                    {
                                        line.Description = string.Empty;
                                    }

                                    line.TaxCode = new TaxCodeLink() { Code = tax_Gstcodes.Code, UID = tax_Gstcodes.UID };
                                    line.UnitPrice = 0;
                                    line.ReceivedQuantity = 1;

                                    line.BillQuantity = 1;
                                    line.Total = 0;
                                    //line.Account = new AccountLink() { UID= UndepositedFunds1.UID, DisplayID= UndepositedFunds1.DisplayID };
                                    line.Type = OrderLineType.Transaction;
                                    if (!string.IsNullOrEmpty(order.Description))
                                    {
                                        if (order.Description.Length > 255)
                                        {
                                            line.Description = order.Description.Substring(0, 254);
                                        }
                                        else
                                        {
                                            line.Description = order.Description;
                                        }

                                    }
                                    else
                                    {
                                        line.Description = string.Empty;
                                    }
                                    purchaseline.Add(line);
                                    var job = JobRepository.FindBy(i => i.Id == item.PurchaseOrderByJob.JobID).ToList();
                                    if (job.Count() > 0)
                                    {
                                        jobno = job.FirstOrDefault().JobNo.ToString();
                                    }

                                    else
                                    {
                                        var invoice = InvoiceRep.FindBy(i => i.Id == item.PurchaseOrderByJob.InvoiceId).ToList();
                                        if (invoice.Count() > 0)
                                        {
                                            jobno = invoice.FirstOrDefault().JobId.ToString();
                                        }
                                    }
                                }
                            }
                            // save purchase order
                            var servicePO = new ItemPurchaseOrder()
                            {
                                Comment = "Adding purchase order",
                                Created = order.CreatedDate,
                                Date = Convert.ToDateTime(order.CreatedDate).Date,
                                OrderType = OrderLayoutType.Service,
                                IsTaxInclusive = false,
                                JournalMemo = "JobNo#" + jobno,
                                LastModified = order.ModifiedDate,
                                Lines = purchaseline.ToArray(),
                                Number = order.Purchaseorderno.ToString(),
                                ShippingMethod = "By Air",
                                ShipToAddress = supList.CompanyName + System.Environment.NewLine + supList.Addresses.FirstOrDefault().Street + System.Environment.NewLine + supList.Addresses.FirstOrDefault().City + " " + supList.Addresses.FirstOrDefault().State + " " + supList.Addresses.FirstOrDefault().PostCode,
                                Supplier = new SupplierLink()
                                {
                                    UID = supList.UID
                                },
                                SupplierInvoiceNumber = !string.IsNullOrEmpty(invoiceno)?"Inv"+invoiceno:String.Empty,
                                TotalAmount = 0,
                                Status = PurchaseOrderStatus.Open,
                                TotalTax = 0,
                                UID = Guid.NewGuid(),
                                IsReportable = false,


                            };
                            poService.Insert(companyFiles[0], servicePO, _CompanyFile);

                            s = s + order.Purchaseorderno + ",";
                            InvoiceRep.UpdatePurchaseOrderStatus(order.ID);
                            // get purchase order
                            //string pageFilters = String.Format("$filter=Number eq '{0}'", order.Purchaseorderno.ToString());
                            //var purchaseOrdersitem = poService.GetRange(companyFiles[0], pageFilters, _CompanyFile);
                            //foreach (var inv in purchaseOrdersitem.Items.ToList())
                            //{
                            //    poService.Delete(companyFiles[0], inv.UID, _CompanyFile);
                            //}
                            // var purchaseOrdersitem = poService.GetRange(companyFiles[0], pageFilters, _CompanyFile);
                        }


                    }

                    else
                    {
                        InvoiceRep.UpdatePurchaseOrderStatus(order.ID);
                    }
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
                    continue;
                }
            }
            // return "1";

        }
        private void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = Server.MapPath("~/ErrorLog/ErrorLog.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

    }
}