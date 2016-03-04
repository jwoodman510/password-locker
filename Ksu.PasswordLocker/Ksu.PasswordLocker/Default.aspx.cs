using System;
using System.Web.UI;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.Encryption;
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
        private readonly IServerLoginDal _serverLoginDal = IoC.Resolve<IServerLoginDal>();

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

            ServerDropDown.DataSource = _serverDal.GetByCompany(companyId).ToSafeList();
            ServerDropDown.DataBind();

            DepartmentDropDown.DataSource = _departmentDal.GetByCompany(companyId).ToSafeList();
            DepartmentDropDown.DataBind();
        }
        
        protected void addLoginSave_OnClick(object sender, EventArgs e)
        {
            var serverId = int.Parse(ServerDropDown.SelectedValue);
            var departmentId = int.Parse(DepartmentDropDown.SelectedValue);

            var error = ValidateNewLogin(serverId, departmentId);

            if (!string.IsNullOrEmpty(error))
            {
                LoginErrorMessage.Text = error;
                AddLoginPopupExtender.Show();
                return;
            }

            try
            {
                var encryptedPassword = CryptoKey.Encrypt(UserPasswordInput.Text, Keys.Biscuits);

                _serverLoginDal.Create(new ServerLogin
                {
                    ServerId = serverId,
                    DepartmentId = departmentId,
                    UserName = UserNameInput.Text,
                    PasswordHash = encryptedPassword
                });

                LoginErrorMessage.Text = string.Empty;
                UserNameInput.Text = string.Empty;
                UserPasswordInput.Text = string.Empty;
                UserConfirmInput.Text = string.Empty;

                InitializeDropdowns();
            }
            catch (Exception ex)
            {
                LoginErrorMessage.Text = "Error occured while creating login.";
                AddLoginPopupExtender.Show();
            }
        }

        private string ValidateNewLogin(int serverId, int departmentId)
        {
            if (string.IsNullOrWhiteSpace(UserNameInput.Text))
                return "User Name cannot be empty.";

            if (string.IsNullOrWhiteSpace(UserPasswordInput.Text))
                return "Password cannot be empty.";

            if (!UserPasswordInput.Text.Equals(UserConfirmInput.Text))
                return "Passwords do not match.";

            if (_serverLoginDal.GetByUserName(UserNameInput.Text, departmentId, serverId) != null)
                return "User Name already exists.";

            return string.Empty;
        }
    }
}