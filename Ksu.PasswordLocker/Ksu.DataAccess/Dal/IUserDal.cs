using System;
using System.Collections.Generic;
using System.Linq;

namespace Ksu.DataAccess.Dal
{
    public interface IUserDal : IDisposable
    {
        AspNetUser Get(string id);
        AspNetUser GetByEmail(string email);
        IEnumerable<AspNetUser> GetByCompany(int companyId);

        AspNetRole GetRole(string id);
        void AddToRole(string id, string roleId);
    }

    public class UserDal : IUserDal
    {
        private readonly IContextProvider _context;
        public UserDal(IContextProvider context)
        {
            _context = context;
        }

        public AspNetUser Get(string id)
        {
            return _context.Users
                   .AsNoTracking()
                   .FirstOrDefault(u => u.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
        }

        public AspNetUser GetByEmail(string email)
        {
            return _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<AspNetUser> GetByCompany(int companyId)
        {
            return _context.Users
                .AsNoTracking()
                .Where(u => u.Companies.Any(c => c.CompanyId == companyId));
        }

        public AspNetRole GetRole(string id)
        {
            var user = _context.Users.Find(id);

            return user.AspNetRoles?.FirstOrDefault();
        }

        public void AddToRole(string id, string roleId)
        {
            var role = _context.Roles.Find(roleId);

            if (role.AspNetUsers.Any(u => u.Id == id))
                return;

            var user = _context.Users.Find(id);

            role.AspNetUsers = role.AspNetUsers ?? new List<AspNetUser>();
            role.AspNetUsers.Add(user);

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
