using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FSM.Startup))]
namespace FSM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
