using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ksu.DataAccess.Exception;

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

        void AddServer(int departmentId, int serverId);

        void RemoveServer(int departmentId, int serverId);
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
            var existing = _context.Departments
                .Include("Servers")
                .Include("AspNetUsers")
                .Include("ServerLogins")
                .Include("DepartmentId")
                .First(d => d.DepartmentId == departmentId);

            if (existing == null)
                return;

            existing.Servers.Clear();
            existing.AspNetUsers.Clear();
            existing.ServerLogins.Clear();

            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        public void AddServer(int departmentId, int serverId)
        {
            var department = _context.Departments
                .Include("Servers")
                .First(d => d.DepartmentId == departmentId);

            var server = _context.Servers.Find(serverId);

            if (server == null)
                throw new NotFoundException("Server not found.");

            if (server.CompanyId != department.CompanyId)
                throw new ValidationException("Invalid company server.");

            if (department.Servers == null)
                department.Servers = new List<Server>();

            department.Servers.Add(server);

            _context.SaveChanges();
        }

        public void RemoveServer(int departmentId, int serverId)
        {
            var department = _context.Departments
                .Include("Servers")
                .First(d => d.DepartmentId == departmentId);

            var found = department.Servers?.FirstOrDefault(s => s.ServerId == serverId);

            if (found == null)
                return;

            department.Servers.Remove(found);

            _context.SaveChanges();
        }
    }
}
