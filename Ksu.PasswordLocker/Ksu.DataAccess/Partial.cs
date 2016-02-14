using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Ksu.DataAccess
{
    public partial class AspNetUser : IdentityUser
    {
        private ICollection<AspNetRole> _aspNetRoles;
        private ICollection<Company> _companies;
        private ICollection<Department> _departments;
        private ICollection<AspNetUserClaim> _aspNetUserClaims;
        private ICollection<AspNetUserLogin> _aspNetUserLogins;
    }

    public partial class Entities
    {
        public static Entities Create()
        {
            return new Entities();
        }
    }

    public partial class AspNetUserClaim : IdentityUserClaim
    {
        
    }

    public partial class AspNetUserLogin : IdentityUserLogin
    {

    }

    public partial class AspNetRole : IdentityRole
    {

    }
}
