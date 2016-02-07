using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PasswordLocker.Startup))]
namespace PasswordLocker
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
