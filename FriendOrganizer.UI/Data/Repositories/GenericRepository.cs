using FriendOrganizer.DataAccess;
using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : EntityBase
    {
        protected readonly FriendOrganizerDbContext context;

        public GenericRepository(FriendOrganizerDbContextFactory contextFactory)
        {
            context = contextFactory.CreateDbContext();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            EntityEntry<T> entityEntry = await context.Set<T>().AddAsync(entity);

            return entityEntry.Entity;
        }

        public virtual async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public virtual void Remove(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().SingleAsync(e => e.Id == id);
        }

        public virtual bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
