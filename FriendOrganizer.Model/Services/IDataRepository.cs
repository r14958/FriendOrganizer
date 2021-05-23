using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Services
{
    public interface IDataRepository<T> where T : EntityBase
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetAsync(int? id);

        Task<T> CreateAsync(T entity);

        Task SaveAsync();

        Task DeleteAsync(T entity);

        bool HasChanges();
    }
}
