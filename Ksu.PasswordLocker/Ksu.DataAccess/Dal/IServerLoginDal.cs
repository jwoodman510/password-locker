using System;
using System.Collections.Generic;
using System.Linq;
using Ksu.DataAccess.Exception;

namespace Ksu.DataAccess.Dal
{
    public interface IServerLoginDal
    {
        IEnumerable<ServerLogin> GetByCompany(int companyId);

        IEnumerable<ServerLogin> GetByUser(string userId);

        ServerLogin GetByUserName(string userName, int departmentId, int serverId);

        ServerLogin Create(ServerLogin login);
    }

    public class ServerLoginDal : IServerLoginDal
    {
        private readonly IContextProvider _context;
        public ServerLoginDal(IContextProvider context)
        {
            _context = context;
        }

        public IEnumerable<ServerLogin> GetByCompany(int companyId)
        {
            return _context.ServerLogins
                .AsNoTracking()
                .Where(s => s.Department.CompanyId == companyId);
        }

        public IEnumerable<ServerLogin> GetByUser(string userId)
        {
            return _context.ServerLogins
                .AsNoTracking()
                .Where(s => s.Department.AspNetUsers.Any(u => u.Id == userId));
        }

        public ServerLogin GetByUserName(string userName, int departmentId, int serverId)
        {
            return _context.ServerLogins
                .AsNoTracking()
                .FirstOrDefault(l =>
                l.DepartmentId == departmentId &&
                l.ServerId == serverId &&
                l.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
        }

        public ServerLogin Create(ServerLogin login)
        {
            if (login == null)
                throw new ArgumentNullException(nameof(login));

            if (login.DepartmentId < 1)
                throw new ValidationException("Missing DepartmentId");

            if (login.ServerId < 1)
                throw new ValidationException("Missing ServerId");

            if(string.IsNullOrWhiteSpace(login.UserName))
                throw new ValidationException("Missing UserName");

            if (string.IsNullOrWhiteSpace(login.PasswordHash))
                throw new ValidationException("Missing Password");

            var result = new ServerLogin
            {
                ServerId = login.ServerId,
                DepartmentId = login.DepartmentId,
                UserName = login.UserName,
                PasswordHash = login.PasswordHash
            };

            _context.ServerLogins.Add(result);

            _context.SaveChanges();

            return result;
        }
    }
}
