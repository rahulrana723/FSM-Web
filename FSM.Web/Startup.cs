using FSM.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

[assembly: OwinStartupAttribute(typeof(FSM.Web.Startup))]
namespace FSM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User 
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin rool
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                        

                var user = new ApplicationUser();
                user.UserName = "admin";
                user.Email = "tsingh@seasiainfotech.com";
                string userPWD = "Mind@123";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role 
            if (!roleManager.RoleExists("OPERATIONS"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "OPERATIONS";
                roleManager.Create(role);
            }

            // creating Creating Employee role 
            if (!roleManager.RoleExists("COMMS_INV"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "COMMS_INV";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("OTRW"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "OTRW";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("ACCOUNTS"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "ACCOUNTS";
                roleManager.Create(role);
            }
        }
    }
}
