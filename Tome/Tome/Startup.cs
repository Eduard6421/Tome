using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tome.Startup))]
namespace Tome
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
