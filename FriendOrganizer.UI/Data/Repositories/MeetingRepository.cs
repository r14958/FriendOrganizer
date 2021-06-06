using FriendOrganizer.DataAccess;
using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class MeetingRepository : GenericRepository<Meeting>, IMeetingRepository
    {
        public MeetingRepository(FriendOrganizerDbContextFactory contextFactory) : base(contextFactory) { }

        public override async Task<Meeting> GetByIdAsync(int id)
        {
            return await context.Meetings
                .Include(m => m.Friends)
                .SingleAsync(m => m.Id == id);
        }

        public async Task<List<Friend>> GetAllFriendsAsync()
        {
            return await context.Set<Friend>().ToListAsync();
        }

        public async Task ReloadFriendAsync(int friendId)
        {
            var entityEntry = context.ChangeTracker.Entries<Friend>()
                .SingleOrDefault(ee => ee.Entity.Id == friendId);

            if (entityEntry != null)
            {
                await entityEntry.ReloadAsync();
            }

        }
    }
}
