using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Services
{
    public interface ILookupDataService<T> where T : EntityBase
    {
        public Task<IEnumerable<LookupItem<T>>> GetLookupAsync();
    }
}
