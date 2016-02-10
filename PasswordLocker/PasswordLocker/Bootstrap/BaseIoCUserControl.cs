using System;
using System.Web.UI;

namespace PasswordLocker.Bootstrap
{
    public class BaseIoCUserControl : UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            IoC.BuildUp(this);
            base.OnLoad(e);
        }
    }
}