using FriendOrganizer.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.Domain.Services
{
    public interface IFriendDataServiceAsync
    {
        Task<Friend> GetByIdAsync(int friendId);

        Task SaveAsync(Friend friend);
    }
}
