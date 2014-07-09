using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Super_BE.Utilities.WWW.Startup))]
namespace Super_BE.Utilities.WWW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
