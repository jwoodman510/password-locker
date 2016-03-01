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
    public partial class DepartmentUsers : Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();
        private readonly IUserDal _userDal = IoC.Resolve<IUserDal>();

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
            var deptId = int.Parse(Request.QueryString[QueryStringParameters.DepartmentId]);
            var dept = _departmentDal.Get(deptId);

            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (dept.CompanyId != user.CompanyId)
                return;

            DepartmentName.Text = $"<h1 style=\"padding-left:10px\">{dept.DepartmentName}</h1>";

            var addedUsers = dept.AspNetUsers?.ToList()
                ?? new List<AspNetUser>();

            var notAdded = _userDal.GetByCompany(dept.CompanyId)
                ?.Except(addedUsers, new UserComparer(CompareSetting.CompareIds)).ToList()
                ?? new List<AspNetUser>();

            if (!string.IsNullOrEmpty(AllSearchText.Text))
                notAdded = notAdded.Where(s => s.UserName.ToLower().Contains(AllSearchText.Text.ToLower())).ToList();

            if (!string.IsNullOrEmpty(AddedSearchText.Text))
                addedUsers = addedUsers.Where(s => s.UserName.ToLower().Contains(AddedSearchText.Text.ToLower())).ToList();

            AllGrid.DataSource = notAdded.Where(n => !n.Id.Equals(userId)).ToList();
            AllGrid.DataBind();

            AddedGrid.DataSource = addedUsers;
            AddedGrid.DataBind();
        }

        protected void AllSearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        protected void AllGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddUser")
            {
                var index = Convert.ToInt32(e.CommandArgument);
                var dataKey = AllGrid.DataKeys[index];

                if (dataKey == null)
                    return;

                var userId = dataKey.Value.ToString();
                var departmentId = int.Parse(Request.QueryString[QueryStringParameters.DepartmentId]);

                _departmentDal.AddUser(departmentId, userId);

                RefreshGrid();
            }
        }

        protected void AddedGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var dataKey = AddedGrid.DataKeys[e.RowIndex];

            if (dataKey == null)
                return;

            var userId = dataKey.Value.ToString();
            var departmentId = int.Parse(Request.QueryString[QueryStringParameters.DepartmentId]);

            _departmentDal.RemoveUser(departmentId, userId);

            RefreshGrid();
        }

        protected void AddedSearchTextButton_OnClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}