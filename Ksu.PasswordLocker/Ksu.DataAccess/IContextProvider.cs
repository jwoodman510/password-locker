using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Ksu.DataAccess.Exception;

namespace Ksu.DataAccess
{
    public interface IContextProvider : IDisposable
    {
        DbSet<Company> Companies { get; set; }

        DbSet<AspNetUser> Users { get; set; }

        DbSet<AspNetRole> Roles { get; set; }

        DbSet<AspNetUserLogin> Logins { get; set; }

        DbSet<AspNetUserClaim> Claims { get; set; }

        DbSet<Department> Departments { get; set; }

        DbSet<Server> Servers { get; set; }

        DbSet<ServerLogin> ServerLogins { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        void SetEntryState<T>(T entity, EntityState state) where T : class;

        void SetEntryModified<T>(T entity) where T : class;

        int SaveChanges();
        
        Task<int> SaveChangesAsync();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
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

        public DbSet<AspNetRole> Roles
        {
            get { return _context.AspNetRoles; }
            set { _context.AspNetRoles = value; }
        }

        public DbSet<AspNetUserLogin> Logins
        {
            get { return _context.AspNetUserLogins; }
            set { _context.AspNetUserLogins = value; }
        }
        public DbSet<AspNetUserClaim> Claims
        {
            get { return _context.AspNetUserClaims; }
            set { _context.AspNetUserClaims = value; }
        }

        public DbSet<Department> Departments
        {
            get { return _context.Departments; }
            set { _context.Departments = value; }
        }

        public DbSet<Server> Servers
        {
            get { return _context.Servers; }
            set { _context.Servers = value; }
        }

        public DbSet<ServerLogin> ServerLogins
        {
            get { return _context.ServerLogins; }
            set { _context.ServerLogins = value; }
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

        public Task<int> SaveChangesAsync()
        {
            try
            {
                return _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                throw new ModificationException("An unexpected error occured. Changes were not saved.", ex);
            }
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
