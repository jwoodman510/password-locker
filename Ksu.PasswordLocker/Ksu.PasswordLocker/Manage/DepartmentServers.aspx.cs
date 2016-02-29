using Ksu.DataAccess.Comparers;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Ksu.DataAccess;

namespace Ksu.PasswordLocker.Manage
{
    public partial class DepartmentServers : System.Web.UI.Page
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
                catch(Exception)
                {
                    // ignore
                }
            }
        }

        private void RefreshGrid()
        {
            var deptId = int.Parse(Request.QueryString[QueryStringParameters.DepartmentId]);
            var dept = _departmentDal.Get(deptId);

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (dept.CompanyId != user.CompanyId)
                return;

            DepartmentName.Text = $"<h1 style=\"padding-left:10px\">{dept.DepartmentName}</h1>";

            var addedServers = dept.Servers?.ToList()
                ?? new List<Server>();

            var notAdded = _serverDal.GetByCompany(dept.CompanyId)
                ?.Except(addedServers, new ServerComparer(CompareSetting.CompareIds)).ToList()
                ?? new List<Server>();
            
            if (!string.IsNullOrEmpty(AllSearchText.Text))
                notAdded = notAdded.Where(s => s.ServerName.ToLower().Contains(AllSearchText.Text.ToLower())).ToList();
            
            if (!string.IsNullOrEmpty(AddedSearchText.Text))
                addedServers = addedServers.Where(s => s.ServerName.ToLower().Contains(AddedSearchText.Text.ToLower())).ToList();

            AllGrid.DataSource = notAdded;
            AllGrid.DataBind();

            AddedGrid.DataSource = addedServers;
            AddedGrid.DataBind();
        }

        protected void AllSearchTextButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void AddedSearchTextButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void AddedGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var dataKey = AddedGrid.DataKeys[e.RowIndex];

            if (dataKey == null)
                return;
            var serverId = (int) dataKey.Value;
            var departmentId = int.Parse(Request.QueryString[QueryStringParameters.DepartmentId]);

            _departmentDal.RemoveServer(departmentId, serverId);

            RefreshGrid();
        }

        protected void AllGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddServer")
            {
                var index = Convert.ToInt32(e.CommandArgument);
                var dataKey = AllGrid.DataKeys[index];

                if (dataKey == null)
                    return;

                var serverId = (int) dataKey.Value;
                var departmentId = int.Parse(Request.QueryString[QueryStringParameters.DepartmentId]);

                _departmentDal.AddServer(departmentId, serverId);

                RefreshGrid();
            }
        }
    }
}