using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CastleApp.Startup))]
namespace CastleApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
