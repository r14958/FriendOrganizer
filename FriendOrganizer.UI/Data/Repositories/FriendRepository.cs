using FriendOrganizer.DataAccess;
using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : GenericRepository<Friend>, IFriendRepository
    {
        public FriendRepository(FriendOrganizerDbContextFactory contextFactory) : base(contextFactory) { }

        public override async Task<Friend> GetByIdAsync(int friendId)
        {
            return await context.Friends
                .Include(f => f.PhoneNumbers)
                .SingleAsync(f => f.Id == friendId);
        }

        public void RemovePhoneNumber(FriendPhoneNumber phoneNumber)
        {
            context.FriendPhoneNumbers.Remove(phoneNumber);
        }

        public async Task<bool> HasMeetingsAsync(int friendId)
        {
            return await context.Meetings.AsNoTracking()
                .Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f => f.Id == friendId));
        }
    }
}
