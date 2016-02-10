using System;
using System.Linq;

namespace Ksu.DataAccess.Dal
{
    public interface ICompanyDal
    {
        Company Get(string name);
    }

    public class CompanyDal : ICompanyDal
    {
        private readonly IContextProvider _context;
        public CompanyDal(IContextProvider context)
        {
            _context = context;
        }

        public Company Get(string name)
        {
            return _context.Companies
                .AsNoTracking()
                .FirstOrDefault(c => c.CompanyName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
