using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services.Repositories
{
    public class DataRepositoryBase<T> : IDataRepository<T>
        where T : EntityBase
    {
        protected readonly FriendOrganizerDbContext context;

        public DataRepositoryBase(FriendOrganizerDbContextFactory contextFactory)
        {
            context = contextFactory.CreateDbContext();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            EntityEntry<T> entityEntry = await context.Set<T>().AddAsync(entity);

            return entityEntry.Entity;
        }

        public virtual async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync<T>();
        }

        public virtual async Task<T> GetAsync(int? id)
        {
            return await context.Set<T>().SingleAsync(e => e.Id == id);
        }

        public virtual bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
