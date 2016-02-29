using System;
using System.Linq;
using System.Web.UI.WebControls;
using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Global.Extensions;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;

namespace Ksu.PasswordLocker.Manage
{
    public partial class Departments : System.Web.UI.Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();

        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Context.User.Identity.GetUserId();
            var user = _userCache.Get(userId);

            if (!Permissions.CanManageDepartments(user?.RoleId))
                Response.Redirect("~/Default.aspx");

            AddDepartment.Visible = true;
            DeptGrid.Visible = true;

            if (!Page.IsPostBack)
                RefreshGrid();
        }
        
        protected void addDepartmentSave_OnClick(object sender, EventArgs e)
        {
            var user = _userCache.Get(Context.User.Identity.GetUserId());
            var error = ValidateNewDepartment(user);

            if (!string.IsNullOrEmpty(error))
            {
                DepErrorMessage.Text = error;
                AddDepartmentPopupExtender.Show();
                return;
            }

            try
            {
                _departmentDal.Create(new DataAccess.Department
                {
                    CompanyId = user.CompanyId,
                    DepartmentName = DepartmentNameInput.Text
                });
            }
            catch (Exception)
            {
                DepErrorMessage.Text = "Error occured while creating department.";
                AddDepartmentPopupExtender.Show();
            }
            finally
            {
                DepErrorMessage.Text = string.Empty;
                DepartmentNameInput.Text = string.Empty;
                RefreshGrid();
            }
        }

        private string ValidateNewDepartment(CachedUser user)
        {
            if (user == null || !Permissions.CanAddDepartment(user.RoleId))
                return "Insufficient Permissions.";

            if (string.IsNullOrWhiteSpace(DepartmentNameInput.Text))
                return "Department name cannot be empty.";

            if (_departmentDal.GetByName(DepartmentNameInput.Text, user.CompanyId) != null)
                return "Department with the same name already exists.";

            return string.Empty;
        }

        private void RefreshGrid()
        {
            var user = _userCache.Get(Context.User.Identity.GetUserId());

            var depts = _departmentDal.GetByCompany(user.CompanyId)?.Select(d => new Department
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName
            }).ToSafeList();

            DeptGrid.DataSource = string.IsNullOrEmpty(SearchText.Text)
                ? depts
                : depts.Where(d => d.DepartmentName.ToLower().Contains(SearchText.Text.ToLower())).ToList();

            DeptGrid.DataBind();
        }

        protected void DeptGrid_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridError.Text = string.Empty;
            var row = DeptGrid.Rows[e.RowIndex];
        }

        protected void DeptGrid_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridError.Text = string.Empty;
            var row = DeptGrid.Rows[e.RowIndex];
        }

        protected void DeptGrid_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            GridError.Text = string.Empty;
            DeptGrid.EditIndex = e.NewEditIndex;
            RefreshGrid();
        }

        protected void DeptGrid_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            DeptGrid.EditIndex = -1;
            RefreshGrid();
        }

        protected void DeptGrid_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ManageServers")
            {
                var index = Convert.ToInt32(e.CommandArgument);
                var id = DeptGrid.DataKeys[index].Value;

                Response.Redirect($"~/Manage/DepartmentServers.aspx?{QueryStringParameters.DepartmentId}={(int)id}");
            }
        }

        protected void SearchTextButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}