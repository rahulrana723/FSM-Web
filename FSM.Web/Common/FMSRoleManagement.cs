using FSM.Core.Interface;
using FSM.Infrastructure.Repository;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Common
{
    public class FMSRoleManagement
    {
        [Dependency]
        public IRoleModuleMappingRepository RoleModuleMappingRepo { get; set; }
        [Dependency]
        public IModule_MasterRepository ModuleMasterRepo { get; set; }
        [Dependency]
        public IAction_MasterRepository ActionMasterRepo { get; set; }
        [Dependency]
        public IRoleModuleActionMappingRepository RoleModuleActionMappingRepo { get; set; }
        [Dependency]
        public IRoleParentActionMappingRepository RoleParentActionMappingRepo { get; set; }
        [Dependency]
        public IModule_ActionDefaultMaster ModuleActionDefaultMasterRepo { get; set; }
        public string ModuleName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Guid RoleId { get; set; }

        public FMSRoleManagement(string _moduleName, string _controllerName, string _actionName, Guid _roleId)
        {
            this.ModuleName = _moduleName;
            this.ControllerName = _controllerName;
            this.ActionName = _actionName;
            this.RoleId = _roleId;
            this.RoleModuleMappingRepo = new RoleModuleMappingRepository();
            this.ModuleMasterRepo = new Module_MasterRepository();
            this.ActionMasterRepo = new Action_MasterRepository();
            this.RoleModuleActionMappingRepo = new RoleModuleActionMappingRepository();
            this.RoleParentActionMappingRepo = new RoleParentActionMappingRepository();
            this.ModuleActionDefaultMasterRepo = new ModuleActionMasterRepository();
        }
        public bool HasModulePermission()
        {
            var moduleId = ModuleMasterRepo.FindBy(m => m.ModuleName == this.ModuleName).Select(m => m.Id)
                           .FirstOrDefault();
            var hasModule = RoleModuleMappingRepo.GetModulesMappingByRole(this.RoleId).Where(m => m.ModuleId == moduleId)
                           .FirstOrDefault();
            if (hasModule == null)
            {
                return false;
            }
            return true;
        }
        public bool HasActionPermission()
        {
            var defaultAction = ModuleActionDefaultMasterRepo.GetDefaultAction(this.ModuleName, this.ControllerName, this.ActionName)
                                .FirstOrDefault();
            if (defaultAction != null)
            {
                return true;
            }

            var hasAction = RoleParentActionMappingRepo.GetActionsByRole(this.RoleId, this.ControllerName, this.ActionName)
                            .FirstOrDefault();
            if (hasAction == null)
            {
                return false;
            }

            return true;
        }
    }
}