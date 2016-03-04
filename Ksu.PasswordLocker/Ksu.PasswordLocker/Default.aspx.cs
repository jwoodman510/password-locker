using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.DataAccess.Exception;
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
            {
                // Not logged in.
                return;
            }

            AddLogin.Visible = true;
            SearchText.Visible = true;

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            SetPermissions(user);

            RoleName.Text = user?.RoleId?.Length > 0
                ? $"{user.CompanyName ?? "Logged In As"}: {Roles.GetName(user.RoleId)}"
                : string.Empty;

            InitializeDropdowns();
            RefreshGrid();
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
                RefreshGrid();
            }
            catch (Exception)
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

        private void RefreshGrid()
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            var logins = Permissions.IsAdmin(user.RoleId)
                ? _serverLoginDal.GetByCompany(user.CompanyId)
                : _serverLoginDal.GetByUser(userId);

            var flattened = logins
                ?.Select(l => new FlattenedServerLogin
                {
                    ServerLoginId = l.ServerLoginId,
                    ServerId = l.ServerId,
                    DepartmentId = l.DepartmentId,
                    UserName = l.UserName,
                    Password = CryptoKey.Decrypt(l.PasswordHash, Keys.Biscuits),
                    DepartmentName = l.Department.DepartmentName,
                    ServerName = l.Server.ServerName
                }).ToSafeList() ?? new List<FlattenedServerLogin>();

            LoginGrid.DataSource = string.IsNullOrEmpty(SearchText.Text)
                ? flattened
                : flattened.Where(l => l.UserName.ToLower().Contains(SearchText.Text.ToLower())).ToList();

            LoginGrid.DataBind();
        }

        protected void SearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void LoginGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridError.Text = string.Empty;

            var id = LoginGrid?.DataKeys[e.RowIndex]?.Value;

            if (id == null)
                return;

            try
            {
                _serverLoginDal.Delete((int)id);
                RefreshGrid();
            }
            catch (Exception)
            {
                GridError.Text = "An error occured while deleting the department.";
            }
        }

        protected void LoginGrid_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridError.Text = string.Empty;

            var id = LoginGrid?.DataKeys[e.RowIndex]?.Value;

            if (id == null)
                return;

            var newName = e.NewValues["UserName"].ToString();
            var newPw = e.NewValues["Password"].ToString();
            var encrypted = CryptoKey.Encrypt(newPw, Keys.Biscuits);

            var login = _serverLoginDal.Get((int)id);

            if (login.UserName == newName && login.PasswordHash == encrypted)
            {
                LoginGrid.EditIndex = -1;
                return;
            }

            try
            {
                _serverLoginDal.Update(new ServerLogin
                {
                    ServerLoginId = (int)id,
                    ServerId = login.ServerId,
                    DepartmentId = login.DepartmentId,
                    UserName = login.UserName,
                    PasswordHash = encrypted
                });
            }
            catch (ValidationException ex)
            {
                GridError.Text = ex.Message;
            }
            catch (NotFoundException ex)
            {
                GridError.Text = ex.Message;

            }
            catch (Exception)
            {
                GridError.Text = "An error occured while updating the department.";
            }
            finally
            {
                LoginGrid.EditIndex = -1;
                RefreshGrid();
            }
        }

        protected void LoginGrid_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridError.Text = string.Empty;
            LoginGrid.EditIndex = e.NewEditIndex;

            RefreshGrid();
        }

        protected void LoginGrid_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            LoginGrid.EditIndex = -1;
            RefreshGrid();
        }
    }
}