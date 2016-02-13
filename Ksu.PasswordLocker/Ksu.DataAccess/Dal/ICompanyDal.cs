using System;
using System.Collections.Generic;
using System.Linq;

namespace Ksu.DataAccess.Dal
{
    public interface ICompanyDal
    {
        Company Get(string name);
        Company Get(int id);

        void AddUser(int companyId, string userId);
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

        public Company Get(int id)
        {
            return _context.Companies
                .AsNoTracking()
                .FirstOrDefault(c => c.CompanyId == id);
        }

        public void AddUser(int companyId, string userId)
        {
            var company = Get(companyId);
            var user = _context.Users.Find(userId);

            company.AspNetUsers = company.AspNetUsers ?? new List<AspNetUser>();
            company.AspNetUsers.Add(user);

            _context.SaveChanges();
        }
    }
}
