using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TenantMNG.Startup))]
namespace TenantMNG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
