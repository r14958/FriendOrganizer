using FriendOrganizer.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IMeetingRepository : IGenericRepository<Meeting>
    {
        public Task<List<Friend>> GetAllFriendsAsync();
    }
}