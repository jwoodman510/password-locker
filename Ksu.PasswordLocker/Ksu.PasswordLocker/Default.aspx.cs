using System;
using System.Web.UI;
using Ksu.Global.Constants;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;

namespace Ksu.PasswordLocker
{
    public partial class _Default : Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Context.User?.Identity?.Name))
                return;

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            SetPermissions(user);
            
            RoleName.Text = user?.RoleId?.Length > 0
                ? $"{user.CompanyName ?? "Logged In As"}: {Roles.GetName(user.RoleId)}"
                : string.Empty;
        }

        private void SetPermissions(CachedUser user)
        {
            ManageDepartments.Visible = Permissions.CanManageDepartments(user?.RoleId);
            ManageServers.Visible = Permissions.CanManageServers(user?.RoleId);
            ManageUsers.Visible = Permissions.CanManageUsers(user?.RoleId);

            ManageDepartments.ServerClick += (s, e) => { Response.Redirect("~/Manage/Departments.aspx"); };
            ManageServers.ServerClick += (s, e) => { Response.Redirect("~/Manage/Servers.aspx"); };
            ManageUsers.ServerClick += (s, e) => { Response.Redirect("~/Manage/Users.aspx"); };
        }
    }
}