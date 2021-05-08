using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Services
{
    public interface IFriendLookupDataServiceAsync
    {
        public Task<IEnumerable<LookupItem>> GetFriendLookupAsync();
    }
}
