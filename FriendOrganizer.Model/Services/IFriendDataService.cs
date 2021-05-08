using FriendOrganizer.Domain.Models;
using System.Collections.Generic;

namespace FriendOrganizer.Domain.Services
{
    public interface IFriendDataService
    {
        IEnumerable<Friend> GetAll();
    }
}