using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services
{
    public class DataServiceAsyncBase<T> : IDataServiceAsync<T>
        where T : EntityBase
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public DataServiceAsyncBase(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        protected DataServiceAsyncBase()
        {
        }

        public async Task<T> CreateAsync(T entity)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            EntityEntry<T> entityEntry = await context.Set<T>().AddAsync(entity);

            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            context.Set<T>().Attach(entity);
            context.Entry<T>(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            T entity = context.Set<T>().FirstOrDefault((e) => e.Id == id);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> GetAsync(int id)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            return await context.Set<T>().AsNoTracking<T>().SingleAsync<T>(e => e.Id == id);
        }
    }
}
