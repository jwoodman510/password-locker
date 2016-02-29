using System;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;

namespace Ksu.PasswordLocker.Manage
{
    public partial class Servers : System.Web.UI.Page
    {
        private readonly IServerDal _serverDal = IoC.Resolve<IServerDal>();
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (!Permissions.CanManageServers(user?.RoleId))
                Response.Redirect("~/Default.aspx");

            AddServer.Visible = Permissions.CanAddDepartment(user?.RoleId);
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