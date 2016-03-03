using System;
using System.Web.UI;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Global.Extensions;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;

namespace Ksu.PasswordLocker
{
    public partial class _Default : Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IServerDal _serverDal = IoC.Resolve<IServerDal>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                return;

            if (string.IsNullOrEmpty(Context.User?.Identity?.Name))
                return;

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            SetPermissions(user);

            RoleName.Text = user?.RoleId?.Length > 0
                ? $"{user.CompanyName ?? "Logged In As"}: {Roles.GetName(user.RoleId)}"
                : string.Empty;

            InitializeDropdowns();
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

        private void InitializeDropdowns()
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);
            var companyId = user.CompanyId;

            _serverDal.GetByCompany(companyId)
                .ToSafeList()
                .ForEach(s => ServerDropDown.Items.Add(s.ServerName));

            _departmentDal.GetByCompany(companyId)
                .ToSafeList()
                .ForEach(d => DepartmentDropDown.Items.Add(d.DepartmentName));
        }
        
        protected void addLoginSave_OnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}