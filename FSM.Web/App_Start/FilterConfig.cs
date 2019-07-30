using FSM.Web.Common;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new FSMSessionExpiresAttribute());
            filters.Add(new RolePermissionsAttribute());
            filters.Add(new FSMHandleErrorAttribute());
          //  filters.Add(new NoCache());
        }
    }
}
