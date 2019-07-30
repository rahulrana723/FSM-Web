using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using FSM.Web.FSMConstant;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.Controllers
{
    public class AssetManagementController : BaseController
    {
        [Dependency]
        public IAssetManagementRepository AssetManageRepo { get; set; }
        [Dependency]
        public IEmployeeJobRepository EmpJobsRepo { get; set; }
        [Dependency]
        public IEmployeeDetailRepository EmpDetailRepo { get; set; }

        // GET: Employee/AssetManagement
        public ActionResult ViewAssetManagement()
        {
            try
            {
                using (AssetManageRepo)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    Nullable<int> SentStatus = string.IsNullOrEmpty(Request.QueryString["AssetAssignStatus"]) ? (int?)null :
                                                 Convert.ToInt32(Request.QueryString["AssetAssignStatus"]);

                    var manageList = AssetManageRepo.GetAssetManagementListBySearch(Searchstring);
                    if (SentStatus == 1)
                    {
                        manageList = manageList.Where(m => m.AssignUserName != null);
                    }
                    else if (SentStatus == 2)
                    {
                        manageList = manageList.Where(m => m.AssignUserName == null);
                    }

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                 Convert.ToInt32(Request.QueryString["page_size"]);
                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<AssetManageCoreViewModel, AssetManageViewModel> mapper = new CommonMapper<AssetManageCoreViewModel, AssetManageViewModel>();
                    List<AssetManageViewModel> assetviewmodel = mapper.MapToList(manageList.ToList());

                    var assetManageSearchViewModel = new AssetManagementSearchViewModel
                    {
                        PageSize = PageSize,
                        searchkeyword = Searchstring,
                        AssetAssignStatus = SentStatus.HasValue ? (Constant.AssetAssignStatus)SentStatus : 0,
                    };
                    var assetManagementListViewModel = new AssetManagementListViewModel
                    {
                        assetMangeListModel = assetviewmodel.OrderByDescending(m => m.CreatedDate),
                        AssetManageSerarchViewmodel = assetManageSearchViewModel
                    };
                    return View(assetManagementListViewModel);
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
        public ActionResult ViewAssetManagement(AssetManagementSearchViewModel searchViewmodel)
        {
            try
            {
                using (AssetManageRepo)
                {
                    string Searchstring = Request.QueryString["Searchkeyword"];
                    var manageList = AssetManageRepo.GetAssetManagementListBySearch(searchViewmodel.searchkeyword);
                    if (searchViewmodel.AssetAssignStatus == FSMConstant.Constant.AssetAssignStatus.Assigned)
                    {
                        manageList = manageList.Where(m => m.AssignUserName != null);
                    }
                    else if (searchViewmodel.AssetAssignStatus == FSMConstant.Constant.AssetAssignStatus.Not_Assigned)
                    {
                        manageList = manageList.Where(m => m.AssignUserName == null);
                    }

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<AssetManageCoreViewModel, AssetManageViewModel> mapper = new CommonMapper<AssetManageCoreViewModel, AssetManageViewModel>();
                    List<AssetManageViewModel> assetviewmodel = mapper.MapToList(manageList.ToList());

                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    var assetManageSearchViewModel = new AssetManagementSearchViewModel
                    {
                        PageSize = PageSize,
                        searchkeyword = Searchstring
                    };

                    var assetManagementListViewModel = new AssetManagementListViewModel
                    {
                        assetMangeListModel = assetviewmodel.OrderByDescending(m => m.CreatedDate),
                        AssetManageSerarchViewmodel = assetManageSearchViewModel
                    };

                    return View(assetManagementListViewModel);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GET: Employee/AssetManagement/AddAssetManage
        /// <summary>
        /// Add Asset Manage
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAssetManage()
        {
            AssetManageViewModel assetManageModel = new AssetManageViewModel();
            assetManageModel.OTRWList = EmpJobsRepo.GetOTRWUser().Select(m => new SelectListItem()
            {
                Text = m.UserName,
                Value = m.Id
            }).ToList();

            return View(assetManageModel);
        }

        // POST: Employee/AddAssetManage
        /// <summary>
        /// Add AssetManage 
        /// </summary>
        /// <param name="stockviewmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddAssetManage(AssetManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (AssetManageRepo)
                    {
                        model.AssetManageID = Guid.NewGuid();
                        model.IsDelete = false;
                        model.CreatedBy = Guid.Parse(base.GetUserId);
                        model.CreatedDate = DateTime.Now;
                        if (model.AssignedTo != null)
                        {
                            model.DateAssigned = DateTime.Now;
                        }
                        CommonMapper<AssetManageViewModel, AssetManagement> mapper = new CommonMapper<AssetManageViewModel, AssetManagement>();
                        AssetManagement assetManage = mapper.Mapper(model);
                        AssetManageRepo.Add(assetManage);
                        AssetManageRepo.Save();
                        TempData["AssetManageMsg"] = 1;
                        return RedirectToAction("ViewAssetManagement", "AssetManagement");
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return View();
        }

        //GET: Employee/AssetManagement/EditAssetManage
        /// <summary>
        /// Update Asset Management
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Model</returns>
        public ActionResult EditAssetManage(string Id)
        {
            try
            {
                using (AssetManageRepo)
                {
                    Guid AssetId = Guid.Parse(Id);
                    AssetManagement assetManage = AssetManageRepo.FindBy(m => m.AssetManageID == AssetId).FirstOrDefault();
                    // mapping entity to viewmodel
                    CommonMapper<AssetManagement, AssetManageViewModel> mapper = new CommonMapper<AssetManagement, AssetManageViewModel>();
                    AssetManageViewModel assetManageModel = mapper.Mapper(assetManage);
                    var userName = "";
                    if (assetManageModel.ModifiedBy == null)
                    {
                        userName = EmpDetailRepo.FindBy(m => m.EmployeeId == assetManageModel.CreatedBy).Select(m => m.UserName).FirstOrDefault();

                    }
                    else
                    {
                        userName = EmpDetailRepo.FindBy(m => m.EmployeeId == assetManageModel.ModifiedBy).Select(m => m.UserName).FirstOrDefault();
                    }
                    if (assetManageModel.ModifiedDate == null)
                    {
                        assetManageModel.CreatedDate = assetManageModel.CreatedDate;
                    }
                    else
                    {
                        assetManageModel.ModifiedDate = assetManageModel.ModifiedDate;
                    }
                    assetManageModel.UserName = userName;
                    assetManageModel.OTRWList = EmpJobsRepo.GetOTRWUser().Select(m => new SelectListItem()
                    {
                        Text = m.UserName,
                        Value = m.Id
                    }).ToList();
                    return View(assetManageModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Employee/AssetManagement/EditAssetManage
        /// <summary>
        /// Edit AssetManage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAssetManage(AssetManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (AssetManageRepo)
                    {
                        model.ModifiedBy = Guid.Parse(base.GetUserId);
                        model.ModifiedDate = DateTime.Now;
                        if (model.AssignedTo != null)
                        {
                            model.DateAssigned = DateTime.Now;
                        }
                        else
                        {
                            model.DateAssigned = null;
                        }
                        //stockviewmodel.Date = !string.IsNullOrEmpty(stockviewmodel.DisplayDate) ? DateTime.Parse(stockviewmodel.DisplayDate) : (DateTime?)null;

                        //mapping viewmodel to entity
                        CommonMapper<AssetManageViewModel, AssetManagement> mapper = new CommonMapper<AssetManageViewModel, AssetManagement>();
                        AssetManagement assetManage = mapper.Mapper(model);
                        AssetManageRepo.Edit(assetManage);
                        AssetManageRepo.Save();
                        TempData["AssetManageMsg"] = 2;
                        return RedirectToAction("ViewAssetManagement", "AssetManagement");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View();
        }
        public ActionResult CancelAssetManage()
        {
            try
            {
                TempData["AssetManageMsg"] = "";
                return RedirectToAction("ViewAssetManagement");
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
        public ActionResult DeleteAssetManage(string AssetId)
        {
            try
            {
                Guid AssetManageId = Guid.Parse(AssetId);

                AssetManagement assetManage = AssetManageRepo.FindBy(m => m.AssetManageID == AssetManageId).FirstOrDefault();

                assetManage.IsDelete = true;
                AssetManageRepo.Edit(assetManage);
                AssetManageRepo.Save();

                TempData["AssetManageMsg"] = 3;
                return Json(new { success = true });
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                AssetManageRepo.Dispose();
            }
        }
    }
}