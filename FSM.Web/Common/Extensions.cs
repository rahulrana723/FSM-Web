using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Common
{
    public static class Extensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static bool HasActionPermission(this ControllerBase controller, string ControllerName, string ModuleName, string ActionName, string RoleId)
        {
            try
            {
                var RoleManagement = new FMSRoleManagement(ModuleName, ControllerName, ActionName, Guid.Parse(RoleId));
                return RoleManagement.HasActionPermission();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool HasModulePermission(this ControllerBase controller, string ControllerName, string ModuleName, string ActionName, string RoleId)
        {
            try
            {
                var RoleManagement = new FMSRoleManagement(ModuleName, ControllerName, ActionName, Guid.Parse(RoleId));
                return RoleManagement.HasModulePermission();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool IsActiveModule(this ControllerBase controller,string ModuleName)
        {
            try
            {
                var baseController = (BaseController)controller;
                if (!baseController.GetUserRoles[0].Equals("Admin",StringComparison.OrdinalIgnoreCase))
                {
                    var existModule = baseController.GetModuleListByRole.FirstOrDefault(m => m.Contains(ModuleName));
                    if (string.IsNullOrEmpty(existModule))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string GetUserRole(this ControllerBase controller)
        {
            try
            {
                var baseController = (BaseController)controller;
                return baseController.GetUserRoles[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string UserNotificationCount(this ControllerBase controller)
        {
            try
            {
                var baseController = (BaseController)controller;
                return baseController.UserNotificationCount;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}