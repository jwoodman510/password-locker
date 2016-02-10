﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DataAccess.Exception;

namespace DataAccess
{
    public interface IContextProvider
    {
        DbSet<Company> Companies { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        void SetEntryState<T>(T entity, EntityState state) where T : class;

        void SetEntryModified<T>(T entity) where T : class;

        int SaveChanges();

        void Dispose();
    }

    public class ContextProvider : IContextProvider
    {
        private readonly PM_SQLEntities _context;
        public ContextProvider(PM_SQLEntities context)
        {
            _context = context;
        }

        public DbSet<Company> Companies
        {
            get { return _context.Companies; }
            set { _context.Companies = value; }
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
