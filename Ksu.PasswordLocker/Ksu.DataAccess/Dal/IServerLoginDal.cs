using System.Collections.Generic;
using System.Linq;

namespace Ksu.DataAccess.Dal
{
    public interface IServerLoginDal
    {
        IEnumerable<ServerLogin> GetByCompany(int companyId);

        IEnumerable<ServerLogin> GetByUser(string userId);
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
    }
}
