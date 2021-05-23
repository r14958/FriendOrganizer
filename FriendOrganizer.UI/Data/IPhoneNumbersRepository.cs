using FriendOrganizer.Domain.Models;
using FriendOrganizer.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public interface IPhoneNumbersRepository : IDataRepository<FriendPhoneNumber>
    {
        Task<IEnumerable<FriendPhoneNumber>> GetAllPhoneNumbersForFriend(int? friendId);
    }
}