namespace Ksu.Global.Constants
{
    public static class Roles
    {
        public static class SysAdmin
        {
            public static string Id = "ceec4ff9-6af4-4057-9812-07b3383a1f16";
            public static string Name = "System Administrator";
        }

        public static class ReadonlySysAdmin
        {
            public static string Id = "dbf445d0-681d-40d2-871f-2ac6bb9b00ff";
            public static string Name = "System Administrator (Readonly)";
        }

        public static class CompanyAdmin
        {
            public static string Id = "6334386f-1e44-40b3-9a8a-e626f8993f64";
            public static string Name = "Company Administrator";
        }

        public static class CompanyUser
        {
            public static string Id = "d1a4c790-d6db-40c1-9c39-c56bacae6f37";
            public static string Name = "Company User";
        }

        public static string GetName(string id)
        {
            if (id == SysAdmin.Id)
                return SysAdmin.Name;

            if (id == ReadonlySysAdmin.Id)
                return ReadonlySysAdmin.Name;

            if (id == CompanyAdmin.Id)
                return CompanyAdmin.Name;

            if (id == CompanyUser.Id)
                return CompanyUser.Name;

            return "Unknown";
        }
    }
}
