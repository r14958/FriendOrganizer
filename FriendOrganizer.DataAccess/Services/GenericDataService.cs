using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.Services
{
    public class GenericDataService<T> : IDataService<T> where T : EntityBase
    {
        private readonly FriendOrganizerDbContextFactory contextFactory;

        public GenericDataService(FriendOrganizerDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public T Create(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public T Get(int id)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> GetAll()
        {
            using FriendOrganizerDbContext context = contextFactory.CreateDbContext();
            IEnumerable<T> entities = context.Set<T>().AsNoTracking().ToList();
            return entities;
        }

        public T Update(int id, T entity)
        {
            throw new NotImplementedException();
        }
    }
}
