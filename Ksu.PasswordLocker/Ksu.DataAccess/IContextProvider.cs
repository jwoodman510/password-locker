using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Ksu.DataAccess.Exception;

namespace Ksu.DataAccess
{
    public interface IContextProvider
    {
        DbSet<Company> Companies { get; set; }

        DbSet<AspNetUser> Users { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        void SetEntryState<T>(T entity, EntityState state) where T : class;

        void SetEntryModified<T>(T entity) where T : class;

        int SaveChanges();
    }

    public class ContextProvider : IContextProvider
    {
        private readonly Entities _context;
        public ContextProvider(Entities context)
        {
            _context = context;
        }

        public DbSet<Company> Companies
        {
            get { return _context.Companies; }
            set { _context.Companies = value; }
        }

        public DbSet<AspNetUser> Users
        {
            get { return _context.AspNetUsers; }
            set { _context.AspNetUsers = value; }
        }

        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return _context.Entry(entity);
        }

        public void SetEntryState<T>(T entity, EntityState state) where T : class
        {
            _context.Entry(entity).State = state;
        }

        public void SetEntryModified<T>(T entity) where T : class
        {
            SetEntryState(entity, EntityState.Modified);
        }

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw new ModificationException("An unexpected error occured. Changes were not saved.", ex);
            }
        }
    }
}
