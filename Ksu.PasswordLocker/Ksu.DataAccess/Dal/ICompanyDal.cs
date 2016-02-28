using System;
using System.Collections.Generic;
using System.Linq;
using Ksu.DataAccess.Exception;

namespace Ksu.DataAccess.Dal
{
    public interface ICompanyDal : IDisposable
    {
        Company Get(string name);
        Company Get(int id);

        void AddUser(int id, string userId);
        Company Create(Company company);
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

        public void AddUser(int id, string userId)
        {
            var company = _context.Companies.Find(id);
            var user = _context.Users.Find(userId);

            company.AspNetUsers = company.AspNetUsers ?? new List<AspNetUser>();
            company.AspNetUsers.Add(user);

            _context.SaveChanges();
        }

        public Company Create(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (string.IsNullOrEmpty(company.CompanyName))
                throw new ValidationException("Company name cannot be empty.");

            if(company.CompanyName.Length > 200)
                throw new ValidationException("Company name cannot exceed 200 characters.");

            var existing = Get(company.CompanyName);

            if (existing != null)
                throw new ValidationException("Company with same name already exists.");

            var result = new Company
            {
                CompanyName = company.CompanyName
            };

            _context.Companies.Add(result);

            _context.SaveChanges();

            return result;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
