using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ksu.DataAccess.Exception;

namespace Ksu.DataAccess.Dal
{
    public interface IServerDal : IDisposable
    {
        Server Get(int id);

        Server GetByName(string name, int companyId);
        IEnumerable<Server> GetByCompany(int companyId);

        Server Create(Server server);

        void Delete(int id);

        void Update(Server server);

        void AddDepartment(int serverId, int departmentId);

        void RemoveDepartment(int serverId, int departmentId);
    }
    
    public class ServerDal : IServerDal
    {
        private readonly IContextProvider _context;
        public ServerDal(IContextProvider context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public Server Get(int id)
        {
            return _context.Servers
                .AsNoTracking()
                .AsQueryable()
                .FirstOrDefault(s => s.ServerId == id);
        }

        public Server GetByName(string name, int companyId)
        {
            return _context.Servers
                .AsNoTracking()
                .AsQueryable()
                .FirstOrDefault(s =>
                    s.CompanyId == companyId &&
                    s.ServerName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<Server> GetByCompany(int companyId)
        {
            return _context.Servers
                .AsNoTracking()
                .AsQueryable()
                .Where(d => d.CompanyId == companyId);
        }

        public Server Create(Server server)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            if (server.CompanyId < 1)
                throw new ValidationException("Missing companyId");

            if (string.IsNullOrEmpty(server.ServerName))
                throw new ValidationException("Server name cannot be empty.");

            var existing = GetByCompany(server.CompanyId)
                .FirstOrDefault(d => d.ServerName == server.ServerName);

            if (existing != null)
                throw new ValidationException("Server with same name already exists.");

            var result = new Server
            {
                ServerName = server.ServerName,
                CompanyId = server.CompanyId
            };

            _context.Servers.Add(result);

            _context.SaveChanges();

            return result;
        }

        public void Delete(int id)
        {
            var existing = _context.Servers
                .Include("Departments")
                .Include("ServerLogins")
                .First(d => d.ServerId == id);

            if (existing == null)
                return;

            existing.Departments.Clear();
            existing.ServerLogins.Clear();

            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        public void Update(Server server)
        {
            var previous = _context.Servers.Find(server.ServerId);

            if (previous == null)
                throw new NotFoundException("Server not found.");

            if (string.IsNullOrWhiteSpace(server.ServerName))
                throw new ValidationException("Server name not specified.");

            if (server.ServerName.Length > 200)
                throw new ValidationException("Server name cannot exceed 200 characters.");

            var duplicate = _context.Servers
                .AsNoTracking()
                .Where(d => d.CompanyId == server.CompanyId)
                .FirstOrDefault(d => d.ServerName.ToLower().Equals(server.ServerName));

            if (duplicate != null)
                throw new ValidationException("Server with same name already exists.");

            var entry = _context.Entry(previous);

            entry.Entity.ServerName = server.ServerName;

            _context.SaveChanges();

            _context.Entry(previous).State = EntityState.Detached;
        }

        public void AddDepartment(int serverId, int departmentId)
        {
            var server = _context.Servers
                   .Include("Departments")
                   .First(d => d.ServerId == serverId);

            var department = _context.Departments.Find(departmentId);

            if (server == null)
                throw new NotFoundException("Department not found.");

            if (server.CompanyId != department.CompanyId)
                throw new ValidationException("Invalid company department.");

            if (server.Departments == null)
                server.Departments = new List<Department>();

            server.Departments.Add(department);

            _context.SaveChanges();
        }

        public void RemoveDepartment(int serverId, int departmentId)
        {
            var server = _context.Servers
                   .Include("Departments")
                   .First(d => d.ServerId == serverId);

            var found = server.Departments?.FirstOrDefault(s => s.DepartmentId == departmentId);

            if (found == null)
                return;

            server.Departments.Remove(found);

            _context.SaveChanges();
        }
    }
}
