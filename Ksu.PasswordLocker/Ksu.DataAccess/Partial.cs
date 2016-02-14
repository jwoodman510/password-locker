using Microsoft.AspNet.Identity.EntityFramework;

namespace Ksu.DataAccess
{
    public partial class AspNetUser : IdentityUser
    {

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
