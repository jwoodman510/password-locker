using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Global.Extensions;
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

            AddUser.Visible = true;
            UserGrid.Visible = true;

            if (!Page.IsPostBack)
                RefreshGrid();
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
                UserPasswordInput.Text = string.Empty;
                UserConfirmInput.Text = string.Empty;
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

        private void RefreshGrid()
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            var users = _userDal.GetByCompany(user.CompanyId)
                .Where(u => !u.Id.Equals(userId))
                .ToSafeList();

            UserGrid.DataSource = string.IsNullOrEmpty(SearchText.Text)
                ? users
                : users.Where(d => d.UserName.ToLower().Contains(SearchText.Text.ToLower())).ToList();

            UserGrid.DataBind();
        }

        private void AddRoleAndCompany(AspNetUser user, int companyId)
        {
            _userDal.AddToRole(user.Id, Roles.CompanyUser.Id);
            _companyDal.AddUser(companyId, user.Id);
        }

        protected void SearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void UserGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridError.Text = string.Empty;

            var id = UserGrid?.DataKeys[e.RowIndex]?.Value;

            if (id == null)
                return;

            var userId = id.ToString();
            
            try
            {
                _userDal.Delete(userId);
                _userCache.Remove(userId);
                RefreshGrid();
            }
            catch (Exception)
            {
                GridError.Text = "An error occured while deleting the user.";
            }
        }

        protected void UserGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            var index = Convert.ToInt32(e.CommandArgument);
            var id = UserGrid?.DataKeys[index]?.Value;

            if (id == null)
                return;
            
            var userId = id.ToString();

            if (e.CommandName == "ManageDepartments")
                Response.Redirect($"~/Manage/UserDepartments.aspx?{QueryStringParameters.UserId}={userId}");
        }
    }
}