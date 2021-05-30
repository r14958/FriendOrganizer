using FriendOrganizer.Domain.Models;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository : IGenericRepository<Friend>
    {
        void RemovePhoneNumber(FriendPhoneNumber entity);

        public Task<bool> HasMeetingsAsync(int id);
    }
}