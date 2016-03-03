using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class UserDepartments : System.Web.UI.Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();
        private readonly IUserDal _userDal = IoC.Resolve<IUserDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (!Permissions.CanManageUsers(user?.RoleId))
                Response.Redirect("~/Default.aspx");

            if (!Page.IsPostBack)
                RefreshGrid();
        }

        private void RefreshGrid()
        {
            var userId = Request.QueryString[QueryStringParameters.UserId];
            var user = _userDal.Get(userId);
            var companyId = user.Companies.FirstOrDefault()?.CompanyId ?? 0;

            var adminUserId = Context.User.Identity.GetUserId();
            var admin = _userCache.Get(adminUserId);

            if (companyId != admin.CompanyId)
                return;

            UserName.Text = $"<h1 style=\"padding-left:10px\">{user.UserName}</h1>";

            var addedDepts = user.Departments?.ToList()
                ?? new List<Department>();

            var notAdded = _departmentDal.GetByCompany(companyId)
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
                var userId = Request.QueryString[QueryStringParameters.UserId];

                _userDal.AddDepartment(userId, departmentId);

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
            var userId = Request.QueryString[QueryStringParameters.UserId];

            _userDal.RemoveDepartment(userId, departmentId);

            RefreshGrid();
        }
    }
}