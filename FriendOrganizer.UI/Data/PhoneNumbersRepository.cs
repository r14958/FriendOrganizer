using FriendOrganizer.DataAccess;
using FriendOrganizer.DataAccess.Services.Repositories;
using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data
{
    public class PhoneNumbersRepository : DataRepositoryBase<FriendPhoneNumber>, IPhoneNumbersRepository
    {


        public PhoneNumbersRepository(FriendOrganizerDbContextFactory contextFactory)
            : base(contextFactory)
        {
        }


        public async Task<IEnumerable<FriendPhoneNumber>> GetAllPhoneNumbersForFriend(int? friendId)
        {
            return await context.Set<FriendPhoneNumber>().Where(pn => pn.FriendId == friendId).ToListAsync();
        }
    }
}
