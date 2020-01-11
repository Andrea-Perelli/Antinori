using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Antinori.Startup))]
namespace Antinori {

    public partial class Startup {

        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}