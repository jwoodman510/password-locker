using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataAccess.Utilities
{
    public static class DbSetExtensions
    {
        public static IEnumerable<T> GetNoTracking<T>(this DbSet<T> dbSet, Func<T, bool> predicate = null) where T : class
        {
            return predicate != null
                ? dbSet.AsNoTracking().Where(predicate)
                : dbSet.AsNoTracking();
        }

        public static Company GetNoTracking(this DbSet<Company> dbSet, int id)
        {
            return dbSet.AsNoTracking()
                .FirstOrDefault(e => e.CompanyId == id);
        }
    }
}
