using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ksu.DataAccess.Exception;

namespace Ksu.DataAccess.Dal
{
    public interface IUserDal : IDisposable
    {
        AspNetUser Find(string id);
        AspNetUser Get(string id);
        AspNetUser GetByEmail(string email);
        IEnumerable<AspNetUser> GetByCompany(int companyId);

        AspNetRole GetRole(string id);
        void AddToRole(string id, string roleId);
        void Delete(string id);

        void AddDepartment(string userId, int departmentId);

        void RemoveDepartment(string userId, int departmentId);
    }

    public class UserDal : IUserDal
    {
        private readonly IContextProvider _context;
        public UserDal(IContextProvider context)
        {
            _context = context;
        }

        public AspNetUser Find(string id)
        {
            return _context.Users.Find(id);
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

        public void Delete(string id)
        {
            var existing = _context.Users
                   .Include("Companies")
                   .Include("Departments")
                   .Include("AspNetRoles")
                   .Include("AspNetUserLogins")
                   .Include("AspNetUserClaims")
                   .First(d => d.Id == id);

            if (existing == null)
                return;

            existing.Companies.Clear();
            existing.Departments.Clear();
            existing.AspNetRoles.Clear();
            existing.AspNetUserLogins.Clear();
            existing.AspNetUserClaims.Clear();

            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        public void AddDepartment(string userId, int departmentId)
        {
            var user = _context.Users
                   .Include("Departments")
                   .First(d => d.Id == userId);

            var department = _context.Departments.Find(departmentId);

            if (department == null)
                throw new NotFoundException("Department not found.");
            
            if (user.Companies.All(c => c.CompanyId != department.CompanyId))
                throw new ValidationException("Invalid company department.");

            if (user.Departments == null)
                user.Departments = new List<Department>();

            user.Departments.Add(department);

            _context.SaveChanges();
        }

        public void RemoveDepartment(string userId, int departmentId)
        {
            var user = _context.Users
                   .Include("Departments")
                   .First(d => d.Id == userId);

            var found = user.Departments?.FirstOrDefault(u => u.DepartmentId.Equals(departmentId));

            if (found == null)
                return;

            user.Departments.Remove(found);

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
