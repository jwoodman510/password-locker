using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ksu.PasswordLocker.Models;

namespace Ksu.PasswordLocker.Account
{
    public partial class Register : Page
    {
        private readonly ICompanyDal _companyDal = IoC.Resolve<ICompanyDal>();
        private readonly IUserDal _userDal = IoC.Resolve<IUserDal>();

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var company = _companyDal.Get(Company.Text);

            if (company == null)
            {
                ErrorMessage.Text = "Company not found.";
                return;
            }

            if (!string.IsNullOrEmpty(company.Domain))
            {
                if (!Email.Text.EndsWith(company.Domain))
                {
                    ErrorMessage.Text = $"Unauthorized company email. Must include the domain: {company.Domain}.";
                    return;
                }
            }

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var user = new AspNetUser { UserName = Email.Text, Email = Email.Text };
            var result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                AddRoleAndCompany(user, company.CompanyId);
                signInManager.SignIn(user, false, false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else 
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }

        private void AddRoleAndCompany(AspNetUser user, int companyId)
        {
            _userDal.AddToRole(user.Id, Roles.CompanyUser.Id);
            _companyDal.AddUser(companyId, user.Id);
        }
    }
}