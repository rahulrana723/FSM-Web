using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FSM.Web.Areas.Employee.Controllers
{
    [Authorize]
    public class StockController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                ().DeclaringType);
        [Dependency]
        public IStockRepository Stock { get; set; }

        [Dependency]
        public IOTRWStockRepository OTRWStock { get; set; }

        [Dependency]
        public IAspNetRolesRepository UserRoles { get; set; }

        [Dependency]
        public IEmployeeDetailRepository Employee { get; set; }

        [Dependency]
        public IJobStockRepository JobStockRepo { get; set; }
        [Dependency]
        public IEmployeeJobRepository EmployeeJob { get; set; }

        // GET: Employee/Stock
        //GET:Employee/Stock
        /// <summary>
        /// Show List Of Stock
        /// </summary>
        /// <returns>Model</returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            try
            {
                using (Stock)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    //var stocks =Stock.GetAll();
                    var stocks = Stock.GetCustomerListBySearchkeyword(Searchstring);

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<FSM.Core.ViewModels.StockViewModel, StockViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.StockViewModel, StockViewModel>();
                    List<StockViewModel> stockviewmodel = mapper.MapToList(stocks.ToList());

                    var StocksearchViewmodel = new StocksearchViewmodel
                    {
                        PageSize = PageSize,
                        searchkeyword = Searchstring
                    };
                    var Stocklistviewmodel = new StockListViewmodel
                    {
                        stocklistviewmodel = stockviewmodel.OrderByDescending(m =>m.ModifiedDate),
                        Stockserarchviewmodel = StocksearchViewmodel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed details of stock.");

                    return View(Stocklistviewmodel);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        //POST:Employee/Stock
        /// <summary>
        /// Search Stock Item 
        /// </summary>
        /// <param name="stocksearchViewmodel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult Index(StocksearchViewmodel stocksearchViewmodel)
        {
            try
            {
                using (Stock)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    var stocks = Stock.GetCustomerListBySearchkeyword(stocksearchViewmodel.searchkeyword);
                    CommonMapper<FSM.Core.ViewModels.StockViewModel, StockViewModel> mapper = new CommonMapper<FSM.Core.ViewModels.StockViewModel, StockViewModel>();
                    List<StockViewModel> stockviewmodel = mapper.MapToList(stocks.ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    var StocksearchViewmodel = new StocksearchViewmodel
                    {
                        PageSize = PageSize,
                        searchkeyword = Searchstring
                    };
                    var Stocklistviewmodel = new StockListViewmodel
                    {
                        stocklistviewmodel = stockviewmodel,
                        Stockserarchviewmodel = StocksearchViewmodel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of stock.");

                    return View(Stocklistviewmodel);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/Stock/AddStock
        /// <summary>
        /// Add stock
        /// </summary>
        /// <returns></returns>
        public ActionResult AddStock()
        {
            return View();
        }

        // POST: Employee/Stock
        /// <summary>
        /// Add Stock 
        /// </summary>
        /// <param name="stockviewmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddStock(StockViewModel stockviewmodel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Stock)
                    {
                        stockviewmodel.ID = Guid.NewGuid();
                        stockviewmodel.IsDelete = false;
                        stockviewmodel.CreatedBy = Guid.Parse(base.GetUserId);
                        stockviewmodel.CreatedDate = DateTime.Now;
                        stockviewmodel.ModifiedDate = DateTime.Now;
                        stockviewmodel.Available = stockviewmodel.Quantity;
                        stockviewmodel.OTRW = 0;
                        stockviewmodel.Date = DateTime.Now;
                        CommonMapper<StockViewModel, Stock> mapper = new CommonMapper<StockViewModel, Stock>();
                        Stock stock = mapper.Mapper(stockviewmodel);
                        Stock.Add(stock);
                        Stock.Save();
                        TempData["stockmsg"] = 1;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " added new stock.");

                        return RedirectToAction("Index", "Stock");
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View();
        }

        //GET: Employee/Stock/EditStock
        /// <summary>
        /// Update stock
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Model</returns>
        public ActionResult EditStock(string Id)
        {
            try
            {
                using (Stock)
                {
                    Guid stockid = Guid.Parse(Id);
                    Stock stock = Stock.FindBy(m => m.ID == stockid).FirstOrDefault();
                    // mapping entity to viewmodel
                    CommonMapper<Stock, StockViewModel> mapper = new CommonMapper<Stock, StockViewModel>();
                    StockViewModel stockviewmodel = mapper.Mapper(stock);
                    var userName = "";
                    if (stockviewmodel.ModifiedBy == null)
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == stockviewmodel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = Employee.FindBy(m => m.EmployeeId == stockviewmodel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (stockviewmodel.ModifiedDate == null)
                    {
                        stockviewmodel.CreatedDate = stockviewmodel.CreatedDate;
                    }
                    else
                    {
                        stock.ModifiedDate = stock.ModifiedDate;
                    }
                    stockviewmodel.UserName = userName;
                    if (stockviewmodel.Date != null)
                    {
                        stockviewmodel.DisplayDate = stockviewmodel.Date.Value.Day + "/" + stockviewmodel.Date.Value.Month + "/" + stockviewmodel.Date.Value.Year;
                    }
                  
                    return View(stockviewmodel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // POST: Employee/Stock/EditStock
        /// <summary>
        /// Edit Stock 
        /// </summary>
        /// <param name="stockviewmodel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditStock(StockViewModel stockviewmodel)
        {
            if (stockviewmodel.Quantity == null)
            {
                stockviewmodel.Quantity = 0;
                ModelState.Remove("Quantity");
            }
            if (stockviewmodel.Available == null)
            {
                stockviewmodel.Available = 0;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (Stock)
                    {
                        stockviewmodel.ModifiedBy = Guid.Parse(base.GetUserId);
                        stockviewmodel.ModifiedDate = DateTime.Now;
                        //stockviewmodel.Date = !string.IsNullOrEmpty(stockviewmodel.DisplayDate) ? DateTime.Parse(stockviewmodel.DisplayDate) : (DateTime?)null;
                        stockviewmodel.Date = DateTime.Now;
                        if (stockviewmodel.AddQuantity == null)
                        {
                            stockviewmodel.AddQuantity = 0;
                        }
                        stockviewmodel.Quantity = stockviewmodel.Quantity + stockviewmodel.AddQuantity;
                        stockviewmodel.Available = stockviewmodel.AddQuantity+stockviewmodel.Available;
                        //mapping viewmodel to entity
                        CommonMapper<StockViewModel, Stock> mapper = new CommonMapper<StockViewModel, Stock>();
                        Stock stock = mapper.Mapper(stockviewmodel);
                        Stock.Edit(stock);
                        Stock.Save();
                        TempData["stockmsg"] = 2;

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " updated an existing stock.");

                        return RedirectToAction("Index", "Stock");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View();
        }


        //GET: Get Stock list
        /// <summary>
        /// Get Stock id and name for dropdown
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult GetStocklist()
        {
            try
            {
                using (Stock)
                {
                    var stocks = Stock.GetAll();
                    var stocklisting = stocks.OrderBy(i => i.Label).ToList();
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<Stock, StockViewModel> mapper = new CommonMapper<Stock, StockViewModel>();
                    List<StockViewModel> stocklist = mapper.MapToList(stocklisting);
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(stocklist);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of stock.");

                    return Json(new { list = json, length = stocklist.Count() });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET: Get Stock Items
        /// <summary>
        /// Get Stock Items
        /// </summary>
        /// <returns></returns>
        public List<StockItem> GeStockItems()
        {
            try
            {
                List<StockItem> list = new List<StockItem>();
                using (Stock)
                {
                    var stocks = Stock.GetAll();
                    stocks = stocks.Where(i => i.IsDelete==false);

                    foreach (var stock in stocks)
                    {
                        StockItem obj = new StockItem();
                        obj.StockId = stock.ID;
                        obj.StockName = stock.Label;
                        list.Add(obj);
                    }

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of stock items.");
                    return list;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        

        //POST: Add Stock to Inventory
        /// <summary>
        /// Get Stock id and name for dropdown
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Quantity"></param>
        ///<param name="unitmeasure"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult AddStocktoInventory(string id, string unitmeasure, string Quantity)
        {
            try
            {
                using (Stock)
                {
                    Guid stockid = Guid.Parse(id);
                    Stock stock = Stock.FindBy(m => m.ID == stockid).FirstOrDefault();
                    stock.Available = Convert.ToInt32(Quantity) + stock.Available;
                    Stock.Edit(stock);
                    Stock.Save();

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " inserted stock inventory.");

                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //POST:Employee/Stock/DeleteStock
        /// <summary>
        /// Delete Stock Using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteStock(string id)
        {
            try
            {
                Guid stockid = Guid.Parse(id);

                List<JobStock> jobStock = JobStockRepo.FindBy(m => m.StockID == stockid).ToList();
                if (jobStock != null)
                {
                    foreach (var item in jobStock)
                    {
                        item.IsDelete = true;
                        JobStockRepo.Edit(item);
                        JobStockRepo.Save();
                    }
                }

                List<OTRWStock> entityOTRWStock = OTRWStock.FindBy(m => m.StockID == stockid).ToList();
                if (entityOTRWStock != null)
                {
                    foreach (var item in entityOTRWStock)
                    {
                        item.IsDelete = true;
                        OTRWStock.Edit(item);
                        OTRWStock.Save();
                    }
                }

                Stock stock = Stock.FindBy(m => m.ID == stockid).FirstOrDefault();
                stock.IsDelete = true;
                Stock.Edit(stock);
                Stock.Save();
                TempData["stockmsg"] = 3;

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted stock.");

                return Json(new { success = true });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                JobStockRepo.Dispose();
                OTRWStock.Dispose();
                Stock.Dispose();
            }
        }
        public ActionResult CancelStock()
        {
            try
            {
                TempData["stockmsg"] = "";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GET: Get Employee list
        /// <summary>
        /// Get List of Employee
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEmployeeLists()
        {
            try
            {
                using (Employee)
                {
                    var employees = Employee.GetAll();
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<EmployeeDetail, EmployeeDetailViewModel> mapper = new CommonMapper<EmployeeDetail, EmployeeDetailViewModel>();
                    List<EmployeeDetailViewModel> emplist = mapper.MapToList(employees.ToList());
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(emplist);

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed list of employees");

                    return Json(new { list = json, length = emplist.Count() });
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<OTRWEmployeelist> OTRWEmployee()
        {
            try
            {
                string OTRWID = "31cf918d-b8fe-4490-b2d7-27324bfe89b4";
                List<OTRWEmployeelist> list = new List<OTRWEmployeelist>();
                using (Employee)
                {
                    var OTRID = Guid.Parse(OTRWID);
                    var employee = Employee.FindBy(i => i.Role == OTRID && i.IsDelete==false && i.IsActive==true);
                    foreach (var emp in employee)
                    {
                        OTRWEmployeelist obj = new OTRWEmployeelist();
                        obj.EmployeeId = emp.EmployeeId;
                        obj.EmployeeName = emp.UserName;
                        list.Add(obj);
                    }
                }
                return list;

            }
            catch (Exception)
            {

                throw;
            }
        }

        //GET: Get Stock 
        /// <summary>
        /// Get Stock Using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult GetStockByStockid(String id)

        {
            try
            {
                Guid stockid = Guid.Parse(id);
                Stock stock = Stock.FindBy(m => m.ID == stockid).FirstOrDefault();
                // mapping entity to viewmodel
                CommonMapper<Stock, StockViewModel> mapper = new CommonMapper<Stock, StockViewModel>();
                StockViewModel stockviewmodel = mapper.Mapper(stock);
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(stockviewmodel);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed stock info by stock id.");

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }

        }

        ///GET: Employee/Stock/_AssignOTRWStock
        /// <summary>
        ///Get OTRW Stock
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _AssignOTRWStock()
        {
            try
            {
                OTRWStockViewmodel otrwstockViewmodel = new OTRWStockViewmodel();

                otrwstockViewmodel.StockItemList = GeStockItems();
                otrwstockViewmodel.OTRWEmployee = OTRWEmployee();

                

                return PartialView(otrwstockViewmodel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //POST:Employee/Stock/_AssignOTRWStock
        /// <summary>
        /// Post OTRW Stock
        /// </summary>
        /// <param name="otrwstockViewmodel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _AssignOTRWStock(OTRWStockViewmodel otrwstockViewmodel)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    using (Stock)
                    {
                        var stock = Stock.FindBy(i => i.ID == otrwstockViewmodel.StockID).FirstOrDefault();
                        stock.Available = stock.Available - otrwstockViewmodel.Quantity;
                        stock.ModifiedBy = Guid.Parse(base.GetUserId);
                        stock.ModifiedDate = DateTime.Now;
                        Stock.Save();
                    }
                    using (OTRWStock)
                    {
                        CommonMapper<OTRWStockViewmodel, OTRWStock> mapper = new CommonMapper<OTRWStockViewmodel, OTRWStock>();
                        otrwstockViewmodel.ID = Guid.NewGuid();
                        otrwstockViewmodel.CreateDate = DateTime.Now;
                        otrwstockViewmodel.CreatedBy = Guid.Parse(base.GetUserId);
                        OTRWStock otrwstock = mapper.Mapper(otrwstockViewmodel);
                        OTRWStock.Add(otrwstock);
                        OTRWStock.Save();

                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;

                        log.Info(base.GetUserName + " assigned a stock to otrw.");
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    otrwstockViewmodel.StockItemList = GeStockItems();
                    otrwstockViewmodel.OTRWEmployee = OTRWEmployee();
                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " assigned a stock to otrw.");
                    return PartialView(otrwstockViewmodel);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        ///GET: Employee/Stock/ViewOTRWDetail
        /// <summary>
        ///Show OTRW Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Model</returns>
        [HttpGet]
        public ActionResult ViewOTRWDetail(string id)
        {
            try
            {
                Guid stockid = Guid.Parse(id);
                var stockLabel = string.Empty;
                int? TotalQuantity;
                int? Available;
                using (Stock)
                {
                    var stock = Stock.FindBy(i => i.ID == stockid).FirstOrDefault();
                    stockLabel = stock.Label;
                    TotalQuantity = stock.Quantity;
                    Available = stock.Available;
                }
                using (OTRWStock)
                {
                    string Searchstring = Request.QueryString["searchkeyword"];
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 5 :
                                   Convert.ToInt32(Request.QueryString["page_size"]);
                    Nullable<int> Type = string.IsNullOrEmpty(Request.QueryString["Type"]) ? (int?)null :
                                                   Convert.ToInt32(Request.QueryString["Type"]);

                    var otrwstock = OTRWStock.GetAssignedStockOTRW(id, Searchstring);
                    var StockotrwsearchViewmodel = new OTRWStocksearchviewmodel
                    {
                        StockLabel = stockLabel.ToString(),
                        Type = Type.HasValue ? Convert.ToInt32(Type) : 0,
                        PageSize = PageSize,
                        searchkeyword = Searchstring,
                        Stockid = stockid
                    };
                    otrwstock = Type.HasValue ? Type > 0 ? otrwstock.Where(i => i.Typeid == Type) : otrwstock : otrwstock;
                    otrwstock = string.IsNullOrEmpty(Searchstring) ? otrwstock : otrwstock.Where(i => i.Detail.ToLower().Contains(Searchstring.ToLower()));
                    CommonMapper<FSM.Core.ViewModels.OTRWSearchViewmodel, OTRWStockViewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.OTRWSearchViewmodel, OTRWStockViewmodel>();
                    List<OTRWStockViewmodel> stockviewmodel = mapper.MapToList(otrwstock.ToList());
                    var Stockotrwlistviewmodel = new OTRWStockListViewmodel
                    {
                        OTRWStockViewListmodel = stockviewmodel,
                        OTRWStockSearchmodel = StockotrwsearchViewmodel,
                        TotalQuantity = TotalQuantity,
                        Availability=Available
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " viewed details of stock.");

                    return View(Stockotrwlistviewmodel);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        //POST:Employee/Stock/ViewOTRWDetail
        /// <summary>
        /// Search OTRW Detail 
        /// </summary>
        /// <param name="otrwsearchviewmodel"></param>
        /// <returns>Model</returns>
        [HttpPost]
        public ActionResult ViewOTRWDetail(OTRWStocksearchviewmodel otrwsearchviewmodel)
        {
            try
            {
                using (OTRWStock)
                {
                    string id = otrwsearchviewmodel.Stockid.ToString();
                    Guid? stockid = otrwsearchviewmodel.Stockid;
                    int? TotalQuantity;
                    int? Available;
                    using (Stock)
                    {
                        var stock = Stock.FindBy(i => i.ID == stockid).FirstOrDefault();
                        TotalQuantity = stock.Quantity;
                        Available = stock.Available;
                    }
                    var otrwstock = OTRWStock.GetAssignedStockOTRW(id, otrwsearchviewmodel.searchkeyword);
                    otrwstock = (int)otrwsearchviewmodel.Type > 0 ? otrwstock.Where(i => i.Typeid ==
                                        (int)otrwsearchviewmodel.Type) : otrwstock;
                    //otrwstock = string.IsNullOrEmpty(otrwsearchviewmodel.searchkeyword) ? otrwstock : otrwstock
                    //             .Where(i => i.Detail.ToLower().Contains(otrwsearchviewmodel.searchkeyword.ToLower()));

                    List<OTRWStockViewmodel> stockviewmodel = null;
                    CommonMapper<FSM.Core.ViewModels.OTRWSearchViewmodel, OTRWStockViewmodel> mapper = new CommonMapper<FSM.Core.ViewModels.OTRWSearchViewmodel, OTRWStockViewmodel>();
                    stockviewmodel = mapper.MapToList(otrwstock.ToList());
                    var Stockotrwlistviewmodel = new OTRWStockListViewmodel
                    {
                        OTRWStockViewListmodel = stockviewmodel,
                        OTRWStockSearchmodel = otrwsearchviewmodel,
                        TotalQuantity = TotalQuantity,
                        Availability = Available
                    };
                    return View(Stockotrwlistviewmodel);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        ///GET: Employee/Stock/_AssignJOBSStock
        /// <summary>
        ///Get JOBS Stock
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _AssignJOBSStock()
        {
            try
            {
                DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();

                displayJobStocksViewModel.stockDetail = GeStockDetailForJob();
                displayJobStocksViewModel.EmployeeJobDetailStockList = GetAllJobs();
                return PartialView(displayJobStocksViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //POST:Employee/Stock/_AssignJOBSStock
        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Redirects Index</returns>
        [HttpPost]
        public ActionResult _AssignJOBSStock(DisplayJobStocksViewModel model)
        {
            if (model != null)
            {
                try
                {
                    Decimal? price = 0.0M;
                    DisplayJobStocksViewModel displayJobStocksViewModel = new DisplayJobStocksViewModel();
                    if (ModelState.IsValid)
                    {
                        displayJobStocksViewModel.JobId = model.JobId;
                        displayJobStocksViewModel.StockID = model.StockID;
                        displayJobStocksViewModel.UnitMeasure = model.UnitMeasure;
                        displayJobStocksViewModel.Quantity = model.Quantity;
                        using (Stock)
                        {
                            var stock = Stock.FindBy(i => i.ID == displayJobStocksViewModel.StockID).FirstOrDefault();
                            stock.Available = stock.Available - displayJobStocksViewModel.Quantity; 
                            stock.ModifiedBy = Guid.Parse(base.GetUserId);
                            price = stock.Price;
                            stock.ModifiedDate = DateTime.Now;
                            Stock.Edit(stock);
                            Stock.Save();
                        }
                        using (JobStockRepo)
                        {
                            displayJobStocksViewModel.CreatedDate = DateTime.Now;
                            displayJobStocksViewModel.CreatedBy = Guid.Parse(base.GetUserId);
                            displayJobStocksViewModel.ID = Guid.NewGuid();
                            displayJobStocksViewModel.Price = (price.HasValue?price:0.0M) * displayJobStocksViewModel.Quantity;
                            CommonMapper<DisplayJobStocksViewModel, JobStock> mapperdoc = new CommonMapper<DisplayJobStocksViewModel, JobStock>();
                            JobStock jobStock = mapperdoc.Mapper(displayJobStocksViewModel);
                            JobStockRepo.Add(jobStock);
                            JobStockRepo.Save();
                         }
                        log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                        log.Info(base.GetUserName + " assigned a job stock to otrw.");
                    }
                    else
                    {
                        displayJobStocksViewModel.stockDetail = GeStockDetailForJob();
                        displayJobStocksViewModel.EmployeeJobDetailStockList = GetAllJobs();
                        return PartialView(displayJobStocksViewModel);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        public List<EmployeeJobDetailStock> GetAllJobs()
        {
            try
            {
                List<EmployeeJobDetailStock> list = new List<EmployeeJobDetailStock>();
                using (EmployeeJob)
                {

                    var employeerJobList = EmployeeJob.GetAll().Where(m=>m.IsDelete==false).AsEnumerable().OrderBy(m => m.JobNo);
                    foreach (var emp in employeerJobList)
                    {
                        EmployeeJobDetailStock obj = new EmployeeJobDetailStock();
                        obj.EmployeeJobId = emp.Id;
                        obj.JobNo = emp.JobNo;
                        obj.Description = "JobId_" + obj.JobNo;
                        list.Add(obj);
                    }
                }
                return list;

            }
            catch (Exception)
            {

                throw;
            }
        }
        //GET: Get Stock Items
        /// <summary>
        /// Get Stock Items
        /// </summary>
        /// <returns></returns>
        public List<StockDetail> GeStockDetailForJob()
        {
            try
            {
                List<StockDetail> list = new List<StockDetail>();
                using (Stock)
                {
                    var stocks = Stock.GetAll();
                    stocks = stocks.Where(i=>i.IsDelete==false);

                    foreach (var stock in stocks)
                    {
                        StockDetail obj = new StockDetail();
                        obj.StockID = stock.ID;
                        obj.Label = stock.Label;
                        list.Add(obj);
                    }
                    return list;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}