namespace Ksu.Global.Constants
{
    public static class Permissions
    {
        public static bool CanAddDepartment(string roleId)
        {
            return roleId == Roles.SysAdmin.Id
                || roleId == Roles.CompanyAdmin.Id;
        }

        public static bool CanAddServer(string roleId)
        {
            return roleId == Roles.SysAdmin.Id
                || roleId == Roles.CompanyAdmin.Id;
        }

        public static bool CanAddUser(string roleId)
        {
            return roleId == Roles.SysAdmin.Id
                || roleId == Roles.CompanyAdmin.Id;
        }

        public static bool CanManageDepartments(string roleId)
        {
            return roleId == Roles.SysAdmin.Id
                || roleId == Roles.CompanyAdmin.Id;
        }

        public static bool CanManageServers(string roleId)
        {
            return roleId == Roles.SysAdmin.Id
                || roleId == Roles.CompanyAdmin.Id;
        }

        public static bool CanManageUsers(string roleId)
        {
            return roleId == Roles.SysAdmin.Id
                || roleId == Roles.CompanyAdmin.Id;
        }
    }
}
