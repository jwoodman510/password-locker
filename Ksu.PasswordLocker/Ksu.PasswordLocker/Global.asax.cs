using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Ksu.PasswordLocker.Bootstrap;

namespace Ksu.PasswordLocker
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            IoC.BuildUp(this);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}