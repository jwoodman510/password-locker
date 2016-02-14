using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Ksu.PasswordLocker
{
    public partial class _Default : Page
    {
        private const string LoggedInAs = "Logged In As:";

        private readonly IUserDal _userDal = IoC.Resolve<IUserDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Context.User?.Identity?.Name))
                RoleName.Text = $"{LoggedInAs} Anonymous User";
            else
            {
                var userId = Context.User.Identity.GetUserId();
                var role = _userDal.GetRole(userId);

                RoleName.Text = $"{LoggedInAs} {Roles.GetName(role?.Id)}";
            }
        }
    }
}