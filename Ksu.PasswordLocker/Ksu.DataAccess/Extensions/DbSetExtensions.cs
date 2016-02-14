using System.Data.Entity;
using System.Threading.Tasks;

namespace Ksu.DataAccess.Extensions
{
    public static class DbSetExtensions
    {
        public static void Create<TEntity>(this DbSet<TEntity> dbSet, TEntity e) where TEntity : class
        {
            dbSet.Add(e);
        }

        public static void Delete<TEntity>(this DbSet<TEntity> dbSet, TEntity e) where TEntity : class
        {
            dbSet.Remove(e);
        }

        public static Task<TEntity> GetByIdAsync<TEntity>(this DbSet<TEntity> dbSet, object id) where TEntity : class
        {
            return dbSet.FindAsync(id);
        }
    }
}
