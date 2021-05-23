using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Services
{
    public interface IDataService<T> where T : EntityBase
    {
        IEnumerable<T> GetAll();

        T Get(int id);

        T Create(T entity);

        T Update(int id, T entity);

        bool Delete(int id);
    }
}
