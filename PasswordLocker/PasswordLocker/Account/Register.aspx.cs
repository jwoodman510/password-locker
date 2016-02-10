using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using DataAccess.Dal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Practices.Unity;
using PasswordLocker.Models;

namespace PasswordLocker.Account
{
    public partial class Register : Page
    {
        [Dependency] public ICompanyDal CompanyDal { get; set; }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var companyId = CompanyDal.Get(Company.Text)?.CompanyId ?? 0;

            if (companyId < 1)
            {
                ErrorMessage.Text = "Invalid Company";
                return;
            }

            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var user = new ApplicationUser { UserName = Email.Text, Email = Email.Text };

            var result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                AddUserToCompany(companyId);

                //todo: finish and validate
                manager.AddToRole(user.Id, "");

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //string code = manager.GenerateEmailConfirmationToken(user.Id);
                //string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                //manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                signInManager.SignIn( user, isPersistent: false, rememberBrowser: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else 
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }

        private void AddUserToCompany(int companyId)
        {
            //todo: this
        }
    }
}