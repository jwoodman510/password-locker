using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Ksu.PasswordLocker.Startup))]
namespace Ksu.PasswordLocker
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
