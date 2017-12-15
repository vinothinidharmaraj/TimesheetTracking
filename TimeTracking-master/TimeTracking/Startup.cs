using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TimeTracking.Startup))]
namespace TimeTracking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
