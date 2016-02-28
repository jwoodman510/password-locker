using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ksu.DataAccess.Comparers;
using Ksu.DataAccess.Exception;
using Ksu.Global.Extensions;

namespace Ksu.DataAccess.Dal
{
    public interface IServerDal : IDisposable
    {
        Server Get(int id);

        Server GetByName(string name, int companyId);
        IEnumerable<Server> GetByCompany(int companyId);

        Server Create(Server server);

        void Delete(int id);
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
            var previous = Get(id);

            if (previous == null)
                return;

            var toDelete = new Server
            {
                ServerId = id
            };

            UpdateServerLoginMappings(toDelete, previous);
            UpdateDepartmentMappings(toDelete, previous);

            var existing = _context.Departments.Find(id);
            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        private void UpdateServerLoginMappings(Server current, Server previous)
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

        private void UpdateDepartmentMappings(Server current, Server previous)
        {
            var previousDepartments = previous.Departments.ToSafeList();
            var currentDepartments = current.Departments.ToSafeList();

            var comparer = new DepartmentComparer(CompareSetting.CompareIds);

            var added = currentDepartments.Except(previousDepartments, comparer).ToSafeList();
            var removed = previousDepartments.Except(currentDepartments, comparer).ToSafeList();

            added.ForEach(u => _context.Entry(u).State = EntityState.Added);
            removed.ForEach(u => _context.Entry(u).State = EntityState.Deleted);

            _context.SaveChanges();
        }
    }
}
