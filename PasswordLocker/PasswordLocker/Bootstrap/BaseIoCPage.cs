using System;
using System.Web.UI;

namespace PasswordLocker.Bootstrap
{
    public class BaseIoCPage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            IoC.BuildUp(this);
        }
    }
}