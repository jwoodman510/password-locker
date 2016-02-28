using System;
using System.Web.UI;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
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
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();
        private readonly IServerDal _serverDal = IoC.Resolve<IServerDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            AddDepartment.Visible = false;
            AddServer.Visible = false;

            if (string.IsNullOrEmpty(Context.User?.Identity?.Name))
                return;

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            AddDepartment.Visible = Permissions.CanAddDepartment(user?.RoleId);
            AddServer.Visible = Permissions.CanAddDepartment(user?.RoleId);

            RoleName.Text = user?.RoleId?.Length > 0
                ? $"{user.CompanyName ?? "Logged In As"}: {Roles.GetName(user.RoleId)}"
                : string.Empty;
        }

        protected void addDepartmentSave_OnClick(object sender, EventArgs e)
        {
            var user = _userCache.Get(Context.User.Identity.GetUserId());
            var error = ValidateNewDepartment(user);

            if (!string.IsNullOrEmpty(error))
            {
                DepErrorMessage.Text = error;
                AddDepartmentPopupExtender.Show();
                return;
            }

            try
            {
                _departmentDal.Create(new Department
                {
                    CompanyId = user.CompanyId,
                    DepartmentName = DepartmentNameInput.Text
                });
            }
            catch (Exception)
            {
                DepErrorMessage.Text = "Error Occured while creating department.";
                AddDepartmentPopupExtender.Show();
            }
            finally
            {
                DepErrorMessage.Text = string.Empty;
                DepartmentNameInput.Text = string.Empty;
            }
        }

        private string ValidateNewDepartment(CachedUser user)
        {
            if (user == null || !Permissions.CanAddDepartment(user.RoleId))
                return "Insufficient Permissions.";

            if (string.IsNullOrWhiteSpace(DepartmentNameInput.Text))
                return "Department name cannot be empty.";

            if (_departmentDal.GetByName(DepartmentNameInput.Text, user.CompanyId) != null)
                return "Department with the same name already exists.";

            return string.Empty;
        }

        protected void addServerSave_OnClick(object sender, EventArgs e)
        {
            var user = _userCache.Get(Context.User.Identity.GetUserId());
            var error = ValidateNewServer(user);

            if (!string.IsNullOrEmpty(error))
            {
                ServErrorMessage.Text = error;
                AddServerPopupExtender.Show();
                return;
            }
            
            try
            {
                _serverDal.Create(new Server
                {
                    CompanyId = user.CompanyId,
                    ServerName = ServerNameInput.Text
                });
            }
            catch (Exception)
            {
                ServErrorMessage.Text = "Error Occured while creating server.";
                AddServerPopupExtender.Show();
            }
            finally
            {
                ServErrorMessage.Text = string.Empty;
                ServerNameInput.Text = string.Empty;
            }
        }

        private string ValidateNewServer(CachedUser user)
        {
            if (user == null || !Permissions.CanAddServer(user.RoleId))
                return "Insufficient Permissions.";

            if (string.IsNullOrWhiteSpace(ServerNameInput.Text))
                return "Server name cannot be empty.";

            if (_serverDal.GetByName(ServerNameInput.Text, user.CompanyId) != null)
                return "Server with the same name already exists.";

            return string.Empty;
        }
    }
}