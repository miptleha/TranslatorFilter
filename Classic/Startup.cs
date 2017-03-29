using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Classic.Startup))]
namespace Classic
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
