using System;
using System.Linq;
using System.Web.UI.WebControls;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using Ksu.DataAccess.Exception;
using Ksu.Global.Constants;
using Ksu.Global.Extensions;
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

            AddServer.Visible = true;
            ServerGrid.Visible = true;

            if (!Page.IsPostBack)
                RefreshGrid();
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
                RefreshGrid();
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

        private void RefreshGrid()
        {
            var user = _userCache.Get(Context.User.Identity.GetUserId());

            var servers = _serverDal.GetByCompany(user.CompanyId).ToSafeList();

            ServerGrid.DataSource = string.IsNullOrEmpty(SearchText.Text)
                ? servers
                : servers.Where(d => d.ServerName.ToLower().Contains(SearchText.Text.ToLower())).ToList();

            ServerGrid.DataBind();
        }

        protected void SearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void ServerGrid_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridError.Text = string.Empty;

            var id = ServerGrid?.DataKeys[e.RowIndex]?.Value;

            if (id == null)
                return;

            var newName = e.NewValues["ServerName"].ToString();

            var server = _serverDal.Get((int)id);

            if (server.ServerName == newName)
            {
                ServerGrid.EditIndex = -1;
                return;
            }

            server.ServerName = newName;

            try
            {
                _serverDal.Update(server);
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
                GridError.Text = "An error occured while updating the server.";
            }
            finally
            {
                ServerGrid.EditIndex = -1;
                RefreshGrid();
            }
        }

        protected void ServerGrid_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridError.Text = string.Empty;
            ServerGrid.EditIndex = e.NewEditIndex;
            RefreshGrid();
        }

        protected void ServerGrid_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            ServerGrid.EditIndex = -1;
            RefreshGrid();
        }

        protected void ServerGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            var index = Convert.ToInt32(e.CommandArgument);
            var id = ServerGrid?.DataKeys[index]?.Value;

            if (id == null)
                return;

            var serverId = (int)id;

            if (e.CommandName == "ManageDepartments")
                Response.Redirect($"~/Manage/ServerDepartments.aspx?{QueryStringParameters.ServerId}={serverId}");
        }

        protected void ServerGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridError.Text = string.Empty;

            var id = ServerGrid?.DataKeys[e.RowIndex]?.Value;

            if (id == null)
                return;

            try
            {
                _serverDal.Delete((int)id);
                RefreshGrid();
            }
            catch (Exception)
            {
                GridError.Text = "An error occured while deleting the server.";
            }
        }
    }
}