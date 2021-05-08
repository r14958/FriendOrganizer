using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services
{
    public class NonQueryDataServiceAsync<T> where T : EntityBase
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public NonQueryDataServiceAsync(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<T> Create(T entity)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            EntityEntry<T> entityEntry = await context.Set<T>().AddAsync(entity);

            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task<T> Update(int id, T entity)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            entity.Id = id;

            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            T entity = context.Set<T>().FirstOrDefault((e) => e.Id == id);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync ();

            return true;
        }
    }
}
