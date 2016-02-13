using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.PasswordLocker.Bootstrap;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Ksu.PasswordLocker.Models;

namespace Ksu.PasswordLocker.Account
{
    public partial class Register : Page
    {
        private readonly ICompanyDal _companyDal = IoC.Resolve<ICompanyDal>();

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
            var user = new ApplicationUser { UserName = Email.Text, Email = Email.Text };
            var result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                AddRoleAndCompany(user, manager, company.CompanyId);
                signInManager.SignIn(user, false, false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else 
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }

        private void AddRoleAndCompany(ApplicationUser user, ApplicationUserManager manager, int companyId)
        {
            manager.AddToRole(user.Id, Roles.CompanyUser);
            _companyDal.AddUser(companyId, user.Id);
        }
    }
}