using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ksu.DataAccess.Comparers;
using Ksu.DataAccess.Exception;
using Ksu.Global.Extensions;

namespace Ksu.DataAccess.Dal
{
    public interface IDepartmentDal : IDisposable
    {
        Department Get(int id);

        Department GetByName(string name, int companyId);

        IEnumerable<Department> GetByCompany(int companyId);

        IEnumerable<Department> GetByUser(string userId);

        Department Create(Department department);

        void Delete(int departmentId);
    }

    public class DepartmentDal : IDepartmentDal
    {
        private readonly IContextProvider _context;
        public DepartmentDal(IContextProvider context)
        {
            _context = context;
        }
        
        public void Dispose()
        {
            _context?.Dispose();
        }

        public Department Get(int id)
        {
            return _context.Departments
                .AsNoTracking()
                .AsQueryable()
                .FirstOrDefault(d => d.DepartmentId == id);
        }

        public Department GetByName(string name, int companyId)
        {
            return _context.Departments
                .AsNoTracking()
                .AsQueryable()
                .FirstOrDefault(d =>
                    d.CompanyId == companyId &&
                    d.DepartmentName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Department> GetByCompany(int companyId)
        {
            return _context.Departments
                .AsNoTracking()
                .AsQueryable()
                .Where(d => d.CompanyId == companyId);
        }

        public IEnumerable<Department> GetByUser(string userId)
        {
            return _context.Departments
                .AsNoTracking()
                .AsQueryable()
                .Where(d => d.AspNetUsers.Any(u => u.Id == userId));
        }

        public Department Create(Department department)
        {
            if(department == null)
                throw new ArgumentNullException(nameof(department));

            if(department.CompanyId < 1)
                throw new ValidationException("Missing companyId");

            if (string.IsNullOrEmpty(department.DepartmentName))
                throw new ValidationException("Department name too long.");

            var existing = GetByCompany(department.CompanyId)
                .FirstOrDefault(d => d.DepartmentName == department.DepartmentName);

            if(existing != null)
                throw new ValidationException("Department with same name already exists.");

            var result = new Department
            {
                DepartmentName = department.DepartmentName,
                CompanyId = department.CompanyId
            };

            _context.Departments.Add(result);

            _context.SaveChanges();
            
            return result;
        }

        public void Delete(int departmentId)
        {
            var previous = Get(departmentId);

            if (previous == null)
                return;

            var toDelete = new Department
            {
                DepartmentId = departmentId
            };

            UpdateUsers(toDelete, previous);
            UpdateServerLoginMappings(toDelete, previous);
            UpdateServerMappings(toDelete, previous);

            var existing = _context.Departments.Find(departmentId);
            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        private void UpdateUsers(Department current, Department previous)
        {
            var previousUsers = previous.AspNetUsers.ToSafeList();
            var currentUsers = current.AspNetUsers.ToSafeList();

            var comparer = new UserComparer(CompareSetting.CompareIds);

            var added = currentUsers.Except(previousUsers, comparer).ToSafeList();
            var removed = previousUsers.Except(currentUsers, comparer).ToSafeList();

            added.ForEach(u => _context.Entry(u).State = EntityState.Added);
            removed.ForEach(u => _context.Entry(u).State = EntityState.Deleted);

            _context.SaveChanges();
        }

        private void UpdateServerLoginMappings(Department current, Department previous)
        {
            var previousLogins = previous.ServerLogins.ToSafeList();
            var currentLogins = current.ServerLogins.ToSafeList();

            var comparer = new ServerLoginComparer(CompareSetting.CompareIds);
            
            var added = currentLogins.Except(previousLogins, comparer).ToSafeList();
            var removed = previousLogins.Except(currentLogins, comparer).ToSafeList();
            
            added.ForEach(u => _context.Entry(u).State = EntityState.Added);
            removed.ForEach(u => _context.Entry(u).State = EntityState.Deleted);

            _context.SaveChanges();
        }

        private void UpdateServerMappings(Department current, Department previous)
        {
            var previousServers = previous.Servers.ToSafeList();
            var currentServers = current.Servers.ToSafeList();

            var comparer = new ServerComparer(CompareSetting.CompareIds);

            var added = currentServers.Except(previousServers, comparer).ToSafeList();
            var removed = previousServers.Except(currentServers, comparer).ToSafeList();

            added.ForEach(u => _context.Entry(u).State = EntityState.Added);
            removed.ForEach(u => _context.Entry(u).State = EntityState.Deleted);

            _context.SaveChanges();
        }
    }
}
