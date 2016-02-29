using System;
using System.Linq;
using System.Web;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Ksu.PasswordLocker.Manage
{
    public partial class Users : System.Web.UI.Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IUserDal _userDal = IoC.Resolve<IUserDal>();
        private readonly ICompanyDal _companyDal = IoC.Resolve<ICompanyDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if(!Permissions.CanManageUsers(user?.RoleId))
                Response.Redirect("~/Default.aspx");

            AddUser.Visible = Permissions.CanAddUser(user?.RoleId);
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
            var newUser = new AspNetUser { UserName = UserEmailInput.Text, Email = UserEmailInput.Text };
            var result = manager.Create(newUser, UserPasswordInput.Text);
            if (result.Succeeded)
            {
                AddRoleAndCompany(newUser, company.CompanyId);
                UserErrorMessage.Text = string.Empty;
                UserEmailInput.Text = string.Empty;
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

            if (!UserPasswordInput.Text.Equals(UserConfirmInput.Text))
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