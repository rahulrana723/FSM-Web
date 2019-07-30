using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Admin.ViewModels;
using FSM.Web.Common;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FSM.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class RoleManagementController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Dependency]
        public IModule_MasterRepository ModuleMasterRepo { get; set; }
        [Dependency]
        public IAction_MasterRepository ActionMasterRepo { get; set; }
        [Dependency]
        public IParentAction_MasterRepository ParentActionMasterRepo { get; set; }
        [Dependency]
        public IRoleModuleMappingRepository RoleModuleMappingRepo { get; set; }
        [Dependency]
        public IRoleModuleActionMappingRepository RoleModuleActionMappingRepo { get; set; }
        [Dependency]
        public IRoleParentActionMappingRepository RoleParentActionMappingRepo { get; set; }
        [Dependency]
        public IAspNetRolesRepository AspNetRolesRepo { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var pageSize = Request.QueryString["PageSize"];
                string searchKeyword = Request.QueryString["searchkeyword"];
                var aspNetRolesList = AspNetRolesRepo.GetAllRoles(searchKeyword).ToList();

                CommonMapper<AspNetRolesCore, AspNetRolesViewModel> mapper = new CommonMapper<AspNetRolesCore, AspNetRolesViewModel>();
                List<AspNetRolesViewModel> aspNetRolesViewModel = mapper.MapToList(aspNetRolesList);

                RoleManagementSearchViewModel roleManagementSearchViewModel = new RoleManagementSearchViewModel();

                int PageSize = !string.IsNullOrEmpty(pageSize) ? int.Parse(pageSize) : 10;
                roleManagementSearchViewModel.PageSize = PageSize;
                roleManagementSearchViewModel.searchkeyword = string.IsNullOrEmpty(searchKeyword) ? "" : searchKeyword;

                var roleManagementViewModel = new RoleManagementViewModel
                {
                    aspNetRolesViewModelList = aspNetRolesViewModel,
                    roleManagementSearchViewModel = roleManagementSearchViewModel
                };
                return View(roleManagementViewModel);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }

        [HttpPost]
        public ActionResult Index(RoleManagementSearchViewModel roleSearchViewModel)
        {
            try
            {

                var aspNetRolesList = AspNetRolesRepo.GetAllRoles(roleSearchViewModel.searchkeyword).ToList();

                CommonMapper<AspNetRolesCore, AspNetRolesViewModel> mapper = new CommonMapper<AspNetRolesCore, AspNetRolesViewModel>();
                List<AspNetRolesViewModel> aspNetRolesViewModel = mapper.MapToList(aspNetRolesList);

                var roleManagementViewModel = new RoleManagementViewModel
                {
                    aspNetRolesViewModelList = aspNetRolesViewModel,
                    roleManagementSearchViewModel = roleSearchViewModel
                };

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " viewed list of roles");

                return View(roleManagementViewModel);

            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }

        [HttpGet]
        public ActionResult IndexParial()
        {
            try
            {
                var pageSize = Request.QueryString["PageSize"];
                string searchKeyword = Request.QueryString["searchkeyword"];

                var aspNetRolesList = AspNetRolesRepo.GetAllRoles(searchKeyword).OrderByDescending(m => m.CreatedDate).ToList();

                CommonMapper<AspNetRolesCore, AspNetRolesViewModel> mapper = new CommonMapper<AspNetRolesCore, AspNetRolesViewModel>();
                List<AspNetRolesViewModel> aspNetRolesViewModel = mapper.MapToList(aspNetRolesList);

                RoleManagementSearchViewModel roleManagementSearchViewModel = new RoleManagementSearchViewModel();

                int PageSize = !string.IsNullOrEmpty(pageSize) ? int.Parse(pageSize) : 10;
                roleManagementSearchViewModel.PageSize = PageSize;

                var roleManagementViewModel = new RoleManagementViewModel
                {
                    aspNetRolesViewModelList = aspNetRolesViewModel,
                    roleManagementSearchViewModel = roleManagementSearchViewModel
                };
                return PartialView("_GetRolesList", roleManagementViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AddRole()
        {
            try
            {
                ModelState.Clear();

                return PartialView("_AddRole");
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddRole(AspNetRolesViewModel aspNetRolesViewModel)
        {
            try
            {
                string PageSize = Request.QueryString["PageSize"];
                string grid_column = Request.QueryString["grid-column"];
                string grid_dir = Request.QueryString["grid-dir"];
                string grid_page = Request.QueryString["grid-page"];
                string searchKeyword = "";
                string Role = aspNetRolesViewModel.Name;

                var regex = new Regex("^[a-zA-Z0-9\\-\\s]+$");

                if (!string.IsNullOrEmpty(Role))
                {
                    var roleExist = AspNetRolesRepo.GetAllRoles(searchKeyword).FirstOrDefault(m => m.Name == Role);
                    if (roleExist != null)
                    {
                        return Json(new { error = "Role already exists !" }, JsonRequestBehavior.AllowGet);
                    }
                    if (Role.Length > 256)
                    {
                        return Json(new { error = "Role length should be less than 256 chars !" }, JsonRequestBehavior.AllowGet);
                    }
                    if (!regex.IsMatch(Role))
                    {
                        return Json(new { error = "Invalid data !" }, JsonRequestBehavior.AllowGet);
                    }

                    // creating role entity
                    AspNetRoles aspNetRoles = new AspNetRoles();
                    aspNetRoles.Id = Guid.NewGuid();
                    aspNetRoles.IsDelete = false;
                    aspNetRoles.Name = Role;
                    aspNetRoles.CreatedBy = Guid.Parse(base.GetUserId);
                    aspNetRoles.CreatedDate = DateTime.Now;

                    // saving role entity
                    AspNetRolesRepo.Add(aspNetRoles);
                    AspNetRolesRepo.Save();

                    // creating route values

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " added new user role " + Role);

                    return RedirectToAction("IndexParial");
                }
                else
                {
                    return Json(new { error = "Role is required !" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult EditRole()
        {
            try
            {
                ModelState.Clear();

                Guid RoleId = Guid.Parse(Request.QueryString["RoleId"]);
                string PageSize = Request.QueryString["PageSize"];
                string grid_column = Request.QueryString["grid-column"];
                string grid_dir = Request.QueryString["grid-dir"];
                string grid_page = Request.QueryString["grid-page"];
                string searchKeyword = "";

                var aspNetRole = AspNetRolesRepo.GetAllRoles(searchKeyword).Where(m => m.Id == RoleId).FirstOrDefault();

                CommonMapper<AspNetRolesCore, AspNetRolesViewModel> mapper = new CommonMapper<AspNetRolesCore, AspNetRolesViewModel>();
                AspNetRolesViewModel rolesViewModel = mapper.Mapper(aspNetRole);
                rolesViewModel.PageSize = PageSize;
                rolesViewModel.grid_column = grid_column;
                rolesViewModel.grid_dir = grid_dir;
                rolesViewModel.grid_page = grid_page;

                return PartialView("_EditRole", rolesViewModel);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }

        [HttpPost]
        public ActionResult EditRole(AspNetRolesViewModel aspNetRolesViewModel)
        {
            var oldRoleName = AspNetRolesRepo.FindBy(m => m.Id == aspNetRolesViewModel.Id).Select(m => m.Name).FirstOrDefault();
            AspNetRoles aspNetRoles = new AspNetRoles();
            aspNetRoles.Id = aspNetRolesViewModel.Id;
            aspNetRoles.Name = aspNetRolesViewModel.Name;
            aspNetRoles.ModifiedBy = Guid.Parse(base.GetUserId);
            aspNetRoles.ModifiedDate = DateTime.Now;
            aspNetRoles.IsDelete = false;

            AspNetRolesRepo.DeAttach(aspNetRoles);
            AspNetRolesRepo.Edit(aspNetRoles);
            AspNetRolesRepo.Save();

            // creating route values
            var routeValues = new RouteValueDictionary();
            routeValues.Add("grid-page", aspNetRolesViewModel.grid_page);
            routeValues.Add("grid-column", aspNetRolesViewModel.grid_column);
            routeValues.Add("grid-dir", aspNetRolesViewModel.grid_dir);
            routeValues.Add("PageSize", aspNetRolesViewModel.PageSize);

            log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
            log.Info(base.GetUserName + " updated role from " + oldRoleName + " to " + aspNetRolesViewModel.Name);

            return RedirectToAction("IndexParial", routeValues);

            //// getting all role
            //var aspNetRolesList = AspNetRolesRepo.GetAllRoles().ToList();
            //CommonMapper<AspNetRolesCore, AspNetRolesViewModel> mapper = new CommonMapper<AspNetRolesCore, AspNetRolesViewModel>();
            //List<AspNetRolesViewModel> rolesViewModel = mapper.MapToList(aspNetRolesList);

            //RoleManagementViewModel roleManagementViewModel = new RoleManagementViewModel();
            //roleManagementViewModel.aspNetRolesViewModelList = rolesViewModel;
            //roleManagementViewModel.PageSize = 10;

            //return PartialView("_GetRolesList", roleManagementViewModel);

        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult DeleteRole()
        {
            try
            {
                ModelState.Clear();

                Guid RoleId = Guid.Parse(Request.QueryString["RoleId"]);
                string PageSize = Request.QueryString["PageSize"];
                string grid_column = Request.QueryString["grid-column"];
                string grid_dir = Request.QueryString["grid-dir"];
                string grid_page = Request.QueryString["grid-page"];
                string searchKeyword = "";

                var aspNetRole = AspNetRolesRepo.GetAllRoles(searchKeyword).Where(m => m.Id == RoleId).FirstOrDefault();

                CommonMapper<AspNetRolesCore, AspNetRoles> mapper = new CommonMapper<AspNetRolesCore, AspNetRoles>();
                AspNetRoles aspNetRoles = mapper.Mapper(aspNetRole);

                // deleting dependent rolemodulemapping data first
                var roleModuleMapping = RoleModuleMappingRepo.GetModulesMappingByRole(RoleId).ToList();
                foreach (var item in roleModuleMapping)
                {
                    RoleModuleMappingRepo.DeAttach(item);
                    RoleModuleMappingRepo.DeleteState(item);
                    RoleModuleMappingRepo.Delete(item);
                    RoleModuleMappingRepo.Save();
                }

                // deleting dependent roleparentactionmapping data first
                var roleParentActionMapping = RoleParentActionMappingRepo.GetParentActionsByRole(RoleId).ToList();
                foreach (var item in roleParentActionMapping)
                {
                    RoleParentActionMappingRepo.DeAttach(item);
                    RoleParentActionMappingRepo.DeleteState(item);
                    RoleParentActionMappingRepo.Delete(item);
                    RoleParentActionMappingRepo.Save();
                }

                // deleting role
                aspNetRoles.IsDelete = true;
                AspNetRolesRepo.Edit(aspNetRoles);
                AspNetRolesRepo.Save();

                // creating route values
                var routeValues = new RouteValueDictionary();
                routeValues.Add("grid-page", grid_page);
                routeValues.Add("grid-column", grid_column);
                routeValues.Add("grid-dir", grid_dir);
                routeValues.Add("PageSize", PageSize);

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " deleted " + aspNetRole.Name);

                return RedirectToAction("IndexParial", routeValues);

                // getting all role
                //var aspNetRolesList = AspNetRolesRepo.GetAllRoles().ToList();
                //CommonMapper<AspNetRolesCore, AspNetRolesViewModel> mapper1 = new CommonMapper<AspNetRolesCore, AspNetRolesViewModel>();
                //List<AspNetRolesViewModel> rolesViewModel = mapper1.MapToList(aspNetRolesList);

                //RoleManagementViewModel roleManagementViewModel = new RoleManagementViewModel();
                //roleManagementViewModel.aspNetRolesViewModelList = rolesViewModel;
                //roleManagementViewModel.PageSize = 10;

                //return PartialView("_GetRolesList", roleManagementViewModel);
            }
            catch (Exception ex)
            {

                log.Error(base.GetUserName + ex.Message);
                throw;
            }
            finally
            {
                AspNetRolesRepo.Dispose();
            }
        }

        [HttpGet]
        public ActionResult ManageRolesActions()
        {
            try
            {
                Guid RoleId = Request.QueryString["RoleId"] != null ? Guid.Parse(Request.QueryString["RoleId"]) : Guid.Empty;
                List<ParentAction_MasterViewModel> parentActionViewModel = null;

                var moduleIds = RoleModuleMappingRepo.FindBy(m => m.RoleId == RoleId).Select(m => m.ModuleId.ToString()).ToList();
                string[] moduleIdArray = moduleIds.Count > 0 ? moduleIds.ToArray() : null;

                ManageRolesViewModel manageRolesViewModel = new ManageRolesViewModel();

                // get all modules for a role
                var getModulesByRole = ModuleMasterRepo.GetModulesByRole(RoleId);
                CommonMapper<ModuleCoreViewModel, Module_MasterViewModel> mapper1 = new CommonMapper<ModuleCoreViewModel, Module_MasterViewModel>();
                List<Module_MasterViewModel> moduleMasterViewModel = mapper1.MapToList(getModulesByRole.ToList());

                if (moduleIdArray != null)
                {
                    // get all actions for a role and modules
                    var getParentActions = ParentActionMasterRepo.GetParentActions(RoleId, moduleIdArray);
                    CommonMapper<ParentActionCoreViewModel, ParentAction_MasterViewModel> mapper2 = new CommonMapper<ParentActionCoreViewModel, ParentAction_MasterViewModel>();
                    parentActionViewModel = mapper2.MapToList(getParentActions.ToList());
                }

                manageRolesViewModel.ModulesList = moduleMasterViewModel;
                manageRolesViewModel.ActionsList = parentActionViewModel;
                manageRolesViewModel.RoleId = RoleId.ToString();

                return View(manageRolesViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult ManageRolesActions(ManageRolesViewModel manageRolesViewModel)
        {
            try
            {
                Guid roleId = Guid.Parse(manageRolesViewModel.RoleId);
                var roleName = AspNetRolesRepo.FindBy(m => m.Id == roleId).Select(m => m.Name).FirstOrDefault();
                var roleModulesList = RoleModuleMappingRepo.GetModulesMappingByRole(roleId).ToList();

                // deleting earlier modules assigned to particular role
                foreach (var roleModule in roleModulesList)
                {
                    RoleModuleMappingRepo.DeAttach(roleModule);
                    RoleModuleMappingRepo.DeleteState(roleModule);
                    RoleModuleMappingRepo.Delete(roleModule);
                    RoleModuleMappingRepo.Save();
                }

                if (manageRolesViewModel.ModulesList != null)
                {
                    var roleModuleCollection = manageRolesViewModel.ModulesList.Where(m => m.IsSelected == true).ToList();

                    // adding modules to a particular role
                    foreach (var roleModule in roleModuleCollection)
                    {
                        RoleModuleMapping roleModuleMapping = new RoleModuleMapping();
                        roleModuleMapping.Id = Guid.NewGuid();
                        roleModuleMapping.RoleId = roleId;
                        roleModuleMapping.ModuleId = roleModule.Id;
                        roleModuleMapping.CreatedDate = DateTime.Now;
                        roleModuleMapping.CreatedBy = Guid.Parse(base.GetUserId);

                        RoleModuleMappingRepo.Add(roleModuleMapping);
                        RoleModuleMappingRepo.Save();
                    }
                }

                var roleModuleActionList = RoleParentActionMappingRepo.GetParentActionsByRole(roleId).ToList();

                // deleting earlier actions assigned to particular role
                foreach (var roleModuleAction in roleModuleActionList)
                {
                    RoleParentActionMappingRepo.DeAttach(roleModuleAction);
                    RoleParentActionMappingRepo.DeleteState(roleModuleAction);
                    RoleParentActionMappingRepo.Delete(roleModuleAction);
                    RoleParentActionMappingRepo.Save();
                }

                if (manageRolesViewModel.ActionsList != null)
                {
                    var roleModuleActionCollection = manageRolesViewModel.ActionsList.Where(m => m.IsSelected == true).ToList();

                    // adding modules to a particular role
                    foreach (var roleModuleAction in roleModuleActionCollection)
                    {
                        RoleParentActionMapping roleParentActionMapping = new RoleParentActionMapping();
                        roleParentActionMapping.Id = Guid.NewGuid();
                        roleParentActionMapping.RoleId = roleId;
                        roleParentActionMapping.ParentActionMasterId = roleModuleAction.Id;
                        roleParentActionMapping.CreatedDate = DateTime.Now;
                        roleParentActionMapping.CreatedBy = Guid.Parse(base.GetUserId);

                        RoleParentActionMappingRepo.Add(roleParentActionMapping);
                        RoleParentActionMappingRepo.Save();
                    }
                }

                log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                log.Info(base.GetUserName + " managed permission of role " + roleName);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GetUserModules()
        {
            try
            {
                Guid RoleId = Request.QueryString["RoleId"] != null ? Guid.Parse(Request.QueryString["RoleId"]) : Guid.Empty;

                // get all modules for a role
                var getModulesByRole = ModuleMasterRepo.GetModulesByRole(RoleId);

                CommonMapper<ModuleCoreViewModel, Module_MasterViewModel> mapper = new CommonMapper<ModuleCoreViewModel, Module_MasterViewModel>();
                List<Module_MasterViewModel> moduleMasterViewModel = mapper.MapToList(getModulesByRole.ToList());


                return PartialView("_GetUserModules", moduleMasterViewModel);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult GetUserActions(ManageRolesViewModel manageRolesViewModel)
        {
            try
            {
                // needed to clear the state of the model otherwise model won't update correctly
                ModelState.Clear();

                Guid RoleId = Guid.Parse(manageRolesViewModel.RoleId);
                List<ParentAction_MasterViewModel> parentActionViewModel = null;

                var moduleIds = manageRolesViewModel.ModulesList.FindAll(m => m.IsSelected == true).Select(m => m.Id.ToString()).ToList();

                string[] moduleIdArray = moduleIds.Count > 0 ? moduleIds.ToArray() : null;

                if (moduleIdArray != null)
                {
                    // get all actions for a role and modules
                    var getParentActions = ParentActionMasterRepo.GetParentActions(RoleId, moduleIdArray);
                    CommonMapper<ParentActionCoreViewModel, ParentAction_MasterViewModel> mapper2 = new CommonMapper<ParentActionCoreViewModel, ParentAction_MasterViewModel>();
                    parentActionViewModel = mapper2.MapToList(getParentActions.ToList());
                }

                manageRolesViewModel.ActionsList = parentActionViewModel;

                return PartialView("_GetUserActions", manageRolesViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}