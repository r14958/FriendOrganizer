using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services
{
    public class NonQueryDataService<T> where T : EntityBase
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public NonQueryDataService(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public T Create(T entity)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            EntityEntry<T> entityEntry = context.Set<T>()
                .Add(entity);

            context.SaveChanges();

            return entityEntry.Entity;
        }

        public T Update(int id, T entity)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            entity.Id = id;

            context.Set<T>().Update(entity);
            context.SaveChanges();

            return entity;
        }

        public bool Delete(int id)
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            T entity = context.Set<T>().FirstOrDefault((e) => e.Id == id);
            context.Set<T>().Remove(entity);
            context.SaveChanges();

            return true;
        }
    }
}
