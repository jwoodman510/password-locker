using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ksu.DataAccess;
using Ksu.DataAccess.Comparers;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;

namespace Ksu.PasswordLocker.Manage
{
    public partial class ServerDepartments : Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();
        private readonly IServerDal _serverDal = IoC.Resolve<IServerDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (!Permissions.CanManageDepartments(user?.RoleId))
                Response.Redirect("~/Default.aspx");

            if (!Page.IsPostBack)
            {
                try
                {
                    RefreshGrid();
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }

        private void RefreshGrid()
        {
            var serverId = int.Parse(Request.QueryString[QueryStringParameters.ServerId]);
            var server = _serverDal.Get(serverId);

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (server.CompanyId != user.CompanyId)
                return;

            ServerName.Text = $"<h1 style=\"padding-left:10px\">{server.ServerName}</h1>";

            var addedDepts = server.Departments?.ToList()
                ?? new List<Department>();

            var notAdded = _departmentDal.GetByCompany(server.CompanyId)
                ?.Except(addedDepts, new DepartmentComparer(CompareSetting.CompareIds)).ToList()
                ?? new List<Department>();

            if (!string.IsNullOrEmpty(AllSearchText.Text))
                notAdded = notAdded.Where(s => s.DepartmentName.ToLower().Contains(AllSearchText.Text.ToLower())).ToList();

            if (!string.IsNullOrEmpty(AddedSearchText.Text))
                addedDepts = addedDepts.Where(s => s.DepartmentName.ToLower().Contains(AddedSearchText.Text.ToLower())).ToList();

            AllGrid.DataSource = notAdded;
            AllGrid.DataBind();

            AddedGrid.DataSource = addedDepts;
            AddedGrid.DataBind();
        }

        protected void AllSearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void AllGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddDepartment")
            {
                var index = Convert.ToInt32(e.CommandArgument);
                var dataKey = AllGrid.DataKeys[index];

                if (dataKey == null)
                    return;

                var departmentId = (int)dataKey.Value;
                var serverId = int.Parse(Request.QueryString[QueryStringParameters.ServerId]);

                _serverDal.AddDepartment(serverId, departmentId);

                RefreshGrid();
            }
        }

        protected void AddedSearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void AddedGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var dataKey = AddedGrid.DataKeys[e.RowIndex];

            if (dataKey == null)
                return;

            var departmentId = (int)dataKey.Value;
            var serverId = int.Parse(Request.QueryString[QueryStringParameters.ServerId]);

            _serverDal.RemoveDepartment(serverId, departmentId);

            RefreshGrid();
        }
    }
}