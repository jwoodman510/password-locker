using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Ksu.PasswordLocker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Ksu.PasswordLocker
{
    public partial class _Default : Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();
        private readonly IServerDal _serverDal = IoC.Resolve<IServerDal>();
        private readonly IUserDal _userDal = IoC.Resolve<IUserDal>();
        private readonly ICompanyDal _companyDal = IoC.Resolve<ICompanyDal>();

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
            AddUser.Visible = Permissions.CanAddUser(user?.RoleId);

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

        protected void addUserSave_OnClick(object sender, EventArgs e)
        {
            var user = _userCache.Get(Context.User.Identity.GetUserId());
            var error = ValidateNewUser(user);

            if (!string.IsNullOrEmpty(error))
            {
                UserErrorMessage.Text = error;
                AddUserPopupExtender.Show();
                return;
            }

            var company = _companyDal.Get(user.CompanyName);
            if (!string.IsNullOrEmpty(company.Domain))
            {
                if (!UserEmailInput.Text.EndsWith(company.Domain))
                {
                    UserErrorMessage.Text = $"Unauthorized company email. Must include the domain: {company.Domain}.";
                    AddUserPopupExtender.Show();
                    return;
                }
            }

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var newUser = new AspNetUser { UserName = UserEmailInput.Text, Email = UserEmailInput.Text };
            var result = manager.Create(newUser, UserPasswordInput.Text);
            if (result.Succeeded)
            {
                AddRoleAndCompany(newUser, company.CompanyId);
                //signInManager.SignIn(user, false, false);
                ServErrorMessage.Text = string.Empty;
                ServerNameInput.Text = string.Empty;
            }
            else
            {
                AddUserPopupExtender.Show();
                UserErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }

        private string ValidateNewUser(CachedUser user)
        {
            if (user == null || !Permissions.CanAddUser(user.RoleId))
                return "Insufficient Permissions.";
            
            if (string.IsNullOrWhiteSpace(UserEmailInput.Text))
                return "Email cannot be empty.";

            if (string.IsNullOrWhiteSpace(UserPasswordInput.Text))
                return "Password cannot be empty.";

            if(!UserPasswordInput.Text.Equals(UserConfirmInput.Text))
                return "Passwords do not match.";

            if (_userDal.GetByEmail(UserEmailInput.Text) != null)
                return "User already exists.";

            return string.Empty;
        }

        private void AddRoleAndCompany(AspNetUser user, int companyId)
        {
            _userDal.AddToRole(user.Id, Roles.CompanyUser.Id);
            _companyDal.AddUser(companyId, user.Id);
        }
    }
}