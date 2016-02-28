using Ksu.DataAccess.Dal;
using Ksu.Global.Constants;
using Ksu.Model;
using Ksu.PasswordLocker.Bootstrap;
using Ksu.PasswordLocker.Identity;
using Microsoft.AspNet.Identity;
using System;

namespace Ksu.PasswordLocker.Manage
{
    public partial class DepartmentServers : System.Web.UI.Page
    {
        private readonly IUserCache _userCache = IoC.Resolve<IUserCache>();
        private readonly IDepartmentDal _departmentDal = IoC.Resolve<IDepartmentDal>();

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
                    InitializeDepartment(user);
                }
                catch(Exception)
                {
                    // ignore
                }
            }
        }

        private void InitializeDepartment(CachedUser user)
        {
            var deptId = Request.QueryString[QueryStringParameters.DepartmentId];
            var dept = _departmentDal.Get(int.Parse(deptId));

            if (dept.CompanyId != user.CompanyId)
                return;

            DepartmentName.Text = $"<h1>{dept.DepartmentName}</h1>";
        }
    }
}